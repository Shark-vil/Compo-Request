using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compo_Shared_Data.Utilites
{
    public class JsonData
    {
        public static string MainDataDirectory = @"Data/";

        public static string GetAppDir()
        {
            string AppDir = Directory.GetCurrentDirectory();

            return AppDir.Replace('\\', '/') + '/' + MainDataDirectory;
        }

        public static bool Exists(string FileName, string DataPath)
        {
            string MainDir = GetAppDir();

            if (!Directory.Exists(MainDir))
                return false;

            string JsonPath = MainDir + DataPath;

            if (!Directory.Exists(JsonPath))
                return false;

            string JsonFullPath = NormalizeFullPath(JsonPath, FileName);

            if (!File.Exists(JsonFullPath))
                return false;

            return true;
        }

        public static void Save(object DataObject, string FileName, string DataPath)
        {
            string MainDir = GetAppDir();

            if (!Directory.Exists(MainDir))
                Directory.CreateDirectory(MainDir);

            string JsonString = JsonConvert.SerializeObject(DataObject);
            string JsonPath = MainDir + DataPath;

            if (!Directory.Exists(JsonPath))
                Directory.CreateDirectory(JsonPath);

            string JsonFullPath = NormalizeFullPath(JsonPath, FileName);

            using (var StreamFile = File.CreateText(JsonFullPath))
            {
                JsonSerializer Serializer = new JsonSerializer();
                Serializer.Serialize(StreamFile, DataObject);
            }
        }

        public static T Read<T>(string FileName, string DataPath)
        {
            string MainDir = GetAppDir();

            string JsonPath = MainDir + DataPath;

            if (Directory.Exists(JsonPath))
            {
                string JsonFullPath = NormalizeFullPath(JsonPath, FileName);

                if (File.Exists(JsonFullPath))
                {
                    using (var StreamFile = File.OpenText(JsonFullPath))
                    {
                        JsonSerializer Serializer = new JsonSerializer();
                        return (T)Serializer.Deserialize(StreamFile, typeof(T));
                    }
                }
            }

            return default;
        }

        private static string NormalizeFullPath(string JsonPath, string FileName)
        {
            if (JsonPath[JsonPath.Length - 1] == '\\' || JsonPath[JsonPath.Length - 1] == '/')
                return JsonPath + "/" + FileName;
            else
                return JsonPath + FileName;
        }
    }
}
