using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Interfaces.Events;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.Register
{
    public class ERegister : IEvent
    {
        public ERegister()
        {
            NetworkDelegates.Add(RegisterUser, "User.Register", 2);
        }

        public void Destruct()
        {
            throw new NotImplementedException();
        }

        private void RegisterUser(MResponse ClientResponse, UserNetwork UserNetwork)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    User user = Package.Unpacking<User>(ClientResponse.DataBytes);

                    if (db.Users.Where(u => u.Email == user.Email).FirstOrDefault() == null 
                        && db.Users.Where(u => u.Login == user.Login).FirstOrDefault() == null)
                    {
                        db.Users.Add(user);
                        db.SaveChanges();
                    }
                    else
                    {
                        Sender.Send(UserNetwork, "User.Register.Error", default, 2);
                        return;
                    }

                    Debug.Log("New user added to database.");

                    Sender.Send(UserNetwork, "User.Register.Confirm", default, 2);
                }
            }
            catch(DbUpdateException ex)
            {
                Debug.LogError("An exception occurred while adding the user to the table. " +
                    "Exeption code:\n " + ex);

                Sender.Send(UserNetwork, "User.Register.Error", default, 2);
            }
        }
    }
}
