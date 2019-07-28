using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Lobby.UI
{
    public class LobbyCountdown : MonoBehaviour
    {
        private TMP_Text _text;
        private CaptainsMess _captainsMess;

        public void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _captainsMess = FindObjectOfType<CaptainsMess>();

            StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            var timer = _captainsMess.countdownDuration;
            while (timer != 0)
            {
                timer = _captainsMess.CountdownTimer();
                _text.text = ((int)timer).ToString();

                const float timerRefreshRateSeconds = 0.2f;
                yield return new WaitForSeconds(timerRefreshRateSeconds);
            }
        }
    }
}
