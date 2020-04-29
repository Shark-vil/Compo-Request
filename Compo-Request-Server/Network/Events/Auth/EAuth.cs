using Compo_Request_Server.Network.Database;
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
    public class EAuth : IEvent
    {
        public EAuth()
        {
            NetworkDelegates.Add(AuthUser, "User.Auth", 1);
        }

        public void Destruct()
        {
            throw new NotImplementedException();
        }

        private void AuthUser(MResponse ClientResponse, UserNetwork UserNetwork)
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
                            var NetUser = new MUserNetwork();
                            NetUser.Uid = UserNetwork.Id;
                            NetUser.Login = user.Login;
                            NetUser.Email = user.Email;

                            Sender.Send(UserNetwork, "User.Auth.Confirm", NetUser, 2);
                            Debug.Log("User is logged in!");

                            return;
                        }
                    }

                    Sender.Send(UserNetwork, "User.Auth.Error", default, 2);
                    return;
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("An exception was raised while trying to retrieve user data. " +
                    "Exeption code:\n " + ex);

                Sender.Send(UserNetwork, "User.Auth.Error", default, 2);
            }
        }
    }
}
