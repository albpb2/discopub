using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public class ActionButtonController : MonoBehaviour
    {
        private string _actionName;
        private Player.Player _player;

        public void SetUp(string actionName, string playerPeerId)
        {
            _actionName = actionName;

            var captainsMessNetworkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;
            _player = captainsMessNetworkManager.LobbyPlayers().Single(p => p.peerId == playerPeerId) as Player.Player;
        }

        public void SubmitAction()
        {
            _player.CmdSubmitAction(_actionName);
        }
    }
}
