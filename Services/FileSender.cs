using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SHAREAZ.Services
{
    public static class FileSender
    {
        public static async Task Send(Action<double> updateProgressBar, string ipAddress, int port, string filePath)
        {
            TcpClient client = new(ipAddress, port);
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            NetworkStream networkStream = client.GetStream();

            BinaryWriter binaryWriter = new BinaryWriter(networkStream);
            long fileSize = fileStream.Length;
            binaryWriter.Write(Path.GetFileName(filePath));
            binaryWriter.Write(fileSize);

            byte[] buffer = new byte[8192];
            int bytesRead;
            long totalBytesRead = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                await networkStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                double percentage = (double)totalBytesRead / fileSize;

                MainThread.BeginInvokeOnMainThread(() => updateProgressBar(percentage));
            }
            stopwatch.Stop();
            fileStream.Close();
            networkStream.Close();
        }
    }
}
