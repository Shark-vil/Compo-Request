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
using System.Windows.Shapes;

namespace Compo_Request.Windows.Editor
{
    /// <summary>
    /// Логика взаимодействия для EditorMainMenuWindow.xaml
    /// </summary>
    public partial class EditorMainMenuWindow : Window
    {
        private MainMenuWindow _MainMenuWindow;
        private Project MProject;

        public EditorMainMenuWindow(MainMenuWindow _MainMenuWindow, Project MProject)
        {
            InitializeComponent();
            LoadWindowParent(_MainMenuWindow, MProject);
            EventsInitialize();
        }

        private void LoadWindowParent(MainMenuWindow _MainMenuWindow, Project MProject)
        {
            this.MProject = MProject;

            this._MainMenuWindow = _MainMenuWindow;
            _MainMenuWindow.Hide();
        }

        /// <summary>
        /// Регистрация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            this.Closing += EditorMainMenuWindow_Closing;           // Событие закрытия текущего окна
            this.Button_OpenMenu.Click += Button_OpenMenu_Click;    // Событие при разворачивании бокового меню
            this.Button_CloseMenu.Click += Button_CloseMenu_Click;  // Событие при сворачивании бокового меню
            this.Button_Back.Click += Button_Back_Click;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Меняет кнопку бокового меню на "Открыть".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CloseMenu_Click(object sender, RoutedEventArgs e)
        {
            Button_OpenMenu.Visibility = Visibility.Visible;
            Button_CloseMenu.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Меняет кнопку бокового меню на "Закрыть".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            Button_OpenMenu.Visibility = Visibility.Collapsed;
            Button_CloseMenu.Visibility = Visibility.Visible;
        }

        private void EditorMainMenuWindow_Closing(object sender, EventArgs e)
        {
            _MainMenuWindow.Show();
        }
    }
}
