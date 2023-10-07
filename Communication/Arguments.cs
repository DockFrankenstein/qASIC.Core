namespace qASIC.Communication
{
    public class OnServerReceiveDataArgs
    {
        public OnServerReceiveDataArgs(Server.Client client, byte[] buffer)
        {
            this.client = client;
            data = buffer;
        }

        public Server.Client client;
        public byte[] data;
    }

    public class CommsComponentArgs
    {
        public CommsComponentArgs(PacketType packetType, Packet packet)
        {
            this.packet = packet;
            this.packetType = packetType;
        }

        public PacketType packetType;
        public Packet packet;
        public Client? client;
        public Server? server;

        public Server.Client? targetServerClient;
    }
}