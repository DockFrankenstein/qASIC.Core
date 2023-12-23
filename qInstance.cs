using qASIC.Communication;
using qASIC.Communication.Components;
using qASIC.CommComponents;

namespace qASIC
{
    public class qInstance
    {
        public const int DEFAULT_REMOTE_PORT = 8174;

        public qInstance(RemoteAppInfo? appInfo = null)
        {
            RemoteInspectorComponents = CommsComponentCollection.GetStandardCollection()
                .AddComponent(cc_log);

            RemoteInspectorServer = new Server(RemoteInspectorComponents, DEFAULT_REMOTE_PORT);
            AppInfo = appInfo ?? new RemoteAppInfo();
        }

        public RemoteAppInfo AppInfo
        {
            get => (RemoteAppInfo)RemoteInspectorServer.AppInfo;
            set => RemoteInspectorServer.AppInfo = value;
        }

        public CommsComponentCollection RemoteInspectorComponents { get; private set; }
        public Server RemoteInspectorServer { get; private set; }
        public bool forwardDebugLogs = true;
        public bool autoStartRemoteInspectorServer = true;

        public readonly CC_Log cc_log = new CC_Log();

        public void Start()
        {
            if (autoStartRemoteInspectorServer)
                RemoteInspectorServer.Start();

            qEnviroment.Initialize();

            qDebug.OnLogWithColor += QDebug_OnLogWithColor;
            qDebug.OnLogWithColorTag += QDebug_OnLogWithColorTag;
        }

        private void QDebug_OnLogWithColorTag(string message, string colorTag)
        {
            if (!forwardDebugLogs) return;
            if (!RemoteInspectorServer.IsActive) return;
            var log = Log.CreateNow(message, colorTag);
            RemoteInspectorServer.SendToAll(CC_Log.BuildLogPacket(log));
        }

        private void QDebug_OnLogWithColor(string message, Color color)
        {
            if (!forwardDebugLogs) return;
            if (!RemoteInspectorServer.IsActive) return;
            var log = Log.CreateNow(message, color);
            RemoteInspectorServer.SendToAll(CC_Log.BuildLogPacket(log));
        }

        public void Stop()
        {
            if (RemoteInspectorServer.IsActive)
                RemoteInspectorServer.Stop();
        }
    }
}