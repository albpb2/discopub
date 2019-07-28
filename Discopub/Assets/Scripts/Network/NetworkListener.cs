using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network
{
    public class NetworkListener : CaptainsMessListener
    {
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
    }
}
