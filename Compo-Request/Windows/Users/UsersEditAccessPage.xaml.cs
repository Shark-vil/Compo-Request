using Compo_Request.Models;
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

namespace Compo_Request.Windows.Users
{
    /// <summary>
    /// Логика взаимодействия для UsersEditAccessPage.xaml
    /// </summary>
    public partial class UsersEditAccessPage : Page, ICustomPage
    {
        internal UsersMainPage _UsersMainPage;
        internal WUser DataUser;
        internal ObservableCollection<WAccess> AccessCollecton = new ObservableCollection<WAccess>();

        public UsersEditAccessPage(UsersMainPage _UsersMainPage, WUser DataUser)
        {
            InitializeComponent();
            LoadParentWindows(_UsersMainPage);
            EventsInitialize();
            NetworkEvents();

            this.DataUser = DataUser;

            DataGrid_Access.ItemsSource = AccessCollecton;
            DataGrid_Access.IsEnabled = false;

            Sender.SendToServer("Access.GetAll");
        }

        private void LoadParentWindows(UsersMainPage _UsersMainPage)
        {
            this._UsersMainPage = _UsersMainPage;
            _UsersMainPage._MainMenuWindow.WindowLogic.SetPage(this);
        }

        private void EventsInitialize()
        {
            Button_SaveAccess.Click += Button_SaveAccess_Click;
        }

        private void Button_SaveAccess_Click(object sender, RoutedEventArgs e)
        {
            if (!DataGrid_Access.IsEnabled)
                return;

            DataGrid_Access.IsEnabled = false;

            var UserPrivileges = new List<UserPrivilege>();

            foreach (var Access in AccessCollecton)
            {
                if (!Access.IsSelected)
                    continue;

                UserPrivileges.Add(new UserPrivilege
                {
                    Privilege = Access.Key,
                    UserId = DataUser.Id
                });

                Debug.Log("Access - " + Access.Key + " : " + Access.IsSelected);
            }

            var MUserPrivileges = new MUserPrivilegeTransport
            {
                UserId = DataUser.Id,
                Privileges = UserPrivileges.ToArray()
            };

            if (!Sender.SendToServer("User.Access.Update", MUserPrivileges))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer, () =>
                {
                    DataGrid_Access.IsEnabled = true;
                });
            }
        }

        private void NetworkEvents()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var Accesses = Package.Unpacking<MAccess[]>(ServerResponse.DataBytes);

                foreach(var Access in Accesses)
                {
                    AccessCollecton.Add(new WAccess
                    {
                        Key = Access.Key,
                        Description = Access.Description,
                        IsSelected = false
                    });
                }

                Sender.SendToServer("User.Access.GetAll", DataUser.Id);

            }, Dispatcher, -1, "Access.GetAll.Confirm", "UsersEditAccessPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var Privileges = Package.Unpacking<UserPrivilege[]>(ServerResponse.DataBytes);

                if (DataUser.Id == 1)
                {
                    for (int i = 0; i < AccessCollecton.Count; i++)
                    {
                        WAccess Access = AccessCollecton[i];
                        if (Access.Key == "admin")
                        {
                            Access.IsSelected = true;
                            break;
                        }
                    }
                }

                
                for (int i = 0; i < AccessCollecton.Count; i++)
                {
                    WAccess Access = AccessCollecton[i];

                    foreach (var DbAccess in Privileges)
                        if (Access.Key == DbAccess.Privilege)
                        {
                            Access.IsSelected = true;
                            break;
                        }
                }

                DataGrid_Access.Items.Refresh();
                DataGrid_Access.IsEnabled = true;

            }, Dispatcher, -1, "User.Access.GetAll.Confirm", "UsersEditAccessPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                DataGrid_Access.IsEnabled = true;

            }, Dispatcher, -1, "User.Access.Update.Confirm", "UsersEditAccessPage");
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("UsersEditAccessPage");
        }

        public void OpenPage()
        {
            //
        }
    }
}
