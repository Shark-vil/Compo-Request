using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamAddPage.xaml
    /// </summary>
    public partial class TeamAddPage : Page, ICustomPage
    {
        internal TeamMainPage _TeamMainPage;

        public TeamAddPage(TeamMainPage _TeamMainPage)
        {
            InitializeComponent();
            LoadWindowParent(_TeamMainPage);
            EventsInitialize();
            NetworkEventsLoad();
        }

        private void LoadWindowParent(TeamMainPage _TeamMainPage)
        {
            this._TeamMainPage = _TeamMainPage;
        }

        private void EventsInitialize()
        {
            Button_TeamAdd.Click += Button_TeamAdd_Click;
        }

        private void NetworkEventsLoad()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var MTeamGroup = Package.Unpacking<TeamGroup>(ServerResponse.DataBytes);

                if (MTeamGroup.Uid == TextBox_TeamUid.Text)
                {
                    new AlertWindow("Оповещение", AlertWindow.AlertCode.AddConfirm, () =>
                    {
                        TextBox_TeamUid.Text = string.Empty;
                        TextBox_TeamName.Text = string.Empty;

                        TextBox_TeamUid.IsEnabled = true;
                        TextBox_TeamName.IsEnabled = true;
                    });
                }

            }, Dispatcher, -1, "TeamGroup.Add.Confirm", "TeamAddPage");
        }

        private void Button_TeamAdd_Click(object sender, RoutedEventArgs e)
        {
            var TGroup = new TeamGroup();
            TGroup.Uid = TextBox_TeamUid.Text;
            TGroup.Title = TextBox_TeamName.Text;

            if (!Sender.SendToServer("TeamGroup.Add", TGroup))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
            else
            {
                TextBox_TeamUid.IsEnabled = false;
                TextBox_TeamName.IsEnabled = false;
            }
        }

        public void OpenPage()
        {
            //
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("TeamAddPage");
        }
    }
}
