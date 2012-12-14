using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using LukeSkywalker.IPNetwork;

namespace ModCloudFlareIIS
{
    public class ModCloudFlare : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
            context.PostLogRequest += new EventHandler(PostLogEvent);
        }

        #endregion

        private static List<string> CloudFlareIPRanges = new List<string>()
        {
            "204.93.240.0/24",
            "204.93.177.0/24",
            "199.27.128.0/21",
            "173.245.48.0/20",
            "103.22.200.0/22",
            "141.101.64.0/18",
            "108.162.192.0/18",
            "190.93.240.0/20",
            "188.114.96.0/20",
            "198.41.128.0/17"
        };

        public static bool IsCloudFlareIP(string ip)
        {
            
            foreach(string block in CloudFlareIPRanges)
            {
                IPNetwork network = IPNetwork.Parse(block);

                if (IPNetwork.Contains(network, IPAddress.Parse(ip)))
                    return true;
            }

            return false;
        }


        public void OnPreRequestHandlerExecute(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            HttpRequest request = app.Context.Request;
                        
            if (!String.IsNullOrEmpty(request["HTTP_CF_CONNECTING_IP"]))
            {
                if (IsCloudFlareIP(request["REMOTE_ADDR"]))
                {
                    request.ServerVariables.Set("REMOTE_ADDR", request["HTTP_CF_CONNECTING_IP"]);
                }
            }
        }

        public void PostLogEvent(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            HttpRequest request = app.Context.Request;

            if (!String.IsNullOrEmpty(request["HTTP_CF_CONNECTING_IP"]))
            {
                app.Response.AppendToLog("CloudFlare_Visitor_IP:" + request["HTTP_CF_CONNECTING_IP"]);
            }
        }
    }
}
