using Compo_Shared_Data.Utilites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Data.Network
{
    public class ServerData
    {
        private static string ServerDataPath = @"Network/";
        private static string ServerDataFilename = "Server.json";

        public static void Save(string[] ServerData)
        {
            JsonData.Save(ServerData, ServerDataFilename, ServerDataPath);
        }

        public static bool Exists()
        {
            if (JsonData.Exists(ServerDataFilename, ServerDataPath))
                return true;

            return false;
        }

        public static string[] Read()
        {
            if (Exists())
                return JsonData.Read<string[]>(ServerDataFilename, ServerDataPath);
            else
            {
                string[] ServerData = new string[] { "127.0.0.1", "8888" };

                Save(ServerData);

                return ServerData;
            }
        }
    }
}
