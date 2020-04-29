﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Compo_Request_Server.Network;
using Compo_Request_Server.Network.Events.Auth;
using Compo_Request_Server.Network.Events.Register;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Debugging;

namespace Compo_Request_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Log("Подготовка сервера...", ConsoleColor.Cyan);

            var Server = new ServerBase();

            try
            {
                RegisterEvents();

                var ServerThread = new Thread(new ThreadStart(Server.Listen));
                ServerThread.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при попытке запустить сервер! Код ошибки:\n" + ex);
                ServerBase.Disconnect();
            }
        }

        private static void RegisterEvents()
        {
            var EventRegister = new ERegister();
            var EventAuth = new EAuth();
        }
    }
}
