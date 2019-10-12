using System.Collections.Generic;
using Assets.Scripts.Scenes;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class NetworkListener : CaptainsMessListener
    {
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private CaptainsMess _captainsMess;

        public override void OnStartGame(List<CaptainsMessPlayer> aStartingPlayers)
        {
            base.OnStartGame(aStartingPlayers);

            if (_captainsMess.LocalPlayer().isServer)
            {
                var localPlayer = _captainsMess.LocalPlayer() as Player.Player;
                localPlayer.StartMatch();
            }
        }

        public override void OnCountdownStarted()
        {
            _lobbyManager.HidePlayerInputs();

            var localPlayer = _captainsMess.LocalPlayer() as Player.Player;
            localPlayer.StopUpdates();
        }

        public override void OnReconnect()
        {
            base.OnReconnect();

            var localPlayer = _captainsMess.LocalPlayer() as Player.Player;
            localPlayer.Reconnect();
        }
    }
}
