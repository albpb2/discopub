using Assets.Scripts.Lobby.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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
        private Dictionary<string, int> _waiterIndexesPerPeerId;
        private Dictionary<string, float> _updateReceivedDates;
        private int _numberOfPlayers;

        protected void Awake()
        {
            _waiterIndexesPerPeerId = new Dictionary<string, int>();
            _updateReceivedDates = new Dictionary<string, float>();
        }

        protected void Start()
        {
            StartCoroutine(RemoveMissingPlayers());
        }

        protected void OnDisable()
        {
            StopAllCoroutines();
        }

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

            _localPlayer.CmdPlayerUpdated(_nameText.text, _localPlayer.peerId);
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
                _localPlayer.CmdSetWaiterReadyStatus(false, _localPlayer.peerId);

                _readyButton.GetComponentInChildren<Text>().text = "¡Estoy lista!";
                _nameText.interactable = true;

                _localPlayer.SendNotReadyToBeginMessage();
            }
            else
            {
                _localPlayer.CmdSetWaiterReadyStatus(true, _localPlayer.peerId);

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

        public void SetWaiterName(int waiterIndex, string waiterName)
        {
            if (_waiters[waiterIndex] == null)
            {
                return;
            }

            ShowWaiter(waiterIndex);
            _waiterNames[waiterIndex].text = waiterName;
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
        
        public int GetWaiterIndex(string peerId)
        {
            if (!_waiterIndexesPerPeerId.TryGetValue(peerId, out int waiterIndex))
            {
                waiterIndex = _numberOfPlayers;
                _waiterIndexesPerPeerId[peerId] = waiterIndex;
                _numberOfPlayers++;
            }

            _updateReceivedDates[peerId] = Time.time;

            return waiterIndex;
        }

        private void AssignPlayer()
        {
            _localPlayer = _captainsMess.LocalPlayer() as Player.Player;
        }
        
        private IEnumerator RemoveMissingPlayers()
        {
            while(true)
            {
                const int updateFrequencySeconds = 5;
                yield return new WaitForSeconds(updateFrequencySeconds);

                RemoveOldPeers();
            }
        }

        private void RemoveOldPeers()
        {
            var now = Time.time;

            var peersToRemove = new List<string>();

            const int MaxSecondsToWait = 3;

            foreach (var peerId in _updateReceivedDates.Keys)
            {
                if (Time.time - _updateReceivedDates[peerId] > MaxSecondsToWait)
                {
                    peersToRemove.Add(peerId);
                }
            }

            foreach (var peerId in peersToRemove)
            {
                Debug.Log($"Removing all peer {peerId}");
                _updateReceivedDates.Remove(peerId);
                _waiterIndexesPerPeerId.Remove(peerId);
                _numberOfPlayers--;
            }
        }
    }
}