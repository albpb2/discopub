using Assets.Scripts.Buttons;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField]
        private GameObject _actionCountdownPrefab;
        [SerializeField]
        private GameObject _networkedObjectsRoot;
        [SerializeField]
        private ProgressBar _actionProgressBar;

        private List<CaptainsMessPlayer> _players;
        private List<ActionCountdown> _actionCountdowns;

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

        protected void Start()
        {
            _players = (CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager).LobbyPlayers();

            if (isServer)
            {
                StartCountdowns();
            }
        }

        private void StartCountdowns()
        {
            _actionCountdowns = new List<ActionCountdown>();
            foreach (var player in _players)
            {
                Debug.Log($"Starting countdown for player {player.peerId}");
                ClientScene.RegisterPrefab(_actionCountdownPrefab);
                var actionCountdownGameObject = Instantiate(_actionCountdownPrefab);
                NetworkServer.Spawn(actionCountdownGameObject);
                actionCountdownGameObject.transform.parent = _networkedObjectsRoot.transform;

                var actionCountdown = actionCountdownGameObject.GetComponent<ActionCountdown>();
                _actionCountdowns.Add(actionCountdown);
                actionCountdown.AssignToPlayer(player);
                actionCountdown.SetTotalTime(Random.Range(5, 15));
                actionCountdown.StartCountdown();
            }
        }
    }
}
