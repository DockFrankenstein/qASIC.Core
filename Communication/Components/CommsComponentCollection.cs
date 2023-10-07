using System;

namespace qASIC.Communication.Components
{
    public class CommsComponentCollection
    {
        private List<CommsComponent> components = new List<CommsComponent>();

        private Dictionary<Type, CommsComponent> componentsDictionary = new Dictionary<Type, CommsComponent>();

        public static CommsComponentCollection GetStandardCollection()
        {
            var comms = new CommsComponentCollection()
                .AddComponent<CC_ConnectData>()
                .AddComponent<CC_Disconnect>()
                .AddComponent<CC_Ping>()
                .AddComponent<CC_Debug>();

            return comms;
        }

        public CommsComponentCollection AddComponent<T>() where T : CommsComponent, new() =>
            AddComponent(new T());

        public CommsComponentCollection AddComponent<T>(T component) where T : CommsComponent
        {
            var type = typeof(T);
            components.Add(component);
            if (!componentsDictionary.ContainsKey(type))
                componentsDictionary.Add(type, component);

            return this;
        }

        public CommsComponent? GetComponent<T>() where T : CommsComponent =>
            GetComponent(typeof(T));

        public CommsComponent? GetComponent(Type type)
        {
            if (!componentsDictionary.ContainsKey(type))
                return null;

            return componentsDictionary[type];
        }

        public void HandlePacketForServer(Server server, Server.Client serverClient, Packet packet)
        {
            var id = packet.ReadString();

            var targetComp = components
                .Where(x => x.GetId() == id)
                .FirstOrDefault();

            if (targetComp == null)
            {
                server.OnLog?.Invoke($"Communication Component of id '{id}' does not exist");
                return;
            }

            var args = new CommsComponentArgs(PacketType.Server, packet)
            {
                server = server,
                targetServerClient = serverClient,
            };

            targetComp.Read(args);
        }

        public void HandlePacketForClient(Client client, Packet packet)
        {
            var id = packet.ReadString();

            var targetComp = components
                .Where(x => x.GetId() == id)
                .FirstOrDefault();

            if (targetComp == null)
                return;

            var args = new CommsComponentArgs(PacketType.Client, packet)
            {
                client = client,
            };

            targetComp.Read(args);
        }
    }
}