﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class Timer : NetworkBehaviour
    {
        private const int ClientRefreshPeriodSeconds = 1;

        [SerializeField]
        private float _totalSeconds;
        [SerializeField]
        private TMPro.TMP_Text _timerText;

        public delegate void TimerEndedHandler();
        public event TimerEndedHandler onTimerEnded;

        private float _remainingSeconds;
        private bool _started;
        private Coroutine _refreshClientsCoroutine;

        public void SetTime(int seconds)
        {
            _totalSeconds = seconds;
            _remainingSeconds = _totalSeconds;
        }

        [ClientRpc]
        public void RpcSetRemainingTime(float seconds)
        {
            if (isServer)
            {
                return;
            }

            _remainingSeconds = seconds;
            RefreshTimerText();
        }

        public void StartTimer()
        {
            Debug.Log("Starting timer");
            _started = true;
            _remainingSeconds = _totalSeconds;

            if (isServer)
            {
                Debug.Log("Server starting refresh clients coroutine");
                _refreshClientsCoroutine = StartCoroutine(SendTimeToClients());
            }
        }

        public IEnumerator SendTimeToClients()
        {
            if (!isServer)
            {
                Debug.Log("It's not server, exiting timer coroutine");
                yield break;
            }

            while (_started)
            {
                yield return new WaitForSeconds(ClientRefreshPeriodSeconds);
                RpcSetRemainingTime(_remainingSeconds);
            }
        }

        void Update()
        {
            if (!_started)
            {
                return;
            }

            _remainingSeconds = _remainingSeconds - Time.deltaTime;

            if (_remainingSeconds <= 0)
            {
                EndTimer();
            }

            RefreshTimerText();
        }

        private void EndTimer()
        {
            _started = false;
            _remainingSeconds = 0;

            if (isServer)
            {
                Debug.Log("Sending final time to client (0)");
                StopCoroutine(_refreshClientsCoroutine);
                RpcSetRemainingTime(0);

                onTimerEnded?.Invoke();
            }
        }

        private void RefreshTimerText()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_remainingSeconds);
            _timerText.text = string.Format("{0:D1}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
