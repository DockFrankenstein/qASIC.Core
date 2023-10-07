using qASIC.Communication;

namespace qASIC
{
    public class RemoteAppInfo : NetworkServerInfo
    {
        public string projectName = string.Empty;
        public string version = string.Empty;
        public string engine = string.Empty;
        public string engineVersion = string.Empty;

        public List<SystemInfo> componentList = new List<SystemInfo>();

        public override Packet Write(Packet packet)
        {
            Write(packet)
                .Write(projectName)
                .Write(version)
                .Write(engine)
                .Write(engineVersion)
                .Write(componentList.Count);

            foreach (var item in componentList)
                packet.Write(item.name)
                    .Write(item.version);

            return packet;
        }

        public override void Read(Packet packet)
        {
            base.Read(packet);
            projectName = packet.ReadString();
            version = packet.ReadString();
            engine = packet.ReadString();
            engineVersion = packet.ReadString();

            componentList.Clear();
            for (int i = 0; i < packet.ReadInt(); i++)
            {
                componentList.Add(new SystemInfo()
                {
                    name = packet.ReadString(),
                    version = packet.ReadString(),
                });
            }
        }

        public struct SystemInfo
        {
            public string name;
            public string version;
        }
    }
}
