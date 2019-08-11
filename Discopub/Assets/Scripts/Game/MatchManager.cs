using Assets.Scripts.Buttons;
using Assets.Scripts.Importers;
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
        [SerializeField]
        private PlayerActionsManager _playerActionsManager;
        [SerializeField]
        private MatchPointsCounter _matchPointsCounter;
        [SerializeField]
        private ActionButtonsPanelCreator _actionButtonsPanelCreator;

        private List<CaptainsMessPlayer> _players;
        private List<ActionCountdown> _actionCountdowns;
        private Dictionary<string, List<string>> _actionsPerPlayer;

        private IEnumerator StartTimer()
        {
            const int secondsToStartTimer = 3;
            yield return new WaitForSeconds(secondsToStartTimer);

            Debug.Log("Starting timer from match manager");
            _timer.StartTimer();

            //_buttonPanelManager.CreatePanel();

            var actions = ActionImporter.ImportActions("Config/Actions", true).ToArray();
            foreach(var player in _players)
            {
                _actionButtonsPanelCreator.TargetCreateActionButtonsPanels(player.connectionToClient, actions);
            }

            var goals = GoalImporter.ImportGoals("Config/Goals", true, 2).ToArray();

            var difficultyLevels = GameDifficultyImporter.ImportGamedifficultyLevels("Config/DifficultyLevels", true).ToArray();
        }

        protected void Start()
        {
            const int timerSeconds = 120;
            _timer.SetTime(timerSeconds);

            StartCoroutine(StartTimer());

            _players = (CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager).LobbyPlayers();

            if (isServer)
            {
                _playerActionsManager.InitializeActions(_players);
                StartCountdowns();
                const int maxPoints = 200;
                _matchPointsCounter.SetMaxPoints(maxPoints);
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
