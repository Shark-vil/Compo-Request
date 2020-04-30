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

namespace Compo_Request.Windows.Projects
{
    /// <summary>
    /// Логика взаимодействия для ProjectsWindow.xaml
    /// </summary>
    public partial class ProjectsWindow : Window
    {
        private MainWindow _MainWindow;

        public ProjectsWindow(MainWindow _MainWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainWindow);
            EventsInitialize();
        }

        public void LoadWindowParent(MainWindow mainWindow)
        {
            this._MainWindow = mainWindow;
        }

        /// <summary>
        /// Иницализация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            // Регистрация события при закрытии главного окна регистрации
            this.Closing += ProjectsWindow_Closing;
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
