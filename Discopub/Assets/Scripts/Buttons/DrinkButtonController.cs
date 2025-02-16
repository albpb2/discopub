﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class DrinkButtonController : MonoBehaviour
    {
        private const string DefaultDrinkButtonActionName = "";

        private string _actionName;
        private Player.Player _player;

        public void SetUp(string actionName, string actionText, string playerPeerId)
        {
            _actionName = actionName;

            var captainsMessNetworkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;
            _player = captainsMessNetworkManager.LobbyPlayers().Single(p => p.peerId == playerPeerId) as Player.Player;

            var buttonText = GetComponentInChildren<Text>();
            buttonText.text = actionText.ToUpper();
        }

        public void SubmitAction()
        {
            _player.CmdSubmitAction(_actionName, DefaultDrinkButtonActionName);
        }
    }
}
