using Compo_Request.Models;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.WPF.Models;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private int UniqueFormIndex { get; set; }
        private ObservableCollection<ModelFormRequest> FormRequestsData = new ObservableCollection<ModelFormRequest>();
        private HeaderedItemViewModel TabItemView { get; set; }
        private string HeaderName { get; set; }
        private string RequestLink { get; set; }
        public DynamicModelEditorRequest EditorRequestData { get; set; }

        public EditorWebRequestControl()
        {
            InitializeComponent();

            EditorRequestData = new DynamicModelEditorRequest();
            DataContext = EditorRequestData;

            DataGrid_FormRequestData.ItemsSource = FormRequestsData;
            DataGrid_FormRequestData.Columns[0].Visibility = Visibility.Hidden;

            DataGrid_FormRequestData.AddingNewItem += DataGrid_FormRequestData_AddingNewItem;
            ComboBox_RequestType.SelectionChanged += ComboBox_RequestType_SelectionChanged;
            FormRequestsData.CollectionChanged += FormRequestsData_CollectionChanged;
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

        private void FormRequestsData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RequestLinkChanged();
        }

        /// <summary>
        /// Проверяет текущий выбранный метод, и перестраивает ссылку если это GET.
        /// </summary>
        private void RequestLinkChanged()
        {
            string SelectRequestLink = EditorRequestData.RequestLink;

            if (ComboBox_RequestType.SelectedItem.ToString() == "GET")
            {
                if (SelectRequestLink.Trim().Length != 0)
                {
                    int GetPos = SelectRequestLink.IndexOf("?", 0);

                    if (GetPos != -1)
                    {
                        SelectRequestLink = DestructGetRequest(SelectRequestLink);
                        SelectRequestLink = ConstructGetRequest(SelectRequestLink);
                    }
                    else
                    {
                        SelectRequestLink = ConstructGetRequest(SelectRequestLink);
                    }
                }
            }
            else
            {
                SelectRequestLink = DestructGetRequest(SelectRequestLink);
            }

            EditorRequestData.RequestLink = SelectRequestLink;
        }

        /// <summary>
        /// Перестраивает ссылку убирая GET параметры, если они есть.
        /// </summary>
        /// <param name="RequestLink">Ссылка с GET параметрами</param>
        /// <returns>Чистая ссылка</returns>
        private string DestructGetRequest(string RequestLink)
        {
            int GetPos = EditorRequestData.RequestLink.IndexOf("?", 0);

            if (GetPos != -1)
            {
                RequestLink = RequestLink.Substring(0, GetPos);
            }

            return RequestLink;
        }

        /// <summary>
        /// Перестраивает ссылку под GET запрос, добавляя параметры для отправки.
        /// </summary>
        /// <param name="RequestLink">Ссылка без параметров</param>
        /// <returns>Ссылка с параметрами</returns>
        private string ConstructGetRequest(string RequestLink)
        {
            RequestLink = RequestLink.Trim();
            RequestLink += "?";

            for(int i = 0; i < FormRequestsData.Count; i++)
            {
                var FormItem = FormRequestsData[i];
                if (i != FormRequestsData.Count - 1)
                    RequestLink += $"{FormItem.Key}={FormItem.Value}&";
                else
                    RequestLink += $"{FormItem.Key}={FormItem.Value}";
            }

            return RequestLink;
        }

        private void ComboBox_RequestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequestLinkChanged();
            SetHeaderName();
        }

        private void SetHeaderName()
        {
            TabItemView.Header = HeaderName + " - " + ComboBox_RequestType.SelectedItem.ToString();
        }

        private void DataGrid_FormRequestData_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ModelFormRequest
            {
                Id = UniqueFormIndex++
            };
        }

        private void ButtonClick_DeleteProject(object sender, RoutedEventArgs e)
        {
            var FormRequest = (sender as Button).DataContext as ModelFormRequest;

            if (FormRequest != null)
            {
                var MFormRequest = FormRequestsData.FirstOrDefault(r => r.Id == FormRequest.Id);
                FormRequestsData.Remove(MFormRequest);
            }
        }
    }
}
