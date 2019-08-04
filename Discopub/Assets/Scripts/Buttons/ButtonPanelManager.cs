using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Buttons
{
    public class ButtonPanelManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] _panels;

        private CaptainsMess _captainsMess;

        [ClientRpc]
        public void RpcCreatePanel()
        {
            var i = 0;
            var peerId = (CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager).peerId;
            var players = (CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager).LobbyPlayers();
            foreach (var player in players)
            {
                if (peerId == player.peerId)
                {
                    _panels[i].SetActive(true);
                }
                i++;
            }
        }

        protected void Awake()
        {
            _captainsMess = FindObjectOfType<CaptainsMess>();
        }
    }
}
