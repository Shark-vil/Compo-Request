using Compo_Request.Models;
using Compo_Request.Network.Client;
using Compo_Request.Network.Utilities;
using Compo_Request.Windows.Editor.Windows;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Compo_Request.WindowsLogic.EditorLogic
{
    public class LEditorNetworkActions
    {
        private static DispatcherTimer TimerLocker_LinkUpdate { get; set; }
        private static DispatcherTimer TimerLocker_MethodUpdate { get; set; }

        /// <summary>
        /// Загружает в VirtualRequestDirs список запросов и каталогов
        /// </summary>
        /// <param name="UC">EditorWebRequestControl</param>
        public static void FirstLoad_ListViewCollection(EditorWebRequestControl UC)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var MB_WebRequests = Package.Unpacking<MBinding_WebRequest[]>(ServerResponse.DataBytes);

                foreach (var MB_WebRequest in MB_WebRequests)
                {
                    //if (UC.RequestDirectory == null || MB_WebRequest.Item.Id != UC.RequestDirectory.WebRequestId)
                    //    return;

                    if (!Array.Exists(UC.VirtualRequestDirs.ToArray(),
                        x => x.Id == MB_WebRequest.Directory.Id))
                            UC.VirtualRequestDirs.Add(new ModelRequestDirectory
                            {
                                Id = MB_WebRequest.Directory.Id,
                                RequestTitle = MB_WebRequest.Item.Title,
                                Title = MB_WebRequest.Directory.Title,
                                RequestMethod = MB_WebRequest.Item.Method,
                                WebRequest = MB_WebRequest.Item.Link,
                                WebRequestId = MB_WebRequest.Item.Id
                            });

                    if (UC.RequestDirectory != null && UC.RequestDirectory.WebRequestId == MB_WebRequest.Item.Id)
                    {
                        WebRequestParamsItem[] RequestsArray = UC.WebRequestItems.ToArray();
                        foreach (var ItemParam in MB_WebRequest.Params)
                            if (!Array.Exists(RequestsArray,
                                x => x.WebRequestItemId == ItemParam.WebRequestItemId))
                                    UC.WebRequestItems.Add(ItemParam);

                        //UC.GeneralLogic.RequestMethod = MB_WebRequest.Item.Method;
                    }
                }

                UC.ListViewCollection.Refresh();
                UC.ListView_WebRequests.Items.Refresh();

            }, UC.Dispatcher, UC.GeneralLogic.MBinding_WebRequest_Get_Uid,
                "WebRequestItem.MBinding_WebRequest.Get", UC.GeneralLogic.UserControl_Uid, true);
        }

        /// <summary>
        /// Обновляет строку с ссылкой
        /// </summary>
        /// <param name="UC">EditorWebRequestControl</param>
        public static void LinkUpdate_Confirm(EditorWebRequestControl UC)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (UC.RequestDirectory == null)
                    return;

                var RequestItem = Package.Unpacking<WebRequestItem>(ServerResponse.DataBytes);

                if (RequestItem.Id != UC.RequestDirectory.WebRequestId)
                    return;

                TimerLocker_LinkUpdate?.Stop();

                UC.TextBox_RequestLink.IsReadOnly = true;
                UC.TextBox_RequestLink.IsEnabled = false;

                TimerLocker_LinkUpdate = CustomTimer.Create(delegate (object sender, EventArgs e)
                {
                    UC.Dispatcher.Invoke(() =>
                    {
                        UC.TextBox_RequestLink.IsEnabled = true;
                        UC.TextBox_RequestLink.IsReadOnly = false;
                    });
                }, new TimeSpan(0, 0, 1));

                UC.EditorRequestData.RequestLink = RequestItem.Link;

                for (int i = 0; i < UC.VirtualRequestDirs.Count; i++)
                {
                    ModelRequestDirectory RequestDirItem = UC.VirtualRequestDirs[i];
                    if (RequestDirItem.WebRequestId == RequestItem.Id)
                    {
                        RequestDirItem.WebRequest = RequestItem.Link;
                        //RequestDirItem.RequestMethod = RequestItem.Method;

                        UC.VirtualRequestDirs[i] = RequestDirItem;
                        break;
                    }
                }

                //UC.UpdateDataGrid_OnTextBox();

            }, UC.Dispatcher, -1, "WebRequestItem.Update.Link.Confirm", UC.GeneralLogic.UserControl_Uid, true);
        }

        /// <summary>
        /// Устанавливает новый метод в списке методов запроса
        /// </summary>
        /// <param name="UC">EditorWebRequestControl</param>
        public static void MethodUpdate_Confirm(EditorWebRequestControl UC)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (UC.RequestDirectory == null)
                    return;

                var RequestItem = Package.Unpacking<WebRequestItem>(ServerResponse.DataBytes);

                if (RequestItem.Id != UC.RequestDirectory.WebRequestId)
                    return;

                if (RequestItem.Method != UC.GeneralLogic.RequestMethod)
                {
                    TimerLocker_MethodUpdate?.Stop();

                    UC.TextBox_RequestLink.IsReadOnly = true;
                    UC.ComboBox_RequestType.IsReadOnly = true;
                    UC.ComboBox_RequestType.IsEnabled = false;

                    TimerLocker_MethodUpdate = CustomTimer.Create(delegate (object sender, EventArgs e)
                    {
                        UC.Dispatcher.Invoke(() =>
                        {
                            UC.ComboBox_RequestType.IsEnabled = true;
                            UC.TextBox_RequestLink.IsReadOnly = false;
                            UC.TextBox_RequestLink.IsReadOnly = false;
                        });
                    }, new TimeSpan(0, 0, 1));

                    int Index = UC.ComboBox_RequestType.Items.IndexOf(RequestItem.Method);
                    if (Index != -1 && UC.ComboBox_RequestType.SelectedIndex != Index)
                    {
                        UC.ComboBox_RequestType.SelectedIndex = Index;
                        UC.GeneralLogic.RequestMethod_SetComboBoxItem(UC.ComboBox_RequestType);

                        for (int i = 0; i < UC.VirtualRequestDirs.Count; i++)
                        {
                            ModelRequestDirectory RequestDirItem = UC.VirtualRequestDirs[i];
                            if (RequestDirItem.WebRequestId == RequestItem.Id)
                            {
                                RequestDirItem.RequestMethod = RequestItem.Method;
                                UC.VirtualRequestDirs[i] = RequestDirItem;
                                break;
                            }
                        }

                        UC.ListView_WebRequests.Items.Refresh();
                    }
                }

            }, UC.Dispatcher, -1, "WebRequestItem.Update.Method.Confirm", UC.GeneralLogic.UserControl_Uid, true);
        }

        /// <summary>
        /// Обновляет список параметров запроса
        /// </summary>
        /// <param name="UC">EditorWebRequestControl</param>
        public static void RequestParamsUpdate_Confirm(EditorWebRequestControl UC)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (UC.RequestDirectory == null)
                    return;

                var RequestParamsItems = Package.Unpacking<WebRequestParamsItem[]>(ServerResponse.DataBytes);

                for (int i = 0; i < RequestParamsItems.Length; i++)
                {
                    bool IsEdit = false;
                    WebRequestParamsItem ParamsItem = RequestParamsItems[i];

                    if (ParamsItem.WebRequestItemId != UC.RequestDirectory.WebRequestId)
                        return;

                    if (UC.WebRequestItems.Count != 0)
                    {
                        for (int k = 0; k < UC.WebRequestItems.Count; k++)
                        {
                            if (UC.WebRequestItems[k].Id == ParamsItem.Id
                            || UC.WebRequestItems[k].Key == ParamsItem.Key)
                            {
                                UC.WebRequestItems[k] = ParamsItem;
                                IsEdit = true;
                                break;
                            }
                        }

                        if (!IsEdit)
                            UC.WebRequestItems.Add(ParamsItem);
                    }
                }

                //UC.WebRequestItems = new ObservableCollection<WebRequestParamsItem>(RequestParamsItems);
                //UC.DataGrid_FormRequestData.Items.Refresh();

            }, UC.Dispatcher, -1, "WebRequestParamsItem.Update.Confirm", UC.GeneralLogic.UserControl_Uid, true);
        }

        /// <summary>
        /// Загружает параметры запроса и заполняет список
        /// </summary>
        /// <param name="u">EditorWebRequestControl</param>
        /// <param name="NetworkUid">Идентификатор отправленного запроса</param>
        public static void RequestParamsItemsGet_Confirm(EditorWebRequestControl u, int NetworkUid)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (u.RequestDirectory == null)
                    return;

                var RequestParams = Package.Unpacking<WebRequestParamsItem[]>(ServerResponse.DataBytes);

                foreach (var RequestParam in RequestParams)
                {
                    if (RequestParam.WebRequestItemId != u.RequestDirectory.WebRequestId)
                        return;

                    if (!Array.Exists(u.WebRequestItems.ToArray(), x => x.Id == RequestParam.Id))
                        if (RequestParam.WebRequestItemId == u.RequestDirectory.WebRequestId)
                            u.WebRequestItems.Add(RequestParam);
                }

            }, u.Dispatcher, NetworkUid, "WebRequestParamsItem.Get.Confirm", u.GeneralLogic.UserControl_Uid);
        }

        public static void RequestParamsDelete_Confirm(EditorWebRequestControl u)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (u.RequestDirectory == null)
                    return;

                var WebParamsId = Package.Unpacking<int>(ServerResponse.DataBytes);

                for (int i = 0; i < u.WebRequestItems.Count; i++)
                {
                    if (u.WebRequestItems[i].Id == WebParamsId)
                    {
                        u.WebRequestItems.RemoveAt(i);
                        break;
                    }
                }

                u.DataGrid_FormRequestData.Items.Refresh();

            }, u.Dispatcher, -1, "WebRequestParamsItem.Delete.Confirm", u.GeneralLogic.UserControl_Uid, true);
        }

        public static void RequestParamsDeleteAll_Confirm(EditorWebRequestControl u)
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                if (u.RequestDirectory == null)
                    return;

                var WebRequestId = Package.Unpacking<int>(ServerResponse.DataBytes);

                if  (u.RequestDirectory.WebRequestId == WebRequestId)
                {
                    u.WebRequestItems.Clear();
                    u.DataGrid_FormRequestData.Items.Refresh();
                }

            }, u.Dispatcher, -1, "WebRequestParamsItem.Delete.All.Confirm", u.GeneralLogic.UserControl_Uid, true);
        }
    }
}
