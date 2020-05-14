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
using System.Reflection;
using RestSharp.Serialization.Json;
using Newtonsoft.Json;

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorWebRequestControl.xaml
    /// </summary>
    public partial class EditorWebRequestControl : UserControl
    {
        public DispatcherTimer Timer_EditorRequestData_PropertyChanged = null;
        public HeaderedItemViewModel TabItemView { get; set; }
        public DynamicModelEditorRequest EditorRequestData { get; set; }
        public ObservableCollection<WebRequestParamsItem> WebRequestItems { get; set; }
        public ObservableCollection<ModelRequestDirectory> VirtualRequestDirs { get; set; }
        public CollectionView ListViewCollection { get; set; }
        public LEditorGeneral GeneralLogic { get; set; }
        public ModelRequestDirectory RequestDirectory { get; set; }

        public EditorWebRequestControl(ModelRequestDirectory RequestDirectory = null)
        {
            InitializeComponent();

            this.RequestDirectory = RequestDirectory;

            WebRequestItems = new ObservableCollection<WebRequestParamsItem>();
            VirtualRequestDirs = new ObservableCollection<ModelRequestDirectory>();

            GeneralLogic = new LEditorGeneral(this);
        }

        /// <summary>
        /// Конструктор редактора.
        /// </summary>
        /// <param name="TabItemView">Собственный TAB компонент</param>
        public void Construct(HeaderedItemViewModel TabItemView)
        {
            this.TabItemView = TabItemView;
            GeneralLogic.HeaderName = TabItemView.Header.ToString();

            Start();
            WindowActions();

            CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                GeneralLogic.RequestMethod_SetComboBoxItem(ComboBox_RequestType);

                if (RequestDirectory != null)
                {
                    int NetworkUid = Guid.NewGuid().GetHashCode();

                    GeneralLogic.LoadRequestDirectory(this, NetworkUid);
                    LEditorNetworkActions.RequestParamsItemsGet_Confirm(this, NetworkUid);
                }
                else
                    GeneralLogic.SetHeaderName(TabItemView);
            }, new TimeSpan(0, 0, 0, 0, 500));
        }

        public void SetHistory(WebRequestHistory HistoryItem)
        {
            CustomTimer.Create(delegate (object sender, EventArgs e)
            {
                GeneralLogic.IsCopy = true;

                EditorRequestData.RequestLink = HistoryItem.Link;
                ComboBox_RequestType.SelectedIndex = ComboBox_RequestType.Items.IndexOf(HistoryItem.Method);

                GeneralLogic.SetViews(HistoryItem.ResponseResult);

                var RequestParams = JsonConvert.DeserializeObject<WebRequestParamsItem[]>(HistoryItem.ParametrsInJson);

                Debug.Log("RequestParams - " + RequestParams.Length);

                foreach (var ItemParam in RequestParams)
                    WebRequestItems.Add(ItemParam);
            }, new TimeSpan(0, 0, 0, 0, 300));
        }

        public void Destruct()
        {
            NetworkDelegates.RemoveByUniqueName(GeneralLogic.UserControl_Uid);
        }

        private void Start()
        {
            GeneralLogic.UserControl_Uid = Guid.NewGuid().ToString();
            GeneralLogic.MBinding_WebRequest_Get_Uid = Guid.NewGuid().GetHashCode();

            EditorRequestData = new DynamicModelEditorRequest();
            DataContext = EditorRequestData;

            DataGrid_FormRequestData.ItemsSource = WebRequestItems;
            DataGrid_FormRequestData.Columns[0].Visibility = Visibility.Hidden;

            ListView_WebRequests.ItemsSource = VirtualRequestDirs;

            ListViewCollection = (CollectionView)CollectionViewSource.GetDefaultView(ListView_WebRequests.ItemsSource);
            var gDescription = new PropertyGroupDescription("Title");
            ListViewCollection.GroupDescriptions.Add(gDescription);

            NetworkActions();

            Sender.SendToServer("WebRequestItem.MBinding_WebRequest.Get", 
                ProjectData.SelectedProject.Id, GeneralLogic.MBinding_WebRequest_Get_Uid);
        }

        private void NetworkActions()
        {
            /**
             * Загружает в VirtualRequestDirs список запросов и каталогов
             */
            LEditorNetworkActions.FirstLoad_ListViewCollection(this);

            /**
             * Обновляет строку с ссылкой
             */
            LEditorNetworkActions.LinkUpdate_Confirm(this);

            /**
             * Устанавливает новый метод в списке методов запроса
             */
            LEditorNetworkActions.MethodUpdate_Confirm(this);

            /**
             * Обновляет список параметров запроса
             */
            LEditorNetworkActions.RequestParamsUpdate_Confirm(this);

            LEditorNetworkActions.RequestParamsDelete_Confirm(this);
            LEditorNetworkActions.RequestParamsDeleteAll_Confirm(this);
        }

        private void WindowActions()
        {
            DataGrid_FormRequestData.CurrentCellChanged += DataGrid_FormRequestData_CurrentCellChanged;
            DataGrid_FormRequestData.CellEditEnding += DataGrid_FormRequestData_CellEditEnding;
            ComboBox_RequestType.SelectionChanged += ComboBox_RequestType_SelectionChanged;
            WebRequestItems.CollectionChanged += FormRequestsData_CollectionChanged;
            Button_SendRequest.Click += Button_SendRequest_Click;
            Button_SaveRequest.Click += Button_SaveRequest_Click;
            Button_RequestList.Click += Button_RequestList_Click;
            EditorRequestData.PropertyChanged += EditorRequestData_PropertyChanged;
            TextBox_RequestLink.TextChanged += TextBox_RequestLink_TextChanged;
            HtmlViewer.Navigated += HtmlViewer_Navigated;
        }

        private void HtmlViewer_Navigated(object sender, NavigationEventArgs e)
        {
            HideScriptErrors(HtmlViewer, true);
        }

        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

        private void TextBox_RequestLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!TextBox_RequestLink.IsReadOnly)
            {
                if (TextBox_RequestLink.IsFocused && RequestDirectory != null)
                {
                    var RequestItem = new WebRequestItem();
                    RequestItem.Id = RequestDirectory.WebRequestId;
                    RequestItem.Link = GeneralLogic.RequestLink;

                    Sender.SendToServer("WebRequestItem.Update.Link", RequestItem);
                }

                if (TextBox_RequestLink.IsFocused && GeneralLogic.RequestMethod == "GET")
                {
                    if (Timer_EditorRequestData_PropertyChanged != null && Timer_EditorRequestData_PropertyChanged.IsEnabled)
                        Timer_EditorRequestData_PropertyChanged.Stop();

                    Timer_EditorRequestData_PropertyChanged = CustomTimer.Create(delegate (object sender, EventArgs e)
                    {
                        if (GeneralLogic.RequestMethod != "GET")
                            return;

                        UpdateDataGrid_OnTextBox();

                    }, new TimeSpan(0, 0, 1));
                }
            }
        }

        internal void UpdateDataGrid_OnTextBox()
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
                    WebRequestParamsItem RequestParamsItem = WebRequestItems[i];
                    if (RequestParamsItem != null && RequestParamsItem.Key != null)
                        if (!Array.Exists(CollectionArray, x => x.Key == RequestParamsItem.Key))
                        {
                            WebRequestItems.RemoveAt(i);
                            GeneralLogic.DataGrid_RemoveParamsById(RequestParamsItem.Id);
                        }
                }

                GeneralLogic.DataGrid_UpdateParamsBroadcast();
            }
            else
            {
                WebRequestItems.Clear();

                if (RequestDirectory != null)
                    GeneralLogic.DataGrid_RemoveParamsAll(RequestDirectory.WebRequestId);
            }
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

            LEditorNetwork.SaveRequest(GeneralLogic.RequestLink, GeneralLogic.RequestMethod, WebRequestItems.ToArray());
        }

        private void EditorRequestData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GeneralLogic.RequestLink = EditorRequestData.RequestLink;
        }

        private void Button_SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (RequestDirectory == null)
            {
                new ConfirmWindow("Предупреждение", "Если вы хотите сохранить всю историю для запроса, вам следует сохранить запрос. " +
                    "Создать новый запрос?",
                    delegate ()
                    {
                        LEditorNetwork.SaveRequest(
                            GeneralLogic.RequestLink,
                            GeneralLogic.RequestMethod,
                            WebRequestItems.ToArray()
                        );
                    },
                    delegate ()
                    {
                        GeneralLogic.WebRequestSend();
                    });
            }
            else
            {
                GeneralLogic.WebRequestSend();
            }
        }

        private void FormRequestsData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            EditorRequestData.RequestLink =
                    GeneralLogic.RequestLinkChanged(EditorRequestData.RequestLink);
        }

        private void DataGrid_FormRequestData_CurrentCellChanged(object sender, EventArgs e)
        {
            GeneralLogic.DataGrid_UpdateParamsBroadcast();

            EditorRequestData.RequestLink =
                    GeneralLogic.RequestLinkChanged(EditorRequestData.RequestLink);
        }

        private void DataGrid_FormRequestData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            /*
            GeneralLogic.DataGrid_UpdateParamsBroadcast();

            EditorRequestData.RequestLink =
                    GeneralLogic.RequestLinkChanged(EditorRequestData.RequestLink);
            */
        }

        internal void RemoveEmptyCollectionValues()
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

        internal void ComboBox_RequestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneralLogic.RequestMethod_SetComboBoxItem(ComboBox_RequestType);

            EditorRequestData.RequestLink =
                    GeneralLogic.RequestLinkChanged(EditorRequestData.RequestLink);

            GeneralLogic.SetHeaderName(TabItemView);

            if (RequestDirectory != null && RequestDirectory.WebRequestId != 0 && GeneralLogic.RequestMethod != null)
            {
                var RequestItem = new WebRequestItem();
                RequestItem.Id = RequestDirectory.WebRequestId;
                RequestItem.Method = GeneralLogic.RequestMethod;

                Sender.SendToServer("WebRequestItem.Update.Method", RequestItem);
            }
        }

        private void ButtonClick_DeleteParamsItem(object sender, RoutedEventArgs e)
        {
            var RequestParamsItem = (sender as Button).DataContext as WebRequestParamsItem;

            if (RequestParamsItem != null)
            {
                var MFormRequest = WebRequestItems.FirstOrDefault(r => r.Id == RequestParamsItem.Id);
                //WebRequestItems.Remove(MFormRequest);

                if (MFormRequest.Id != 0)
                    GeneralLogic.DataGrid_RemoveParamsById(MFormRequest.Id);
            }
        }

        private void ButtonClick_OpenWebRequest(object sender, RoutedEventArgs e)
        {
            var RowRequestDirectory = (sender as Button).DataContext as ModelRequestDirectory;

            if (ProjectData.TabCollecton != null)
            {
                ModelRequestDirectory RequestDirectoryItem = Array.Find(VirtualRequestDirs.ToArray(),
                    x => x.Id == RowRequestDirectory.Id);

                ProjectData.TabCollecton.Items.Add(
                    BoundNewItem.AddTab(
                        RequestDirectoryItem.RequestTitle,
                        RequestDirectoryItem
                    )
                );
            }
        }

        private void ButtonClick_DeleteWebRequest(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}
