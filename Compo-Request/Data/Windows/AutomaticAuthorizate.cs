using Compo_Shared_Data.Utilites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Data.Windows
{
    public class AutomaticAuthorizate
    {
        // Папка для сохранения
        private static string UserDataPath = @"User\Data\";
        // Название файла
        private static string UserDataFilename = "UserAuthData.json";

        /// <summary>
        /// Кеширует данные пользователя в папке приложения.
        /// </summary>
        /// <param name="UserData">Данные пользователя</param>
        public static void Save(string[] UserData)
        {
            JsonData.Save(UserData, UserDataFilename, UserDataPath);
        }

        /// <summary>
        /// Проверяет наличие файла пользователя в папке приложения.
        /// </summary>
        /// <returns>Вернёт True если файл существует</returns>
        public static bool Exists()
        {
            if (JsonData.Exists(UserDataFilename, UserDataPath))
                return true;

            return false;
        }

        /// <summary>
        /// Возвращает данные пользователя из кешированного файла.
        /// </summary>
        /// <returns>Вернёт null в том случае, если файла не существует</returns>
        public static string[] Read()
        {
            if (Exists())
                return JsonData.Read<string[]>(UserDataFilename, UserDataPath);

            return null;
        }
    }
}
