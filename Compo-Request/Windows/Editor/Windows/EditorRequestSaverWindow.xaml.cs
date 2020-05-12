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
        internal DynamicEditorRequestSaver DC_EditorRequestSaver;

        public EditorRequestSaverWindow(string RequestName, string RequestDirectoryName)
        {
            InitializeComponent();
            WindowActions();

            DC_EditorRequestSaver = new DynamicEditorRequestSaver(RequestName, RequestDirectoryName);

            DataContext = DC_EditorRequestSaver;
        }

        private void WindowActions()
        {
            Button_Cancel.Click += Button_Cancel_Click;
            Button_Save.Click += Button_Save_Click;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
