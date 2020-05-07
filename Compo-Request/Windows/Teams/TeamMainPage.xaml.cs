using Compo_Request.Network.Client;
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
using System.Threading;
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

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamMainPage.xaml
    /// </summary>
    public partial class TeamMainPage : Page
    {
        internal MainMenuWindow _MainMenuWindow;
        internal TeamAddPage _TeamAddPage;
        internal TeamEditPage _TeamEditPage;

        internal ObservableCollection<WpfTeamGroup> TeamGroups = new ObservableCollection<WpfTeamGroup>();

        public TeamMainPage(MainMenuWindow _MainMenuWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainMenuWindow);
            EventsInitialize();
            NetworkEventsLoad();

            DataGrid_Teams.ItemsSource = TeamGroups;
            Sender.SendToServer("TeamGroup.GetAll", default, 4);
        }

        internal void DataGridReload()
        {
            DataGrid_Teams.Items.Refresh();
        }

        private void LoadWindowParent(MainMenuWindow _MainMenuWindow)
        {
            this._MainMenuWindow = _MainMenuWindow;

            _TeamAddPage = new TeamAddPage(this);
        }

        private void EventsInitialize()
        {
            Button_AddTeamMenuOpen.Click += Button_AddTeamMenuOpen_Click;
        }

        private void NetworkEventsLoad()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                var TeamGroups = Package.Unpacking<WpfTeamGroup[]>(ServerResponse.DataBytes);

                foreach (var TGroup in TeamGroups)
                    this.TeamGroups.Add(TGroup);

            }, Dispatcher, 4, "TeamGroup.GetAll");

            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                var TGroup = Package.Unpacking<WpfTeamGroup>(ServerResponse.DataBytes);

                TeamGroups.Remove(TeamGroups.SingleOrDefault(t => t.Id == TGroup.Id));

                Debug.Log("Remove");

            }, Dispatcher, -1, "TeamGroup.Delete.Confirm");
        }

        private void Button_AddTeamMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            _MainMenuWindow.WindowLogic.SetPage(_TeamAddPage);
        }

        private void ButtonClick_EditTeamGroup(object sender, RoutedEventArgs e)
        {
            WpfTeamGroup TGroup = (sender as Button).DataContext as WpfTeamGroup;

            _TeamEditPage = new TeamEditPage(this, TGroup.Id, TGroup.Title, TGroup.TeamUid);
            _MainMenuWindow.Frame_Main.Content = _TeamEditPage;
        }

        private void ButtonClick_DeleteTeamGroup(object sender, RoutedEventArgs e)
        {
            WpfTeamGroup TGroup = (sender as Button).DataContext as WpfTeamGroup;

            if (Sender.SendToServer("TeamGroup.Delete", TGroup))
            {
                Debug.Log("Ok");
            }
        }
    }
}
