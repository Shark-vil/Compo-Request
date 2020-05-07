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
    /// Логика взаимодействия для TeamEditPage.xaml
    /// </summary>
    public partial class TeamEditPage : Page
    {
        internal TeamMainPage _TeamMainPage;
        internal int Id;

        public TeamEditPage(TeamMainPage _TeamMainPage, int Id, string Title, string Uid)
        {
            InitializeComponent();
            LoadWindowParent(_TeamMainPage);
            EventsInitialize();
            NetworkEventsLoad();

            this.Id = Id;

            TextBox_TeamName.Text = Title;
            TextBox_TeamUid.Text = Uid;
        }

        private void LoadWindowParent(TeamMainPage _TeamMainPage)
        {
            this._TeamMainPage = _TeamMainPage;
        }

        private void EventsInitialize()
        {
            Button_TeamUpdate.Click += Button_TeamUpdate_Click;
        }

        private void NetworkEventsLoad()
        {
            //
        }

        private void Button_TeamUpdate_Click(object sender, RoutedEventArgs e)
        {
            var TGroup = new WpfTeamGroup();
            TGroup.Id = Id;
            TGroup.TeamUid = TextBox_TeamUid.Text;
            TGroup.Title = TextBox_TeamName.Text;

            if (Sender.SendToServer("TeamGroup.Update", TGroup))
            {
                Debug.Log("Отправка");
            }
            else
            {
                new AlertWindow("Ошибка", "Нихуя не вышло");
            }
        }
    }
}
