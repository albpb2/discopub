using Assets.Scripts.Buttons;
using Assets.Scripts.Game.Goals;
using Assets.Scripts.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class RoundManager : MonoBehaviour
    {
        private const int SecondsToStartRound = 3;
        private const int ExpectedRoundSetUpDurationSeconds = 1;

        [SerializeField]
        private Timer _timer;
        [SerializeField]
        private GameObject _actionCountdownPrefab;
        [SerializeField]
        private GameObject _playerGoalManagerPrefab;
        [SerializeField]
        private GameObject _networkedObjectsRoot;
        [SerializeField]
        private ActionButtonsPanelCreator _actionButtonsPanelCreator;
        [SerializeField]
        private EndOfRoundPanel _endOfRoundPanel;
        [SerializeField]
        private MatchPointsCounter _matchPointsCounter;
        [SerializeField]
        private GoalProvider _goalProvider;
        [SerializeField]
        private MultiValueControlsManager _multiValueControlsManager;
        [SerializeField]
        private List<string> _debugForceActionControlTypes;

        private List<Player.Player> _players;
        private Dictionary<string, ActionCountdown> _actionCountdowns;
        private Dictionary<string, PlayerGoalManager> _playerGoalManagers;
        private int _currentRound;
        private bool _isCurrentRoundActive;
        private System.Random _random;
        private List<Action> _roundActions;
        private DifficultyLevelManager _difficultyLevelManager;
        private ActionsManager _actionsManager;

        public void ScheduleRoundStart()
        {
            StartCoroutine(StartRoundWithDelay());
        }

        protected void Awake()
        {
            _random = new System.Random();
            LoadDependencies();
        }

        protected void Start()
        {
            _timer.onTimerEnded += EndRound;
            _playerGoalManagers = new Dictionary<string, PlayerGoalManager>();
            _actionCountdowns = new Dictionary<string, ActionCountdown>();

            var networkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;

            _players = networkManager.LobbyPlayers().Cast<Player.Player>().ToList();

            ClientScene.RegisterPrefab(_actionCountdownPrefab);
            ClientScene.RegisterPrefab(_playerGoalManagerPrefab);

            foreach (var player in _players)
            {
                var actionCountdown = InstantiateActionCountdown(player);
                InstantiatePlayerGoalManager(player, actionCountdown);
            }

            ScheduleRoundStart();
            StartCoroutine(SetUpRoundWithDelay(1, ExpectedRoundSetUpDurationSeconds));
        }

        protected void OnDisable()
        {
            _timer.onTimerEnded -= EndRound;
        }

        private void LoadDependencies()
        {
            _difficultyLevelManager = new DifficultyLevelManager();
            _actionsManager = ActionsManager.Instance;
        }

        private void SetRoundTime(int roundNumber, GameDifficulty roundDifficulty)
        {
            const int MinActionsPerPlayer = 5;
            var minRoundActionsPerPlayer = MinActionsPerPlayer + roundNumber;
            _timer.SetTime(roundDifficulty.ActionSeconds * minRoundActionsPerPlayer);
        }

        private List<Action> ChooseRoundActions(GameDifficulty roundDifficulty)
        {
            var actions = _actionsManager.GetShuffledActionsList();
            var roundActions = new List<Action>();

            foreach (var player in _players)
            {
                Debug.Log($"Creating actions for player {player.peerId}.");
                var playerActions = ChoosePlayerActions(actions, roundDifficulty);

                _actionButtonsPanelCreator.TargetCreateActionButtonsPanel(player.connectionToClient, JsonConvert.SerializeObject(playerActions), player.peerId);

                roundActions.AddRange(playerActions);
            }

            return roundActions;
        }

        private List<Action> ChoosePlayerActions(List<Action> actions, GameDifficulty roundDifficulty)
        {
            var playerActions = new List<Action>();
            var remainingActionPoints = roundDifficulty.ActionPoints;

            if (Debug.isDebugBuild)
            {
                var debugForcedActions = GetDebugForcedActions(actions);
                playerActions.AddRange(debugForcedActions);
                remainingActionPoints -= debugForcedActions.Sum(a => a.ActionPoints);
            }

            const int MinSimpleActions = 2;
            const int MaxSimpleActions = 3;
            var numberOfActionButtons = _random.Next(MinSimpleActions, MaxSimpleActions + 1);
            for (var i = 0; i < numberOfActionButtons && remainingActionPoints > 0; i++)
            {
                var action = actions.First(a => a.ActionPoints == 1);
                playerActions.Add(action);
                actions.Remove(action);

                remainingActionPoints -= action.ActionPoints;
            }

            while (remainingActionPoints > 0)
            {
                var action = actions.First(a => a.ActionPoints <= remainingActionPoints);
                playerActions.Add(action);
                actions.Remove(action);
                remainingActionPoints -= action.ActionPoints;
            }

            return playerActions;
        }

        private List<Action> GetDebugForcedActions(List<Action> actions)
        {
            var selectedActions = new List<Action>();

            if (_debugForceActionControlTypes != null && _debugForceActionControlTypes.Any())
            {
                foreach (var controlType in _debugForceActionControlTypes)
                {
                    var action = actions.First(a => a.ControlType == controlType);
                    selectedActions.Add(action);
                    actions.Remove(action);
                }
            }

            return selectedActions;
        }

        private void SetActionTimes(GameDifficulty roundDifficulty)
        {
            foreach (var actionCountdown in _actionCountdowns.Values)
            {
                actionCountdown.SetTotalTime(roundDifficulty.ActionSeconds);
            }
        }

        private ActionCountdown InstantiateActionCountdown(Player.Player player)
        {
            Debug.Log($"Instantiating countdown for player {player.peerId}");
            var actionCountdownGameObject = Instantiate(_actionCountdownPrefab);
            NetworkServer.Spawn(actionCountdownGameObject);

            var actionCountdown = actionCountdownGameObject.GetComponent<ActionCountdown>();
            _actionCountdowns[player.peerId] = actionCountdown;
            actionCountdown.AssignToPlayer(player);

            return actionCountdown;
        }

        private PlayerGoalManager InstantiatePlayerGoalManager(Player.Player player, ActionCountdown actionCountdown)
        {
            Debug.Log($"Instantiating goal manager for player {player.peerId}");
            var playerGoalManagerGameObject = Instantiate(_playerGoalManagerPrefab, _networkedObjectsRoot.transform);
            NetworkServer.Spawn(playerGoalManagerGameObject);

            var playerGoalManager = playerGoalManagerGameObject.GetComponent<PlayerGoalManager>();
            _playerGoalManagers[player.peerId] = playerGoalManager;
            playerGoalManager.SetPlayer(player, actionCountdown);

            return playerGoalManager;
        }

        private void EndRound()
        {
            foreach(var playerGoalManager in _playerGoalManagers.Values)
            {
                playerGoalManager.RemoveGoals();
            }

            _actionButtonsPanelCreator.RpcDestroyPanel();

            _endOfRoundPanel.RpcShowPanel();

            ScheduleRoundStart();
            SetUpRound(_currentRound++);
        }

        private IEnumerator SetUpRoundWithDelay(int roundNumber, int secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            SetUpRound(roundNumber);
        }

        private void SetUpRound(int roundNumber)
        {
            _currentRound = roundNumber;

            var roundDifficulty = _difficultyLevelManager.GetDifficultyLevel(roundNumber);

            SetRoundTime(roundNumber, roundDifficulty);

            _roundActions = ChooseRoundActions(roundDifficulty);

            _goalProvider.SetAvailableGoals(_roundActions);

            _multiValueControlsManager.SetUp(_roundActions);

            SetActionTimes(roundDifficulty);

            _matchPointsCounter.ResetCounter();
        }

        private IEnumerator StartRoundWithDelay()
        {
            yield return new WaitForSeconds(SecondsToStartRound);
            StartRound();
        }

        private void StartRound()
        {
            foreach (var player in _players)
            {
                Debug.Log($"Enabling actions for player {player.peerId}.");
                _actionButtonsPanelCreator.TargetEnableActionButtonsPannel(player.connectionToClient);
            }

            _timer.StartTimer();

            foreach (var player in _players)
            {
                _playerGoalManagers[player.peerId].StartNextGoal();
            }

            _endOfRoundPanel.RpcHidePanel();
        }
    }
}
