using Compo_Request.Models;
using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
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
    /// Логика взаимодействия для EditorHistoryRequestsControl.xaml
    /// </summary>
    public partial class EditorHistoryRequestsControl : UserControl, ICustomPage
    {
        private ObservableCollection<WebRequestHistory> WebRequestsHistory;

        public EditorHistoryRequestsControl()
        {
            InitializeComponent();
            NetworkActions();

            WebRequestsHistory = new ObservableCollection<WebRequestHistory>();

            DataGrid_History.ItemsSource = WebRequestsHistory;

            Sender.SendToServer("RequestsHistory.GetAll", ProjectData.SelectedProject.Id);
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
        }

        private void ButtonClick_RequestOpen(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClick_RequestEdit(object sender, RoutedEventArgs e)
        {

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
