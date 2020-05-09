﻿using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamEditPage.xaml
    /// </summary>
    public partial class TeamEditPage : Page
    {
        internal TeamMainPage _TeamMainPage;
        private WTeamGroup TGroup;

        public TeamEditPage(TeamMainPage _TeamMainPage, WTeamGroup TGroup)
        {
            InitializeComponent();
            LoadWindowParent(_TeamMainPage);
            EventsInitialize();

            this.TGroup = TGroup;

            TextBox_TeamName.Text = TGroup.Title;
            TextBox_TeamUid.Text = TGroup.TeamUid;
        }

        private void LoadWindowParent(TeamMainPage _TeamMainPage)
        {
            this._TeamMainPage = _TeamMainPage;
        }

        private void EventsInitialize()
        {
            Button_TeamUpdate.Click += Button_TeamUpdate_Click;
        }

        private void Button_TeamUpdate_Click(object sender, RoutedEventArgs e)
        {
            TGroup.TeamUid = TextBox_TeamUid.Text;
            TGroup.Title = TextBox_TeamName.Text;

            if (!Sender.SendToServer("TeamGroup.Update", TGroup))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
        }
    }
}
