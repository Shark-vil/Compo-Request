using Compo_Request.Windows.Editor.Windows;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Compo_Request.Utilities;
using Compo_Request.Network.Utilities;
using System.Linq;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.Debugging;
using System.Windows.Documents;
using System.Windows;
using System.IO;
using Compo_Request.Models;
using Newtonsoft.Json;

namespace Compo_Request.WindowsLogic.EditorLogic
{
    public class LEditorGeneral
    {
        private EditorWebRequestControl u;

        public bool IsCopy { get; set; }
        public string HeaderName { get; set; }
        public string RequestMethod { get; set; }
        public string RequestLink { get; set; }

        public string UserControl_Uid { get; set; }
        public int MBinding_WebRequest_Get_Uid { get; set; }

        public LEditorGeneral(EditorWebRequestControl u)
        {
            this.u = u;
        }

        public void RequestMethod_SetComboBoxItem(ComboBox ComboBox_RequestType)
        {
            RequestMethod = ComboBox_RequestType.SelectedItem.ToString();
        }

        public void SetHeaderName(HeaderedItemViewModel TabItemView)
        {
            if (IsCopy)
                TabItemView.Header = "[Copy] " + HeaderName + " - " + RequestMethod;
            else
                TabItemView.Header = HeaderName + " - " + RequestMethod;
        }

        public void WebRequestSend()
        {
            RequestElements_Block();

            var t = new Thread(new ThreadStart(delegate ()
            {
                u.Dispatcher.Invoke(delegate ()
                {
                    WebResponseTemplate WebResponce;

                    try
                    {
                        WebResponce = ToolWebRequest.RestRequest(
                            u.GeneralLogic.RequestMethod,
                            u.GeneralLogic.RequestLink,
                            u.WebRequestItems
                        );
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError("Возникло исключение при отправке запроса. Код ошибки:\n" + ex);

                        RequestElements_Unblock();
                        return; 
                    }

                    if (WebResponce != null && WebResponce.Response != null)
                    {
                        if (u.RequestDirectory != null)
                        {
                            var HistoryItem = new WebRequestHistory();
                            HistoryItem.Title = HeaderName;
                            HistoryItem.Link = RequestLink;
                            HistoryItem.Method = RequestMethod;
                            HistoryItem.ProjectId = ProjectData.SelectedProject.Id;
                            HistoryItem.WebRequestItemId = u.RequestDirectory.WebRequestId;
                            HistoryItem.ResponseDate = DateTime.Now;
                            HistoryItem.ResponseResult = WebResponce.Response;
                            HistoryItem.ParametrsInJson = JsonConvert.SerializeObject(
                                u.WebRequestItems.ToArray(), Formatting.Indented);

                            Sender.SendToServer("RequestsHistory.Add", HistoryItem);
                        }

                        SetViews(WebResponce.Response);
                    }

                    Debug.Log("\n" + WebResponce.Info + "\n");

                    RequestElements_Unblock();
                });
            }));
            t.Start();
        }

        public void SetViews(string WebContent)
        {
            try
            {
                u.TextViewer.Document.Blocks.Clear();
                u.TextViewer.Document.Blocks.Add(new Paragraph(new Run(WebContent)));
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникло исключение при попытке загрузить " +
                    "ответ в виде JSON строки. Код ошибки:\n" + ex);
            }

            try
            {
                u.JsonViewer.Load(WebContent);
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникло исключение при попытке загрузить " +
                    "ответ в виде JSON строки. Код ошибки:\n" + ex);
            }

