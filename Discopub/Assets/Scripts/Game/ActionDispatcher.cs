using Assets.Scripts.Game.Goals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ActionDispatcher : MonoBehaviour
    {
        private const int PointsToWinPerRightAction = 10;
        private const int PointsToLosePerWrongAction = 5;

        [SerializeField]
        private MatchPointsCounter _matchPointsCounter;

        private Dictionary<string, PlayerGoalManager> _playerGoalManagers;
        private Dictionary<string, List<GoalAction>> _playerGoalActions;

        public void DispatchAction(string actionName, string actionValue, string peerId)
        {
            bool isRightAction = false;
            foreach (var playerGoalAction in _playerGoalActions)
            {
                var requiredAction = playerGoalAction.Value.FirstOrDefault();
                if (requiredAction.Name == actionName &&  requiredAction.Value == actionValue)
                {
                    isRightAction = true;
                    _matchPointsCounter.IncreasePoints(PointsToWinPerRightAction);
                    _playerGoalManagers[playerGoalAction.Key].StartNextGoal();
                    break;
                }
            }

            if (!isRightAction)
            {
                FailAction(peerId);
            }
        }

        public void FailAction(string playerPeerId)
        {
            _matchPointsCounter.DecreasePoints(PointsToLosePerWrongAction);
        }

        public void SetPlayerGoalActions(string playerPeerId, List<GoalAction> goalActions)
        {
            _playerGoalActions[playerPeerId] = goalActions;
        }

        public void SetPlayerGoalManager(string playerPeerId, PlayerGoalManager playerGoalManager)
        {
            _playerGoalManagers[playerPeerId] = playerGoalManager;
        }

        protected void Awake()
        {
            _playerGoalActions = new Dictionary<string, List<GoalAction>>();
            _playerGoalManagers = new Dictionary<string, PlayerGoalManager>();
        }

        private void RemoveGoalAction(string actionName, string actionValue)
        {
            foreach(var playerGoalActions in _playerGoalActions.Values)
            {
                if (playerGoalActions.Any() && playerGoalActions[0].Name == actionName && playerGoalActions[0].Value == actionValue)
                {
                    playerGoalActions.RemoveAt(0);
                    break;
                }
            }
        }
    }
}
