using Compo_Request.Models.Windows;
using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
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
    /// Логика взаимодействия для EditorRequestRenameWindow.xaml
    /// </summary>
    public partial class EditorRequestRenameWindow : Window
    {
        private bool IsSaveClick = false;
        private EventEditorRequestRename SaveConfirm;
        private EventEditorRequestRename CancelEvent;

        public ModelRequestDirectory RequestDirectory;
        public string RequestLink;
        public string RequestMethod;

        public delegate void EventEditorRequestRename();
        public DynamicEditorRequestSaver DC_EditorRequestSaver;

        public EditorRequestRenameWindow(string RequestLink, string RequestMethod, ModelRequestDirectory RequestDirectory,
            EventEditorRequestRename SaveConfirm = null, EventEditorRequestRename CancelEvent = null)
        {
            InitializeComponent();
            WindowActions();
            NetworkActions();

            DC_EditorRequestSaver = new DynamicEditorRequestSaver(RequestDirectory.RequestTitle, RequestDirectory.Title);
            this.DataContext = DC_EditorRequestSaver;

            this.RequestDirectory = RequestDirectory;
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
            RequestDirectory.RequestTitle = DC_EditorRequestSaver.RequestName;
            RequestDirectory.Title = DC_EditorRequestSaver.RequestDirectoryName;

            if (!Sender.SendToServer("WebRequestDir.RequestDirectory.Update", RequestDirectory, 1337))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer, () =>
                {
                    TextBox_WebRequest.IsEnabled = true;
                    TextBox_WebRequestDirectory.IsEnabled = true;
                });
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
                var WebRequestDirectory = Package.Unpacking<ModelRequestDirectory>(ServerResponse.DataBytes);

                new AlertWindow("Оповещение", "Запись успешно сохранена", () =>
                {
                    TextBox_WebRequest.IsEnabled = true;
                    TextBox_WebRequestDirectory.IsEnabled = true;
                });

            }, Dispatcher, -1, "WebRequestDir.RequestDirectory.Update.Confirm", "EditorRequestRenameWindow");
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditorRequestSaverWindow_Closed(object sender, EventArgs e)
        {
            if (!IsSaveClick)
                CancelEvent?.Invoke();

            NetworkDelegates.RemoveByUniqueName("EditorRequestRenameWindow");
        }
    }
}
