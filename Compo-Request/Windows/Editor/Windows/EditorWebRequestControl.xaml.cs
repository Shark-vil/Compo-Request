using Compo_Request.Models;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Dragablz;
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

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorWebRequestControl.xaml
    /// </summary>
    public partial class EditorWebRequestControl : UserControl
    {
        private HeaderedItemViewModel TabItemView { get; set; }
        private string HeaderName { get; set; }
        public DynamicModelEditorRequest EditorRequestData { get; set; }

        public EditorWebRequestControl()
        {
            InitializeComponent();

            EditorRequestData = new DynamicModelEditorRequest();
            DataContext = EditorRequestData;

            ComboBox_RequestType.SelectionChanged += ComboBox_RequestType_SelectionChanged;
        }

        private void ComboBox_RequestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetHeaderName();
        }

        public void Construct(HeaderedItemViewModel TabItemView)
        {
            this.TabItemView = TabItemView;
            HeaderName = TabItemView.Header.ToString();

            CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                SetHeaderName();
            }, new TimeSpan(0, 0, 1));
        }

        private void SetHeaderName()
        {
            TabItemView.Header = HeaderName + " - " + ComboBox_RequestType.SelectedItem.ToString();
        }
    }
}
