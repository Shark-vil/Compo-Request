using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Server
{
    public class Users
    {
        public class UserStructure
        {
            public MNetworkClient NetworkClient;
            public MUserNetwork NetworkUser;
        }

        public static List<UserStructure> ActiveUsersMoreInfo = new List<UserStructure>();
        public static List<MUserNetwork> ActiveUsers = new List<MUserNetwork>();

        public static void Add(MNetworkClient NetworkClient, MUserNetwork NetworkUser)
        {
            ActiveUsersMoreInfo.Add(new UserStructure
            {
                NetworkClient = NetworkClient,
                NetworkUser = NetworkUser
            });

            ActiveUsers.Add(NetworkUser);

            Debug.Log("Пользователь добавлен в системный список");

            Sender.Send(NetworkClient, "Users.Update", ActiveUsers.ToArray());
            Sender.SendOmit(NetworkClient, "Users.Add", NetworkClient);
        }

        public static UserStructure GetUsById(string ClientId)
        {
            return ActiveUsersMoreInfo.Find(x => x.NetworkClient.Id == ClientId);
        }

        public static MUserNetwork GetById(string ClientId)
        {
            return ActiveUsers.Find(x => x.Id == ClientId);
        }

        public static void Remove(MNetworkClient NetworkClient)
        {
            RemoveById(NetworkClient.Id);
        }

        public static void RemoveById(string ClientId)
        {
            if (ActiveUsersMoreInfo.Exists(x => x.NetworkClient.Id == ClientId))
            {
                MNetworkClient NetworkClient = ActiveUsersMoreInfo.Find(x => x.NetworkClient.Id == ClientId).NetworkClient;

                ActiveUsersMoreInfo.RemoveAll(x => x.NetworkClient == NetworkClient);
                ActiveUsers.RemoveAll(x => x.Id == ClientId);

                Debug.Log("Пользователь удалён из системного списка");

                Sender.SendOmit(NetworkClient, "Users.Remove", ClientId);
            }
        }
    }
}
