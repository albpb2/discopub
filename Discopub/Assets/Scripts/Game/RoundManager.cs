using Assets.Scripts.Buttons;
using Assets.Scripts.Extensions;
using Assets.Scripts.Game.Goals;
using Assets.Scripts.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Actions;

namespace Assets.Scripts.Game
{
    public class RoundManager : MonoBehaviour
    {
        private const float MinSecondsToStartFirstRound = 0.3f;
        private const float AdditionalSecondsToStartRoundPerPlayer = 0.2f;
        private const float SecondsToStartRound = 4f;
        private const int FirstRoundNumber = 1;

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
        private DrinkButtonsPanelCreator _drinkButtonsPanelCreator;
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
        [SerializeField] 
        private MatchUIComponentsManager _matchUiComponentsManager;

        private List<Player.Player> _players;
        private Dictionary<string, ActionCountdown> _actionCountdowns;
        private Dictionary<string, PlayerGoalManager> _playerGoalManagers;
        private int _currentRound;
        private bool _isCurrentRoundActive;
        private System.Random _random;
        private List<Action> _roundActions;
        private DifficultyLevelManager _difficultyLevelManager;
        private ActionsManager _actionsManager;

        protected void Awake()
        {
            _random = new System.Random();
            LoadDependencies();
        }

        protected void Start()
        {
            _timer.onTimerEnded += EndRoundWithDefeat;
            _matchPointsCounter.onMaxPointsReached += EndRoundWithVictory;
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

            ScheduleRoundStart(FirstRoundNumber);
            StartCoroutine(SetUpFirstRound());
        }

        protected void OnDisable()
        {
            _timer.onTimerEnded -= EndRoundWithDefeat;
        }

        public void ScheduleRoundStart(int roundNumber)
        {
            StartCoroutine(StartRoundWithDelay(roundNumber));
        }

        private void LoadDependencies()
        {
            _difficultyLevelManager = new DifficultyLevelManager();
            _actionsManager = ActionsManager.Instance;
        }

        private void SetRoundTime(int roundNumber, GameDifficulty roundDifficulty)
        {
            _timer.SetTime(roundDifficulty.RoundTime);
        }

        private List<Action> ChooseRoundActions(GameDifficulty roundDifficulty)
        {
            var actions = _actionsManager.GetSuffledActionsList();
            var drinks = _actionsManager.GetShuffledDrinksList();
            var roundActions = new List<Action>();

            foreach (var player in _players)
            {
                Debug.Log($"Creating actions for player {player.peerId}.");

                var playerActions = ChoosePlayerActions(actions, roundDifficulty);
                _actionButtonsPanelCreator.TargetCreateActionButtonsPanel(player.connectionToClient, JsonConvert.SerializeObject(playerActions), player.peerId);
                roundActions.AddRange(playerActions);

                var playerDrinks = ChoosePlayerDrinks(drinks, roundDifficulty);
                _drinkButtonsPanelCreator.TargetCreateDrinkButtonsPanel(player.connectionToClient, JsonConvert.SerializeObject(playerDrinks), player.peerId);
                roundActions.AddRange(playerDrinks);
            }

            roundActions.Shuffle();

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

            playerActions.Shuffle();

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

        private List<Action> ChoosePlayerDrinks(List<Action> drinks, GameDifficulty roundDifficulty)
        {
            var chosenDrinks = drinks.Take(roundDifficulty.NumberOfDrinks).ToList();

            drinks.RemoveAll(d => chosenDrinks.Contains(d));

            return chosenDrinks;
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

        private IEnumerator SetUpFirstRound()
        {
            const float secondsToWaitForPlayersToBeInScene = 0.2f;
            yield return new WaitForSeconds(secondsToWaitForPlayersToBeInScene);
            SetUpRound(FirstRoundNumber);
        }

        private void SetUpRound(int roundNumber)
        {
            Debug.Log($"Setting up round {roundNumber}");

            _currentRound = roundNumber;

            var roundDifficulty = _difficultyLevelManager.GetDifficultyLevel(roundNumber);

            SetRoundTime(roundNumber, roundDifficulty);

            _roundActions = ChooseRoundActions(roundDifficulty);

            _goalProvider.SetAvailableGoals(_roundActions);

            _multiValueControlsManager.SetUp(_roundActions);

            SetActionTimes(roundDifficulty);

            _matchPointsCounter.SetMaxPoints(roundDifficulty.TargetScore);
            _matchPointsCounter.ResetCounter();

            Debug.Log($"Round {roundNumber} setup finished");
        }

        private IEnumerator StartRoundWithDelay(int roundNumber)
        {
            var secondsToStartRound = roundNumber == FirstRoundNumber 
                ? MinSecondsToStartFirstRound + _players.Count * AdditionalSecondsToStartRoundPerPlayer
                : SecondsToStartRound;
            yield return new WaitForSeconds(secondsToStartRound);
            StartRound();
        }

        private void StartRound()
        {
            Debug.Log($"Starting round {_currentRound}");

            _matchUiComponentsManager.RpcEnableMatchUIComponents();
            
            foreach (var player in _players)
            {
                Debug.Log($"Enabling actions for player {player.peerId}.");
                _actionButtonsPanelCreator.TargetEnableActionButtonsPannel(player.connectionToClient);
                _drinkButtonsPanelCreator.TargetEnableDrinkButtonsPannel(player.connectionToClient);
            }

            _timer.StartTimer();

            foreach (var player in _players)
            {
                _playerGoalManagers[player.peerId].StartNextGoal();
            }

            _endOfRoundPanel.RpcHidePanel();
        }

        private void EndRoundWithDefeat()
        {
            _actionButtonsPanelCreator.RpcDestroyPanel();
            _drinkButtonsPanelCreator.RpcDestroyPanel();

            foreach (var player in _players)
            {
                if (player.isServer)
                {
                    var captainsMess = FindObjectOfType<CaptainsMessNetworkManager>();
                    captainsMess.FinishGame();
                    player.EndMatch();
                }
            }
        }

        private void EndRoundWithVictory()
        {
            _matchUiComponentsManager.RpcDisableMatchUIComponents();

            foreach (var playerGoalManager in _playerGoalManagers.Values)
            {
                playerGoalManager.RemoveGoals();
            }

            _actionButtonsPanelCreator.RpcDestroyPanel();
            _drinkButtonsPanelCreator.RpcDestroyPanel();

            _endOfRoundPanel.RpcShowPanel();

            ScheduleRoundStart(_currentRound + 1);
            SetUpRound(_currentRound + 1);
        }
    }
}
