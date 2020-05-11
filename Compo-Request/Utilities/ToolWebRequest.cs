using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using Compo_Shared_Data.Debugging;
using RestSharp;
using Compo_Shared_Data.Models;

namespace Compo_Request.Utilities
{
    public class ToolWebRequest
    {
        /*
        private static object CreateSpecificityCollection(ObservableCollection<ModelFormRequest> FormRequestsData)
        {
            dynamic Params = new ExpandoObject();

            for (int i = 0; i < FormRequestsData.Count; i++)
            {
                ModelFormRequest RequestItem = FormRequestsData[i];
                ((IDictionary<string, object>)Params).Add(RequestItem.Key, RequestItem.Value);
            }

            return (object)Params;
        }
        */

        private static string GetFirstLink(string Link)
        {
            int EndFirstLinkHttp = Link.IndexOf("http://");
            int EndFirstLinkHttps = Link.IndexOf("https://");

            if (EndFirstLinkHttp != -1)
            {
                int SecondLink = Link.IndexOf("/", 7);
                if (SecondLink != -1)
                    return Link.Substring(0, SecondLink);
            }

            if (EndFirstLinkHttps != -1)
            {
                int SecondLink = Link.IndexOf("/", 8);
                if (SecondLink != -1)
                    return Link.Substring(0, SecondLink);
            }

            int _SecondLink = Link.IndexOf("/");
            if (_SecondLink != -1)
            {
                return Link.Substring(0, _SecondLink);
            }


            return "";
        }

        private static string GetSecondLink(string Link)
        {
            int EndFirstLinkHttp = Link.IndexOf("http://");
            int EndFirstLinkHttps = Link.IndexOf("https://");

            if (EndFirstLinkHttp != -1)
            {
                int SecondLink = Link.IndexOf("/", 7);
                if (SecondLink != -1)
                    return Link.Substring(SecondLink + 1, Link.Length - SecondLink - 1);
            }

            if (EndFirstLinkHttps != -1)
            {
                int SecondLink = Link.IndexOf("/", 8);
                if (SecondLink != -1)
                    return Link.Substring(SecondLink + 1, Link.Length - SecondLink - 1);
            }

            int _SecondLink = Link.IndexOf("/");
            if (_SecondLink != -1)
            {
                return Link.Substring(_SecondLink + 1, Link.Length - _SecondLink - 1);
            }


            return "";
        }

        public static string RestRequest(string Method, string Link, ObservableCollection<WebRequestItem> FormRequestsData = null)
        {
            var Client = new RestClient(GetFirstLink(Link));
            IRestResponse Response = null;

            try
            {
                if (Method == "GET")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.GET);
                    Response = Client.Execute(Request);
                }
                else
                {
                    if (Method == "POST")
                    {
                        var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.POST);
                        Response = Client.Execute(Request);
                    }
                }

                if (Response.ErrorException != null)
                {
                    const string ErrorMessage = "Возникла ошибка при получении ответа на WEB-запрос. Ознакомьтесь с деталями в консоли.";
                    var Exeption = new Exception(ErrorMessage, Response.ErrorException);
                    throw Exeption;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Возникло исключение при отправке WEB-запроса на сайт {Link}. Код ошибки:\n" + ex);
            }

            return (Response != null) ? Response.Content : null;
        }
    }
}
