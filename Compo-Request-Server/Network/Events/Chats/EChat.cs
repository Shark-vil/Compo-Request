using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.Chats
{
    public class EChat
    {
        public EChat()
        {
            NetworkDelegates.Add(ChatMessagesGetAll, "Chat.Messages.GetAll");
            NetworkDelegates.Add(ChatMessagesNew, "Chat.Messages.New");
        }

        private void ChatMessagesNew(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                var PreMessage = Package.Unpacking<Chat>(ClientResponse.DataBytes);

                using (var db = new DatabaseContext())
                {
                    PreMessage.UserId = Users.GetUserById(NetworkClient.Id).Id;
                    PreMessage.Date = DateTime.Now;

                    db.Chats.Add(PreMessage);
                    db.SaveChanges();

                    Sender.Broadcast("Chat.Messages.New.Confirm", GetModelMessage(db, PreMessage));
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при добавлении нового сообщения. Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "Chat.Messages.New.Error");
            }
        }

        private void ChatMessagesGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                int ProjectId = Package.Unpacking<int>(ClientResponse.DataBytes);

                using (var db = new DatabaseContext())
                {
                    Chat[] ChatMessages = db.Chats.Where(x => x.ProjectId == ProjectId).ToArray();

                    if (ChatMessages.Length != 0)
                    {
                        List<ModelChatMessage> mChatMessages = new List<ModelChatMessage>();

                        foreach (var Message in ChatMessages)
                        {
                            mChatMessages.Add(GetModelMessage(db, Message));
                        }

                        Sender.Send(NetworkClient, "Chat.Messages.GetAll.Confirm", mChatMessages.ToArray());
                    }
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при получении списка сообщений. Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "Chat.Messages.GetAll.Error");
            }
        }

        private ModelChatMessage GetModelMessage(DatabaseContext db, Chat ChatMessage)
        {
            User user = db.Users.FirstOrDefault(x => x.Id == ChatMessage.UserId);

            return new ModelChatMessage
            {
                Id = ChatMessage.Id,
                Date = ChatMessage.Date,
                Email = user.Email,
                FullName = $"{user.Name} {user.Surname} {user.Patronymic}",
                Message = ChatMessage.Message,
                ProjectId = ChatMessage.ProjectId,
                UserId = ChatMessage.UserId
            };
        }
    }
}
