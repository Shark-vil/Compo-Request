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
    /// Логика взаимодействия для ProjectTeamAddPage.xaml
    /// </summary>
    public partial class ProjectTeamAddPage : Page
    {
        public ProjectTeamAddPage()
        {
            InitializeComponent();

            Button_NextTeam.Click += Button_NextTeam_Click;
            Button_BeforeTeam.Click += Button_BeforeTeam_Click;
            Button_ProjectSave.Click += Button_ProjectSave_Click;
        }

        private void Button_ProjectSave_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_BeforeTeam_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_NextTeam_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
