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

        private bool _requestForUpdates;
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

            if (isServer)
            {
                _requestForUpdates = true;
                _updatesRequestCoroutine = StartCoroutine(RequestForPlayerUpdates());
            }
        }

        public void StartMatch()
        {
            Debug.Log("Starting match");
            _requestForUpdates = false;
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

        [ClientRpc]
        public void RpcSendUpdate()
        {
            Debug.Log("Request for player update received");
            _lobbyManager.SendPlayerUpdate();
        }

        protected void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private IEnumerator RequestForPlayerUpdates()
        {
            while (_requestForUpdates)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("Server requesting for player updates");
                RpcSendUpdate();
            }
        }
    }
}
