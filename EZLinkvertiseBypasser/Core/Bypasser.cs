using System;
using System.IO;
using System.Net;

namespace EZLinkvertiseBypasser.Core
{
    internal class Bypasser
    {
        /// <summary>
        /// Deactivated to see if they caused the issues that led into blank bypasses
        /// </summary>

        private static string[] patterns =
        {
                "https://linkvertise.com/",
                "https://up-to-down.net/",
                "https://link-to.net/",
                "https://direct-link.net/",
                "https://file-link.net",
                "https://link-hub.net",
                "https://link-center.net"
        };

        public static string Destination { get; private set; }
        public static string Query { get; private set; }
        public static string Plugin { get; private set; }
        public static string[] Patterns { get => patterns; set => patterns = value; }

        public static bool Success { get; private set; }

        public static int Time { get; private set; }
        public static long Cache_ttl { get; private set; }

        public Bypasser(string Response)
        {
            string[] parameters = Response.Replace("{", "").Replace("}", "").Split(',');
            foreach (string field in parameters)
            {
                int fieldLength = field.IndexOf('"', 2) - 2;
                string fieldValue = field.Substring(field.IndexOf(':') + 2).Replace("\"", "");

                switch (field.Substring(2, fieldLength))
                {
                    case "success":
                        Success = !fieldValue.ToLower().Contains("fal");
                        break;

                    case "destination":
                        Destination = fieldValue;
                        break;

                    case "uery":
                        Query = fieldValue;
                        break;

                    case "time_ms":
                        Time = int.Parse(fieldValue);
                        break;

                    case "cache_ttl":
                        Cache_ttl = long.Parse(fieldValue);
                        break;

                    case "plugin":
                        Plugin = fieldValue;
                        break;
                }
            }
        }

        public static Bypasser BypassURL(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://bypass.bot.nu/bypass2?url={URL}");
            request.Referer = "https://thebypasser.com";
            request.Headers.Add("DNT", "1");
            request.Accept = "*/*";
            try
            {
                Bypasser result = new Bypasser(new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd());
                return Success ? result : throw new Exception("There was an error.");
            }
            catch (WebException ex) 
            {
                throw ex;
            }
        }
    }
}
