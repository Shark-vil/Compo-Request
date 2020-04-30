﻿using Compo_Request.Network.Client;
using Compo_Shared_Data.Debugging;
using System;
using System.Threading;

namespace Compo_Request.Network.Utilities
{
    public class ConnectService
    {
        private static Thread ServiceThread = null;
        public static Thread ClientThread = null;

        public static void Start()
        {
            if (ServiceThread == null)
            {
                Debug.Log("Подготовка службы поддержки соединения с сервером", ConsoleColor.Cyan);

                ServiceThread = new Thread(new ThreadStart(Process));
                ServiceThread.IsBackground = true;
                ServiceThread.Start();

                Debug.Log("Служба поддержки соединения с сервером запущена", ConsoleColor.Green);
            }
        }

        public static void Stop()
        {
            if (ServiceThread != null)
            {
                if (ServiceThread.IsAlive)
                    ServiceThread.Abort();

                if (ClientThread.IsAlive)
                    ClientThread.Abort();

                ServiceThread = null;
                ClientThread = null;

                Debug.Log("Служба поддержки соединения с сервером остановлена");
            }
        }

        private static void Process()
        {
            while (true)
            {
                try
                {
                    if (NetworkBase.ClientNetwork == null || !NetworkBase.ClientNetwork.Connected)
                    {
                        if (NetworkBase.Setup("127.0.0.1", 8888))
                        {
                            Debug.Log("Попытка соединиться с сервером", ConsoleColor.Cyan);
                            ConnectToServer();
                        }
                    }
                }
                catch
                {

                }

                Thread.Sleep(2000);
            }
        }

        private static void ConnectToServer()
        {
            if (ClientThread == null || !ClientThread.IsAlive)
            {
                ClientThread = new Thread(new ThreadStart(ClientBase.Process));
                ClientThread.IsBackground = true;
                ClientThread.Start();
                ClientBase.SelfThread = ClientThread;

                Debug.Log("Подключение к серверу установлено", ConsoleColor.Green);
            }
        }
    }
}
