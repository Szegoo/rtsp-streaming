using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
		public int clientRTPport;
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
			rtpController = new RTPcontroller();
		}
		private void Play()
		{
			//FileInfo info = new FileInfo("./video.mjpeg");
			byte[] bytes = readFileBytes(0, 1000);
			RTPpacket packet = new RTPpacket(0, 0, bytes);
			rtpController.sendPacket(packet, clientRTPport);
		}
		private void handleRequest(string request)
		{
			string[] options = request.Split(" ");
			setState(options[0]);
		}
		private void setState(string state)
		{
			Enum.TryParse(state, true, out this.state);
			switch (this.state)
			{
				case STATE.SETUP:
					Setup();
					break;
				case STATE.PLAY:
					Play();
					break;
			}
		}
		private static byte[] readFileBytes(int from, int count)
		{
			var data = new byte[1000];
			FileStream stream = new FileStream("./video.mjpeg", FileMode.Open);
			using (BinaryReader reader = new BinaryReader(stream))
			{
				reader.BaseStream.Seek(from, SeekOrigin.Begin);
				reader.Read(data, 0, count);
			}

			return data;
		}
	}
}