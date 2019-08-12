using Assets.Scripts.Buttons;
using Assets.Scripts.Extensions;
using Assets.Scripts.Game.Goals;
using Assets.Scripts.Importers;
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
        private const int MaxPoints = 500;

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
        private GameObject _endOfRoundPanel;
        [SerializeField]
        private MatchPointsCounter _matchPointsCounter;

        private List<Player.Player> _players;
        private Dictionary<string, ActionCountdown> _actionCountdowns;
        private Dictionary<string, PlayerGoalManager> _playerGoalManagers;
        private List<Action> _actions;
        private int _currentRound;
        private bool _isCurrentRoundActive;
        private List<GameDifficulty> _difficultyLevels;
        private System.Random _random;
        private List<Goal> _goals;
        private List<Action> _roundActions;
        private List<Goal> _roundGoals;

        public void SetUpRound(int roundNumber)
        {
            _currentRound = roundNumber;

            var roundDifficulty = _difficultyLevels.Single(d => 
                (!d.MinRound.HasValue || roundNumber >= d.MinRound.Value)
                && (!d.MaxRound.HasValue || roundNumber <= d.MaxRound.Value));

            SetRoundTime(roundNumber, roundDifficulty);

            var serializedActions = JsonUtility.ToJson(_actions);
            var clonedActions = JsonUtility.FromJson<List<Action>>(serializedActions);
            clonedActions.Shuffle();

            var serializedGoals = JsonUtility.ToJson(_goals);
            var clonedGoals = JsonUtility.FromJson<List<Goal>>(serializedGoals);
            clonedGoals.Shuffle();

            _roundActions = new List<Action>();

            foreach (var player in _players)
            {
                var playerActions = new List<Action>();

                const int MinSimpleActions = 2;
                const int MaxSimpleActions = 3;
                var numberOfActionButtons = _random.Next(MinSimpleActions, MaxSimpleActions + 1);
                for (var i = 0; i < numberOfActionButtons; i++)
                {
                    var action = clonedActions.First(a => a.ActionPoints == 1);
                    playerActions.Add(action);
                    clonedActions.Remove(action);
                }

                var remainingActionPoints = roundDifficulty.ActionPoints - numberOfActionButtons;

                while (remainingActionPoints > 0)
                {
                    var action = clonedActions.First(a => a.ActionPoints <= remainingActionPoints);
                    playerActions.Add(action);
                    clonedActions.Remove(action);
                }

                _actionButtonsPanelCreator.TargetCreateActionButtonsPanels(player.connectionToClient, playerActions.ToArray());

                _roundActions.AddRange(playerActions);
            }

            _roundGoals = new List<Goal>();
            foreach (var goal in clonedGoals)
            {
                if (goal.RequiredActions.All(a => _roundActions.Any(sa => sa.Name == a.Name)))
                {
                    _roundGoals.Add(goal);
                }
            }

            foreach(var actionCountdown in _actionCountdowns.Values)
            {
                actionCountdown.SetTotalTime(roundDifficulty.ActionSeconds);
            }

            _matchPointsCounter.SetMaxPoints(MaxPoints);
            _matchPointsCounter.SetPoints(0);
        }

        public void ScheduleRoundStart()
        {
            StartCoroutine(StartRoundWithDelay());
        }

        protected void Awake()
        {
            _random = new System.Random();
            _timer.onTimerEnded += EndRound;
        }

        protected void Start()
        {
            ImportActions();
            ImportDifficultyLevels();
            
            var networkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;

            _players = networkManager.LobbyPlayers().Cast<Player.Player>().ToList();

            ClientScene.RegisterPrefab(_actionCountdownPrefab);
            ClientScene.RegisterPrefab(_playerGoalManagerPrefab);

            foreach (var player in _players)
            {
                var actionCountdown = InstantiateActionCountdown(player);
                InstantiatePlayerGoalManager(player, actionCountdown);
            }
        }

        protected void OnDisable()
        {
            _timer.onTimerEnded -= EndRound;
        }

        private void SetRoundTime(int roundNumber, GameDifficulty roundDifficulty)
        {
            const int MinActionsPerPlayer = 5;
            var minRoundActionsPerPlayer = MinActionsPerPlayer + roundNumber;
            _timer.SetTime(roundDifficulty.ActionSeconds * minRoundActionsPerPlayer);
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

        private void ImportActions()
        {
            _actions = ActionImporter.ImportActions("Config/Actions", true).ToList();
        }

        private void ImportDifficultyLevels()
        {
            _difficultyLevels = GameDifficultyImporter.ImportGamedifficultyLevels("Config/DifficultyLevels", true).ToList();
        }

        private void ImportGoals()
        {
            _goals = GoalImporter.ImportGoals("Config/Goals", true, 2).ToList();
        }

        private void EndRound()
        {
            foreach(var playerGoalManager in _playerGoalManagers.Values)
            {
                playerGoalManager.RemoveGoals();
            }

            _endOfRoundPanel.SetActive(true);
            SetUpRound(_currentRound++);
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
                _actionButtonsPanelCreator.EnableActionButtonsPannel(player.connectionToClient);
            }

            _timer.StartTimer();

            foreach (var player in _players)
            {
                _playerGoalManagers[player.peerId].StartNextGoal();
            }
        }
    }
}
