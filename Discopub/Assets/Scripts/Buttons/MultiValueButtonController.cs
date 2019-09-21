using Assets.Scripts.Game;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class MultiValueButtonController : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        private string _actionName;
        private Player.Player _player;
        private MultiValueControlsManager _multiValueControlsManager;

        public void SetUp(string actionName, string actionText, string playerPeerId)
        {
            _actionName = actionName;

            var captainsMessNetworkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;
            _player = captainsMessNetworkManager.LobbyPlayers().Single(p => p.peerId == playerPeerId) as Player.Player;
            _multiValueControlsManager = FindObjectOfType<MultiValueControlsManager>();

            _text.text = actionText;
        }

        public void SetValue(string value)
        {
            _player.CmdSubmitAction(_actionName, value);
            _multiValueControlsManager.CmdSetButtonValue(_actionName, value);
        }
    }
}
