using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Request.Properties;
using Compo_Request.Windows.Projects;
using Compo_Request.Windows.Teams;
using Compo_Request.Windows.Users;
using Compo_Request.WindowsLogic;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network.Models;
using System.IO;
using System.Windows;

namespace Compo_Request.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProjectsWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        // Основная логика текущего окна
        internal LMainMenu WindowLogic;
        // Окно авторизации (Главное окно)
        internal MainWindow _MainWindow;
        // Главное окно команд
        internal TeamMainPage _TeamMainPage;
        // Главное окно проектов
        internal ProjectsMainPage _ProjectsMainPage;
        // Окно пользователей
        internal UsersMainPage _UsersMainPage;
        // Профиль пользователя
        internal ProfileView _ProfileView;
        internal UserEditPage _UserEditPage;

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

            //_TeamMainPage = new TeamMainPage(this);
            //_ProjectsMainPage = new ProjectsMainPage(this);
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
            this.Button_Users.Click += Button_Users_Click;
            this.Button_Exit.Click += Button_Exit_Click;
            this.Button_Profile.Click += Button_Profile_Click;
            this.Button_Settings.Click += Button_Settings_Click;
            this.Button_Help.Click += Button_Help_Click;
        }

        private void Button_Help_Click(object sender, RoutedEventArgs e)
        {
            string FilePath = Directory.GetCurrentDirectory() + "/manual.pdf";

            if (!File.Exists(FilePath))
                using (FileStream fs = File.Create(FilePath))
                    fs.Write(Properties.Resources.manual, 0, Properties.Resources.manual.Length);  

            var p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo(FilePath)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            _UserEditPage?.ClosePage();
            _UserEditPage = new UserEditPage(this, UserInfo.MUserToWUser(UserInfo.NetworkSelf));

            WindowLogic.SetPage(_UserEditPage);
        }

        private void Button_Profile_Click(object sender, RoutedEventArgs e)
        {
            _ProfileView?.ClosePage();
            _ProfileView = new ProfileView(this);

            WindowLogic.SetPage(_ProfileView);
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Users_Click(object sender, RoutedEventArgs e)
        {
            _UsersMainPage?.ClosePage();
            _UsersMainPage = new UsersMainPage(this);

            WindowLogic.SetPage(_UsersMainPage);
        }

        /// <summary>
        /// Устанавливает в форму страницу проектов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Projects_Click(object sender, RoutedEventArgs e)
        {
            _ProjectsMainPage?.ClosePage();
            _ProjectsMainPage = new ProjectsMainPage(this);

            WindowLogic.SetPage(_ProjectsMainPage);
        }

        /// <summary>
        /// Устанавливает в форму страницу команд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Teams_Click(object sender, RoutedEventArgs e)
        {
            _TeamMainPage?.ClosePage();
            _TeamMainPage = new TeamMainPage(this);

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
            Sender.SendToServer("User.Disconnected");
            _MainWindow.Show();
        }
    }
}
