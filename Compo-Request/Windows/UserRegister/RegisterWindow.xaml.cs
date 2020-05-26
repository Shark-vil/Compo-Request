using Compo_Request.Network.Utilities.Validators;
using Compo_Request.WindowsLogic.UserRegister;
using Compo_Shared_Data.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace Compo_Request.Windows.UserRegister
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private MainWindow _MainWindow;
        internal LRegister WindowLogic;

        public RegisterWindow(MainWindow _MainWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainWindow);
            EventsInitialize();

            WindowLogic = new LRegister(this);
            WindowLogic.NetworkEventsLoad();
        }

        public void LoadWindowParent(MainWindow mainWindow)
        {
            this._MainWindow = mainWindow;
        }

        /// <summary>
        /// Регистрация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            this.Closing += RegisterWindow_Closed;
            this.Button_BackToMainWindow.Click += Button_BackToMainWindow_Click;
            this.Button_Register.Click += Button_Register_Click;
            this.Button_NextToPage_2.Click += Button_NextToPage_2_Click;
            this.Button_BackToPage_1.Click += Button_BackToPage_1_Click;
        }

        private void Button_BackToPage_1_Click(object sender, RoutedEventArgs e)
        {
            StackPanel_Page_1.Visibility = Visibility.Visible;
            StackPanel_Page_2.Visibility = Visibility.Hidden;
        }

        private void Button_NextToPage_2_Click(object sender, RoutedEventArgs e)
        {
            StackPanel_Page_1.Visibility = Visibility.Hidden;
            StackPanel_Page_2.Visibility = Visibility.Visible;
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterElements_Block();

            User UserEntity = new User();
            UserEntity.Email = TextBox_Email.Text;
            UserEntity.Login = TextBox_Login.Text;
            UserEntity.Name = TextBox_Name.Text;
            UserEntity.Surname = TextBox_Surname.Text;
            UserEntity.Patronymic = TextBox_Patronymic.Text;

            if (!UserValid.Email(UserEntity.Email, RegisterElements_Unblock))
                return;

            if (!UserValid.Login(UserEntity.Login, RegisterElements_Unblock))
                return;

            if (!UserValid.Password(PasswordBox_Password.Password, PasswordBox_PasswordConfim.Password, RegisterElements_Unblock))
                return;
            else
                UserEntity.Password = PasswordBox_Password.Password;

            if (!UserValid.Name(UserEntity.Name, RegisterElements_Unblock))
                return;

            if (!UserValid.Surname(UserEntity.Surname, RegisterElements_Unblock))
                return;

            if (!UserValid.Patronymic(UserEntity.Patronymic, RegisterElements_Unblock))
                return;

            WindowLogic.RegisterAccount(UserEntity);
        }

        /// <summary>
        /// Блокирует элементы регистрации.
        /// </summary>
        internal void RegisterElements_Block()
        {
            Button_BackToMainWindow.IsEnabled = false;
            Button_Register.IsEnabled = false;
            TextBox_Email.IsEnabled = false;
            TextBox_Login.IsEnabled = false;
            PasswordBox_Password.IsEnabled = false;
            PasswordBox_PasswordConfim.IsEnabled = false;
            TextBox_Name.IsEnabled = false;
            TextBox_Surname.IsEnabled = false;
            TextBox_Patronymic.IsEnabled = false;
        }

        /// <summary>
        /// Снимает блокировку с элементов регистрации.
        /// </summary>
        internal void RegisterElements_Unblock()
        {
            Button_BackToMainWindow.IsEnabled = true;
            Button_Register.IsEnabled = true;
            TextBox_Email.IsEnabled = true;
            TextBox_Login.IsEnabled = true;
            PasswordBox_Password.IsEnabled = true;
            PasswordBox_PasswordConfim.IsEnabled = true;
            TextBox_Name.IsEnabled = true;
            TextBox_Surname.IsEnabled = true;
            TextBox_Patronymic.IsEnabled = true;
        }

        internal void RegisterWindow_CloseEvvent()
        {
            this.Close();
        }

        private void Button_BackToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow_CloseEvvent();
        }

        private void RegisterWindow_Closed(object sender, EventArgs e)
        {
            WindowLogic.NetworkEventsUnload();

            _MainWindow.Show();
        }
    }
}
