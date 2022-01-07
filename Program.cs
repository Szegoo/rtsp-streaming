using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace RTSP_Server
{
	class Program
	{
		public static int rtpPort;
		public const int RTSP_PORT = 5004;
		private static void startRTPserver()
		{
			UdpClient server = new UdpClient(rtpPort);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, rtpPort);
			System.Console.WriteLine($"RTP Server is running on port {rtpPort} 🚀");
			while (true)
			{
				byte[] bytes = server.Receive(ref groupEP);

				Console.WriteLine($"{Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
			}
		}
		private static void startRTSPserver()
		{
			UdpClient server = new UdpClient(RTSP_PORT);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, RTSP_PORT);
			System.Console.WriteLine("RTSP Server is running 🚀");
			while (true)
			{
				byte[] bytes = server.Receive(ref groupEP);

				Console.WriteLine($"{Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
			}
		}
		private static byte[] readFileBytes(int from, int count)
		{
			var data = new byte[(int)Math.Pow(10, 9)];
			FileStream stream = new FileStream("./video.mjpeg", FileMode.Open);
			using (BinaryReader reader = new BinaryReader(stream))
			{
				reader.BaseStream.Seek(from, SeekOrigin.Begin);
				reader.Read(data, 0, count);
			}

			return data;
		}
		static void Main(string[] args)
		{
			rtpPort = Convert.ToInt32(args[0]);
			startRTPserver();
			/*FileInfo info = new FileInfo("./video.mjpeg");
			byte[] bytes = readFileBytes(0, 4000);
			File.WriteAllBytes("./video2.mjpeg", bytes);
			*/
		}
	}
}
