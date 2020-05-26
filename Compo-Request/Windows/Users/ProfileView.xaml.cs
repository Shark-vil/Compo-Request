using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
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
    /// Логика взаимодействия для ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Page, ICustomPage
    {
        internal MainMenuWindow _MainMenuWindow;

        public ProfileView(MainMenuWindow _MainMenuWindow)
        {
            InitializeComponent();

            this._MainMenuWindow = _MainMenuWindow;
        }

        public void ClosePage()
        {
            //
        }

        public void OpenPage()
        {
            TextBox_Login.Text = UserInfo.NetworkSelf.Login;
            TextBox_Email.Text = UserInfo.NetworkSelf.Email;
            TextBox_Name.Text = UserInfo.NetworkSelf.Name;
            TextBox_Surname.Text = UserInfo.NetworkSelf.Surname;
            TextBox_Patronymic.Text = UserInfo.NetworkSelf.Patronymic;
        }
    }
}
