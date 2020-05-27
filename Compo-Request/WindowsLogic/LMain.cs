using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Request.Network.Utilities.Validators;
using Compo_Request.Windows;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Compo_Request.WindowsLogic
{
    public class LMain
    {
        // Главное окно
        private MainWindow _MainWindow;
        // Поток главного окна
        private Dispatcher _Dispatcher;
        
        // Таймер входа в систему
        internal DispatcherTimer AuthTimer;

        /// <summary>
        /// Для хранения данных пользователя.
        /// UserData[0] - Логин
        /// UserData[1] - Пароль
        /// </summary>
        internal string[] UserData;

        /// <summary>
        /// Конструктор сщуности логики главного окна.
        /// </summary>
        /// <param name="_MainWindow">Главное окно</param>
        public LMain(MainWindow _MainWindow)
        {
            this._MainWindow = _MainWindow;
            // Регистрация потока окна
            _Dispatcher = _MainWindow.Dispatcher;
        }

        /// <summary>
        /// Осуществляет вход в систему при наличии файла авторизации.
        /// </summary>
        internal void AutomaticAuthorizate()
        {
            if (Data.Windows.AutomaticAuthorizate.Exists())
            {
                _MainWindow.CheckBox_AutoAuth.IsChecked = true;

                UserData = Data.Windows.AutomaticAuthorizate.Read();
                _MainWindow.TextBox_LoginOrEmail.Text = UserData[0];
                _MainWindow.PasswordBox_Password.Password = UserData[1];

                UserAuthorization(UserData);
            }
        }

        /// <summary>
        /// Регистрирует сетевые события.
        /// </summary>
        internal void NetworkEventsLoad()
        {
            /**
             * Вызывается при удачной авторизации в системе.
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                AuthTimer?.Stop();

                var NetUser = Package.Unpacking<MUserNetwork>(ServerResponse.DataBytes);

                UserInfo.NetworkSelf = NetUser;
                UserInfo.NetworkUsers.Add(NetUser);

                _MainWindow._MainMenuWindow = new MainMenuWindow(_MainWindow);
                _MainWindow.Hide();
                _MainWindow._MainMenuWindow.Show();

                _MainWindow.AuthElements_Unblock();

                if ((bool)_MainWindow.CheckBox_AutoAuth.IsChecked)
                    Data.Windows.AutomaticAuthorizate.Save(UserData);

            }, _Dispatcher, 2, "User.Auth.Confirm", "MainWindow");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var NetUsers = Package.Unpacking<MUserNetwork[]>(ServerResponse.DataBytes);

                Debug.Log($"Получен список пользователей в количестве {NetUsers.Length} записей");

                var NewUsers = new List<MUserNetwork>();
                foreach (var NetUser in NetUsers)
                    if (!Array.Exists(UserInfo.NetworkUsers.ToArray(), x => x.NetworkId == NetUser.NetworkId))
                    {
                        UserInfo.NetworkUsers.Add(NetUser);
                        Debug.Log($"Пользователь {NetUser.NetworkId} был добавлен в список.");
                    }

            }, _Dispatcher, -1, "Users.Update", "MainWindow");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var NetUser = Package.Unpacking<MUserNetwork>(ServerResponse.DataBytes);

                if (!Array.Exists(UserInfo.NetworkUsers.ToArray(), x => x.NetworkId == NetUser.NetworkId))
                {
                    UserInfo.NetworkUsers.Add(NetUser);
                    Debug.Log($"Пользователь {NetUser.NetworkId} был добавлен в список.");
                }

            }, _Dispatcher, -1, "Users.Add", "MainWindow");

            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var NetworkId = Package.Unpacking<string>(ServerResponse.DataBytes);

                MUserNetwork UserNetwork = UserInfo.NetworkUsers.Find(x => x.NetworkId == NetworkId);

                if (UserNetwork != null)
                {
                    if (UserNetwork.NetworkId == UserInfo.NetworkSelf.NetworkId)
                    {
                        UserInfo.NetworkSelf = null;
                        Debug.Log("Собственный компонент пользователя был удалён.");
                        _MainWindow._MainMenuWindow?.Close();
                    }

                    UserInfo.NetworkUsers.Remove(UserNetwork);
                    Debug.Log($"Пользователь {NetworkId} был удалён из списка.");
                }

            }, _Dispatcher, -1, "Users.Remove", "MainWindow");

            /**
             * Вызывается при неудачной авторизации в системе.
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                AuthTimer?.Stop();

                if (StringValid.IsValidEmail(_MainWindow.TextBox_LoginOrEmail.Text))
                    new AlertWindow("Ошибка", "Не верно указана почта или пароль!");
                else
                    new AlertWindow("Ошибка", "Не верно указан логин или пароль!");

                _MainWindow.AuthElements_Unblock();

            }, _Dispatcher, 2, "User.Auth.Error", "MainWindow");
        }

        /// <summary>
        /// Выгружает сетевые события.
        /// </summary>
        internal void NetworkEventsUnload()
        {
            NetworkDelegates.RemoveByUniqueName("MainWindow");
        }

        /// <summary>
        /// Осуществляет вход в систему.
        /// </summary>
        /// <param name="UserData">Данные пользователя</param>
        /// <returns>Вернёт True в случае успешной отправки запроса на сервер</returns>
        internal bool UserAuthorization(string[] UserData)
        {
            if (Sender.SendToServer("User.Auth", UserData, 1))
            {
                AuthTimer = new DispatcherTimer();
                AuthTimer.Tick += new EventHandler(AuthorizateTimeout);
                AuthTimer.Interval = new TimeSpan(0, 0, 5);
                AuthTimer.Start();

                _MainWindow.AuthElements_Block();

                return true;
            }
            else
            {
                new AlertWindow("Ошибка", "Не удалось соединиться с сервером.\n" +
                    "Возможно сервер выключен или присутствуют неполадки в вашем интернет-соединении.",
                    _MainWindow.AuthElements_Unblock);
            }

            return false;
        }

        /// <summary>
        /// Событие вызывающееся по окончанию пяти секунд работы таймера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthorizateTimeout(object sender, EventArgs e)
        {
            AuthTimer?.Stop();

            new AlertWindow("Ошибка", "Время ожидания ответа от сервера истекло.\n" +
                "Возможно введены некорректные данные.",
                _MainWindow.AuthElements_Unblock);
        }
    }
}
