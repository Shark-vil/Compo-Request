using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Models;
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
            //
        }

        private void Button_TeamAdd_Click(object sender, RoutedEventArgs e)
        {
            var TGroup = new WTeamGroup();
            TGroup.TeamUid = TextBox_TeamUid.Text;
            TGroup.Title = TextBox_TeamName.Text;

            if (!Sender.SendToServer("TeamGroup.Add", TGroup))
            {
                new AlertWindow("Ошибка", "Нихуя не вышло");
            }
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
