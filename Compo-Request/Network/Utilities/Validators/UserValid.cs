using Compo_Request.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Compo_Request.Windows.AlertWindow;

namespace Compo_Request.Network.Utilities.Validators
{
    public class UserValid
    {
        public static bool Email(string Email, CloseEventDelegate CloseEvent = null)
        {
            if (Email.Length == 0 || !StringValid.IsValidEmail(Email))
            {
                new AlertWindow("Ошибка", "Не верно задан формат почты!\r\nПример: MyMail@mail.com", CloseEvent);
                return false;
            }

            return true;
        }

        public static bool Login(string Login, CloseEventDelegate CloseEvent = null)
        {
            if (Login.Length >= 3)
            {
                if (!new Regex("^[a-z|A-Z|0-9|._-]+$").IsMatch(Login))
                {
                    new AlertWindow("Ошибка", "Поле логина не может содержать любые " +
                        "символы кроме чисел и латинских букв!",
                        CloseEvent);

                    return false;
                }
            }
            else
            {
                new AlertWindow("Ошибка", "Логин должен содержать минимум 3 символа!",
                    CloseEvent);

                return false;
            }

            return true;
        }

        public static bool Password(string Password, string ConfirmPassword, CloseEventDelegate CloseEvent = null)
        {
            if (Password.Length >= 6)
            {
                if (Password == ConfirmPassword)
                    return true;
                else
                {
                    new AlertWindow("Ошибка", "Пароли не совпадают!", CloseEvent);

                    return false;
                }
            }
            else
            {
                new AlertWindow("Ошибка", "Пароль должен содержать минимум 6 символов!", CloseEvent);

                return false;
            }
        }

        public static bool Name(string Name, CloseEventDelegate CloseEvent = null)
        {
            if (Name.Length == 0)
            {
                new AlertWindow("Ошибка", "Поле имени не может быть пустым!", CloseEvent);

                return false;
            }
            else if (new Regex("^[0-9]+$").IsMatch(Name))
            {
                new AlertWindow("Ошибка", "Поле имени не может содержать в себе цифры!", CloseEvent);

                return false;
            }

            return true;
        }

        public static bool Surname(string Surname, CloseEventDelegate CloseEvent = null)
        {
            if (Surname.Length == 0)
            {
                new AlertWindow("Ошибка", "Поле отчества не может быть пустым!", CloseEvent);

                return false;
            }
            else if (new Regex("^[0-9]+$").IsMatch(Surname))
            {
                new AlertWindow("Ошибка", "Поле фамилии не может содержать в себе цифры!", CloseEvent);

                return false;
            }

            return true;
        }

        public static bool Patronymic(string Patronymic, CloseEventDelegate CloseEvent = null)
        {
            if (new Regex("^[0-9]+$").IsMatch(Patronymic))
            {
                new AlertWindow("Ошибка", "Поле отчества не может содержать в себе цифры!", CloseEvent);

                return false;
            }

            return true;
        }
    }
}
