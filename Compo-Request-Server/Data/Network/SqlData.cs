using Compo_Shared_Data.Utilites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Data.Network
{
    public class SqlData
    {
        private static string SqlDataPath = @"Sql/";
        private static string SqlDataFilename = "ConnectInfo.json";

        public static void Save(string[] ServerData)
        {
            JsonData.Save(ServerData, SqlDataFilename, SqlDataPath);
        }

        public static bool Exists()
        {
            if (JsonData.Exists(SqlDataFilename, SqlDataPath))
                return true;

            return false;
        }

        public static string[] Read()
        {
            if (Exists())
                return JsonData.Read<string[]>(SqlDataFilename, SqlDataPath);
            else
            {
                string[] ServerData = new string[] { "localhost", "compo-request", "HYOuv8pBtMdMgVlp", "compo-request" };

                Save(ServerData);

                return ServerData;
            }
        }
    }
}
