using Assets.Scripts.Lobby.UI;
using System.Collections.Generic;
using Assets.Scripts.Scenes;
using TMPro;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Network
{
    public class NetworkListener : CaptainsMessListener
    {
        [SerializeField]
        private TMP_Text _loadingText;
        [SerializeField]
        private LobbyCountdown _lobbyCountdown;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private CaptainsMess _captainsMess;

        public override void OnStartGame(List<CaptainsMessPlayer> aStartingPlayers)
        {
            base.OnStartGame(aStartingPlayers);

            foreach(var player in aStartingPlayers)
            {
                if (player.isServer)
                {
                    var discoPubPlayer = player as Player.Player;
                    discoPubPlayer.StartMatch();
                }
            }
        }

        public override void OnCountdownStarted()
        {
            foreach (var player in _captainsMess.Players().Cast<Player.Player>())
            {
                player.TargetHideReadyButton(player.connectionToClient);
            }

            _loadingText.gameObject.SetActive(true);
            _lobbyCountdown.gameObject.SetActive(true);
        }
    }
}
