using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.WPF.Models;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для EditorWebRequestPage.xaml
    /// </summary>
    public partial class EditorWebRequestPage : Page, ICustomPage
    {
        private EditorMainMenuWindow _EditorMainMenuWindow;
        private Project MProject;

        private ObservableCollection<WTestTab> TestCollection = new ObservableCollection<WTestTab>();

        public EditorWebRequestPage(EditorMainMenuWindow _EditorMainMenuWindow, Project MProject)
        {
            InitializeComponent();

            this._EditorMainMenuWindow = _EditorMainMenuWindow;
            this.MProject = MProject;

            TabControl_Requests.NewItemFactory += TabFactory;

            TabControl_Requests.ItemsSource = TestCollection;

            CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                TestCollection.Add(new WTestTab
                {
                    Header = "Test"
                });
            }, new TimeSpan(0, 0, 3), false);
        }

        public Func<TabItem> TabFactory
        {
            get
            {
                return () =>
                {
                    return new TabItem()
                    {
                        Header = "Тест"
                    };
                };
            }
        }

        public void ClosePage()
        {
            //
        }

        public void OpenPage()
        {
            //
        }
    }
}
