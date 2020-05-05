using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
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

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamAddPage.xaml
    /// </summary>
    public partial class TeamAddPage : Page
    {
        internal TeamMainPage _TeamMainPage;

        public TeamAddPage(TeamMainPage _TeamMainPage)
        {
            InitializeComponent();

            this._TeamMainPage = _TeamMainPage;

            Button_TeamAdd.Click += Button_TeamAdd_Click;
        }

        private void Button_TeamAdd_Click(object sender, RoutedEventArgs e)
        {
            var TeamGroupEntity = new TeamGroup();
            TeamGroupEntity.TeamUid = TextBox_TeamUid.Text;
            TeamGroupEntity.Title = TextBox_TeamName.Text;

            if (Sender.SendToServer("TeamGroup.Add", TeamGroupEntity))
            {
                NetworkDelegates.Add(delegate (MResponse ServerResponse)
                {
                    var TGroup = Package.Unpacking<WpfTeamGroup>(ServerResponse.DataBytes);

                    _TeamMainPage.TeamGroups.Add(TGroup);

                }, Dispatcher, -1, "TeamGroup.Add.Confirm", "TeamMainPage");
            }
            else
            {
                new AlertWindow("Ошибка", "Нихуя не вышло");
            }
        }
    }
}
