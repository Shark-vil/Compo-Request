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
    /// Логика взаимодействия для TeamMainPage.xaml
    /// </summary>
    public partial class TeamMainPage : Page
    {
        internal MainMenuWindow _MainMenuWindow;
        internal TeamAddPage _TeamAddPage;

        internal List<WpfTeamGroup> TeamGroups = new List<WpfTeamGroup>();

        public TeamMainPage(MainMenuWindow _MainMenuWindow)
        {
            InitializeComponent();

            this._MainMenuWindow = _MainMenuWindow;

            _TeamAddPage = new TeamAddPage(this);

            Button_AddTeamMenuOpen.Click += Button_AddTeamMenuOpen_Click;

            DataGrid_Teams.ItemsSource = TeamGroups;

            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                var TeamGroups = Package.Unpacking<WpfTeamGroup[]>(ServerResponse.DataBytes);

                foreach (var TGroup in TeamGroups)
                    this.TeamGroups.Add(TGroup);

            }, Dispatcher, 4, "TeamGroup.GetAll");

            Sender.SendToServer("TeamGroup.GetAll", default, 4);
        }

        private void Button_AddTeamMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            _MainMenuWindow.WindowLogic.SetPage(_TeamAddPage);
        }

        private void ButtonClick_EditTeamGroup(object sender, RoutedEventArgs e)
        {
            WpfTeamGroup TGroup = (sender as Button).DataContext as WpfTeamGroup;

            Debug.Log("Редактирование");
            Debug.Log(TGroup.Title);
        }

        private void ButtonClick_DeleteTeamGroup(object sender, RoutedEventArgs e)
        {
            WpfTeamGroup TGroup = (sender as Button).DataContext as WpfTeamGroup;

            Debug.Log("Удаление");
            Debug.Log(TGroup.Title);
        }
    }
}
