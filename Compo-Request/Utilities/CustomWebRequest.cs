using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Compo_Request.Utilities
{
    public class WebResponseTemplate
    {
        public string Response;
        public string Info;
    }

    public class CustomWebRequest
    {
        public static WebResponseTemplate Send(string Method, string Link)
        {
            var WebResponse = new WebResponseTemplate();

            // Создать объект запроса
            WebRequest request = WebRequest.Create(Link);
            request.Method = Method;

            // Получить ответ с сервера
            WebResponse response = request.GetResponse();

            // Получаем поток данных из ответа
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                WebResponse.Response = stream.ReadToEnd();
            }

            WebResponse.Info = GetResponseInfo(request, response);

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
