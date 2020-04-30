using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Request.Network.Utilities.Validators;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Compo_Request.Windows.UserRegister
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private MainWindow _MainWindow;
        private DispatcherTimer RegFormUnblockTimer;

        public RegisterWindow(MainWindow _MainWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainWindow);
            EventsInitialize();
            LoadDelegates();
        }

        private void LoadDelegates()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                RegFormUnblockTimer_Destroy();

                var Alert = new AlertWindow("Успешная регистрация", "Вы были зарегистрированы в системе.",
                    RegisterWindow_CloseEvvent);

            }, Dispatcher, 2, "User.Register.Confirm", "RegisterWindow");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                RegFormUnblockTimer_Destroy();

                var Alert = new AlertWindow("Ошибка", "Такой пользователь уже существует!",
                    RegisterForm_Unblock);

            }, Dispatcher, 2, "User.Register.Error", "RegisterWindow");
        }

        private void UnloadDelegates()
        {
            NetworkDelegates.RemoveByUniqueName("RegisterWindow");
        }

        public void LoadWindowParent(MainWindow mainWindow)
        {
            this._MainWindow = mainWindow;
        }

        /// <summary>
        /// Иницализация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            // Регистрация события при закрытии главного окна регистрации
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
            RegisterForm_Block();

            User user = new User();
            user.Email = TextBox_Email.Text;
            user.Login = TextBox_Login.Text;
            user.Name = TextBox_Name.Text;
            user.Surname = TextBox_Surname.Text;
            user.Patronymic = TextBox_Patronymic.Text;

            if (user.Email.Length == 0 || !StringValid.IsValidEmail(user.Email))
            {
                var Alert = new AlertWindow("Ошибка", "Не верно задан формат почты!\r\nПример: MyMail@mail.com", 
                    RegisterForm_Unblock);
                return;
            }

            if (user.Login.Length >= 3)
            {
                if (!new Regex("^[a-z|A-Z|0-9|._-]+$").IsMatch(user.Login))
                {
                    var Alert = new AlertWindow("Ошибка", "Поле логина не может содержать любые " +
                        "символы кроме чисел и латинских букв!", 
                        RegisterForm_Unblock);
                    return;
                }
            }
            else
            {
                var Alert = new AlertWindow("Ошибка", "Логин должен содержать минимум 3 символа!", 
                    RegisterForm_Unblock);
                return;
            }

            if (PasswordBox_Password.Password.Length >= 6)
            {
                if (PasswordBox_Password.Password == PasswordBox_PasswordConfim.Password)
                    user.Password = PasswordBox_Password.Password;
                else
                {
                    var Alert = new AlertWindow("Ошибка", "Пароли не совпадают!",
                        RegisterForm_Unblock);
                    return;
                }
            }
            else
            {
                var Alert = new AlertWindow("Ошибка", "Пароль должен содержать минимум 6 символов!", 
                    RegisterForm_Unblock);
                return;
            }

            if (user.Name.Length == 0)
            {
                var Alert = new AlertWindow("Ошибка", "Поле имени не может быть пустым!",
                    RegisterForm_Unblock);
                return;
            }
            else if (new Regex("^[0-9]+$").IsMatch(user.Name))
            {
                var Alert = new AlertWindow("Ошибка", "Поле имени не может содержать в себе цифры!",
                    RegisterForm_Unblock);
                return;
            }

            if (user.Surname.Length == 0)
            {
                var Alert = new AlertWindow("Ошибка", "Поле отчества не может быть пустым!",
                    RegisterForm_Unblock);
                return;
            }
            else if (new Regex("^[0-9]+$").IsMatch(user.Surname))
            {
                var Alert = new AlertWindow("Ошибка", "Поле фамилии не может содержать в себе цифры!",
                    RegisterForm_Unblock);
                return;
            }

            if (new Regex("^[0-9]+$").IsMatch(user.Patronymic))
            {
                var Alert = new AlertWindow("Ошибка", "Поле отчества не может содержать в себе цифры!",
                    RegisterForm_Unblock);
                return;
            }

            if (Sender.SendToServer("User.Register", user, 2))
            {
                RegFormUnblockTimer = new DispatcherTimer();
                RegFormUnblockTimer.Tick += new EventHandler(RegisterForm_UnblockTimer);
                RegFormUnblockTimer.Interval = new TimeSpan(0, 0, 5);
                RegFormUnblockTimer.Start();
            }
            else
            {
                var Alert = new AlertWindow("Ошибка", "Не удалось соединиться с сервером.\n" +
                    "Возможно сервер выключен или присутствуют неполадки в вашем интернет-соединении.", 
                    RegisterForm_Unblock);
            }
        }

        private void RegisterForm_UnblockTimer(object sender, EventArgs e)
        {
            RegFormUnblockTimer_Destroy();

            var Alert = new AlertWindow("Ошибка", "Время ожидания ответа от сервера истекло.\n" +
                "Возможно введены некорректные данные.", 
                RegisterForm_Unblock);
        }

        private void RegFormUnblockTimer_Destroy()
        {
            if (RegFormUnblockTimer != null && RegFormUnblockTimer.IsEnabled)
                RegFormUnblockTimer.Stop();
        }

        private void RegisterForm_Block()
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

        private void RegisterForm_Unblock()
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

            RegFormUnblockTimer_Destroy();
        }

        private void RegisterWindow_CloseEvvent()
        {
            this.Close();
        }

        private void Button_BackToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow_CloseEvvent();
        }

        private void RegisterWindow_Closed(object sender, EventArgs e)
        {
            UnloadDelegates();

            _MainWindow.Show();
        }
    }
}
