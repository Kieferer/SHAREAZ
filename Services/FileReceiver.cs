using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SHAREAZ.Services
{
    public static class FileReceiver
    {
        public static string PERCENTAGE_STRING;
        public static double PERCENTAGE_DOUBLE;
        private readonly static int PORT = 9999;
        public static async void Receive(ProgressBar progressBar, string saveDirectoryPath)
        {
            IPAddress ipAddress = IPAddress.Any;
            TcpListener listener = new(ipAddress, PORT);
            listener.Start();

            TcpClient client = await listener.AcceptTcpClientAsync();
            NetworkStream networkStream = client.GetStream();

            BinaryReader binaryReader = new BinaryReader(networkStream);
            string fileName = binaryReader.ReadString();
            long fileSize = binaryReader.ReadInt64();

            string savePath = Path.Combine(saveDirectoryPath, fileName);
            string directoryPath = Path.GetDirectoryName(savePath);
            Directory.CreateDirectory(directoryPath);

            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            byte[] buffer = new byte[8192];
            int bytesRead;

            long totalBytesRead = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                PERCENTAGE_DOUBLE = (double)totalBytesRead / fileSize * 100;
                PERCENTAGE_STRING = $"{PERCENTAGE_DOUBLE:F2}%";
                progressBar.Progress = PERCENTAGE_DOUBLE;
            }

            stopwatch.Stop();
            networkStream.Close();
        }
    }
}
