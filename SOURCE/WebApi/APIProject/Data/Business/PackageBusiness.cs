﻿using Data.DB;
using Data.Model;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class PackageBusiness : GenericBusiness
    {
        public PackageBusiness(MrFishEntities context = null) : base()
        {
        }



        public string PushOneSignals(string value)
        {
            string url = SystemParam.URL_ONESIGNAL;

            var request = HttpWebRequest.Create(string.Format(url));

            request.Headers["Authorization"] = SystemParam.Authorization;
            request.Headers["https"] = SystemParam.URL_BASE_https;
            var byteData = Encoding.UTF8.GetBytes(value);
            request.ContentType = "application/json";
            request.Method = "POST";
            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }


        public string GetJson(string url)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(url));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            Console.WriteLine(WebResp.StatusCode);
            Console.WriteLine(WebResp.Server);

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            return jsonString;
        }

    }
}
