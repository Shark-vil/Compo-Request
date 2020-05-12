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
using Compo_Request.Network.Client;
using Compo_Shared_Data.Network;
using Compo_Request.Windows.Editor.Pages;

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorWebRequestControl.xaml
    /// </summary>
    public partial class EditorWebRequestControl : UserControl
    {
        private DispatcherTimer Timer_EditorRequestData_PropertyChanged = null;
        private DispatcherTimer Timer_BlockEvent = null;

        private HeaderedItemViewModel TabItemView { get; set; }
        private DynamicModelEditorRequest EditorRequestData { get; set; }
        private ModelRequestDirectory RequestDirectory { get; set; }

        private ObservableCollection<WebRequestParamsItem> WebRequestItems = new ObservableCollection<WebRequestParamsItem>();
        private ObservableCollection<ModelRequestDirectory> VirtualRequestDirs = new ObservableCollection<ModelRequestDirectory>();
        private CollectionView ListViewCollection { get; set; }

        private bool IsBlock = false;
        private string HeaderName { get; set; }
        private string RequestMethod { get; set; }
        private string RequestLink { get; set; }

        public EditorWebRequestControl()
        {
            InitializeComponent();
            Start();
            WindowActions();
            NetworkActions();
        }

        public void Construct(HeaderedItemViewModel TabItemView, ModelRequestDirectory RequestDirectory = null)
        {
            this.TabItemView = TabItemView;
            HeaderName = TabItemView.Header.ToString();

            this.RequestDirectory = RequestDirectory;

            CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                RequestMethodUpdate();
                if (RequestDirectory != null)
                    LoadRequestDirectory();
                else
                    SetHeaderName();
            }, new TimeSpan(0, 0, 0, 0, 500));
        }

        private void Start()
        {
            EditorRequestData = new DynamicModelEditorRequest();
            DataContext = EditorRequestData;

            DataGrid_FormRequestData.ItemsSource = WebRequestItems;
            DataGrid_FormRequestData.Columns[0].Visibility = Visibility.Hidden;

            ListView_WebRequests.ItemsSource = VirtualRequestDirs;

            ListViewCollection = (CollectionView)CollectionViewSource.GetDefaultView(ListView_WebRequests.ItemsSource);
            var gDescription = new PropertyGroupDescription("Title");
            ListViewCollection.GroupDescriptions.Add(gDescription);

            Sender.SendToServer("WebRequestItem.MBinding_WebRequest.Get", ProjectData.SelectedProject.Id, 85537);
        }

        private void NetworkActions()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var MB_WebRequests = Package.Unpacking<MBinding_WebRequest[]>(ServerResponse.DataBytes);

                foreach(var MB_WebRequest in MB_WebRequests)
                {
                    VirtualRequestDirs.Add(new ModelRequestDirectory
                    {
                        Id = MB_WebRequest.Directory.Id,
                        RequestTitle = MB_WebRequest.Item.Title,
                        Title = MB_WebRequest.Directory.Title,
                        RequestMethod = MB_WebRequest.Item.Method,
                        WebRequest = MB_WebRequest.Item.Link,
                        WebRequestId = MB_WebRequest.Item.Id
                    });
                }

                ListViewCollection.Refresh();
                ListView_WebRequests.Items.Refresh();

            }, Dispatcher, 85537, "WebRequestItem.MBinding_WebRequest.Get", "EditorWebRequestControl");
        }

        private void WindowActions()
        {
            DataGrid_FormRequestData.CurrentCellChanged += DataGrid_FormRequestData_CurrentCellChanged;
            ComboBox_RequestType.SelectionChanged += ComboBox_RequestType_SelectionChanged;
            WebRequestItems.CollectionChanged += FormRequestsData_CollectionChanged;
            Button_SendRequest.Click += Button_SendRequest_Click;
            Button_SaveRequest.Click += Button_SaveRequest_Click;
            Button_RequestList.Click += Button_RequestList_Click;
            EditorRequestData.PropertyChanged += EditorRequestData_PropertyChanged;
        }

        private void RequestMethodUpdate()
        { 
            RequestMethod = ComboBox_RequestType.SelectedItem.ToString();
        }

        private void Button_RequestList_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_WebRequests.Visibility == Visibility.Collapsed)
            {
                ListView_WebRequests.Visibility = Visibility.Visible;
                DataGrid_FormRequestData.Visibility = Visibility.Collapsed;
            }
            else
            {
                ListView_WebRequests.Visibility = Visibility.Collapsed;
                DataGrid_FormRequestData.Visibility = Visibility.Visible;
            }
        }

        private void Button_SaveRequest_Click(object sender, RoutedEventArgs e)
        {
            // Удаление пустых строк
            RemoveEmptyCollectionValues();

            LEditorNetwork.SaveRequest(RequestLink, RequestMethod, WebRequestItems.ToArray());
        }

        private void EditorRequestData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RequestLink = EditorRequestData.RequestLink;

            if (!IsBlock && e.PropertyName == "RequestLink" && RequestMethod == "GET")
            {
                if (Timer_EditorRequestData_PropertyChanged != null && Timer_EditorRequestData_PropertyChanged.IsEnabled)
                    Timer_EditorRequestData_PropertyChanged.Stop();

                Timer_EditorRequestData_PropertyChanged = CustomTimer.Create(delegate (object sender, EventArgs e)
                {
                    if (RequestMethod != "GET")
                        return;

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

        private void Button_SendRequest_Click(object sender, RoutedEventArgs e)
        {
            new ConfirmWindow("Предупреждение", "Если вы хотите сохранить всю историю для запроса, вам следует сохранить запрос. " +
                "Создать новый запрос?",
                delegate ()
                {
                    LEditorNetwork.SaveRequest(RequestLink, RequestMethod, WebRequestItems.ToArray());
                },
                delegate()
                {
                    WebRequestSend();
                });
        }

        private void WebRequestSend()
        {
            DataGrid_FormRequestData.IsReadOnly = false;
            TextBox_RequestLink.IsReadOnly = false;
            Button_SendRequest.IsEnabled = false;
            Button_SaveRequest.IsEnabled = false;

            var t = new Thread(new ThreadStart(delegate()
            {
                Dispatcher.BeginInvoke(delegate ()
                {
                    try
                    {
                        var Response = ToolWebRequest.RestRequest(RequestMethod, RequestLink, WebRequestItems);

                        JsonViewer.Load(Response);
                    }
                    catch { }

                    DataGrid_FormRequestData.IsReadOnly = true;
                    TextBox_RequestLink.IsReadOnly = true;
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

        private void LoadRequestDirectory()
        {
            if (RequestDirectory != null)
            {
                HeaderName = RequestDirectory.RequestTitle;
                RequestMethod = RequestDirectory.RequestMethod;
                SetHeaderName();

                int Index = ComboBox_RequestType.Items.IndexOf(RequestMethod);
                if (Index != -1)
                    ComboBox_RequestType.SelectedIndex = Index;

                TextBox_RequestLink.Text = RequestDirectory.WebRequest;

                int NetworkUid = Guid.NewGuid().GetHashCode();

                NetworkDelegates.Add(delegate (MResponse ServerResponse)
                {
                    var RequestParams = Package.Unpacking<WebRequestParamsItem[]>(ServerResponse.DataBytes);

                    foreach (var RequestParam in RequestParams)
                    {
                        if (!Array.Exists(WebRequestItems.ToArray(), x => x.Id == RequestParam.Id))
                            if (RequestParam.WebRequestItemId == RequestDirectory.WebRequestId)
                                WebRequestItems.Add(RequestParam);
                    }

                    Debug.Log("Данные с сервера получены!");

                }, Dispatcher, NetworkUid, "WebRequestParamsItem.Get.Confirm", "EditorWebRequestControl");

                EnableBlockTimer();
                Sender.SendToServer("WebRequestParamsItem.Get", RequestDirectory.WebRequestId, NetworkUid);
            }
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

        private void ButtonClick_OpenWebRequest(object sender, RoutedEventArgs e)
        {
            var RowRequestDirectory = (sender as Button).DataContext as ModelRequestDirectory;

            //Debug.Log(RowRequestDirectory);
            //Debug.Log(ProjectData.TabCollecton);

            if (ProjectData.TabCollecton != null)
            {
                ProjectData.TabCollecton.Items.Add(BoundNewItem.AddTab(RowRequestDirectory.RequestTitle, RowRequestDirectory));
            }
        }

        private void ButtonClick_DeleteWebRequest(object sender, RoutedEventArgs e)
        {

        }
    }
}
