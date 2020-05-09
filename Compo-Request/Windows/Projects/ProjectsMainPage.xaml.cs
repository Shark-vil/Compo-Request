using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
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

namespace Compo_Request.Windows.Projects
{
    /// <summary>
    /// Логика взаимодействия для ProjectsMainPage.xaml
    /// </summary>
    public partial class ProjectsMainPage : Page, ICustomPage
    {
        internal MainMenuWindow _MainMenuWindow;
        internal ProjectAddPage _ProjectAddPage;

        internal ObservableCollection<Project> Projects = new ObservableCollection<Project>();

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
        }

        private void ButtonClick_AddProjectMenuOpen(object sender, RoutedEventArgs e)
        {
            _MainMenuWindow.WindowLogic.SetPage(_ProjectAddPage);
        }

        private void ButtonClick_EditProject(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClick_DeleteProject(object sender, RoutedEventArgs e)
        {

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
