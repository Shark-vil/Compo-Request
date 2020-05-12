using Compo_Request.Models.Windows;
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

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorRequestSaverWindow.xaml
    /// </summary>
    public partial class EditorRequestSaverWindow : Window
    {
        private bool IsSaveClick = false;
        private EventEditorRequestSaver SaveClick;
        private EventEditorRequestSaver CancelClick;

        public delegate void EventEditorRequestSaver();
        public DynamicEditorRequestSaver DC_EditorRequestSaver;

        public EditorRequestSaverWindow(string RequestName, string RequestDirectoryName,
            EventEditorRequestSaver SaveClick = null, EventEditorRequestSaver CancelClick = null)
        {
            InitializeComponent();
            WindowActions();

            DC_EditorRequestSaver = new DynamicEditorRequestSaver(RequestName, RequestDirectoryName);
            DataContext = DC_EditorRequestSaver;

            this.SaveClick = SaveClick;
            this.CancelClick = CancelClick;
        }

        private void WindowActions()
        {
            this.Closed += EditorRequestSaverWindow_Closed;
            Button_Cancel.Click += Button_Cancel_Click;
            Button_Save.Click += Button_Save_Click;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            IsSaveClick = true;
            SaveClick?.Invoke();
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditorRequestSaverWindow_Closed(object sender, EventArgs e)
        {
            if (!IsSaveClick)
                CancelClick?.Invoke();
        }
    }
}
