using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
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
using System.Windows.Threading;

namespace Compo_Request.Windows.Projects
{
    /// <summary>
    /// Логика взаимодействия для ProjectsMainPage.xaml
    /// </summary>
    public partial class ProjectsMainPage : Page, ICustomPage
    {
        internal MainMenuWindow _MainMenuWindow;
        internal ProjectAddPage _ProjectAddPage;
        internal ProjectEditPage _ProjectEditPage;

        internal ObservableCollection<Project> Projects = new ObservableCollection<Project>();

        private DispatcherTimer ServerResponseDelay = null;

        public ProjectsMainPage(MainMenuWindow _MainMenuWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainMenuWindow);
            NetworkEvemtsLoad();

            DataGrid_Projects.ItemsSource = Projects;

            if (!Sender.SendToServer("Project.GetAll", default, 7))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
        }

        private void LoadWindowParent(MainMenuWindow _MainMenuWindow)
        {
            this._MainMenuWindow = _MainMenuWindow;
            _ProjectAddPage = new ProjectAddPage(this);
        }

        private void NetworkEvemtsLoad()
        {
            /**
             * Получение данных списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                var DbProjects = Package.Unpacking<Project[]>(ServerResponse.DataBytes);

                foreach (var MProject in DbProjects)
                    this.Projects.Add(MProject);

            }, Dispatcher, 7, "Project.GetAll", "ProjectsMainPage");

            /**
             * Добавление элемента списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var DbProject = Package.Unpacking<Project>(ServerResponse.DataBytes);

                Projects.Add(DbProject);

            }, Dispatcher, -1, "Project.Add.Confirm", "ProjectsMainPage");

            /**
             * Удаление элемента списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse) {

                var DbProject = Package.Unpacking<Project>(ServerResponse.DataBytes);

                Projects.Remove(Projects.SingleOrDefault(t => t.Id == DbProject.Id));

                DataGrid_Projects.IsEnabled = true;

                if (ServerResponseDelay != null)
                {
                    ServerResponseDelay.Stop();
                    ServerResponseDelay = null;
                }

            }, Dispatcher, -1, "Project.Delete.Confirm", "ProjectsMainPage");

            /**
             * Обновление списка
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var DbProject = Package.Unpacking<Project>(ServerResponse.DataBytes);
                var MProject = Projects.FirstOrDefault(x => x.Id == DbProject.Id);

                if (MProject != null)
                {
                    MProject.Title = DbProject.Title;
                    MProject.Uid = DbProject.Uid;

                    DataGrid_Projects.Items.Refresh();
                }

            }, Dispatcher, -1, "Project.Update.Confirm", "TeamMainPage");
        }

        private void ButtonClick_AddProjectMenuOpen(object sender, RoutedEventArgs e)
        {
            _MainMenuWindow.WindowLogic.SetPage(_ProjectAddPage);
        }

        private void ButtonClick_EditProject(object sender, RoutedEventArgs e)
        {
            Project MProject = (sender as Button).DataContext as Project;
            MProject = Projects.Where(p => p.Uid == MProject.Uid).FirstOrDefault();

            Debug.Log("Project update open:\n" +
                $"Id - {MProject.Id}\n" +
                $"Title - {MProject.Title}\n" +
                $"Uid - {MProject.Uid}\n" +
                $"UserId - {MProject.UserId}");

            _ProjectEditPage = new ProjectEditPage(this, MProject);
            _MainMenuWindow.Frame_Main.Content = _ProjectEditPage;
        }

        private void ButtonClick_DeleteProject(object sender, RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Вы уверены что хотите удалить элемент?", delegate ()
            {
                Project TGroup = (sender as Button).DataContext as Project;

                if (Sender.SendToServer("Project.Delete", TGroup))
                {
                    DataGrid_Projects.IsEnabled = false;

                    ServerResponseDelay = CustomTimer.Create(delegate (object sender, EventArgs e)
                    {
                        ServerResponseDelay = null;

                        new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer,
                            () => DataGrid_Projects.IsEnabled = true);

                    }, new TimeSpan(0, 0, 5), true);
                }
            });
        }

        private void ButtonClick_AddTeamToProject(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClick_OpenProject(object sender, RoutedEventArgs e)
        {

        }

        public void ClosePage()
        {
            //
        }

        public void OpenPage()
        {
            //
        }
    }
}
