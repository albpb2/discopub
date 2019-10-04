using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Scenes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Players
{
    public class PlayersManager : NetworkBehaviour
    {
        [SerializeField] private LobbyManager _lobbyManager;
        
        private SyncListString _playerNames;
        private CaptainsMessNetworkManager _captainsMessNetworkManager;
        private List<CaptainsMessPlayer> _players;

        protected void Awake()
        {
            _playerNames = new SyncListString();
        }

        protected void Start()
        {
            _playerNames.Callback = PlayerNamesChanged;
        }

        [Command]
        public void CmdSetPlayerName(int playerConnectionId, string playerName)
        {
            RefreshPlayers();

            var player = _players.Single(p => p.connectionToServer.connectionId == playerConnectionId);
            var playerIndex = _players.IndexOf(player);

            _playerNames[playerIndex] = playerName;
        }

        public void NotifyPlayerJoined(int connectionId)
        {
            if (!isServer)
            {
                return;
            }

            RefreshPlayers();
            var joinedPlayer = _players.Single(p => p.connectionToServer.connectionId == connectionId);
            RpcShowWaiter(_players.IndexOf(joinedPlayer));
        }

        [ClientRpc]
        private void RpcShowWaiter(int playerIndex)
        {
            _lobbyManager.ShowWaiter(playerIndex);
        }

        private void RefreshPlayers()
        {
            if (_captainsMessNetworkManager == null)
            {
                _captainsMessNetworkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;
            }

            _players = _captainsMessNetworkManager.LobbyPlayers();

            for (var i = _playerNames.Count; i < _players.Count; i++)
            {
                _playerNames.Add("");
            }
        }
        
        private void PlayerNamesChanged(SyncListString.Operation op, int itemIndex)
        {
            _lobbyManager.SetWaiterName(itemIndex, _playerNames[itemIndex]);
        }
    }
}