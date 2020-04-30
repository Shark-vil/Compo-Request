using Compo_Request.Windows.Projects;
using Compo_Request.Windows.Teams;
using Compo_Shared_Data.Debugging;
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
using System.Windows.Shapes;

namespace Compo_Request.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProjectsWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        private MainWindow _MainWindow;
        private TeamMainPage _TeamMainPage;
        private ProjectsMainPage _ProjectsMainPage;

        protected Page CurrentPage;

        public MainMenuWindow(MainWindow _MainWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainWindow);
            EventsInitialize();
        }

        public void LoadWindowParent(MainWindow mainWindow)
        {
            this._MainWindow = mainWindow;

            _TeamMainPage = new TeamMainPage();
            _ProjectsMainPage = new ProjectsMainPage();
        }

        /// <summary>
        /// Иницализация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            // Регистрация события при закрытии главного окна регистрации
            this.Closing += ProjectsWindow_Closing;
            this.Button_OpenMenu.Click += Button_OpenMenu_Click;
            this.Button_CloseMenu.Click += Button_CloseMenu_Click;
            this.Button_Teams.Click += Button_Teams_Click;
            this.Button_Projects.Click += Button_Projects_Click;
        }

        private void Button_Projects_Click(object sender, RoutedEventArgs e)
        {
            Frame_Content.Content = _ProjectsMainPage;
        }

        private void Button_Teams_Click(object sender, RoutedEventArgs e)
        {
            Frame_Content.Content = _TeamMainPage;
        }

        private void Button_CloseMenu_Click(object sender, RoutedEventArgs e)
        {
            Button_OpenMenu.Visibility = Visibility.Visible;
            Button_CloseMenu.Visibility = Visibility.Collapsed;
        }

        private void Button_OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            Button_OpenMenu.Visibility = Visibility.Collapsed;
            Button_CloseMenu.Visibility = Visibility.Visible;
        }

        private void ProjectsWindow_CloseEvvent()
        {
            this.Close();
        }

        private void ProjectsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _MainWindow.Show();
        }
    }
}
