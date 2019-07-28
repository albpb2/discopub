namespace Assets.Scripts.Player
{
    public class Player : CaptainsMessPlayer
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            var playerCommandSender = FindObjectOfType<PlayerCommandSender>();
            playerCommandSender.SetPlayer(this);

        }
    }
}
