﻿using Compo_Request.Network.Client;
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
using System.Windows.Controls;
using System.Windows.Documents;

namespace Compo_Request.Windows.Teams
{
    /// <summary>
    /// Логика взаимодействия для TeamUserAddPage.xaml
    /// </summary>
    public partial class TeamUserAddPage : Page, ICustomPage
    {
        private TeamMainPage _TeamMainPage;
        internal TeamGroup TGroup;

        internal ObservableCollection<WUser> UsersNotTeam = new ObservableCollection<WUser>();
        internal ObservableCollection<WUser> UsersOnTeam = new ObservableCollection<WUser>();

        public TeamUserAddPage(TeamMainPage _TeamMainPage)
        {
            InitializeComponent();

            this._TeamMainPage = _TeamMainPage;

            //DataGrid_Users.ItemsSource = UsersNotTeam;
            //DataGrid_UsersOnTeam.ItemsSource = UsersOnTeam;

            Button_NextUser.Click += Button_NextUser_Click;
            Button_BeforeUser.Click += Button_BeforeUser_Click;
            Button_TeamSave.Click += Button_TeamSave_Click;
        }

        private void Button_TeamSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Вы уверены что хотите сохранить изменения?", delegate ()
            {
                var TeamUserTeamId = new WTeamUserTeamId
                {
                    TeamGroupId = TGroup.Id,
                    Users = UsersOnTeam.ToArray()
                };

                if (!Sender.SendToServer("TeamUser.Save", TeamUserTeamId))
                {
                    new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
                }
            });
        }

        private void Button_BeforeUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<WUser> RemoveUsers = new List<WUser>();

            foreach (var User in UsersOnTeam)
                if (User.IsSelected)
                {
                    UsersNotTeam.Add(User);
                    RemoveUsers.Add(User);
                }

            foreach (var User in RemoveUsers)
                UsersOnTeam.Remove(User);
        }

        private void Button_NextUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<WUser> RemoveUsers = new List<WUser>();

            foreach (var User in UsersNotTeam)
                if (User.IsSelected)
                {
                    UsersOnTeam.Add(User);
                    RemoveUsers.Add(User);
                }

            foreach (var User in RemoveUsers)
                UsersNotTeam.Remove(User);
        }

        public void OpenPage()
        {
            DataGrid_Users.ItemsSource = UsersNotTeam;
            DataGrid_UsersOnTeam.ItemsSource = UsersOnTeam;

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var TeamUsersCompilation = Package.Unpacking<WTeamUserCompilation>(ServerResponse.DataBytes);

                /*
                Debug.Log("TeamUsersCompilation.Users");
                TeamUsersCompilation.Users.ToList().ForEach(
                    i => Console.WriteLine(i.Login));

                Debug.Log("TeamUsersCompilation.TeamUsers");
                TeamUsersCompilation.TeamUsers.ToList().ForEach(
                    i => Console.WriteLine(i.UserId));
                */

                foreach (var User in TeamUsersCompilation.Users)
                    if (!Array.Exists(TeamUsersCompilation.TeamUsers, 
                        u => u.UserId == User.Id && u.TeamGroupId == TGroup.Id))
                    {
                        if (UsersNotTeam.Where(u => u.Id == User.Id).FirstOrDefault() == null)
                            UsersNotTeam.Add(User);
                    }
                    else
                    {
                        if (UsersOnTeam.Where(u => u.Id == User.Id).FirstOrDefault() == null)
                            UsersOnTeam.Add(User);
                    }

            }, Dispatcher, 6, "TeamUser.Get", "TeamUserAddPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (_TeamMainPage._MainMenuWindow.IsActive)
                    new AlertWindow("Оповещение", AlertWindow.AlertCode.UpdateConfirm);

            }, Dispatcher, -1, "TeamUser.Save.Confirm", "TeamUserAddPage");

            Sender.SendToServer("TeamUser.Get", TGroup, 6);
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("TeamUserAddPage");
        }
    }
}
