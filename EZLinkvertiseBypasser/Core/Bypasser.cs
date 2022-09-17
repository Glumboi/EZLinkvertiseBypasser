using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace EZLinkvertiseBypasser.Core
{
    internal class Bypasser
    {
        public static string Destination { get; set; } //This prop will hold the result of our bypass
        public static string Query { get; private set; } //This prop will hold our link that we bypassed
        public static string Plugin { get; private set; }
        public static string[] Patterns { get; private set; } =
        {
            "https://linkvertise.com/",
            "https://up-to-down.net/",
            "https://link-to.net/",
            "https://direct-link.net/",
            "https://file-link.net",
            "https://link-hub.net",
            "https://link-center.net",
            "https://link-target.net"
        };

        private static bool Success { get; set; }
        public static int Time { get; private set; }
        private static long Cache_ttl { get; set; }

        private Bypasser(string Response)
        {
            var parameters = Response.Replace("{", "").Replace("}", "").Split(',');
            foreach (string field in parameters)
            {
                var fieldLength = field.IndexOf('"', 2) - 2;
                var fieldValue = field.Substring(field.IndexOf(':') + 2).Replace("\"", "");

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
            var request = (HttpWebRequest)WebRequest.Create($"https://bypass.bot.nu/bypass2?url={URL}");
            request.Referer = "https://thebypasser.com";
            request.Headers.Add("DNT", "1");
            request.Accept = "*/*";
            try
            {
                var result = new Bypasser(new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd());
                return Success ? result : throw new Exception("There was an error.");
            }
            catch (WebException ex) 
            {
                throw ex;
            }
        }
    }
}
