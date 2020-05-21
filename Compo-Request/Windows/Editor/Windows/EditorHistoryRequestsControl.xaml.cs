using Compo_Request.Models;
using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Request.Windows.Editor.Pages;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorHistoryRequestsControl.xaml
    /// </summary>
    public partial class EditorHistoryRequestsControl : UserControl, ICustomPage
    {
        private EditorMainMenuWindow _EditorMainMenuWindow;
        private ObservableCollection<WebRequestHistory> WebRequestsHistory;

        public EditorHistoryRequestsControl(EditorMainMenuWindow _EditorMainMenuWindow)
        {
            InitializeComponent();
            WindowsRegister(_EditorMainMenuWindow);
            NetworkActions();

            WebRequestsHistory = new ObservableCollection<WebRequestHistory>();

            DataGrid_History.ItemsSource = WebRequestsHistory;

            Sender.SendToServer("RequestsHistory.GetAll", ProjectData.SelectedProject.Id);
        }

        private void WindowsRegister(EditorMainMenuWindow _EditorMainMenuWindow)
        {
            this._EditorMainMenuWindow = _EditorMainMenuWindow;
        }

        private void NetworkActions()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var HistoryRequests = Package.Unpacking<WebRequestHistory[]>(ServerResponse.DataBytes);

                WebRequestHistory[] WebRequestsHistoryTemp = WebRequestsHistory.ToArray();
                foreach (var HistoryItem in HistoryRequests)
                {
                    if (!Array.Exists(WebRequestsHistoryTemp, x => x.Id == HistoryItem.Id))
                    {
                        WebRequestsHistory.Add(HistoryItem);
                    }
                }

            }, Dispatcher, -1, "RequestsHistory.GetAll.Confirm", "EditorHistoryRequestsControl");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var HistoryRequestItem = Package.Unpacking<WebRequestHistory>(ServerResponse.DataBytes);

                if(!Array.Exists(WebRequestsHistory.ToArray(), x => x.Id == HistoryRequestItem.Id))
                {
                    WebRequestsHistory.Add(HistoryRequestItem);
                }

            }, Dispatcher, -1, "RequestsHistory.Add.Confirm", "EditorHistoryRequestsControl");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var RequestDirectory = Package.Unpacking<ModelRequestDirectory>(ServerResponse.DataBytes);

                if (_EditorMainMenuWindow._EditorWebRequestPage != null)
                    _EditorMainMenuWindow._EditorWebRequestPage.ClosePage();

                _EditorMainMenuWindow._EditorWebRequestPage = new EditorWebRequestPage(_EditorMainMenuWindow);
                _EditorMainMenuWindow._EditorWebRequestPage.OpenPage();

                _EditorMainMenuWindow.Frame_Main.Content = _EditorMainMenuWindow._EditorWebRequestPage;

                ProjectData.TabCollecton.Items.Add(
                    BoundNewItem.AddTab(
                        RequestDirectory.RequestTitle,
                        RequestDirectory
                    )
                );

            }, Dispatcher, -1, "WebRequestDir.History.Edit.Confirm", "EditorHistoryRequestsControl");
        }

        private void ButtonClick_RequestOpen(object sender, RoutedEventArgs e)
        {
            var HistoryItem = ((sender as Button).DataContext as WebRequestHistory);

            /*
            var RequestDirectory = new ModelRequestDirectory();
            RequestDirectory.RequestMethod = HistoryItem.Method;
            RequestDirectory.RequestTitle = HistoryItem.Title;
            RequestDirectory.WebRequestId = HistoryItem.WebRequestItemId;
            */

            /**
             * Открытие списка запросов. Копипаста с _EditorMainMenuWindow.
             * Хочу сдохнуть уже...
             */

            if (_EditorMainMenuWindow._EditorWebRequestPage != null)
                _EditorMainMenuWindow._EditorWebRequestPage.ClosePage();

            _EditorMainMenuWindow._EditorWebRequestPage = new EditorWebRequestPage(_EditorMainMenuWindow);
            _EditorMainMenuWindow._EditorWebRequestPage.OpenPage();

            _EditorMainMenuWindow.Frame_Main.Content = _EditorMainMenuWindow._EditorWebRequestPage;

            /**
             * Конец копипасты
             */

            ProjectData.TabCollecton.Items.Add(BoundNewItem.AddHistoryTab(
                HistoryItem.Title, HistoryItem));
        }

        private void ButtonClick_RequestEdit(object sender, RoutedEventArgs e)
        {
            var HistoryItem = ((sender as Button).DataContext as WebRequestHistory);

            if (!Sender.SendToServer("WebRequestDir.History.Edit", HistoryItem))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
        }

        public void OpenPage()
        {
            //
        }

        public void ClosePage()
        {
            //
        }
    }
}
