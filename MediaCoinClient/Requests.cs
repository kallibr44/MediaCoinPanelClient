using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using NLog;

namespace MediaCoinClient
{
    class Requests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public JObject Get(string url)
        {
            /// <summary>
            /// <param name="url">String URL</param>
            /// </summary>
            /// 
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.QueryString.Add("format", "json");
                    var response = webClient.DownloadString(url);
                    Console.WriteLine(response.ToString());
                    JObject json = JObject.Parse(response);
                    return json;
                }
                catch (Exception ex)
                {
                    logger.Error("Request error. Trace: " + ex.ToString());
                    JObject json = JObject.Parse("{'status':'error'}");
                    return json;
                }
            }

        }

        public void Post(string url, NameValueCollection data)
        {
            using (var webClient = new WebClient())
            {


                // Посылаем параметры на сервер
                // Может быть ответ в виде массива байт
                var response = webClient.UploadValues(url, data);
            }
        }

    }
}
