using Compo_Request.Models;
using Compo_Request.Models.Windows;
using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
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
using System.Windows.Shapes;

namespace Compo_Request.Windows.Editor.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditorRequestSaverWindow.xaml
    /// </summary>
    public partial class EditorRequestSaverWindow : Window
    {
        private bool IsSaveClick = false;
        private EventEditorRequestSaver SaveConfirm;
        private EventEditorRequestSaver CancelEvent;

        public WebRequestParamsItem[] WR_ParamsItem;
        public string RequestLink;
        public string RequestMethod;

        public delegate void EventEditorRequestSaver();
        public DynamicEditorRequestSaver DC_EditorRequestSaver;

        public EditorRequestSaverWindow(string RequestLink, string RequestMethod, WebRequestParamsItem[] WR_ParamsItem, 
            EventEditorRequestSaver SaveConfirm = null, EventEditorRequestSaver CancelEvent = null)
        {
            InitializeComponent();
            WindowActions();
            NetworkActions();

            DC_EditorRequestSaver = new DynamicEditorRequestSaver(string.Empty, "Default");
            this.DataContext = DC_EditorRequestSaver;

            this.WR_ParamsItem = WR_ParamsItem;
            this.RequestLink = RequestLink;
            this.RequestMethod = RequestMethod;

            this.SaveConfirm = SaveConfirm;
            this.CancelEvent = CancelEvent;
        }

        private void WindowActions()
        {
            this.Closed += EditorRequestSaverWindow_Closed;
            Button_Cancel.Click += Button_Cancel_Click;
            Button_Save.Click += Button_Save_Click;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SendNetwork();
        }

        private void SendNetwork()
        {
            var WebRequestBinding = new MBinding_WebRequestSaver
            {
                Item = new WebRequestItem
                {
                    Link = RequestLink,
                    ProjectId = ProjectData.SelectedProject.Id,
                    Title = DC_EditorRequestSaver.RequestName,
                    Method = RequestMethod
                },
                Params = WR_ParamsItem
            };

            if (!Sender.SendToServer("WebRequestItem.MBinding_WebRequestSaver.Save", WebRequestBinding, 1337))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
            else
            {
                TextBox_WebRequest.IsEnabled = false;
                TextBox_WebRequestDirectory.IsEnabled = false;
            }
        }

        private void NetworkActions()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var WebRequestDirectory = Package.Unpacking<WebRequestDir>(ServerResponse.DataBytes);

                TextBox_WebRequest.IsEnabled = true;
                TextBox_WebRequestDirectory.IsEnabled = true;

            }, Dispatcher, -1, "WebRequestDir.Save.Confirm", "EditorRequestSaverWindow");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var WebRequestBinding = Package.Unpacking<MBinding_WebRequestSaver>(ServerResponse.DataBytes);

                var WebRequestDirectory = new WebRequestDir
                {
                    Title = DC_EditorRequestSaver.RequestDirectoryName,
                    WebRequestItemId = WebRequestBinding.Item.Id
                };

                if (!Sender.SendToServer("WebRequestDir.Save", WebRequestDirectory, 1337))
                {
                    new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
                }

            }, Dispatcher, -1, "WebRequestItem.MBinding_WebRequestSaver.Save.Confirm", "EditorRequestSaverWindow");
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditorRequestSaverWindow_Closed(object sender, EventArgs e)
        {
            if (!IsSaveClick)
                CancelEvent?.Invoke();

            NetworkDelegates.RemoveByUniqueName("EditorRequestSaverWindow");
        }
    }
}
