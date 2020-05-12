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
    /// Логика взаимодействия для ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        // Шаблон делегата
        public delegate void CloseEventDelegate();
        // Событие выполняющееся после закрытия окна
        private CloseEventDelegate YesEvent;
        //
        private CloseEventDelegate NotEvent;

        private bool IsYes = false;

        public ConfirmWindow(string WindowName, string Message,
            CloseEventDelegate YesEvent = null, CloseEventDelegate NotEvent = null)
        {
            InitializeComponent();
            EventsInitialize();

            this.YesEvent = YesEvent;
            this.NotEvent = NotEvent;

            this.Title = WindowName;
            TextBlock_Message.Text = Message;

            this.Show();
        }

        /// <summary>
        /// Регистрация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            this.Button_Yes.Click += Button_Yes_Click;
            this.Button_Not.Click += Button_Not_Click;
            this.Closing += ConfirmWindow_Closing;
        }

        private void Button_Not_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            IsYes = true;

            YesEvent?.Invoke();

            this.Close();
        }

        /// <summary>
        /// Вызывается при закрытии окна оповещения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsYes)
                NotEvent?.Invoke();
        }
    }
}
