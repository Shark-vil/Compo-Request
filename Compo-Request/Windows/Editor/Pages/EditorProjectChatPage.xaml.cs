using Compo_Request.Network.Interfaces;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compo_Request.Windows.Editor.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditorProjectChatPage.xaml
    /// </summary>
    public partial class EditorProjectChatPage : Page, ICustomPage
    {
        EditorMainMenuWindow _EditorMainMenuWindow;

        public EditorProjectChatPage(EditorMainMenuWindow _EditorMainMenuWindow)
        {
            InitializeComponent();
            this._EditorMainMenuWindow = _EditorMainMenuWindow;
        }

        public void ClosePage()
        {
            throw new NotImplementedException();
        }

        public void OpenPage()
        {
            throw new NotImplementedException();
        }
    }
}
