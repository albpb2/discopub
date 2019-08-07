using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Controls the target actions for every player.
    /// </summary>
    public class PlayerActionsManager : MonoBehaviour
    {
        private Dictionary<string, List<string>> _actionsPerPlayer;

        public void InitializeActions(List<CaptainsMessPlayer> players)
        {
            _actionsPerPlayer = new Dictionary<string, List<string>>();

            var i = 0;

            foreach (var player in players)
            {
                _actionsPerPlayer.Add(player.peerId, new List<string>());
                _actionsPerPlayer[player.peerId].Add($"Player {i} test target rpc 1");
                _actionsPerPlayer[player.peerId].Add($"Player {i} test target rpc 2");
                i++;
            }
        }
    }
}
