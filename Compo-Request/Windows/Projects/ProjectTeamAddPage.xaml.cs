using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Models.NotDatabase;
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

namespace Compo_Request.Windows.Projects
{
    /// <summary>
    /// Логика взаимодействия для ProjectTeamAddPage.xaml
    /// </summary>
    public partial class ProjectTeamAddPage : Page, ICustomPage
    {
        internal Project MProject;

        internal ObservableCollection<WTeamGroup> NotTeamGroup = new ObservableCollection<WTeamGroup>();
        internal ObservableCollection<WTeamGroup> OnTeamGroup = new ObservableCollection<WTeamGroup>();

        public ProjectTeamAddPage(Project MProject)
        {
            InitializeComponent();

            this.MProject = MProject;

            Button_NextTeam.Click += Button_NextTeam_Click;
            Button_BeforeTeam.Click += Button_BeforeTeam_Click;
            Button_ProjectSave.Click += Button_ProjectSave_Click;
        }

        private void Button_ProjectSave_Click(object sender, RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Вы уверены что хотите сохранить изменения?", delegate ()
            {
                var TeamGroupProjectId = new WTeamGroupProjectId
                {
                    ProjectId = MProject.Id,
                    TeamGroups = OnTeamGroup.ToArray()
                };

                if (!Sender.SendToServer("TeamProject.Save", TeamGroupProjectId))
                {
                    new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
                }
            });
        }

        private void Button_BeforeTeam_Click(object sender, RoutedEventArgs e)
        {
            var RemoveTeams = new List<WTeamGroup>();

            foreach (var TeamGroup in OnTeamGroup)
                if (TeamGroup.IsSelected)
                {
                    TeamGroup.ProjectId = MProject.Id;
                    NotTeamGroup.Add(TeamGroup);
                    RemoveTeams.Add(TeamGroup);
                }

            foreach (var TeamGroup in RemoveTeams)
                OnTeamGroup.Remove(TeamGroup);
        }

        private void Button_NextTeam_Click(object sender, RoutedEventArgs e)
        {
            var RemoveTeams = new List<WTeamGroup>();

            foreach (var TeamGroup in NotTeamGroup)
                if (TeamGroup.IsSelected)
                {
                    TeamGroup.ProjectId = MProject.Id;
                    OnTeamGroup.Add(TeamGroup);
                    RemoveTeams.Add(TeamGroup);
                }

            foreach (var TeamGroup in RemoveTeams)
                NotTeamGroup.Remove(TeamGroup);
        }

        public void OpenPage()
        {
            DataGrid_Teams.ItemsSource = NotTeamGroup;
            DataGrid_TeamsOnProject.ItemsSource = OnTeamGroup;

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var MTeamGroupCompilation = Package.Unpacking<WTeamGroupCompilation>(ServerResponse.DataBytes);

                Debug.Log($"MProject info: \n" +
                    $"Id - {MProject.Id}\n" +
                    $"Title - {MProject.Title}\n" +
                    $"Uid - {MProject.Uid}");

                Debug.Log("TeamUsersCompilation.TeamGroups");
                MTeamGroupCompilation.WTeamGroups.ToList().ForEach(
                    i => Console.WriteLine($"Title - {i.Title} : Uid - {i.Uid}"));

                Debug.Log("TeamUsersCompilation.TeamProjects");
                MTeamGroupCompilation.WTeamProjects.ToList().ForEach(
                    i => Console.WriteLine($"TeamGroupId - {i.TeamGroupId} : ProjectId - {i.ProjectId}"));

                foreach (var TeamGroup in MTeamGroupCompilation.WTeamGroups)
                    if (!Array.Exists(MTeamGroupCompilation.WTeamProjects,
                        p => p.TeamGroupId == TeamGroup.Id && p.ProjectId == MProject.Id))
                    {
                        if (NotTeamGroup.Where(p => p.Id == TeamGroup.Id).FirstOrDefault() == null)
                            NotTeamGroup.Add(TeamGroup);
                    }
                    else
                    {
                        if (OnTeamGroup.Where(p => p.Id == TeamGroup.Id).FirstOrDefault() == null)
                            OnTeamGroup.Add(TeamGroup);
                    }

            }, Dispatcher, 8, "TeamProject.Get", "ProjectTeamAddPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                new AlertWindow("Оповещение", AlertWindow.AlertCode.UpdateConfirm);
            }, Dispatcher, -1, "TeamProject.Save.Confirm", "ProjectTeamAddPage");

            Sender.SendToServer("TeamProject.Get", MProject, 8);
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("ProjectTeamAddPage");
        }
    }
}
