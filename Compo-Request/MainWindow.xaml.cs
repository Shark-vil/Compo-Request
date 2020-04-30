﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Compo_Request.Network;
using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Request.Network.Utilities.Validators;
using Compo_Request.Windows;
using Compo_Request.Windows.Projects;
using Compo_Request.Windows.UserRegister;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;

namespace Compo_Request
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RegisterWindow _RegisterWindow;
        private ProjectsWindow _ProjectsWindow;
        private DispatcherTimer MainWindowElementsUnblockTimer;

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        public MainWindow()
        {
#if DEBUG
            AllocConsole();
            Debug.Log("Запущена консоль отладки", ConsoleColor.Green);
#endif
            InitializeComponent();
            EventsInitialize();

            ConnectService.Start();

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var NetUser = Package.Unpacking<MUserNetwork>(ServerResponse.DataBytes);

                _ProjectsWindow = new ProjectsWindow(this);
                this.Hide();
                _ProjectsWindow.Show();

                MainWindowElements_Unblock();

            }, Dispatcher, 2, "User.Auth.Confirm", "MainWindow");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (StringValid.IsValidEmail(TextBox_LoginOrEmail.Text))
                {
                    var Alert = new AlertWindow("Ошибка", "Не верно указана почта или пароль!");
                }
                else
                {
                    var Alert = new AlertWindow("Ошибка", "Не верно указан логин или пароль!");
                }

                MainWindowElements_Unblock();

            }, Dispatcher, 2, "User.Auth.Error", "MainWindow");
        }

        /// <summary>
        /// Иницализация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            // Регистрация события нажатия на кнопку регистрации
            Button_Register.Click += Button_Register_Click;
            Button_Login.Click += Button_Login_Click;
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            var UserData = new string[]
            {
                TextBox_LoginOrEmail.Text,
                PasswordBox_Password.Password
            };

            if (Sender.SendToServer("User.Auth", UserData, 1))
            {
                MainWindowElementsUnblockTimer = new DispatcherTimer();
                MainWindowElementsUnblockTimer.Tick += new EventHandler(MainWindowElements_UnblockTimer);
                MainWindowElementsUnblockTimer.Interval = new TimeSpan(0, 0, 5);
                MainWindowElementsUnblockTimer.Start();

                MainWindowElements_Block();
            }
            else
            {
                var Alert = new AlertWindow("Ошибка", "Не удалось соединиться с сервером.\n" +
                    "Возможно сервер выключен или присутствуют неполадки в вашем интернет-соединении.",
                    MainWindowElements_Unblock);
            }
        }

        private void MainWindowElements_UnblockTimer(object sender, EventArgs e)
        {
            MainWindowElementsTimer_Destroy();

            var Alert = new AlertWindow("Ошибка", "Время ожидания ответа от сервера истекло.\n" +
                "Возможно введены некорректные данные.",
                MainWindowElements_Unblock);
        }

        private void MainWindowElementsTimer_Destroy()
        {
            if (MainWindowElementsUnblockTimer != null && MainWindowElementsUnblockTimer.IsEnabled)
                MainWindowElementsUnblockTimer.Stop();
        }

        /// <summary>
        /// Действие происходящее при нажатии кнопки регистрации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            _RegisterWindow = new RegisterWindow(this);
            this.Hide();
            _RegisterWindow.Show();
        }

        private void MainWindowElements_Block()
        {
            TextBox_LoginOrEmail.IsEnabled = false;
            PasswordBox_Password.IsEnabled = false;
            Button_Login.IsEnabled = false;
            Button_Register.IsEnabled = false;
        }

        private void MainWindowElements_Unblock()
        {
            TextBox_LoginOrEmail.IsEnabled = true;
            PasswordBox_Password.IsEnabled = true;
            Button_Login.IsEnabled = true;
            Button_Register.IsEnabled = true;

            MainWindowElementsTimer_Destroy();
        }
    }
}
