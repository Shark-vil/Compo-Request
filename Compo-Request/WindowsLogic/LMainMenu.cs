﻿using Compo_Request.Network.Interfaces;
using Compo_Request.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Compo_Request.WindowsLogic
{
    public class LMainMenu
    {
        private MainMenuWindow _MainMenuWindow;
        private static ICustomPage SelectedFramePage;

        /// <summary>
        /// Конструктор логики.
        /// </summary>
        /// <param name="_MainMenuWindow">Главное меню</param>
        public LMainMenu(MainMenuWindow _MainMenuWindow)
        {
            this._MainMenuWindow = _MainMenuWindow;
        }

        /// <summary>
        /// Устанавливает страницу фрейма окна.
        /// </summary>
        /// <param name="FramePage">Страница</param>
        internal void SetPage(ICustomPage FramePage)
        {
            if (SelectedFramePage != null)
                SelectedFramePage.ClosePage();

            _MainMenuWindow.Frame_Main.Content = FramePage;
            FramePage.OpenPage();

            SelectedFramePage = FramePage;
        }
    }
}
