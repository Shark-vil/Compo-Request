using Compo_Request.Models;
using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace Compo_Request.Windows.Editor.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditorProjectChatPage.xaml
    /// </summary>
    public partial class EditorProjectChatPage : Page, ICustomPage
    {
        private DispatcherTimer MessageDelay;
        private EditorMainMenuWindow _EditorMainMenuWindow;

        public ObservableCollection<DynamicModelChat> Messages = new ObservableCollection<DynamicModelChat>();

        public EditorProjectChatPage(EditorMainMenuWindow _EditorMainMenuWindow)
        {
            InitializeComponent();
            this._EditorMainMenuWindow = _EditorMainMenuWindow;

            ListView_Chat.ItemsSource = Messages;

            if (!Sender.SendToServer("Chat.Messages.GetAll", ProjectData.SelectedProject.Id))
            {
                new AlertWindow("Ошибка!", AlertWindow.AlertCode.SendToServer);
            }

            this.Button_Send.Click += Button_Send_Click;
            this.TextBox_Message.KeyDown += TextBox_Message_KeyDown;
        }

        private void TextBox_Message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ChatSendMessage();
            }
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            ChatSendMessage();
        }

        private void ChatSendMessage()
        {
            string Message = TextBox_Message.Text;

            if (Message.Trim() != string.Empty)
            {
                Chat PreMessage = new Chat();
                PreMessage.Message = Message;
                PreMessage.ProjectId = ProjectData.SelectedProject.Id;

                if (!Sender.SendToServer("Chat.Messages.New", PreMessage))
                {
                    new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
                }
                else
                {
                    TextBox_Message.IsEnabled = false;
                    Button_Send.IsEnabled = false;

                    MessageDelay = CustomTimer.Create((object sender, EventArgs e) =>
                    {
                        new AlertWindow("Ошибка", "Превышено время ожидания ответа от сервера. Сообщение не отправлено.",
                            () =>
                            {
                                if (this != null)
                                {
                                    TextBox_Message.IsEnabled = true;
                                    Button_Send.IsEnabled = true;
                                }
                            });

                    }, new TimeSpan(0, 0, 3));
                }
            }
        }

        private void ChatToBottom()
        {
            ListView_Chat.SelectedIndex = ListView_Chat.Items.Count - 1;
            ListView_Chat.ScrollIntoView(ListView_Chat.SelectedItem);
        }

        public void ClosePage()
        {
            //NetworkDelegates.RemoveByUniqueName("EditorProjectChatPage");
        }

        public void OpenPage()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var DbMessages = Package.Unpacking<ModelChatMessage[]>(ServerResponse.DataBytes);

                foreach(var Message in DbMessages)
                {
                    Messages.Add(new DynamicModelChat
                    {
                        Id = Message.Id,
                        FullName = Message.FullName,
                        Message = Message.Message,
                        Date = Message.Date,
                        Email = Message.Email,
                        ProjectId = Message.ProjectId,
                        UserId = Message.UserId
                    });
                }

                ChatToBottom();

            }, Dispatcher, -1, "Chat.Messages.GetAll.Confirm", "EditorProjectChatPage");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var Message = Package.Unpacking<ModelChatMessage>(ServerResponse.DataBytes);

                if (Message.ProjectId != ProjectData.SelectedProject.Id)
                    return;

                if (Message.UserId == UserInfo.NetworkSelf.Id)
                {
                    if (MessageDelay.IsEnabled)
                    {
                        MessageDelay.Stop();

                        TextBox_Message.IsEnabled = true;
                        Button_Send.IsEnabled = true;

                        TextBox_Message.Text = string.Empty;
                        TextBox_Message.Focus();
                    }
                }

                Messages.Add(new DynamicModelChat
                {
                    Id = Message.Id,
                    FullName = Message.FullName,
                    Message = Message.Message,
                    Date = Message.Date,
                    Email = Message.Email,
                    ProjectId = Message.ProjectId,
                    UserId = Message.UserId
                });

                ChatToBottom();

            }, Dispatcher, -1, "Chat.Messages.New.Confirm", "EditorProjectChatPage");
        }
    }
}
