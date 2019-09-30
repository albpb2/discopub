using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Players
{
    public class PlayersManager : NetworkBehaviour
    {
        [SerializeField] 
        private Text[] _playerNameTexts;
        
        private SyncListString _playerNames;
        private CaptainsMessNetworkManager _captainsMessNetworkManager;
        private List<CaptainsMessPlayer> _players;

        protected void Awake()
        {
            _playerNames = new SyncListString();
        }

        public void SetPlayerName(string peerId, string playerName)
        {
            RefreshPlayers();

            var player = _players.Single(p => p.peerId == peerId);
            var playerIndex = _players.IndexOf(player);

            _playerNames[playerIndex] = playerName;
            _playerNameTexts[playerIndex].text = playerName;
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
    }
}