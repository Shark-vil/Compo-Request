using Compo_Request.Models;
using Compo_Request.Network.Utilities;
using Compo_Request.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Compo_Request.WindowsLogic.EditorLogic;

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorWebRequestControl.xaml
    /// </summary>
    public partial class EditorWebRequestControl : UserControl
    {
        private DispatcherTimer Timer_EditorRequestData_PropertyChanged = null;

        private DispatcherTimer Timer_BlockEvent = null;
        private bool IsBlock = false;

        private ObservableCollection<WebRequestParamsItem> WebRequestItems = new ObservableCollection<WebRequestParamsItem>();
        private HeaderedItemViewModel TabItemView { get; set; }
        private string HeaderName { get; set; }

        private string RequestMethod { get; set; }
        private void RequestMethodUpdate() { RequestMethod = ComboBox_RequestType.SelectedItem.ToString(); }

        private string RequestLink { get; set; }
        private DynamicModelEditorRequest EditorRequestData { get; set; }

        private ObservableCollection<ModelRequestDirectory> VirtualRequestDirs = new ObservableCollection<ModelRequestDirectory>();

        private DispatcherTimer WebRequestWait = new DispatcherTimer();

        public EditorWebRequestControl()
        {
            InitializeComponent();

            EditorRequestData = new DynamicModelEditorRequest();
            DataContext = EditorRequestData;

            DataGrid_FormRequestData.ItemsSource = WebRequestItems;
            DataGrid_FormRequestData.Columns[0].Visibility = Visibility.Hidden;

            ListView_WebRequests.ItemsSource = VirtualRequestDirs;

            CollectionView ViewCollection = (CollectionView)CollectionViewSource.GetDefaultView(ListView_WebRequests.ItemsSource);
            PropertyGroupDescription gDescription = new PropertyGroupDescription("RequestDir");
            ViewCollection.GroupDescriptions.Add(gDescription);

            VirtualRequestDirs.Add(new ModelRequestDirectory
            {
                RequestDir = "Test category",
                RequestMethod = "POST",
                WebRequest ="http://google.ru"
            });

            VirtualRequestDirs.Add(new ModelRequestDirectory
            {
                RequestDir = "Test category",
                RequestMethod = "POST",
                WebRequest = "http://google.ru"
            });

            VirtualRequestDirs.Add(new ModelRequestDirectory
            {
                RequestDir = "Test category",
                RequestMethod = "POST",
                WebRequest = "http://google.ru"
            });

            //ListView_WebRequests.ItemsSource = null;
            //ListView_WebRequests.Items.Clear();

            for (int i = 0; i < VirtualRequestDirs.Count; i++)
                VirtualRequestDirs[i].RequestDir = "LOL";

            //DataGrid_FormRequestData.ItemsSource = WebRequestItems;
            ListView_WebRequests.Items.Refresh();
            ViewCollection.Refresh();

            DataGrid_FormRequestData.CurrentCellChanged += DataGrid_FormRequestData_CurrentCellChanged;
            ComboBox_RequestType.SelectionChanged += ComboBox_RequestType_SelectionChanged;
            WebRequestItems.CollectionChanged += FormRequestsData_CollectionChanged;
            Button_SendRequest.Click += Button_SendRequest_Click;
            Button_SaveRequest.Click += Button_SaveRequest_Click;
            Button_RequestList.Click += Button_RequestList_Click;
            EditorRequestData.PropertyChanged += EditorRequestData_PropertyChanged;
        }

        private void Button_RequestList_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_WebRequests.Visibility == Visibility.Collapsed)
                ListView_WebRequests.Visibility = Visibility.Visible;
            else
                ListView_WebRequests.Visibility = Visibility.Collapsed;
        }

        private void Button_SaveRequest_Click(object sender, RoutedEventArgs e)
        {
            // Удаление пустых строк
            RemoveEmptyCollectionValues();
        }

        private void EditorRequestData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!IsBlock && e.PropertyName == "RequestLink" && RequestMethod == "GET")
            {
                RequestLink = EditorRequestData.RequestLink;

                if (Timer_EditorRequestData_PropertyChanged != null && Timer_EditorRequestData_PropertyChanged.IsEnabled)
                    Timer_EditorRequestData_PropertyChanged.Stop();

                Timer_EditorRequestData_PropertyChanged = CustomTimer.Create(delegate (object sender, EventArgs e)
                {
                    ObservableCollection<WebRequestParamsItem> Collection = 
                        GetLinkRequestToCollection.GetCollection(EditorRequestData.RequestLink);

                    if (Collection != null)
                    {
                        WebRequestParamsItem[] CollectionArray;

                        CollectionArray = WebRequestItems.ToArray();
                        foreach (var WebRequestItem in Collection)
                        {
                            if (WebRequestItem != null && WebRequestItem.Key != null)
                            {
                                WebRequestParamsItem GWebRequestItem = Array.Find(CollectionArray, x => x.Key == WebRequestItem.Key);

                                if (GWebRequestItem == null)
                                    WebRequestItems.Add(WebRequestItem);
                                else
                                {
                                    GWebRequestItem.Key = WebRequestItem.Key;
                                    GWebRequestItem.Value = WebRequestItem.Value;

                                    DataGrid_FormRequestData.Items.Refresh();
                                }
                            }
                        }

                        CollectionArray = Collection.ToArray();
                        var RemoveIndexs = new List<int>();
                        for (int i = 0; i < WebRequestItems.Count; i++)
                        {
                            WebRequestParamsItem WebRequestItem = WebRequestItems[i];
                            if (WebRequestItem != null && WebRequestItem.Key != null)
                                if (!Array.Exists(CollectionArray, x => x.Key == WebRequestItem.Key))
                                    WebRequestItems.RemoveAt(i);
                        }
                    }
                    else
                    {
                        WebRequestItems.Clear();
                    }

                }, new TimeSpan(0, 0, 1));
            }
        }

        public void Construct(HeaderedItemViewModel TabItemView, MResponse ServerResponse = null)
        {
            this.TabItemView = TabItemView;
            HeaderName = TabItemView.Header.ToString();

            CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                RequestMethodUpdate();
                SetHeaderName();
            }, new TimeSpan(0, 0, 1));
        }

        private void Button_SendRequest_Click(object sender, RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Если вы хотите сохранить всю историю для запроса, вам следует сохранить запрос. " +
                "Создать новый запрос?",
                delegate ()
                {
                    //LEditorNetwork.SaveRequest();
                },
                delegate()
                {
                    WebRequestSend();
                });
        }

        private void WebRequestSend()
        {
            var t = new Thread(new ThreadStart(delegate()
            {
                Dispatcher.Invoke(delegate ()
                {
                    try
                    {
                        DataGrid_FormRequestData.IsEnabled = false;
                        TextBox_RequestLink.IsEnabled = false;
                        Button_SendRequest.IsEnabled = false;
                        Button_SaveRequest.IsEnabled = false;

                        var Response = ToolWebRequest.RestRequest(RequestMethod, RequestLink, WebRequestItems);

                        JsonViewer.Load(Response);
                    }
                    catch { }

                    DataGrid_FormRequestData.IsEnabled = true;
                    TextBox_RequestLink.IsEnabled = true;
                    Button_SendRequest.IsEnabled = true;
                    Button_SaveRequest.IsEnabled = true;
                });
            }));
            t.Start();
        }

        private void FormRequestsData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (RequestMethod != "GET")
            {
                EnableBlock();
                RequestLinkChanged();
                EnableBlockTimer();
            }
        }

        private void DataGrid_FormRequestData_CurrentCellChanged(object sender, EventArgs e)
        {
            EnableBlock();
            RequestLinkChanged();
            EnableBlockTimer();
        }

        private void EnableBlock()
        {
            if (Timer_BlockEvent != null && Timer_BlockEvent.IsEnabled)
                Timer_BlockEvent.Stop();

            IsBlock = true;
        }

        private void EnableBlockTimer()
        {
            Timer_BlockEvent = CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                if (IsBlock)
                    IsBlock = false;
            }, new TimeSpan(0, 0, 1));
        }

        private void RemoveEmptyCollectionValues()
        {
            try
            {
                var RemoveIndexs = new List<int>();

                for (int i = 0; i < WebRequestItems.Count; i++)
                {
                    WebRequestParamsItem WebRequestItem = WebRequestItems[i];

                    if (WebRequestItem != null && WebRequestItem.Key != null && WebRequestItem.Value != null)
                    {
                        if (WebRequestItem.Key.Trim() == string.Empty && WebRequestItem.Value.Trim() == string.Empty)
                            RemoveIndexs.Add(i);
                    }
                    else
                        RemoveIndexs.Add(i);
                }

                foreach(var ItemIndex in RemoveIndexs)
                    WebRequestItems.RemoveAt(ItemIndex);

            } catch { }
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

            if (RequestLink.IndexOf('?') == -1)
                RequestLink += "?";

            for(int i = 0; i < WebRequestItems.Count; i++)
            {
                var FormItem = WebRequestItems[i];
                if (i != WebRequestItems.Count - 1 
                    && (FormItem.Key != null && FormItem.Key.Trim() != string.Empty)
                    && (FormItem.Value != null && FormItem.Value.Trim() != string.Empty))
                    RequestLink += $"{FormItem.Key}={FormItem.Value}&";
                else
                {
                    if ((FormItem.Key != null && FormItem.Key.Trim() != string.Empty)
                        && (FormItem.Value != null && FormItem.Value.Trim() != string.Empty))
                        RequestLink += $"{FormItem.Key}={FormItem.Value}";
                    else if ((FormItem.Key != null && FormItem.Key.Trim() != string.Empty))
                        RequestLink += $"{FormItem.Key}";
                }
            }

            return RequestLink;
        }

        private void ComboBox_RequestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequestMethodUpdate();
            RequestLinkChanged();
            SetHeaderName();
        }

        private void SetHeaderName()
        {
            TabItemView.Header = HeaderName + " - " + RequestMethod;
        }

        private void ButtonClick_DeleteProject(object sender, RoutedEventArgs e)
        {
            var FormRequest = (sender as Button).DataContext as WebRequestParamsItem;

            if (FormRequest != null)
            {
                var MFormRequest = WebRequestItems.FirstOrDefault(r => r.Id == FormRequest.Id);
                WebRequestItems.Remove(MFormRequest);
            }
        }
    }
}
