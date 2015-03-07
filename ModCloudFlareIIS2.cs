using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using LukeSkywalker.IPNetwork;
using System.Net;
using eExNetworkLibrary;
using System.Net.Sockets;
using System.Collections;
using eExNetworkLibrary.IP;

namespace ModCloudFlareIIS2
{
    public class ModCloudFlareIIS2 : IHttpModule
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
                "103.21.244.0/22",
                "103.22.200.0/22",
                "103.31.4.0/22",
                "141.101.64.0/18",
                "108.162.192.0/18",
                "190.93.240.0/20",
                "188.114.96.0/20",
                "197.234.240.0/22",
                "198.41.128.0/17",
                "162.158.0.0/15",
                "104.16.0.0/12"
        };

        private static List<string> CloudFlareIPv6Ranges = new List<string>()
        {
            "2400:cb00::/32",
            "2606:4700::/32",
            "2803:f800::/32",
            "2405:b500::/32",
            "2405:8100::/32",
        };

        public static bool IsCloudFlareIP(string ip)
        {
            IPAddress address = IPAddress.Parse(ip);

            if (address.AddressFamily == AddressFamily.InterNetwork) // IPv4
            {
                // Check if IPv4 address is in CloudFlareIPRanges
                foreach (string block in CloudFlareIPRanges)
                {
                    IPNetwork network = IPNetwork.Parse(block);

                    if (IPNetwork.Contains(network, address))
                        return true;
                }
            }
            else if (address.AddressFamily == AddressFamily.InterNetworkV6) // IPv6
            {
                // Check if IPv6 input address is in CloudFlare ranges
                foreach (string block in CloudFlareIPv6Ranges)
                {
                    if (GetIPv6Range(block).Contains(address))
                        return true;
                }
            }
            return false;
        }

        private static IPAddress[] GetIPv6Range(string strIn)
        {
            //Split the string in parts for address and prefix
            string strAddress = strIn.Substring(0, strIn.IndexOf('/'));
            string strPrefix = strIn.Substring(strIn.IndexOf('/') + 1);

            int ipPrefix = Int32.Parse(strPrefix);
            IPAddress ipAddress = IPAddress.Parse(strAddress);

            //Convert the prefix length to a valid SubnetMask
            int ipMaskLength = 128;

            BitArray btArray = new BitArray(ipMaskLength);
            for (int iC1 = 0; iC1 < ipMaskLength; iC1++)
            {
                //Index calculation is a bit strange, since you have to make your mind about byte order.
                int iIndex = (int)((ipMaskLength - iC1 - 1) / 8) * 8 + (iC1 % 8);

                if (iC1 < (ipMaskLength - ipPrefix))
                {
                    btArray.Set(iIndex, false);
                }
                else
                {
                    btArray.Set(iIndex, true);
                }
            }

            byte[] bMaskData = new byte[ipMaskLength / 8];

            btArray.CopyTo(bMaskData, 0);

            //Create subnetmask
            Subnetmask smMask = new Subnetmask(bMaskData);

            //Get the IP range
            IPAddress ipAddressStart = IPAddressAnalysis.GetClasslessNetworkAddress(ipAddress, smMask);
            IPAddress ipAddressEnd = IPAddressAnalysis.GetClasslessBroadcastAddress(ipAddress, smMask);

            //Omit the following lines if your network range is large
            return IPAddressAnalysis.GetIPRange(ipAddressStart, ipAddressEnd);
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
               app.Response.AppendToLog("[CloudFlare_Visitor_IP:" + request["HTTP_CF_CONNECTING_IP"] + "]");

            if (!String.IsNullOrEmpty(request["HTTP_CF_RAY"]))
                app.Response.AppendToLog("[CF_RAY:" + request["HTTP_CF_RAY"] + "]");
        
        }
    }
}
