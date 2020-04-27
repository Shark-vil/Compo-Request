using Grpc.Net.Client;
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

namespace Compo_Request.Windows.UserRegister
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private MainWindow mainWindow;

        public RegisterWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadWindowParent(mainWindow);
            EventsInitialize();
        }

        public void LoadWindowParent(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        /// <summary>
        /// Иницализация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            // Регистрация события при закрытии главного окна регистрации
            this.Closing += RegisterWindow_Closed;
            this.Button_BackToMainWindow.Click += Button_BackToMainWindow_Click;
        }

        private void Button_BackToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterWindow_Closed(object sender, EventArgs e)
        {
            if (!IsVisible)
                return;

            mainWindow.LoadWindowParent();
            mainWindow.Show();
        }
    }
}
