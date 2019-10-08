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
        private Coroutine _updatesRequestCoroutine;

        protected void Start()
        {
            _lobbyManager = FindObjectOfType<LobbyManager>();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            var playerCommandSender = FindObjectOfType<PlayerCommandSender>();
            playerCommandSender.SetPlayer(this);

            _updatePlayerLobbyInfo = true;
            _updatesRequestCoroutine = StartCoroutine(RequestForPlayerUpdates());
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

        protected void Awake()
        {
            DontDestroyOnLoad(this);
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
