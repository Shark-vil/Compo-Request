using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Compo_Request_Server.Network.Client;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;

namespace Compo_Request_Server.Network.Server
{
    public class ServerBase
    {
        public ServerBase()
        {
            NetworkBase.Setup(8888);
        }

        protected internal void AddConnection(UserNetwork UserNetwork)
        {
            NetworkBase.UsersNetwork.Add(UserNetwork);

            Debug.Log($"Клиент подключён: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}");
        }

        protected internal void RemoveConnection(string id)
        {

            UserNetwork UserNetwork = NetworkBase.UsersNetwork.FirstOrDefault(c => c.Id == id);

            if (UserNetwork != null)
                NetworkBase.UsersNetwork.Remove(UserNetwork);

            Debug.Log($"Клиент отключён: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}");
        }

        protected internal void Listen()
        {
            var ServerThreadWriter = new Thread(new ThreadStart(ConsoleWriter));
            ServerThreadWriter.Start();

            try
            {
                NetworkBase.Listener.Listen(10);

                Debug.Log("Сервер запущен!", ConsoleColor.Green);

                while (true)
                {
                    Debug.Log("Ожидание подключений...");

                    Socket ClientNetwork = NetworkBase.Listener.Accept();

                    Debug.Log("Получен запрос на подключение!");

                    Debug.Log("Создание компонента клиента.");
                    ClientBase Client = new ClientBase(new UserNetwork(ClientNetwork), this);

                    Debug.Log("Запуск клиентского процесса.");
                    Thread ClientThread = new Thread(new ThreadStart(Client.Process));
                    ClientThread.IsBackground = true;
                    ClientThread.Start();

                    Debug.Log("Клиент инициализирован!");
                }
            }
            catch (ThreadAbortException ex)
            {
                Debug.LogError("Возникла ошибка в главном серверном потоке! Код ошибки:\n" + ex);

                if (ServerThreadWriter.IsAlive)
                    ServerThreadWriter.Abort();

                Disconnect();
            }
        }

        protected internal void ConsoleWriter()
        {
            try
            {
                Debug.Log("Запуск потока ввода данных");

                while (true)
                {
                    string Command = Console.ReadLine();

                    switch (Command)
                    {
                        case "quit":
                            Disconnect();
                            break;
                        default:
                            Debug.Log("Введена неизвестная команда");
                            break;
                    }
                }
            }
            catch { }
        }

        public static void Disconnect()
        {
            Debug.Log("Процесс отключения сервера...");

            Sender.Broadcast("Server.Disconnect");

            Debug.Log("Отключение пользователей...", ConsoleColor.Cyan);
            for (int i = 0; i < NetworkBase.UsersNetwork.Count; i++)
            {
                NetworkBase.UsersNetwork[i].ClientNetwork.Close();
            }
            Debug.Log("Все пользователи отключены", ConsoleColor.Green);

            Debug.Log("Закрытие процесса слушателя", ConsoleColor.Cyan);
            if (NetworkBase.Listener.Connected)
            {
                NetworkBase.Listener.Blocking = false;
                NetworkBase.Listener.Close();
            }
            Debug.Log("Прослушивание подключений остановлено", ConsoleColor.Green);

            Debug.Log("Сервер завершит работу через 5 секунд");

            Thread.Sleep(5000);

            Environment.Exit(0);
        }
    }
}
