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
        }

        protected void Awake()
        {
            DontDestroyOnLoad(this);
        }

        protected void OnLevelWasLoaded(int level)
        {
            if (level == 0)
            {
                _lobbyManager = FindObjectOfType<LobbyManager>();

                if (_lobbyManager != null && IsReady())
                {
                    _lobbyManager.ChangeReadyStatus();
                }

                if (isLocalPlayer)
                {
                    _updatePlayerLobbyInfo = true;
                    StartCoroutine(RequestForPlayerUpdates());
                }
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            var playerCommandSender = FindObjectOfType<PlayerCommandSender>();
            playerCommandSender.SetPlayer(this);

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
        public void CmdPlayerUpdated(string name, int connectionId)
        {
            Debug.Log("Player update arrived to server. Resending it");
            RpcPlayerUpdated(connectionId, name);
        }

        [ClientRpc]
        public void RpcPlayerUpdated(int connectionId, string name)
        {
            Debug.Log("Player update received");
            _lobbyManager.SetWaiterName(connectionId, name);
        }

        [TargetRpc]
        public void TargetHideReadyButton(NetworkConnection connection)
        {
            _lobbyManager.HidePlayerInputs();
        }

        [Command]
        public void CmdSetWaiterReadyStatus(bool ready, int connectionId)
        {
            RpcSetReadyStatus(ready, connectionId);
        }

        [ClientRpc]
        public void RpcSetReadyStatus(bool ready, int connectionId)
        {
            _lobbyManager.SetReadyStatus(connectionId, ready);
        }

        private IEnumerator RequestForPlayerUpdates()
        {
            while (_updatePlayerLobbyInfo)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("Updating player status");
                _lobbyManager?.SendPlayerUpdate();
            }
        }
    }
}
