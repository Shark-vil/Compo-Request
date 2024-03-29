﻿using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Interfaces.Events;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using CryptSharp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.Auth
{
    public class EAuth
    {
        public EAuth()
        {
            NetworkDelegates.Add(AuthUser, "User.Auth", 1);
            NetworkDelegates.Add(UserDisconnected, "User.Disconnected");
        }

        private void UserDisconnected(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            Users.Remove(NetworkClient);

            Sender.Send(NetworkClient, "User.Disconnected.Confirm");
        }

        private void AuthUser(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var UserData = Package.Unpacking<string[]>(ClientResponse.DataBytes);
                    var user = db.Users.Where(u => u.Email == UserData[0]).FirstOrDefault();

                    if (user == null)
                        user = db.Users.Where(u => u.Login == UserData[0]).FirstOrDefault();

                    if (user != null)
                    {
                        if (Crypter.CheckPassword(UserData[1], user.Password))
                        {
                            if (Users.ActiveUsers.Exists(x => x.Login == user.Login))
                            {
                                Sender.Send(NetworkClient, "User.Auth.Error", default, 2);
                                return;
                            }

                            var UserNetwork = new MUserNetwork();
                            UserNetwork.Id = user.Id;
                            UserNetwork.NetworkId = NetworkClient.Id;
                            UserNetwork.Login = user.Login;
                            UserNetwork.Email = user.Email;
                            UserNetwork.Name = user.Name;
                            UserNetwork.Surname = user.Surname;
                            UserNetwork.Patronymic = user.Patronymic;

                            Sender.Send(NetworkClient, "User.Auth.Confirm", UserNetwork, 2);
                            Debug.Log("Пользователь вошёл в систему!");

                            Server.Users.Add(NetworkClient, UserNetwork);

                            return;
                        }
                    }

                    Sender.Send(NetworkClient, "User.Auth.Error", default, 2);
                    return;
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при авторизации пользователя в системе! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "User.Auth.Error", default, 2);
            }
        }
    }
}
