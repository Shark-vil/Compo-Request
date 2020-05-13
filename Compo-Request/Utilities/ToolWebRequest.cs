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

        public static WebResponseTemplate RestRequest(string Method, string Link, ObservableCollection<WebRequestParamsItem> FormRequestsData = null)
        {
            var Client = new RestClient(GetFirstLink(Link));
            var FinalResponse = new WebResponseTemplate();
            IRestResponse Response = null;

            try
            {
                if (Method == "GET")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.GET);
                    Response = Client.Execute(Request);
                }
                else if(Method == "POST")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.POST);
                    Response = Client.Execute(Request);
                }
                else if (Method == "PUT")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.PUT);
                    Response = Client.Execute(Request);
                }
                else if (Method == "PATHCH")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.PATCH);
                    Response = Client.Execute(Request);
                }
                else if (Method == "DELETE")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.DELETE);
                    Response = Client.Execute(Request);
                }
                else if (Method == "COPY")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.COPY);
                    Response = Client.Execute(Request);
                }
                else if (Method == "HEAD")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.HEAD);
                    Response = Client.Execute(Request);
                }
                else if (Method == "OPTIONS")
                {
                    var Request = new RestRequest(GetSecondLink(Link), RestSharp.Method.OPTIONS);
                    Response = Client.Execute(Request);
                }
                else if (Method == "LINK")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
                }
                else if (Method == "UNLINK")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
                }
                else if (Method == "PURGE")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
                }
                else if (Method == "LOCK")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
                }
                else if (Method == "UNLOCK")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
                }
                else if (Method == "PROPFIND")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
                }
                else if (Method == "VIEW")
                {
                    var Request = CustomWebRequest.Send(Method, Link);
                    FinalResponse = Request;
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
                Debug.LogError($"Возникло исключение при отправке WEB-запроса на адрес {Link}. Код ошибки:\n" + ex);
            }

            if (Response != null)
            {
                FinalResponse.Response = Response.Content;
                FinalResponse.Info = CustomWebRequest.GetResponseInfo(Link, Method, Response);
                return FinalResponse;
            }
            else
            {
                return FinalResponse;
            }
        }
    }
}
