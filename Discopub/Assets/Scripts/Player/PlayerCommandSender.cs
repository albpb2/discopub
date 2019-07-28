using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCommandSender : MonoBehaviour
    {
        private Player _player;

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        public void SendMessage() => _player.SendMessage();
    }
}
