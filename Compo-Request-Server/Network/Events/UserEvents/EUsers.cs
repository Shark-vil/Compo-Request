using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using CryptSharp;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.UserEvents
{
    public class EUsers
    {
        public EUsers()
        {
            NetworkDelegates.Add(UsersGetAll, "Users.GetAll");
            NetworkDelegates.Add(UsersUpdate, "Users.Update");
            NetworkDelegates.Add(UsersDelete, "Users.Delete");
        }

        private void UsersDelete(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                var UserId = Package.Unpacking<int>(ClientResponse.DataBytes);

                if (UserId == 1)
                    return;

                using (var db = new DatabaseContext())
                {
                    MUserNetwork NetUser = Users.GetUserById(NetworkClient.Id);

                    if (!AccessController.IsPrivilege(NetworkClient, "users.delete"))
                        return;

                    var DbUser = db.Users.FirstOrDefault(x => x.Id == UserId);

                    db.Users.Remove(DbUser);
                    db.SaveChanges();

                    if (UserId == NetUser.Id)
                    {
                        Users.Remove(NetworkClient);
                        Debug.LogWarning($"Пользователь {NetUser.Login} удалил сам себя!");
                    }
                    else
                        Sender.Send(NetworkClient, "Users.Delete.Confirm", UserId);
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при попытке удалить пользователя. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "Users.Delete.Error");
            }
        }

        private void UsersUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                var User = Package.Unpacking<User>(ClientResponse.DataBytes);

                using (var db = new DatabaseContext())
                {
                    MUserNetwork NetUser = Users.GetUserById(NetworkClient.Id);

                    if (NetUser.Id != User.Id)
                    {
                        if (User.Id == 1 && NetUser.Id != 1)
                            return;

                        if (!AccessController.IsPrivilege(NetworkClient, "users.edit"))
                            return;
                    }

                    var DbUser = db.Users.FirstOrDefault(x => x.Id == User.Id);
                    DbUser.Login = User.Login;
                    DbUser.Email = User.Email;
                    DbUser.Name = User.Name;
                    DbUser.Surname = User.Surname;
                    DbUser.Patronymic = User.Patronymic;

                    if (User.Password != null && User.Password.Length != 0 && User.Password.Trim() != string.Empty)
                        DbUser.Password = Crypter.Blowfish.Crypt(User.Password);

                    db.SaveChanges();

                    Sender.Send(NetworkClient, "Users.Update.Confirm", User);
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при обновлении данных пользователя. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "Users.Update.Error");
            }
        }

        private void UsersGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "users"))
                return;

            try
            {
                using(var db = new DatabaseContext())
                {
                    var DbUsers = db.Users.ToArray();
                    var WUsers = DbConvertToWpf.ConvertUser(DbUsers);

                    if (AccessController.IsPrivilege(NetworkClient, "users"))
                        Sender.Send(NetworkClient, "Users.GetAll.Confirm", WUsers);
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при получении списка пользователей. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "Users.GetAll.Error");
            }
        }
    }
}
