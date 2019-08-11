using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField]
        private ProgressBar _actionProgressBar;
        [SerializeField]
        private MatchPointsCounter _matchPointsCounter;
        [SerializeField]
        private RoundManager _roundManager;

        private IEnumerator StartFirstRound()
        {
            const int secondsToStartFirstRound = 3;
            yield return new WaitForSeconds(secondsToStartFirstRound);
        }

        protected void Start()
        {
            StartCoroutine(StartFirstRound());

            if (isServer)
            {
                _roundManager.SetUpRound(1);
                const int maxPoints = 200;
                _matchPointsCounter.SetMaxPoints(maxPoints);
            }
        }
    }
}
