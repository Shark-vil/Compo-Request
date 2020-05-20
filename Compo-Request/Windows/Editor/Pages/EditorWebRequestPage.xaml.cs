using Compo_Request.Models;
using Compo_Request.Network.Interfaces;
using Compo_Request.Windows.Editor.Windows;
using System.Windows.Controls;

namespace Compo_Request.Windows.Editor.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditorWebRequestPage.xaml
    /// </summary>
    public partial class EditorWebRequestPage : Page, ICustomPage
    {
        private EditorMainMenuWindow _EditorMainMenuWindow;
        //public BoundModel TabCollecton;

        public EditorWebRequestPage(EditorMainMenuWindow _EditorMainMenuWindow)
        {
            InitializeComponent();

            //TabCollecton = new BoundModel();
            ProjectData.TabCollecton = new BoundModel();
            DataContext = ProjectData.TabCollecton;

            ProjectData.TabCollecton.Items.Add(BoundNewItem.AddTab());

            this._EditorMainMenuWindow = _EditorMainMenuWindow;
            TabControl_Requests.SelectionChanged += TabControl_Requests_SelectionChanged;
        }

        private void TabControl_Requests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int Index = this.TabControl_Requests.SelectedIndex;

            if (Index != -1)
            {
               ProjectData.RequestDirectory = (
                    (EditorWebRequestControl)ProjectData.TabCollecton.Items[Index].Content
                ).RequestDirectory;

                //ProjectData.WebRequestParamsItems = (
                //    (EditorWebRequestControl)ProjectData.TabCollecton.Items[Index].Content
                //).WebRequestItems;
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
