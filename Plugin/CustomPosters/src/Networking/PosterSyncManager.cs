using LethalNetworkAPI;
using Unity.Netcode;
using CustomPosters.Utils;

namespace CustomPosters.Networking
{
    internal static class PosterSyncManager
    {
        private const string PackSyncIdentifier = "CustomPosters_SyncPack";
        private static readonly LNetworkMessage<string> SyncPackMessage = LNetworkMessage<string>.Connect(
            identifier: PackSyncIdentifier,
            onClientReceived: PosterManager.SetPackForClients
        );

        private const string VideoRequestIdentifier = "CustomPosters_RequestVideoTime";
        private const string VideoSyncIdentifier = "CustomPosters_SyncVideoTime";

        private static readonly LNetworkMessage<string> RequestVideoTimeMessage = LNetworkMessage<string>.Connect(
            identifier: VideoRequestIdentifier,
            onServerReceived: OnVideoTimeRequested
        );

        private static readonly LNetworkMessage<VideoSyncData> SyncVideoTimeMessage = LNetworkMessage<VideoSyncData>.Connect(
            identifier: VideoSyncIdentifier,
            onClientReceived: OnVideoTimeReceived
        );
        
        public static void SendPacket(string packName)
        {
            if (!Plugin.ModConfig.EnableNetworking.Value)
            {
                Plugin.Log.LogDebug("Networking disabled.");
                return;
            }

            if (NetworkManager.Singleton == null)
            {
                Plugin.Log.LogWarning("NetworkManager.Singleton is null, cannot send pack sync");
                return;
            }

            if (NetworkManager.Singleton.IsHost)
            {
                Plugin.Log.LogInfo($"[Host] Sending selected pack to all clients: {PathUtils.GetPrettyPath(packName)}");
                SyncPackMessage.SendClients(packName);
            }
        }
        
        public static void OnClientConnected(ulong clientId)
        {
            if (!Plugin.ModConfig.EnableNetworking.Value)
            {
                return;
            }

            if (NetworkManager.Singleton == null)
            {
                Plugin.Log.LogWarning("NetworkManager.Singleton is null in OnClientConnected");
                return;
            }

            if (NetworkManager.Singleton.IsHost && clientId != NetworkManager.Singleton.LocalClientId)
            {
                if (!string.IsNullOrEmpty(PosterManager.SelectedPack))
                {
                    Plugin.Log.LogInfo($"[Host] New client joined, sending pack: {PathUtils.GetPrettyPath(PosterManager.SelectedPack)}");
                    SyncPackMessage.SendClient(PosterManager.SelectedPack, clientId);
                }
            }
    }

        public static void RequestVideoTimeFromServer(string posterName)
        {
            if (!Plugin.ModConfig.EnableNetworking.Value)
            {
                Plugin.Log.LogDebug("Networking disabled, skipping video time sync request");
                return;
            }

            if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsHost)
            {
                return;
            }

            Plugin.Log.LogDebug($"[Client] Requesting video time for poster: {posterName}");
            RequestVideoTimeMessage.SendServer(posterName);
        }

        private static void OnVideoTimeRequested(string posterName, ulong clientId)
        {
            double? time = PosterManager.GetVideoTimeForPoster(posterName);
            if (time.HasValue)
            {
                Plugin.Log.LogDebug($"[Sync] Host received request for '{posterName}'. Sending time: {time.Value} to client {clientId}");
                var syncData = new VideoSyncData { PosterName = posterName, VideoTime = time.Value };
                SyncVideoTimeMessage.SendClient(syncData, clientId);
            }
        }

        private static void OnVideoTimeReceived(VideoSyncData syncData)
        {
            Plugin.Log.LogDebug($"[Sync] Client received sync time for '{syncData.PosterName}': {syncData.VideoTime}");
            PosterManager.SetVideoTimeForPoster(syncData.PosterName, syncData.VideoTime);
        }
    }
}