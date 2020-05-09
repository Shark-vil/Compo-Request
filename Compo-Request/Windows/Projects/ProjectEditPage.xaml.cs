using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
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

namespace Compo_Request.Windows.Projects
{
    /// <summary>
    /// Логика взаимодействия для ProjectEditPage.xaml
    /// </summary>
    public partial class ProjectEditPage : Page, ICustomPage
    {
        internal ProjectsMainPage _ProjectsMainPage;

        private Project MProject;

        public ProjectEditPage(ProjectsMainPage _ProjectsMainPage, Project MProject)
        {
            InitializeComponent();
            EventsInitialize();

            TextBox_ProjectName.Text = MProject.Title;
            TextBox_ProjectUid.Text = MProject.Uid;

            this._ProjectsMainPage = _ProjectsMainPage;
            this.MProject = MProject;
        }

        private void EventsInitialize()
        {
            Button_ProjectAdd.Click += Button_ProjectAdd_Click;
        }

        private void Button_ProjectAdd_Click(object sender, RoutedEventArgs e)
        {
            MProject.Uid = TextBox_ProjectUid.Text;
            MProject.Title = TextBox_ProjectName.Text;

            if (!Sender.SendToServer("Project.Update", MProject))
            {
                new AlertWindow("Ошибка", "Нихуя не вышло");
            }
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
