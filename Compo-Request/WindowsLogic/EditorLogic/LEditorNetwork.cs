using Compo_Request.Network.Utilities;
using Compo_Request.Windows.Editor.Windows;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Compo_Request.Windows.Editor.Windows.EditorRequestRenameWindow;
using static Compo_Request.Windows.Editor.Windows.EditorRequestSaverWindow;

namespace Compo_Request.WindowsLogic.EditorLogic
{
    public class LEditorNetwork
    {
        public static void SaveRequest(string RequestLink, string RequestMethod, WebRequestParamsItem[] WR_ParamsItem,
            EventEditorRequestSaver SaveConfirm = null, EventEditorRequestSaver CancelEvent = null)
        {
            var EditorRequestSaver = new EditorRequestSaverWindow(RequestLink, RequestMethod, WR_ParamsItem, SaveConfirm, CancelEvent);
            EditorRequestSaver.Show();
        }

        public static void ReanmeRequest(string RequestLink, string RequestMethod, ModelRequestDirectory RequestDirectory,
            EventEditorRequestRename SaveConfirm = null, EventEditorRequestRename CancelEvent = null)
        {
            var EditorRequestSaver = new EditorRequestRenameWindow(RequestLink, RequestMethod, RequestDirectory, SaveConfirm, CancelEvent);
            EditorRequestSaver.Show();
        }
    }
}
