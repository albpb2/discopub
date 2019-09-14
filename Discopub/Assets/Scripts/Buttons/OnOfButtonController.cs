using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class OnOfButtonController : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        private bool _enabled = false;

        public const string OnValue = "ON";
        public const string OffValue = "OFF";

        private string _actionName;
        private Player.Player _player;

        public void SetUp(string actionName, string actionText, string playerPeerId)
        {
            _actionName = actionName;

            var captainsMessNetworkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;
            _player = captainsMessNetworkManager.LobbyPlayers().Single(p => p.peerId == playerPeerId) as Player.Player;

            _text.text = actionText;
        }

        public void SetOn()
        {
            if (_enabled)
            {
                return;
            }

            _player.CmdSubmitAction(_actionName, OnValue);
        }

        public void SetOff()
        {
            if (!_enabled)
            {
                return;
            }

            _player.CmdSubmitAction(_actionName, OffValue);
        }
    }
}
