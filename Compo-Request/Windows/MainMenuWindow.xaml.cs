using Compo_Request.Windows.Projects;
using Compo_Request.Windows.Teams;
using Compo_Request.WindowsLogic;
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
        // Основная логика текущего окна
        private LMainMenu WindowLogic;
        // Окно авторизации (Главное окно)
        internal MainWindow _MainWindow;
        // Главное окно команд
        internal TeamMainPage _TeamMainPage;
        // Главное окно проектов
        internal ProjectsMainPage _ProjectsMainPage;

        /// <summary>
        /// Конструктор главного окна меню
        /// </summary>
        /// <param name="_MainWindow">Окно авторизации</param>
        public MainMenuWindow(MainWindow _MainWindow)
        {
            InitializeComponent();
            LoadWindowParent(_MainWindow);
            EventsInitialize();

            WindowLogic = new LMainMenu(this);
        }

        /// <summary>
        /// Регистрирует зависимые окна.
        /// </summary>
        /// <param name="mainWindow">Окно авторизации</param>
        public void LoadWindowParent(MainWindow mainWindow)
        {
            this._MainWindow = mainWindow;

            _TeamMainPage = new TeamMainPage();
            _ProjectsMainPage = new ProjectsMainPage();
        }

        /// <summary>
        /// Регистрация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            this.Closing += MainMenuWindow_Closing;                 // Событие закрытия текущего окна
            this.Button_OpenMenu.Click += Button_OpenMenu_Click;    // Событие при разворачивании бокового меню
            this.Button_CloseMenu.Click += Button_CloseMenu_Click;  // Событие при сворачивании бокового меню
            this.Button_Teams.Click += Button_Teams_Click;          // Событие при нажатии на кнопку пункта меню "Команды"
            this.Button_Projects.Click += Button_Projects_Click;    // Событие при нажатии на кнопку пункта меню "Проекты"
        }

        /// <summary>
        /// Устанавливает в форму страницу проектов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Projects_Click(object sender, RoutedEventArgs e)
        {
            WindowLogic.SetPage(_ProjectsMainPage);
        }

        /// <summary>
        /// Устанавливает в форму страницу команд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Teams_Click(object sender, RoutedEventArgs e)
        {
            WindowLogic.SetPage(_TeamMainPage);
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

        /// <summary>
        /// Открывает окно авторизации и удаляет пользователя с сервера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _MainWindow.Show();
        }
    }
}
