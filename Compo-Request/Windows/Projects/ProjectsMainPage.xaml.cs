using Compo_Request.Network.Interfaces;
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
    /// Логика взаимодействия для ProjectsMainPage.xaml
    /// </summary>
    public partial class ProjectsMainPage : Page, ICustomPage
    {
        public ProjectsMainPage()
        {
            InitializeComponent();
        }

        public void ClosePage()
        {
            //
        }

        public void OpenPage()
        {
            //
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
    }
}
