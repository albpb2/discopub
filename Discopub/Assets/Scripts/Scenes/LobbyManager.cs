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
        [SerializeField] private Image[] _waiterChecks;

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

        public void SetReady()
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
                _localPlayer.CmdSetWaiterReadyStatus(false);
                _readyButton.GetComponentInChildren<Text>().text = "¡Estoy lista!";
                _localPlayer.SendNotReadyToBeginMessage();
            }
            else
            {
                _localPlayer.CmdSetWaiterReadyStatus(true);
                _readyButton.GetComponentInChildren<Text>().text = "¡Espera!";
                _localPlayer.SendReadyToBeginMessage();
            }
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

        public void HidePlayerInputs()
        {
            _readyButton.gameObject.SetActive(false);
            _nameText.gameObject.SetActive(false);
        }

        public void SetReadyStatus(int connectionId, bool ready)
        {
            _waiterChecks[connectionId].gameObject.SetActive(ready);
        }

        private void AssignPlayer()
        {
            _localPlayer = _captainsMess.LocalPlayer() as Player.Player;
        }
    }
}