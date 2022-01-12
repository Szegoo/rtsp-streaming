using System.IO;
using System.Text;

namespace RTSP_Server
{
	class RTPpacket
	{
		public byte[] payload;
		public int sequenceNumber;
		public int timestamp;
		public RTPpacket(byte sequenceNumber, int timestamp, byte[] payload)
		{
			this.payload = payload;
			this.sequenceNumber = sequenceNumber;
			this.timestamp = timestamp;
		}
		public byte[] generatePacket()
		{
			byte[] headers = Encoding.ASCII.GetBytes($",{sequenceNumber},{timestamp}");
			byte[] packet = new byte[payload.Length + headers.Length];
			//System.Console.WriteLine($"Packet length: {packet.Length}, payload: {payload.Length}, header: {headers.Length}");
			headers.CopyTo(packet, payload.Length);
			for (int i = 0; i < payload.Length; i++)
			{
				packet[i] = payload[i];
			}
			using (FileStream stream = new FileStream("./video2.webm", FileMode.Append))
			{
				stream.Write(payload, 0, payload.Length);
			}
			//File.WriteAllBytes("./video2.webm", payload);
			return payload;
		}
	}
}
