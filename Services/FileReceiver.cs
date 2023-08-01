using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SHAREAZ.Services
{
    public static class FileReceiver
    {
        private readonly static int PORT = 9999;

        public static async Task Receive(Action<double> updateProgressBar, string saveDirectoryPath)
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
                double percentage = (double)totalBytesRead / fileSize;
                Device.BeginInvokeOnMainThread(() =>
                {
                    updateProgressBar(percentage);
                });
            }

            stopwatch.Stop();
            fileStream.Close();
            networkStream.Close();
        }
    }
}
