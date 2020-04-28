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

            Debug.Log($"User connected: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}");
        }

        protected internal void RemoveConnection(string id)
        {

            UserNetwork UserNetwork = NetworkBase.UsersNetwork.FirstOrDefault(c => c.Id == id);

            if (UserNetwork != null)
                NetworkBase.UsersNetwork.Remove(UserNetwork);

            Debug.Log($"User disconnected: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}");
        }

        protected internal void Listen()
        {
            try
            {
                NetworkBase.Listener.Listen(10);

                Debug.Log("Server is running!");

                while (true)
                {
                    Socket ClientNetwork = NetworkBase.Listener.Accept();

                    Debug.Log("Initialize user connection...");

                    Debug.Log("Creating a user component.");
                    ClientBase Client = new ClientBase(new UserNetwork(ClientNetwork), this);

                    Debug.Log("Starting a user process in a separate thread.");
                    Thread ClientThread = new Thread(new ThreadStart(Client.Process));
                    ClientThread.IsBackground = true;
                    ClientThread.Start();

                    Debug.Log("User connected successfully!");

                    Console.Read();
                    try
                    {
                        Disconnect();
                    }
                    catch
                    {
                        Console.Read();
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                Debug.LogError("Server forcibly terminated with an error:\n" + ex);

                Disconnect();
            }
        }

        public static void Disconnect()
        {
            Debug.Log("Server shutdown process...");

            Sender.Broadcast("Server.Disconnect");

            NetworkBase.Listener.Close();

            Debug.Log("Closing the server listener.");
            Debug.Log("User disconnect process...");

            for (int i = 0; i < NetworkBase.UsersNetwork.Count; i++)
            {
                NetworkBase.UsersNetwork[i].ClientNetwork.Close();
            }

            Debug.Log("All users are disconnected!");

            Debug.Log("The server is shutting down.");
            Environment.Exit(0);
        }
    }
}
