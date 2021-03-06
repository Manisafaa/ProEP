﻿using System;
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
        public static IPAddress[] GetAllIPs()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            List<IPAddress> IPs = new List<IPAddress>();

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    IPs.Add(ip);
                }
            }

            IPAddress[] ips = new IPAddress[(IPs.Count)];
            for (int i = 0; i < ips.Length; ++i)
                ips[i] = IPs[i];

            return ips;
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


        public static IPAddress[] GetAllSubnetMasks(IPAddress[] IPs)
        {
            List<IPAddress> subnetMasks = new List<IPAddress>();

            foreach (var ip in IPs)
                subnetMasks.Add(GetSubnetMask(ip));

            IPAddress[] ips = new IPAddress[(subnetMasks.Count)];
            for (int i = 0; i < ips.Length; ++i)
                ips[i] = subnetMasks[i];

            return ips;
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


        public static IPAddress GetSameSubnetIP(IPAddress[] fromIPs, IPAddress[] fromSMs, IPAddress[] toIPs, IPAddress[] toSMs)
        {
            for (int i = 0; i < fromIPs.Count() && i < fromSMs.Count(); ++i)
            {
                for (int j = 0; j < toIPs.Count() && j < toSMs.Count(); ++j)
                {
                    bool isSubnetMaskEqual = (fromSMs[i].ToString() == toSMs[j].ToString());
                    bool isBroadcastAddressEqual = (GetBroadcastAddress(fromIPs[i], fromSMs[i]).ToString() == GetBroadcastAddress(toIPs[j], toSMs[j]).ToString());
                    bool isIPDifferent = (fromIPs[i].ToString() != toIPs[j].ToString());

                    if (isSubnetMaskEqual && isBroadcastAddressEqual && isIPDifferent)
                        return toIPs[i];
                }
            }

            return null;
        }
    }
}
