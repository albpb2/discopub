using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField]
        private RoundManager _roundManager;

        protected void Start()
        {
            if (isServer)
            {
                _roundManager.SetUpRound(1);
                _roundManager.ScheduleRoundStart();
            }
        }
    }
}
