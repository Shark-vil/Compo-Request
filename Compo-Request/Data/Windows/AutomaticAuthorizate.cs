using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Data.Windows
{
    public class AutomaticAuthorizate
    {
        private static string UserDataPath = @"User\Data\";
        private static string UserDataFilename = "UserAuthData.json";

        public static void Save(string[] UserData)
        {
            JsonData.Save(UserData, UserDataFilename, UserDataPath);
        }

        public static bool Exists()
        {
            if (JsonData.Exists(UserDataFilename, UserDataPath))
                return true;

            return false;
        }

        public static string[] Read()
        {
            return JsonData.Read<string[]>(UserDataFilename, UserDataPath);
        }
    }
}
