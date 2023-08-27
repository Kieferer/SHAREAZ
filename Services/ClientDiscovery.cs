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

        public static void StartDiscovery()
        {
            Debug.WriteLine("Start discovery");
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;

            Thread broadcastThread = new Thread(BroadcastLocalIP);
            broadcastThread.Start();

            ListenForBroadcasts();
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
                    Debug.WriteLine("lListening");
                    byte[] bytes = listener.Receive(ref groupEP);
                    string receivedIP = Encoding.UTF8.GetString(bytes);
                    Debug.WriteLine("Received IP: " + receivedIP);
                }
            }
            finally
            {
                listener.Close();
            }
        }

        static string GetLocalIPAddress()
        {
            string localIP = "";
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ipAddress in localIPs)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ipAddress.ToString();
                    break;
                }
            }

            return localIP;
        }
    }
}