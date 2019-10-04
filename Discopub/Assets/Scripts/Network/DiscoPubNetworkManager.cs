using Assets.Scripts.Players;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{
    public class DiscoPubNetworkManager : CaptainsMessNetworkManager
    {
        [SerializeField]
        private PlayersManager _playersManager;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);

            _playersManager.NotifyPlayerJoined(conn.connectionId);
        }
    }
}
