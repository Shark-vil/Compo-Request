using Compo_Shared_Data.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Text.Encodings;
using System.Text.Encodings.Web;

namespace Compo_Request.Utilities
{
    public class WebResponseTemplate
    {
        public string Response = "";
        public string Info = "";
    }

    public class CustomWebRequest
    {
        public static WebResponseTemplate Send(string Method, string Link, 
            ObservableCollection<WebRequestParamsItem> FormRequestsData = null)
        {
            var WebResponse = new WebResponseTemplate();

            // Создать объект запроса
            //var request = WebRequest.Create(Link);
            var request = (HttpWebRequest)WebRequest.Create(Link);
            request.Method = Method;
            if (Method != "GET")
            {
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                if (FormRequestsData != null)
                {
                    string RequestParams = "";

                    for (int i = 0; i < FormRequestsData.Count; i++)
                    {
                        WebRequestParamsItem ParamsItem = FormRequestsData[i];
                        if (i == FormRequestsData.Count - 1)
                            RequestParams += $"{ParamsItem.Key}={ParamsItem.Value}";
                        else
                            RequestParams += $"{ParamsItem.Key}={ParamsItem.Value}&";
                    }

                    byte[] DataStream = Encoding.UTF8.GetBytes(RequestParams);
                    request.ContentLength = DataStream.Length;

                    using (var stream = request.GetRequestStream())
                        stream.Write(DataStream, 0, DataStream.Length);
                }
            }

            try
            {
                // Получить ответ с сервера
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Получаем поток данных из ответа
                using (var reader = new StreamReader(response.GetResponseStream()))
                    WebResponse.Response = reader.ReadToEnd();

                WebResponse.Info = GetResponseInfo(request, response);
            }
            catch(WebException ex)
            {
                if (ex.Response != null)
                    using (StreamReader stream = new StreamReader(ex.Response.GetResponseStream()))
                        WebResponse.Response = stream.ReadToEnd();

                WebResponse.Info = GetResponseInfo(request, ex.Response);
            }

            return WebResponse;
        }

        public static string GetResponseInfo(WebRequest request, WebResponse response)
        {
            // Получаем некоторые данные о сервере
            string RequestInfo = "Целевой URL: \t" + request.RequestUri + "\nМетод запроса: \t" + request.Method +
                 "\nТип полученных данных: \t" + response.ContentType + "\nДлина ответа: \t" + response.ContentLength + "\nЗаголовки";

            // Получаем заголовки, используем LINQ
            WebHeaderCollection whc = response.Headers;
            var headers = Enumerable.Range(0, whc.Count)
            .Select(p =>
            {
                return new
                {
                    Key = whc.GetKey(p),
                    Names = whc.GetValues(p)
                };
            });

            foreach (var item in headers)
            {
                RequestInfo += "\n  " + item.Key + ":";
                foreach (var n in item.Names)
                    RequestInfo += "\t" + n;
            }

            return RequestInfo;
        }

        public static string GetResponseInfo(string Link, string Method, IRestResponse Response)
        {
            // Получаем некоторые данные о сервере
            string RequestInfo = "Целевой URL: \t" + Link + "\nМетод запроса: \t" + Method +
                 "\nТип полученных данных: \t" + Response.ContentType + "\nДлина ответа: \t" + Response.ContentLength + "\nЗаголовки";


            Dictionary<string, string> HeadersList = new Dictionary<string, string>();

            foreach (var item in Response.Headers)
            {
                string[] KeyPairs = item.ToString().Split('=');
                HeadersList.Add(KeyPairs[0], KeyPairs[1]);
            }

            foreach (var item in HeadersList)
            {
                RequestInfo += "\n  " + item.Key + ":" + item.Value;
            }

            return RequestInfo;
        }
    }
}
