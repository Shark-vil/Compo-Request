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
using Compo_Request.Network.Utilities.Validators;
using Compo_Request.Windows;
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
            EventsInitialize();

            ConnectService.Start();

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var NetUser = Package.Unpacking<MUserNetwork>(ServerResponse.DataBytes);

                var Alert = new AlertWindow("Успешный вход", "Вы успешно вошли в систему! Ваш уникальный ID:\n" +
                    $"{NetUser.Uid}");

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

            Sender.SendToServer("User.Auth", UserData, 1);
        }

        /// <summary>
        /// Действие происходящее при нажатии кнопки регистрации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            registerWindow = new RegisterWindow(this);
            this.Hide();
            registerWindow.Show();
        }
    }
}
