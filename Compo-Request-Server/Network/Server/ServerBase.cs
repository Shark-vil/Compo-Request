using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Compo_Request_Server.Data.Network;
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
            string[] ServerInfo = ServerData.Read();

            NetworkBase.Setup(ServerInfo[0], Convert.ToInt32(ServerInfo[1]));
        }

        protected internal void AddConnection(MNetworkClient NetworkClient)
        {
            NetworkBase.NetworkClients.Add(NetworkClient);

            Debug.Log($"Клиент подключён: [{NetworkClient.Id}] {NetworkClient.Ip}:{NetworkClient.Port}");
        }

        protected internal void RemoveConnection(string id)
        {

            MNetworkClient NetworkClient = NetworkBase.NetworkClients.FirstOrDefault(c => c.Id == id);

            if (NetworkClient != null)
                NetworkBase.NetworkClients.Remove(NetworkClient);

            Debug.Log($"Клиент отключён: [{NetworkClient.Id}] {NetworkClient.Ip}:{NetworkClient.Port}");
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

                    Socket SocketClient = NetworkBase.Listener.Accept();

                    Debug.Log("Получен запрос на подключение!");

                    Debug.Log("Создание компонента клиента.");
                    ClientBase Client = new ClientBase(new MNetworkClient(SocketClient), this);

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
            for (int i = 0; i < NetworkBase.NetworkClients.Count; i++)
            {
                NetworkBase.NetworkClients[i].ClientNetwork.Close();
            }
            Debug.Log("Все пользователи отключены", ConsoleColor.Green);

            Debug.Log("Закрытие процесса слушателя", ConsoleColor.Cyan);
            try
            {
                NetworkBase.Listener.Blocking = false;
                NetworkBase.Listener.Shutdown(SocketShutdown.Both);
                NetworkBase.Listener.Close();
            }
            catch { }
            Debug.Log("Прослушивание подключений остановлено", ConsoleColor.Green);

            Environment.Exit(0);
        }
    }
}
