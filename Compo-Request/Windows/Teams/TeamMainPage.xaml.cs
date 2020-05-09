using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamMainPage.xaml
    /// </summary>
    public partial class TeamMainPage : Page, ICustomPage
    {
        internal MainMenuWindow _MainMenuWindow;
        internal TeamAddPage _TeamAddPage;
        internal TeamUserAddPage _TeamUserAddPage;
        internal TeamEditPage _TeamEditPage;

        internal ObservableCollection<TeamGroup> TeamGroups = new ObservableCollection<TeamGroup>();
        internal TeamGroup[] DbTeamGroups;

        private DispatcherTimer ServerResponseDelay = null;

        public TeamMainPage(MainMenuWindow _MainMenuWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainMenuWindow);
            NetworkEvemtsLoad();
            EventsInitialize();

            DataGrid_Teams.ItemsSource = TeamGroups;
            Sender.SendToServer("TeamGroup.GetAll", default, 4);
        }

        private void NetworkEvemtsLoad()
        {
            /**
             * Получение данных списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                DbTeamGroups = Package.Unpacking<TeamGroup[]>(ServerResponse.DataBytes);

                foreach (var TGroup in DbTeamGroups)
                    this.TeamGroups.Add(TGroup);

            }, Dispatcher, 4, "TeamGroup.GetAll", "TeamMainPage");

            /**
             * Добавление элемента списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var TGroup = Package.Unpacking<TeamGroup>(ServerResponse.DataBytes);

                TeamGroups.Add(TGroup);

            }, Dispatcher, -1, "TeamGroup.Add.Confirm", "TeamMainPage");

            /**
             * Удаление элемента списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                var TGroup = Package.Unpacking<TeamGroup>(ServerResponse.DataBytes);

                TeamGroups.Remove(TeamGroups.SingleOrDefault(t => t.Id == TGroup.Id));

                DataGrid_Teams.IsEnabled = true;

                if (ServerResponseDelay != null)
                {
                    ServerResponseDelay.Stop();
                    ServerResponseDelay = null;
                }

            }, Dispatcher, -1, "TeamGroup.Delete.Confirm", "TeamMainPage");

            /**
             * Обновление списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var TGroup = Package.Unpacking<TeamGroup>(ServerResponse.DataBytes);
                var TGroupItem = TeamGroups.FirstOrDefault(x => x.Id == TGroup.Id);

                if (TGroupItem != null && TGroup != null)
                {
                    TGroupItem.Title = TGroup.Title;
                    TGroupItem.Uid = TGroup.Uid;

                    DataGridReload();
                }

            }, Dispatcher, -1, "TeamGroup.Update.Confirm", "TeamMainPage");
        }

        internal void DataGridReload()
        {
            DataGrid_Teams.Items.Refresh();
        }

        private void LoadWindowParent(MainMenuWindow _MainMenuWindow)
        {
            this._MainMenuWindow = _MainMenuWindow;

            _TeamAddPage = new TeamAddPage(this);
            _TeamUserAddPage = new TeamUserAddPage(this);
        }

        private void EventsInitialize()
        {
            Button_AddTeamMenuOpen.Click += Button_AddTeamMenuOpen_Click;
        }

        private void Button_AddTeamMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            _MainMenuWindow.WindowLogic.SetPage(_TeamAddPage);
        }

        private void ButtonClick_AddUserToTeam(object sender, RoutedEventArgs e)
        {
            TeamGroup TGroup = (sender as Button).DataContext as TeamGroup;
            TGroup = TeamGroups.FirstOrDefault(t => t.Uid == TGroup.Uid);

            _TeamUserAddPage.TGroup = TGroup;
            _MainMenuWindow.WindowLogic.SetPage(_TeamUserAddPage);
            _TeamUserAddPage.UpdateData();
        }

        private void ButtonClick_EditTeamGroup(object sender, RoutedEventArgs e)
        {
            TeamGroup TGroup = (sender as Button).DataContext as TeamGroup;
            TGroup = TeamGroups.FirstOrDefault(t => t.Uid == TGroup.Uid);

            _TeamEditPage = new TeamEditPage(this, TGroup);
            _MainMenuWindow.Frame_Main.Content = _TeamEditPage;
        }

        private void ButtonClick_DeleteTeamGroup(object sender, RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Вы уверены что хотите удалить элемент?", delegate ()
            {
                TeamGroup TGroup = (sender as Button).DataContext as TeamGroup;
                TGroup = TeamGroups.FirstOrDefault(t => t.Uid == TGroup.Uid);

                if (Sender.SendToServer("TeamGroup.Delete", TGroup))
                {
                    DataGrid_Teams.IsEnabled = false;

                    ServerResponseDelay = CustomTimer.Create(delegate (object sender, EventArgs e)
                    {
                        ServerResponseDelay = null;

                        new AlertWindow("Ошибка", "Время ожидания ответа от сервера истекло.",
                            () => DataGrid_Teams.IsEnabled = true);

                    }, new TimeSpan(0, 0, 5), true);
                }
            });
        }

        public void OpenPage()
        {
            //
        }

        public void ClosePage()
        {
            //
        }
    }
}
