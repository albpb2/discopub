using Assets.Scripts.Lobby.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private CaptainsMess _captainsMess;
        [SerializeField]
        private GameObject _introPanel;
        [SerializeField]
        private TMP_InputField _nameText;
        [SerializeField]
        private Button _readyButton;
        [SerializeField]
        private Text[] _waiterNames;
        [SerializeField]
        private GameObject[] _waiters;
        [SerializeField]
        private Image[] _waiterChecks;
        [SerializeField]
        private TMP_Text _loadingText;
        [SerializeField]
        private LobbyCountdown _lobbyCountdown;

        private GameObject _activePanel;
        private Player.Player _localPlayer;

        public void OpenLobby()
        {
            _introPanel.SetActive(false);

            if (!_captainsMess.IsConnected())
            {
                _captainsMess.AutoConnect();
            }
        }

        public void OpenPanel(GameObject panel)
        {
            _introPanel.SetActive(false);

            panel.SetActive(true);

            _activePanel = panel;
        }

        public void GoBackToIntroPanel()
        {
            _activePanel.SetActive(false);

            _introPanel.SetActive(true);
        }

        public void OnNameEdited()
        {
            _readyButton.interactable = !string.IsNullOrEmpty(_nameText.text);

            if (_localPlayer == null)
            {
                AssignPlayer();
            }

            if (_localPlayer != null)
            {
                SendPlayerUpdate();
            }
            else
            {
                // If there's not an online lobby yet we'll update the first waiter which will always be displayed
                SetWaiterName(0, _nameText.text);
            }
        }

        public void SendPlayerUpdate()
        {
            if (_localPlayer == null)
            {
                AssignPlayer();
            }

            _localPlayer.CmdPlayerUpdated(_nameText.text, _localPlayer.connectionToServer.connectionId);
        }

        public void ChangeReadyStatus()
        {
            if (_localPlayer == null)
            {
                AssignPlayer();
            }

            if (_localPlayer == null)
            {
                return;
            }
            
            if (_localPlayer.IsReady())
            {
                _localPlayer.CmdSetWaiterReadyStatus(false, _localPlayer.connectionToServer.connectionId);

                _readyButton.GetComponentInChildren<Text>().text = "¡Estoy lista!";
                _nameText.interactable = true;

                _localPlayer.SendNotReadyToBeginMessage();
            }
            else
            {
                _localPlayer.CmdSetWaiterReadyStatus(true, _localPlayer.connectionToServer.connectionId);

                _readyButton.GetComponentInChildren<Text>().text = "¡Espera!";
                _nameText.interactable = false;

                _localPlayer.SendReadyToBeginMessage();
            }
        }

        public void ShowWaiter(int connectionId)
        {
            if (_waiterNames[connectionId] == null)
            {
                return;
            }

            var lobbyManagers = FindObjectsOfType<LobbyManager>();
            _waiterNames[connectionId].gameObject.SetActive(true);
            _waiters[connectionId].SetActive(true);
        }

        public void SetWaiterName(int connectionId, string waiterName)
        {
            if (_waiters[connectionId] == null)
            {
                return;
            }

            ShowWaiter(connectionId);
            _waiterNames[connectionId].text = waiterName;
        }

        public void HidePlayerInputs()
        {
            _readyButton.gameObject.SetActive(false);
            _nameText.gameObject.SetActive(false);

            _loadingText.gameObject.SetActive(true);
            _lobbyCountdown.gameObject.SetActive(true);
        }

        public void SetReadyStatus(int connectionId, bool ready)
        {
            if (_waiterChecks[connectionId] != null)
            {
                _waiterChecks[connectionId].gameObject.SetActive(ready);
            }
        }

        private void AssignPlayer()
        {
            _localPlayer = _captainsMess.LocalPlayer() as Player.Player;
        }
    }
}