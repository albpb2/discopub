using System.Collections;
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
        [SerializeField] private Text[] _waiterNames;
        [SerializeField] private GameObject[] _waiters;

        private GameObject _activePanel;
        private Player.Player _localPlayer;

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

            if (_localPlayer == null)
            {
                AssignPlayer();
            }

            if (_localPlayer != null)
            {
                SendPlayerUpdate();
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

        public void SetReady()
        {
            _nameText.interactable = false;
            
            var captainsMess = FindObjectOfType<CaptainsMess>();
            captainsMess.LocalPlayer().SendReadyToBeginMessage();
        }

        public void ShowWaiter(int connectionId)
        {
            _waiterNames[connectionId].gameObject.SetActive(true);
            _waiters[connectionId].SetActive(true);
        }

        public void SetWaiterName(int connectionId, string waiterName)
        {
            ShowWaiter(connectionId);
            _waiterNames[connectionId].text = waiterName;
        }

        private void AssignPlayer()
        {
            _localPlayer = _captainsMess.LocalPlayer() as Player.Player;
        }
    }
}