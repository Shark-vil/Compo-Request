using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Request.Windows;
using Compo_Request.Windows.UserRegister;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Compo_Request.WindowsLogic.UserRegister
{
    public class LRegister
    {
        // Окно регистрации
        private RegisterWindow _RegisterWindow;
        // Поток окна
        private Dispatcher _Dispatcher;

        // Таймер регистрации в системе
        internal DispatcherTimer RegisterTimer;

        /// <summary>
        /// Конструктор логики.
        /// </summary>
        /// <param name="_RegisterWindow">Окно регистрации</param>
        public LRegister(RegisterWindow _RegisterWindow)
        {
            this._RegisterWindow = _RegisterWindow;
            _Dispatcher = _RegisterWindow.Dispatcher;
        }

        /// <summary>
        /// Регистрирует сетевые события.
        /// </summary>
        internal void NetworkEventsLoad()
        {
            /**
             * Вызывается при удачной регистрации на сервере.
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                RegisterTimer?.Stop();

                new AlertWindow("Успешная регистрация", "Вы были зарегистрированы в системе.",
                    _RegisterWindow.RegisterWindow_CloseEvvent);

            }, _Dispatcher, 2, "User.Register.Confirm", "RegisterWindow");

            /**
             * Вызывается при неудачной регистрации на сервере.
             */
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                RegisterTimer?.Stop();

                new AlertWindow("Ошибка", "Такой пользователь уже существует!",
                    _RegisterWindow.RegisterElements_Unblock);

            }, _Dispatcher, 2, "User.Register.Error", "RegisterWindow");
        }

        /// <summary>
        /// Выгружает сетевые события.
        /// </summary>
        internal void NetworkEventsUnload()
        {
            NetworkDelegates.RemoveByUniqueName("RegisterWindow");
        }

        /// <summary>
        /// Регестирует аккаунт в системе.
        /// </summary>
        /// <param name="UserEntity">Сущность пользователя</param>
        internal void RegisterAccount(User UserEntity)
        {
            if (Sender.SendToServer("User.Register", UserEntity, 2))
            {
                RegisterTimer = new DispatcherTimer();
                RegisterTimer.Tick += new EventHandler(RegisterForm_UnblockTimer);
                RegisterTimer.Interval = new TimeSpan(0, 0, 5);
                RegisterTimer.Start();
            }
            else
            {
                var Alert = new AlertWindow("Ошибка", "Не удалось соединиться с сервером.\n" +
                    "Возможно сервер выключен или присутствуют неполадки в вашем интернет-соединении.",
                    _RegisterWindow.RegisterElements_Unblock);
            }
        }

        /// <summary>
        /// Событие вызывающееся по окончанию работы таймера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterForm_UnblockTimer(object sender, EventArgs e)
        {
            RegisterTimer?.Stop();

            var Alert = new AlertWindow("Ошибка", "Время ожидания ответа от сервера истекло.\n" +
                "Возможно введены некорректные данные.",
                _RegisterWindow.RegisterElements_Unblock);
        }
    }
}
