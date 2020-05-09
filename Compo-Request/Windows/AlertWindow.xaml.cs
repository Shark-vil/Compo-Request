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
    /// Логика взаимодействия для AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        // Шаблон делегата
        public delegate void CloseEventDelegate();
        // Событие выполняющееся после закрытия окна
        private CloseEventDelegate CloseEvent;

        public enum AlertCode
        {
            SendToServer = 0,
        }

        /// <summary>
        /// Конструктор оповещения.
        /// </summary>
        /// <param name="WindowName">Название окна</param>
        /// <param name="Message">Текст оповещения</param>
        /// <param name="CloseEvent">Событие при закрытии окна</param>
        public AlertWindow(string WindowName, string Message, CloseEventDelegate CloseEvent = null)
        {
            InitializeComponent();
            EventsInitialize();

            this.CloseEvent = CloseEvent;

            this.Title = WindowName;
            TextBlock_Message.Text = Message;

            this.Show();
        }

        public AlertWindow(string WindowName, AlertCode NAlertCode, CloseEventDelegate CloseEvent = null)
        {
            InitializeComponent();
            EventsInitialize();

            this.CloseEvent = CloseEvent;

            this.Title = WindowName;


            if (NAlertCode == AlertCode.SendToServer)
                TextBlock_Message.Text = "Не удалось установить соединение с сервером. " +
                    "Возможно сервер выключен, или у вас проблемы с интернет-соединением.";

            this.Show();
        }

        /// <summary>
        /// Регистрация событий элементов.
        /// </summary>
        private void EventsInitialize()
        {
            this.Button_Close.Click += Button_Close_Click;  // Событие зарытия окна по нажатию кнопки.
            this.Closing += AlertWindow_Closing;
        }

        /// <summary>
        /// Вызывается при закрытии окна оповещения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseEvent?.Invoke();
        }

        /// <summary>
        /// Закрывает окно оповещения и вызывает событие при его наличии.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
