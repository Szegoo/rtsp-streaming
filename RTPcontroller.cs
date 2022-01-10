using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RTSP_Server
{
	class RTPcontroller
	{
		Socket socket;
		IPAddress client;
		public RTPcontroller()
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			client = IPAddress.Parse("127.0.0.1");
		}
		public void sendPacket(RTPpacket packet, int clientPort)
		{
			byte[] dgram = packet.generatePacket();
			System.Console.WriteLine(dgram);
			IPEndPoint ep = new IPEndPoint(client, 3000);

			socket.SendTo(dgram, ep);
		}
	}
}