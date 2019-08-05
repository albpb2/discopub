using Assets.Scripts.Buttons;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField]
        private Timer _timer;
        [SerializeField]
        private ButtonPanelManager _buttonPanelManager;

        void Awake()
        {
            const int timerSeconds = 120;
            _timer.SetTime(timerSeconds);

            StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            const int secondsToStartTimer = 3;
            yield return new WaitForSeconds(secondsToStartTimer);

            Debug.Log("Starting timer from match manager");
            _timer.StartTimer();

            _buttonPanelManager.CreatePanel();
        }
    }
}
