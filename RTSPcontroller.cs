using System;
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
			//rtpController.sendPacket();
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
			}
		}
	}
}