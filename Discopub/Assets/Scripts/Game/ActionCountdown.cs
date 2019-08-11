using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class ActionCountdown : NetworkBehaviour
    {
        private ProgressBar _progressBar;
        private CaptainsMessPlayer _player;
        private float _totalSeconds;
        private float _remainingSeconds;
        private bool _active;

        public delegate void ActionCountdownFinished();
        public event ActionCountdownFinished onActionCountdownFinished;

        public void AssignToPlayer(CaptainsMessPlayer player)
        {
            _player = player;
        }

        public void SetTotalTime(float seconds)
        {
            Debug.Log($"Total time set to {seconds}");
            if (_active)
            {
                throw new InvalidOperationException();
            }

            _totalSeconds = seconds;
        }

        public void StartCountdown()
        {
            _remainingSeconds = _totalSeconds;
            UpdateProgressBar(_remainingSeconds);
            _active = true;
            StartCoroutine(UpdateProgressBar());
        }

        public void StopCountdown()
        {
            _active = false;
        }

        public void ResetCountdown()
        {
            _remainingSeconds = _totalSeconds;
            UpdateProgressBar(_remainingSeconds);
        }

        private void UpdateProgressBar(float value)
        {
            var normalizedValue = value * 100 / _totalSeconds;
            TargetUpdateProgressBar(_player.connectionToClient, normalizedValue);
        }

        [TargetRpc]
        public void TargetUpdateProgressBar(NetworkConnection connection, float value)
        {
            if (_progressBar == null)
            {
                _progressBar = FindObjectOfType<ProgressBar>();
            }

            _progressBar.BarValue = value;
        }

        protected void Update()
        {
            if (_active)
            {
                _remainingSeconds = _remainingSeconds - Time.deltaTime;

                if (_remainingSeconds <= 0)
                {
                    FinishTimer();
                }
            }
        }

        private void FinishTimer()
        {
            UpdateProgressBar(_remainingSeconds);
            StopCountdown();

            if (onActionCountdownFinished != null)
            {
                onActionCountdownFinished();
            }

            StartCoroutine(RestartTimer());
        }

        private IEnumerator UpdateProgressBar()
        {
            while (_active)
            {
                const float updatePeriodSeconds = 0.5f;
                yield return new WaitForSeconds(updatePeriodSeconds);
                UpdateProgressBar(_remainingSeconds);
            }
        }

        private IEnumerator RestartTimer()
        {
            const float secondsToRestartTimer = 1.5f;
            yield return new WaitForSeconds(secondsToRestartTimer);
            ResetCountdown();
            StartCountdown();
        }
    }
}
