using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RTSP_Server
{
	public class RTSPcontroller
	{
		private enum STATE
		{
			SETUP,
			PLAY,
			PAUSE,
			TEARDOWN
		}
		private STATE state;
		public int clientRTPport = 3000;
		public string requestedVideo;
		public const int RTSP_PORT = 554;
		private int sequenceNumber = 0;
		private RTPcontroller rtpController;
		public RTSPcontroller()
		{
			UdpClient server = new UdpClient(RTSP_PORT);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, RTSP_PORT);
			System.Console.WriteLine("RTSP Server is running 🚀");
			while (true)
			{
				byte[] bytes = server.Receive(ref groupEP);

				string request = $"{Encoding.ASCII.GetString(bytes, 0, bytes.Length)}";
				handleRequest(request);
				System.Console.WriteLine($"State: {state}");
			}
		}
		private void Setup()
		{
			sequenceNumber = 0;
			rtpController = new RTPcontroller();
		}
		private void Play(string[] options)
		{
			ThreadStart threadStart = new ThreadStart(() =>
			{
				int packetsSent = 0;
				long length = new FileInfo("./video.mp4").Length;
				System.Console.WriteLine("File length: " + length);
				while (state == STATE.PLAY && sequenceNumber < Convert.ToInt32(length))
				{
					if (packetsSent == 0)
					{
						sequenceNumber = Convert.ToInt32(options[1]);
					}
					int packetSize = Math.Min(64000, Convert.ToInt32(length));
					System.Console.WriteLine($"Sequence number: {sequenceNumber}");
					byte[] bytes = readFileBytes(sequenceNumber, packetSize);
					RTPpacket packet = new RTPpacket(0, 0, bytes);
					rtpController.sendPacket(packet, clientRTPport);
					sequenceNumber += 64001;
					packetsSent++;
					Thread.Sleep(500);
				}
			});
			Thread thread = new Thread(threadStart);
			thread.Start();
		}
		private void handleRequest(string request)
		{
			string[] options = request.Split(" ");
			setState(options);
		}
		private void setState(string[] options)
		{
			string state = options[0];
			Enum.TryParse(state, true, out this.state);
			switch (this.state)
			{
				case STATE.SETUP:
					Setup();
					break;
				case STATE.PLAY:
					Play(options);
					break;
			}
		}
		private static byte[] readFileBytes(int from, int count)
		{
			var data = new byte[64000];
			FileStream stream = new FileStream("./video.mp4", FileMode.Open);
			using (BinaryReader reader = new BinaryReader(stream))
			{
				reader.BaseStream.Seek(from, SeekOrigin.Begin);
				reader.Read(data, 0, count);
			}

			return data;
		}
	}
}