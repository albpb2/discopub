using Assets.Scripts.Game;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    public class Player : CaptainsMessPlayer
    {
        private ActionDispatcher _actionDispatcher;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            var playerCommandSender = FindObjectOfType<PlayerCommandSender>();
            playerCommandSender.SetPlayer(this);

            SendReadyToBeginMessage();
        }

        public void StartMatch()
        {
            Debug.Log("Starting match");
            CaptainsMessNetworkManager.singleton.ServerChangeScene("MatchScene");
        }

        [Command]
        public void CmdSubmitAction(string actionName)
        {
            if (_actionDispatcher == null)
            {
                _actionDispatcher = FindObjectOfType<ActionDispatcher>();
            }

            _actionDispatcher.DispatchAction(actionName, peerId);
        }

        protected void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
