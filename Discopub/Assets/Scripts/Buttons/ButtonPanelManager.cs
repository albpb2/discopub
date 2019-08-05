using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class ButtonPanelManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] _panels;

        private CaptainsMess _captainsMess;
        
        public void CreatePanel()
        {
            if (!isServer)
            {
                return;
            }

            var i = 0;
            var peerId = (CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager).peerId;
            var players = (CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager).LobbyPlayers();
            foreach (var player in players)
            {
                TargetEnablePanel(player.connectionToClient, new[]
                {
                    $"Player {i} test target rpc 1",
                    $"Player {i} test target rpc 2",
                    $"Player {i} test target rpc 3"
                });
                i++;
            }
        }

        [TargetRpc]
        public void TargetEnablePanel(NetworkConnection connection, string[] buttonNames)
        {
            _panels[0].SetActive(true);
            var buttons = _panels[0].GetComponentsInChildren<Button>();
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponentInChildren<TMPro.TMP_Text>().text = buttonNames[i];
            }
        }

        protected void Awake()
        {
            _captainsMess = FindObjectOfType<CaptainsMess>();
        }
    }
}
