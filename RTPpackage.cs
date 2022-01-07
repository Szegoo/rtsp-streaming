using System.Text;

namespace RTP
{
	class RTPpackage
	{
		public byte[] payload;
		public int sequenceNumber;
		public int timestamp;
		public RTPpackage(byte sequenceNumber, int timestamp, byte[] payload)
		{
			this.payload = payload;
			this.sequenceNumber = sequenceNumber;
			this.timestamp = timestamp;
		}
		public byte[] generatePacket()
		{
			byte[] headers = Encoding.ASCII.GetBytes($",{sequenceNumber},{timestamp}");
			byte[] packet = new byte[payload.Length + headers.Length];
			headers.CopyTo(packet, payload.Length);
			for (int i = 0; i < payload.Length; i++)
			{
				headers[i] = payload[i];
			}
			return packet;
		}
	}
}