            try
            {
                u.XmlViewer.Load(WebContent);
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникло исключение при попытке загрузить " +
                    "ответ в виде XML файла. Код ошибки:\n" + ex);
            }

            try
            {
                u.HtmlViewer.NavigateToString(WebContent);
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникло исключение при попытке загрузить " +
                    "ответ в виде HTML страницы. Код ошибки:\n" + ex);
            }
        }

        private void RequestElements_Block()
        {
            u.DataGrid_FormRequestData.IsEnabled = false;
            u.TextBox_RequestLink.IsEnabled = false;
            u.Button_SendRequest.IsEnabled = false;
            u.Button_SaveRequest.IsEnabled = false;
        }

        private void RequestElements_Unblock()
        {
            u.DataGrid_FormRequestData.IsEnabled = true;
            u.TextBox_RequestLink.IsEnabled = true;
            u.Button_SendRequest.IsEnabled = true;
            u.Button_SaveRequest.IsEnabled = true;
        }

        public void LoadRequestDirectory(EditorWebRequestControl UC, int NetworkUid)
        {
            if (UC.RequestDirectory != null)
            {
                UC.GeneralLogic.HeaderName = UC.RequestDirectory.RequestTitle;
                UC.GeneralLogic.RequestMethod = UC.RequestDirectory.RequestMethod;
                UC.GeneralLogic.SetHeaderName(UC.TabItemView);

                int Index = UC.ComboBox_RequestType.Items.IndexOf(UC.GeneralLogic.RequestMethod);
                if (Index != -1)
                    UC.ComboBox_RequestType.SelectedIndex = Index;

                UC.TextBox_RequestLink.Text = UC.RequestDirectory.WebRequest;

                Sender.SendToServer("WebRequestParamsItem.Get", UC.RequestDirectory.WebRequestId, NetworkUid);
            }
        }

        /// <summary>
        /// Проверяет текущий выбранный метод, и перестраивает ссылку если это GET.
        /// </summary>
        public string RequestLinkChanged(string RequestLink)
        {
            string SelectRequestLink = RequestLink;

            if (u == null)
                return RequestLink;

            if (u.ComboBox_RequestType == null || u.ComboBox_RequestType.Items.Count == 0)
                return RequestLink;

            if (u.ComboBox_RequestType.SelectedItem.ToString() == "GET")
            {
                if (SelectRequestLink.Trim().Length != 0)
                {
                    int GetPos = SelectRequestLink.IndexOf("?", 0);

                    if (GetPos != -1)
                    {
                        SelectRequestLink = DestructGetRequest(SelectRequestLink);
                        SelectRequestLink = ConstructGetRequest(SelectRequestLink);
                    }
                    /*
                    else
                    {
                        SelectRequestLink = ConstructGetRequest(SelectRequestLink);
                    }
                    */
                }
            }
            else
            {
                SelectRequestLink = DestructGetRequest(SelectRequestLink);
            }

            return SelectRequestLink;
        }

        /// <summary>
        /// Перестраивает ссылку убирая GET параметры, если они есть.
        /// </summary>
        /// <param name="RequestLink">Ссылка с GET параметрами</param>
        /// <returns>Чистая ссылка</returns>
        public string DestructGetRequest(string RequestLink)
        {
            int GetPos = u.EditorRequestData.RequestLink.IndexOf("?", 0);

            if (GetPos != -1)
            {
                RequestLink = RequestLink.Substring(0, GetPos);
            }

            return RequestLink;
        }

        /// <summary>
        /// Перестраивает ссылку под GET запрос, добавляя параметры для отправки.
        /// </summary>
        /// <param name="RequestLink">Ссылка без параметров</param>
        /// <returns>Ссылка с параметрами</returns>
        public string ConstructGetRequest(string RequestLink)
        {
            RequestLink = RequestLink.Trim();

            if (RequestLink.IndexOf('?') == -1)
                RequestLink += "?";

            for (int i = 0; i < u.WebRequestItems.Count; i++)
            {
                var FormItem = u.WebRequestItems[i];
                if (i != u.WebRequestItems.Count - 1
                    && (FormItem.Key != null && FormItem.Key.Trim() != string.Empty)
                    && (FormItem.Value != null && FormItem.Value.Trim() != string.Empty))
                    RequestLink += $"{FormItem.Key}={FormItem.Value}&";
                else
                {
                    if ((FormItem.Key != null && FormItem.Key.Trim() != string.Empty)
                        && (FormItem.Value != null && FormItem.Value.Trim() != string.Empty))
                        RequestLink += $"{FormItem.Key}={FormItem.Value}";
                    else if ((FormItem.Key != null && FormItem.Key.Trim() != string.Empty))
                        RequestLink += $"{FormItem.Key}";
                }
            }

            return RequestLink;
        }

        public void DataGrid_UpdateParamsBroadcast()
        {
            if (u.WebRequestItems.Count != 0 && u.RequestDirectory != null)
            {
                var WebRequestBinding = new MBinding_WebRequestSaver
                {
                    Item = new WebRequestItem
                    {
                        Id = u.RequestDirectory.WebRequestId
                    },
                    Params = u.WebRequestItems.ToArray()
                };

                Sender.SendToServer("WebRequestParamsItem.Update", WebRequestBinding);
            }
        }

        public void DataGrid_RemoveParamsById(int Id)
        {
            if (Id != 0)
                Sender.SendToServer("WebRequestParamsItem.Delete", Id);
        }

        public void DataGrid_RemoveParamsAll(int WebRequestId)
        {
            if (WebRequestId != 0)
                Sender.SendToServer("WebRequestParamsItem.Delete.All", WebRequestId);
        }
    }
}
