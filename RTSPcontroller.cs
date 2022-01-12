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
			this.initializeServer(server, groupEP);
		}
		private void initializeServer(UdpClient server, IPEndPoint groupEP)
		{
			System.Console.WriteLine("RTSP Server is running 🚀");
			while (true)
			{
				string request = ReceiveRequest(server, groupEP);
				handleRequest(request);
				System.Console.WriteLine($"State: {state}");
			}
		}
		private string ReceiveRequest(UdpClient server, IPEndPoint groupEP)
		{
			byte[] bytes = server.Receive(ref groupEP);

			string request = $"{Encoding.ASCII.GetString(bytes, 0, bytes.Length)}";
			return request;
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
				long length = getVideoLengthInBytes();
				bool isFirstPacket = true;
				while (shouldPlay(length))
				{
					sequenceNumber = getNextSequenceNumber(options, isFirstPacket);
					int packetLength = getNextPacketLength(length);
					System.Console.WriteLine($"Sequence number: {sequenceNumber}");
					byte[] bytes = readFileBytes(sequenceNumber, packetLength);
					sendPacket(bytes);
					isFirstPacket = false;
				}
			});
			Thread thread = new Thread(threadStart);
			thread.Start();
		}
		private int getNextSequenceNumber(string[] options, bool isFirstPacket)
		{
			if (isFirstPacket)
			{
				return Convert.ToInt32(options[1]);
			}
			return sequenceNumber;
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
			FileStream stream = new FileStream("./video.webm", FileMode.Open);
			using (BinaryReader reader = new BinaryReader(stream))
			{
				reader.BaseStream.Seek(from, SeekOrigin.Begin);
				reader.Read(data, 0, count);
			}

			return data;
		}
		private void sendPacket(byte[] bytes)
		{
			RTPpacket packet = new RTPpacket(0, 0, bytes);
			rtpController.sendPacket(packet, clientRTPport);
			sequenceNumber += 64000;
			Thread.Sleep(200);
		}
		private int getNextPacketLength(long length)
		{
			return Math.Min(64000, Convert.ToInt32(length));
		}
		private long getVideoLengthInBytes()
		{
			long length = new FileInfo("./video.webm").Length;
			System.Console.WriteLine("File length: " + length);
			return length;
		}
		private bool shouldPlay(long length)
		{
			return state == STATE.PLAY && isVideoCompleted(length);
		}
		private bool isVideoCompleted(long length)
		{
			return sequenceNumber < Convert.ToInt32(length);
		}
	}
}