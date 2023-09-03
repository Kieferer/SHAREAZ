using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SHAREAZ.Services
{
    public static class ClientDiscovery
    {
        static UdpClient udpClient;
        static int broadcastPort = 12345;
        static int broadcastInterval = 5000;
        static Thread discoveryThread;
        public static void StartDiscovery()
        {
            Debug.WriteLine("Start discovery");
            Debug.WriteLine($"Broadcasting on: {GetLocalIPAddress()}");

            discoveryThread = new Thread(DiscoveryThreadMain)
            {
                IsBackground = true
            };
            discoveryThread.Start();
        }

        private static void DiscoveryThreadMain()
        {
            while (true)
            {
                ListenForBroadcasts();
                BroadcastLocalIP();

                Thread.Sleep(broadcastInterval);
            }
        }

        static void BroadcastLocalIP()
        {
            while (true)
            {
                Debug.WriteLine("Broadcast");
                string localIP = GetLocalIPAddress();
                byte[] data = Encoding.UTF8.GetBytes(localIP);

                udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, broadcastPort));

                Thread.Sleep(broadcastInterval);
            }
        }

        static void ListenForBroadcasts()
        {
            UdpClient listener = new UdpClient(broadcastPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, broadcastPort);

            try
            {
                while (true)
                {
                    Debug.WriteLine("Listening");
                    byte[] bytes = listener.Receive(ref groupEP);
                    string receivedIP = Encoding.UTF8.GetString(bytes);
                    if (!receivedIP.Equals(GetLocalIPAddress()))
                    {
                        Debug.WriteLine("Received IP: " + receivedIP);
                    }
                }
            }
            finally
            {
                listener.Close();
            }
        }

        static string GetLocalIPAddress()
        {
            string localIP = String.Empty;

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ipAddress in localIPs)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ipAddressString = ipAddress.ToString();
                    localIP = ipAddressString;
                    break;
                }
            }

            return localIP;
        }

    }
}