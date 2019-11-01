using Assets.Scripts.Game;
using Assets.Scripts.Scenes;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    public class Player : CaptainsMessPlayer
    {
        private ActionDispatcher _actionDispatcher;
        private LobbyManager _lobbyManager;
        private CaptainsMess _captainsMess;

        private bool _updatePlayerLobbyInfo;

        protected void Start()
        {
            _lobbyManager = FindObjectOfType<LobbyManager>();
            _captainsMess = FindObjectOfType<CaptainsMess>();
        }

        protected void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            _updatePlayerLobbyInfo = true;
            StartCoroutine(RequestForPlayerUpdates());
        }

        public void StopUpdates()
        {
            _updatePlayerLobbyInfo = false;
        }

        public void StartMatch()
        {
            Debug.Log("Starting match");
            _updatePlayerLobbyInfo = false;
            CaptainsMessNetworkManager.singleton.ServerChangeScene("MatchScene");
        }

        public void EndMatch()
        {
            Debug.Log("Ending match");
            _updatePlayerLobbyInfo = false;
            CaptainsMessNetworkManager.singleton.ServerChangeScene("EOGScene");
        }

        [Command]
        public void CmdSubmitAction(string actionName, string actionValue)
        {
            if (_actionDispatcher == null)
            {
                _actionDispatcher = FindObjectOfType<ActionDispatcher>();
            }

            _actionDispatcher.DispatchAction(actionName, actionValue, peerId);
        }

        [Command]
        public void CmdPlayerUpdated(string name, string peerId)
        {
            var waiterIndex = _lobbyManager.GetWaiterIndex(peerId);

            RpcPlayerUpdated(waiterIndex, name);
        }

        [ClientRpc]
        public void RpcPlayerUpdated(int waiterIndex, string name)
        {
            if (_captainsMess.IsConnected() && _lobbyManager != null)
            {
                _lobbyManager.SetWaiterName(waiterIndex, name);
            }
            else
            {
                if (!_captainsMess.IsConnected())
                {
                    Debug.Log("Cannot set waiter name because captains mess is not connected");
                }
                else
                {
                    Debug.Log("Cannot set waiter name because lobbyManager is null");
                }
            }
        }

        [TargetRpc]
        public void TargetHideReadyButton(NetworkConnection connection)
        {
            _lobbyManager.HidePlayerInputs();
        }

        [Command]
        public void CmdSetWaiterReadyStatus(bool ready, string peerId)
        {
            var waiterIndex = _lobbyManager.GetWaiterIndex(peerId);

            RpcSetReadyStatus(ready, waiterIndex);
        }

        [ClientRpc]
        public void RpcSetReadyStatus(bool ready, int waiterIndex)
        {
            _lobbyManager.SetReadyStatus(waiterIndex, ready);
        }

        public void Reconnect()
        {
            _lobbyManager = FindObjectOfType<LobbyManager>();
            _captainsMess = FindObjectOfType<CaptainsMess>();

            if (isLocalPlayer)
            {
                if (_lobbyManager != null && IsReady())
                {
                    _lobbyManager.ChangeReadyStatus();
                }

                _updatePlayerLobbyInfo = true;
                StartCoroutine(RequestForPlayerUpdates());
            }
        }

        protected void OnLevelWasLoaded(int level)
        {
            if (level == 0)
            {
                _lobbyManager = FindObjectOfType<LobbyManager>();
                _captainsMess = FindObjectOfType<CaptainsMess>();
            }
        }

        private IEnumerator RequestForPlayerUpdates()
        {
            Debug.Log("Starting to send player updates");
            while (_updatePlayerLobbyInfo)
            {
                yield return new WaitForSeconds(1);
                _lobbyManager?.SendPlayerUpdate();
            }
            Debug.Log("Stopping sending player updates");
        }
    }
}
