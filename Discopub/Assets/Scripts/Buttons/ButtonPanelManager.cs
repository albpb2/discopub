using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class ButtonPanelManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] _panels;
        [SerializeField]
        private GameObject _actionButtonPrefab;

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
                TargetEnablePanel(player.connectionToClient, player.peerId, new[]
                {
                    $"Player {i} test target rpc 1",
                    $"Player {i} test target rpc 2",
                    $"Player {i} test target rpc 3"
                });
                i++;
            }
        }

        [TargetRpc]
        public void TargetEnablePanel(NetworkConnection connection, string playerPeerId, string[] buttonNames)
        {
            _panels[0].SetActive(true);
            var buttons = _panels[0].GetComponentsInChildren<Button>();

            for (var i = 0; i < buttons.Length; i++)
            {
                var actionButtonGameObject = Instantiate(_actionButtonPrefab);

                var actionButton = actionButtonGameObject.GetComponentInChildren<Button>();
                actionButton.transform.parent = buttons[i].transform.parent;
                actionButton.transform.localScale = buttons[i].transform.localScale;
                actionButton.transform.localPosition = buttons[i].transform.localPosition;
                Destroy(buttons[i].gameObject);

                var actionButtonController = actionButtonGameObject.GetComponentInChildren<ActionButtonController>();
                actionButtonController.SetUp(buttonNames[i], playerPeerId);

                var actionButtonText = actionButton.GetComponentInChildren<Text>();
                actionButtonText.text = buttonNames[i];
            }
        }

        protected void Awake()
        {
            _captainsMess = FindObjectOfType<CaptainsMess>();
        }
    }
}
