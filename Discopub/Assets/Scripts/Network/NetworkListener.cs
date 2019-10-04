using Assets.Scripts.Lobby.UI;
using System.Collections.Generic;
using Assets.Scripts.Scenes;
using TMPro;
using UnityEngine;

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
            _loadingText.gameObject.SetActive(false);
            _lobbyCountdown.gameObject.SetActive(true);
        }
    }
}
