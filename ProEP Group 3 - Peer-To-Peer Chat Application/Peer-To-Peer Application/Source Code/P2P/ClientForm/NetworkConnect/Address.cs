using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetworkConnect
{
    public class Address
    {
        public static List<IPAddress> GetAllIPs()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            List<IPAddress> IPs = new List<IPAddress>();

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                    IPs.Add(ip);
            }

            return IPs;
        }


        public static IPAddress GetSubnetMask(IPAddress ip)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (ip.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", ip));
        }


        public static IPAddress GetBroadcastAddress(IPAddress ip, IPAddress subnetMask)
        {
            byte[] ipAddress = ip.GetAddressBytes();
            byte[] subnetMaskAddress = subnetMask.GetAddressBytes();
            byte[] broadcastAddress = new byte[4];
            for (int i = 0; i < 4; ++i)
                broadcastAddress[i] = Convert.ToByte(Convert.ToByte(ipAddress[i]) | (~Convert.ToByte(subnetMaskAddress[i]) & 255));

            return new IPAddress(broadcastAddress);
        }


        public static int GetCorrectIPIndex(List<IPAddress> fromIPs, IPAddress toIP)
        {
            for (int i = 0; i < fromIPs.Count(); ++i)
            {
                IPAddress subnetMask = GetSubnetMask(fromIPs[i]);

                bool isBroadcastAddressEqual = (GetBroadcastAddress(fromIPs[i], subnetMask).ToString() == GetBroadcastAddress(toIP, subnetMask).ToString());
                bool isIPDifferent = (fromIPs[i].ToString() != toIP.ToString());

                if (isBroadcastAddressEqual && isIPDifferent)
                    return i;
            }

            throw new ArgumentException(string.Format("Can't find IP in same subnet of '{0}'", toIP));
        }
    }
}
