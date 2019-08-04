using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField]
        private Timer _timer;

        void Awake()
        {
            const int timerSeconds = 120;
            _timer.SetTime(timerSeconds);
        }

        private IEnumerator StartTimer()
        {
            const int secondsToStartTimer = 3;
            yield return new WaitForSeconds(secondsToStartTimer);

            Debug.Log("Starting timer from match manager");
            _timer.StartTimer();
        }
    }
}
