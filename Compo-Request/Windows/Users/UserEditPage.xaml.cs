using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Request.Network.Utilities.Validators;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compo_Request.Windows.Users
{
    /// <summary>
    /// Логика взаимодействия для UserEditPage.xaml
    /// </summary>
    public partial class UserEditPage : Page, ICustomPage
    {
        internal MainMenuWindow _MainMenuWindow;
        internal WUser User;

        public UserEditPage(MainMenuWindow _MainMenuWindow, WUser User)
        {
            InitializeComponent();
            LoadWindowParent(_MainMenuWindow);
            EventsInitialize();

            this.User = User;
            this.DataContext = this.User;
        }

        private void LoadWindowParent(MainMenuWindow _MainMenuWindow)
        {
            this._MainMenuWindow = _MainMenuWindow;
        }

        private void EventsInitialize()
        {
            Button_UserUpdate.Click += Button_UserUpdate_Click;
        }

        private void Button_UserUpdate_Click(object sender, RoutedEventArgs e)
        {
            User UserEntity = new User();
            UserEntity.Id = User.Id;
            UserEntity.Email = TextBox_Email.Text;
            UserEntity.Login = TextBox_Login.Text;
            UserEntity.Name = TextBox_Name.Text;
            UserEntity.Surname = TextBox_Surname.Text;
            UserEntity.Patronymic = TextBox_Patronymic.Text;

            if (!UserValid.Email(UserEntity.Email))
                return;

            if (!UserValid.Login(UserEntity.Login))
                return;

            if (PasswordBox_Password.Password.Length != 0 && PasswordBox_Password.Password.Trim() != string.Empty)
                if (!UserValid.Password(PasswordBox_Password.Password, PasswordBox_ConfirmPassword.Password))
                    return;
                else
                    UserEntity.Password = PasswordBox_Password.Password;

            if (!UserValid.Name(UserEntity.Name))
                return;

            if (!UserValid.Surname(UserEntity.Surname))
                return;

            if (!UserValid.Patronymic(UserEntity.Patronymic))
                return;

            if (!Sender.SendToServer("Users.Update", UserEntity))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("UserEditPage");
        }

        public void OpenPage()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                new AlertWindow("Оповещение", AlertWindow.AlertCode.UpdateConfirm);

            }, Dispatcher, -1, "Users.Update.Confirm", "UserEditPage");
        }
    }
}
