using Compo_Shared_Data.Utilites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Data.Windows
{
    public class CheckAutomaticAuthorizate
    {
        // Папка для сохранения
        private static string IsAuthDataPath = @"User\Data\";
        // Название файла
        private static string IsAuthDataFilename = "IsAuth.json";

        /// <summary>
        /// Кеширует данные пользователя в папке приложения.
        /// </summary>
        /// <param name="UserData">Данные пользователя</param>
        public static void Save(bool IsAuth)
        {
            JsonData.Save(IsAuth, IsAuthDataFilename, IsAuthDataPath);
        }

        /// <summary>
        /// Проверяет наличие файла пользователя в папке приложения.
        /// </summary>
        /// <returns>Вернёт True если файл существует</returns>
        public static bool Exists()
        {
            if (JsonData.Exists(IsAuthDataFilename, IsAuthDataPath))
                return true;

            return false;
        }

        /// <summary>
        /// Возвращает данные пользователя из кешированного файла.
        /// </summary>
        /// <returns>Вернёт null в том случае, если файла не существует</returns>
        public static bool Read()
        {
            if (Exists())
                return JsonData.Read<bool>(IsAuthDataFilename, IsAuthDataPath);

            return false;
        }
    }
}
