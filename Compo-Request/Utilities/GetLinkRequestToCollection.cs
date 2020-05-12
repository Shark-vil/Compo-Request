using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Compo_Request.Utilities
{
    public class GetLinkRequestToCollection
    {
        public static ObservableCollection<WebRequestParamsItem> GetCollection(string WebLink)
        {
            try
            {
                int QuestionMark = CutQuestionMark(WebLink);
                if (QuestionMark != -1)
                {
                    string[] Nbsps = CutNbspMark(WebLink, QuestionMark);
                    if (Nbsps != null)
                    {
                        ObservableCollection<WebRequestParamsItem> WebRequestItems = CutEqualMark(Nbsps);
                        if (WebRequestItems != null)
                            return WebRequestItems;
                    }
                }
            }
            catch { }

            return null;
        }

        private static int CutQuestionMark(string WebLink)
        {
            int QuestionMark = WebLink.IndexOf('?');
            return QuestionMark;
        }

        private static string[] CutNbspMark(string WebLink, int QuestionMark)
        {
            if (QuestionMark != -1)
            {
                WebLink = WebLink_Redux_OnQuestiOnMark(WebLink, QuestionMark);
                int NbspMark = WebLink.IndexOf('&');

                if (NbspMark != -1)
                {
                    if (NbspMark - 1 != QuestionMark)
                    {
                        string[] NbspArrayCut = WebLink.Split('&');

                        if (NbspArrayCut.Length != 0)
                            return NbspArrayCut;
                    }
                }
                else
                {
                    return new string[] { WebLink };
                }
            }

            return null;
        }

        private static ObservableCollection<WebRequestParamsItem> CutEqualMark(string[] NbspArrayCut)
        {
            ObservableCollection<WebRequestParamsItem> WebRequestItems = new ObservableCollection<WebRequestParamsItem>();

            for (int i = 0; i < NbspArrayCut.Length; i++)
            {
                string EqualItem = NbspArrayCut[i];
                int EqualMark = EqualItem.IndexOf('=');

                if (EqualItem.Length != 1)
                {
                    if (EqualMark != -1)
                    {
                        string[] Cuts = EqualItem.Split('=');

                        var WebRequestItem = new WebRequestParamsItem();

                        if (Cuts[0].Trim() != string.Empty)
                        {
                            WebRequestItem.Key = (Cuts[0].Trim() != string.Empty) ? Cuts[0] : "";
                            WebRequestItem.Value = (Cuts[1].Trim() != string.Empty) ? Cuts[1] : "";

                            WebRequestItems.Add(WebRequestItem);
                        }
                    }
                    else
                    {
                        var WebRequestItem = new WebRequestParamsItem();
                        WebRequestItem.Key = EqualItem;
                        WebRequestItem.Value = "";

                        WebRequestItems.Add(WebRequestItem);
                    }
                }
            }

            return (WebRequestItems.Count == 0) ? null : WebRequestItems;
        }

        private static string WebLink_Redux_OnQuestiOnMark(string WebLink, int QuestionMark)
        {
            return WebLink.Substring(QuestionMark + 1);
        }
    }
}
