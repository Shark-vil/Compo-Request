using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Compo_Request_Server.Network.Client;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Utilities;

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
        }

        protected internal void RemoveConnection(string id)
        {

            UserNetwork UserNetwork = NetworkBase.UsersNetwork.FirstOrDefault(c => c.Id == id);

            if (UserNetwork != null)
                NetworkBase.UsersNetwork.Remove(UserNetwork);
        }

        protected internal void Listen()
        {
            try
            {
                NetworkBase.Listener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient ClientNetwork = NetworkBase.Listener.AcceptTcpClient();

                    ClientBase Client = new ClientBase(new UserNetwork(ClientNetwork), this);

                    Thread ClientThread = new Thread(new ThreadStart(Client.Process));
                    ClientThread.IsBackground = true;
                    ClientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void Disconnect()
        {
            NetworkBase.Listener.Stop();

            for (int i = 0; i < NetworkBase.UsersNetwork.Count; i++)
            {
                NetworkBase.UsersNetwork[i].ClientNetwork.Close();
            }

            Environment.Exit(0);
        }
    }
}
