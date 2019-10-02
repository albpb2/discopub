using System.Collections;
using Assets.Scripts.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private CaptainsMess _captainsMess;
        [SerializeField] private GameObject _introPanel;
        [SerializeField] private TMP_InputField _nameText;
        [SerializeField] private Button _readyButton;
        [SerializeField] private PlayersManager _playersManager;
        [SerializeField] private Text[] _waiterNames;
        [SerializeField] private GameObject[] _waiters;

        private GameObject _activePanel;
        private string _playerPeerId;

        public void Connect()
        {
            _introPanel.SetActive(false);

            _captainsMess.AutoConnect();
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

            if (string.IsNullOrEmpty(_playerPeerId))
            {
                AssignPlayer();
            }
            
            _playersManager.CmdSetPlayerName(_playerPeerId, _nameText.text);
        }

        public void SetReady()
        {
            _nameText.interactable = false;
            
            var captainsMess = FindObjectOfType<CaptainsMess>();
            captainsMess.LocalPlayer().SendReadyToBeginMessage();
        }

        public void NotifyPlayerJoined()
        {
            AssignPlayer();
            _playersManager.CmdNotifyPlayerJoined(_playerPeerId);
        }

        public void EnableLobbyControls()
        {
            _nameText.interactable = true;
        }

        public void ShowWaiter(int waiterIndex)
        {
            _waiterNames[waiterIndex].gameObject.SetActive(true);
            _waiters[waiterIndex].SetActive(true);
        }

        public void SetWaiterName(int waiterIndex, string waiterName)
        {
            _waiterNames[waiterIndex].text = waiterName;
        }

        private void AssignPlayer()
        {
            var captainsMess = FindObjectOfType<CaptainsMess>();
            _playerPeerId = captainsMess.LocalPlayer().peerId;
        }
    }
}