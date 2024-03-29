﻿using Compo_Request.Data.Network;
using Compo_Request.Network.Client;
using Compo_Shared_Data.Debugging;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Compo_Request.Network.Utilities
{
    public class ConnectService
    {
        private static Thread ServiceThread = null;
        public static Thread ClientThread = null;

        public delegate void ConnectBrokenDelegate();
        public static ConnectBrokenDelegate ConnectBrokenEvents;

        internal static MainWindow _MainWindow;
        internal static Dispatcher _MainWindowDispatcher;
        internal static DispatcherTimer _MainWindowTimer;

        private static bool IsFirstLoad = true;

        public static void Start(MainWindow _MainWindow)
        {
            if (ServiceThread == null)
            {
                ConnectService._MainWindow = _MainWindow;
                ConnectService._MainWindowDispatcher = _MainWindow.Dispatcher;

                if (Data.Windows.AutomaticAuthorizate.Exists())
                {
                    if (IsFirstLoad)
                    {
                        _MainWindow.TextBox_LoginOrEmail.IsEnabled = false;
                        _MainWindow.PasswordBox_Password.IsEnabled = false;
                    }

                    _MainWindowTimer = CustomTimer.Create(delegate (object sender, EventArgs e)
                    {
                        if (IsFirstLoad)
                        {
                            _MainWindow.TextBox_LoginOrEmail.IsEnabled = true;
                            _MainWindow.PasswordBox_Password.IsEnabled = true;
                            IsFirstLoad = false;

                            _MainWindow.WindowLogic.AutomaticAuthorizate(true);
                        }
                    }, new TimeSpan(0, 0, 5));
                }
                else
                    IsFirstLoad = false;

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
                    SetupConnection();
                }
                catch
                {

                }

                Thread.Sleep(2000);
            }
        }

        private static void SetupConnection()
        {
            if (NetworkBase.ClientNetwork == null || !NetworkBase.ClientNetwork.Connected)
            {

                string[] ServerInfo = ServerData.Read();

                if (NetworkBase.Setup(ServerInfo[0], Convert.ToInt32(ServerInfo[1])))
                {
                    Debug.Log("Попытка соединиться с сервером", ConsoleColor.Cyan);
                    ConnectToServer();
                }
                else
                {
                    Debug.LogError("Не удаётся установить соединение с сервером!");
                }
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

                if (IsFirstLoad)
                    _MainWindowDispatcher.Invoke(() =>
                    {
                        IsFirstLoad = false;

                        if (_MainWindowTimer != null && _MainWindowTimer.IsEnabled)
                        {
                            _MainWindowTimer.Stop();
                            _MainWindowTimer = null;
                        }

                        _MainWindow.WindowLogic.AutomaticAuthorizate();
                    });
            }
        }
    }
}
