using System;
using System.Runtime.InteropServices;
using System.Windows;
using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Request.Windows;
using Compo_Request.Windows.UserRegister;
using Compo_Request.WindowsLogic;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network.Models;

namespace Compo_Request
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Основная логика текущего окна
        private LMain WindowLogic;
        // Окно регистрации
        internal RegisterWindow _RegisterWindow;
        // Главное меню после входа
        internal MainMenuWindow _MainMenuWindow;
        // Консоль разработчика
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        /// <summary>
        /// Конструктор главного окна.
        /// </summary>
        public MainWindow()
        {
#if DEBUG
            // Запускает консоль разработчика, если билд является DEBUG
            AllocConsole();
            Debug.Log("Запущена консоль разработчика", ConsoleColor.Green);
#endif
            InitializeComponent();
            EventsInitialize();

            ConnectService.Start();

            // Создание сущности логики главного окна.
            WindowLogic = new LMain(this);
            WindowLogic.NetworkEventsLoad();
            WindowLogic.AutomaticAuthorizate();

            ConnectService.ConnectBrokenEvents += 
            () => {
                Dispatcher.Invoke(() =>
                {
                    SelfUserDisconnected();
                    _MainMenuWindow.Close();
                });
            };

            NetworkDelegates.Add((MResponse ServerResponse) =>
            {
                SelfUserDisconnected();
            }, Dispatcher, -1, "User.Disconnected.Confirm");
        }

        private void SelfUserDisconnected()
        {
            UserInfo.NetworkSelf = null;
            UserInfo.NetworkUsers.Clear();

            Debug.LogWarning("Списки пользователей были системно очищены.");
        }

        /// <summary>
        /// Регистрация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            Button_Register.Click += Button_Register_Click;     // Вызывается при нажатии на кнопку регистрации
            Button_Login.Click += Button_Login_Click;           // Вызывается при нажатии на кнопку авторизации
        }

        /// <summary>
        /// Осуществляет вход в систему.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняет данные из поля логина и пароля
            WindowLogic.UserData = new string[]
            {
                TextBox_LoginOrEmail.Text,
                PasswordBox_Password.Password
            };

            WindowLogic.UserAuthorization(WindowLogic.UserData);
        }

        /// <summary>
        /// Создаёт окно регистрации и скрывает окно авторизации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            _RegisterWindow = new RegisterWindow(this);     // Создание окна регистрации
            this.Hide();                                    // Скрытие окна авторизации
            _RegisterWindow.Show();                         // Показ окна регистрации
        }

        /// <summary>
        /// Блокирует элементы авторизации.
        /// </summary>
        internal void AuthElements_Block()
        {
            CheckBox_AutoAuth.IsEnabled = false;            // Отключение галочки для автовхода
            TextBox_LoginOrEmail.IsEnabled = false;         // Отключение поля ввода Email / Логина
            PasswordBox_Password.IsEnabled = false;         // Отключение поля ввода пароля
            Button_Login.IsEnabled = false;                 // Отключение кнопки входа
            Button_Register.IsEnabled = false;              // Отключение кнопки регистрации
        }

        /// <summary>
        /// Снимает блокировку с элементов авторизации.
        /// </summary>
        internal void AuthElements_Unblock()
        {
            CheckBox_AutoAuth.IsEnabled = true;             // Включение галочки для автовхода
            TextBox_LoginOrEmail.IsEnabled = true;          // Включение поля ввода Email / Логина
            PasswordBox_Password.IsEnabled = true;          // Включение поля ввода пароля
            Button_Login.IsEnabled = true;                  // Включение кнопки входа
            Button_Register.IsEnabled = true;               // Включение кнопки регистрации
        }
    }
}
