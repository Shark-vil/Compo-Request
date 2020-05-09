using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
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
    /// Логика взаимодействия для ProjectAddPage.xaml
    /// </summary>
    public partial class ProjectAddPage : Page, ICustomPage
    {
        internal ProjectsMainPage _ProjectsMainPage;

        public ProjectAddPage(ProjectsMainPage _ProjectsMainPage)
        {
            InitializeComponent();
            EventsInitialize();

            this._ProjectsMainPage = _ProjectsMainPage;
        }

        private void EventsInitialize()
        {
            Button_ProjectAdd.Click += Button_ProjectAdd_Click;
        }

        private void Button_ProjectAdd_Click(object sender, RoutedEventArgs e)
        {
            var MProject = new Project();
            MProject.Uid = TextBox_ProjectUid.Text;
            MProject.Title = TextBox_ProjectName.Text;

            if (!Sender.SendToServer("Project.Add", MProject))
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
