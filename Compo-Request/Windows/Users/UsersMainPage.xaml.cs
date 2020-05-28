using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Windows.Threading;

namespace Compo_Request.Windows.Users
{
    /// <summary>
    /// Логика взаимодействия для UsersMainPage.xaml
    /// </summary>
    public partial class UsersMainPage : Page, ICustomPage
    {
        internal MainMenuWindow _MainMenuWindow;
        internal UsersEditAccessPage _UsersEditAccessPage;
        internal UserEditPage _UserEditPage;

        internal ObservableCollection<WUser> WUsers = new ObservableCollection<WUser>();
        internal DispatcherTimer ServerResponseDelay;

        public UsersMainPage(MainMenuWindow _MainMenuWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainMenuWindow);
            NetworkEvemtsLoad();
            EventsInitialize();

            DataGrid_Users.ItemsSource = WUsers;

            Sender.SendToServer("Users.GetAll");
        }

        private void EventsInitialize()
        {
            //
        }

        private void NetworkEvemtsLoad()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var DataUsers = Package.Unpacking<WUser[]>(ServerResponse.DataBytes);

                foreach (var DataUser in DataUsers)
                {
                    WUsers.Add(DataUser);
                }

            }, Dispatcher, -1, "Users.GetAll.Confirm", "UsersMainPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var UserId = Package.Unpacking<int>(ServerResponse.DataBytes);

                WUsers.Remove(WUsers.SingleOrDefault(t => t.Id == UserId));

                DataGrid_Users.IsEnabled = true;

                if (ServerResponseDelay != null)
                {
                    ServerResponseDelay.Stop();
                    ServerResponseDelay = null;
                }

                if (_MainMenuWindow.IsActive)
                    new AlertWindow("Оповещение", AlertWindow.AlertCode.DeleteConfirm);

            }, Dispatcher, -1, "Users.Delete.Confirm", "UsersMainPage");
        }

        private void LoadWindowParent(MainMenuWindow _MainMenuWindow)
        {
            this._MainMenuWindow = _MainMenuWindow;
        }

        private void ButtonClick_AccessEdit(object sender, RoutedEventArgs e)
        {
            WUser DataUser = (sender as Button).DataContext as WUser;

            _UsersEditAccessPage?.ClosePage();
            _UsersEditAccessPage = new UsersEditAccessPage(this, DataUser);
            _UsersEditAccessPage.OpenPage();
        }

        private void ButtonClick_UserEdit(object sender, RoutedEventArgs e)
        {
            WUser DataUser = (sender as Button).DataContext as WUser;

            _UserEditPage?.ClosePage();
            _UserEditPage = new UserEditPage(_MainMenuWindow, DataUser);
            _UserEditPage.OpenPage();

            _MainMenuWindow.WindowLogic.SetPage(_UserEditPage);
        }

        private void ButtonClick_UserDelete(object sender, RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Вы уверены что хотите удалить элемент?", delegate ()
            {
                WUser DataUser = (sender as Button).DataContext as WUser;

                if (Sender.SendToServer("Users.Delete", DataUser.Id))
                {
                    DataGrid_Users.IsEnabled = false;

                    ServerResponseDelay = CustomTimer.Create(delegate (object sender, EventArgs e)
                    {
                        ServerResponseDelay = null;

                        new AlertWindow("Ошибка", "Время ожидания ответа от сервера истекло.",
                            () => DataGrid_Users.IsEnabled = true);

                    }, new TimeSpan(0, 0, 5), true);
                }
                else
                {
                    new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
                }
            });
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("UsersMainPage");
        }

        public void OpenPage()
        {
            //
        }
    }
}
