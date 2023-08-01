//using Java.Nio.FileNio.Attributes;
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
        public static string PERCENTAGE = String.Empty;
        private readonly static int PORT = 9999;
        public static void Send(string ipAddress, string filePath)
        {
            TcpClient client = new(ipAddress, PORT);
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            NetworkStream networkStream = client.GetStream();

            BinaryWriter binaryWriter = new BinaryWriter(networkStream);
            binaryWriter.Write(Path.GetFileName(filePath));
            binaryWriter.Write(fileStream.Length);

            byte[] buffer = new byte[8192];
            int bytesRead;
            long totalBytesRead = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                Debug.WriteLine(totalBytesRead);
                networkStream.Write(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                double progressPercentage = (double)totalBytesRead / fileStream.Length * 100;
                PERCENTAGE = $"{progressPercentage:F2}%";
            }
            stopwatch.Stop();
            networkStream.Close();
            Debug.WriteLine("File sending ended");
        }
    }
}
