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

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamEditAccessPage.xaml
    /// </summary>
    public partial class TeamEditAccessPage : Page, ICustomPage
    {
        internal TeamMainPage _TeamMainPage;
        internal TeamGroup TGroup;
        internal ObservableCollection<WAccess> AccessCollecton = new ObservableCollection<WAccess>();

        public TeamEditAccessPage(TeamMainPage _TeamMainPage, TeamGroup TGroup)
        {
            InitializeComponent();
            LoadParentWindows(_TeamMainPage);
            NetworkEvents();
            EventsInitialize();

            this.TGroup = TGroup;

            DataGrid_Access.ItemsSource = AccessCollecton;
            DataGrid_Access.IsEnabled = false;

            Sender.SendToServer("Access.GetAll");
        }

        private void LoadParentWindows(TeamMainPage _TeamMainPage)
        {
            this._TeamMainPage = _TeamMainPage;
            _TeamMainPage._MainMenuWindow.WindowLogic.SetPage(this);
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

            var TeamPrivileges = new List<TeamPrivilege>();

            foreach (var Access in AccessCollecton)
            {
                if (!Access.IsSelected)
                    continue;

                TeamPrivileges.Add(new TeamPrivilege
                {
                    Privilege = Access.Key,
                    TeamGroupId = TGroup.Id
                });

                Debug.Log("Access - " + Access.Key + " : " + Access.IsSelected);
            }

            var MTeamPrivileges = new MTeamPrivilegeTransport
            {
                TeamGroupId = TGroup.Id,
                Privileges = TeamPrivileges.ToArray()
            };

            if (!Sender.SendToServer("Team.Access.Update", MTeamPrivileges))
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

                foreach (var Access in Accesses)
                {
                    AccessCollecton.Add(new WAccess
                    {
                        Key = Access.Key,
                        Description = Access.Description,
                        IsSelected = false
                    });
                }

                Sender.SendToServer("Team.Access.GetAll", TGroup.Id);

            }, Dispatcher, -1, "Access.GetAll.Confirm", "TeamEditAccessPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var Privileges = Package.Unpacking<TeamPrivilege[]>(ServerResponse.DataBytes);

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

            }, Dispatcher, -1, "Team.Access.GetAll.Confirm", "TeamEditAccessPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                new AlertWindow("Оповещение", AlertWindow.AlertCode.UpdateConfirm, () =>
                {
                    DataGrid_Access.IsEnabled = true;
                });

            }, Dispatcher, -1, "Team.Access.Update.Confirm", "TeamEditAccessPage");
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("TeamEditAccessPage");
        }

        public void OpenPage()
        {
            //
        }
    }
}
