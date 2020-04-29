using System;
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
using Compo_Request.Network;
using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
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
        private RegisterWindow registerWindow;

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        public MainWindow()
        {
#if DEBUG
            AllocConsole();
            Debug.Log("The console is running!");
#endif
            InitializeComponent();
            LoadWindowParent();
            EventsInitialize();

            ConnectService.Start();

            NetworkDelegates.Add(ServerShutdown, Dispatcher, default, "Server.Disconnect");
        }

        private void ServerShutdown(MResponse receiver)
        {
            TextBox_LoginOrEmail.Text = "Сервер завершил работу!";
        }

        public void LoadWindowParent()
        {
            registerWindow = new RegisterWindow(this);
        }

        /// <summary>
        /// Иницализация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            // Регистрация события нажатия на кнопку регистрации
            Button_Register.Click += Button_Register_Click;
            Button_Login.Click += Button_Login_Click;
            this.Closed += MainWindow_Closed;
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            Sender.SendToServer("OnClient", TextBox_LoginOrEmail.Text, 1);
        }

        /// <summary>
        /// Действие происходящее при нажатии кнопки регистрации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            registerWindow.Show();
        }

        /// <summary>
        /// Действие происходящее при закрытии главного окна авторизации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            registerWindow.Close();
        }
    }
}
