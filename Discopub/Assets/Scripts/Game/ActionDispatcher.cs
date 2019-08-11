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

        private Dictionary<string, List<GoalAction>> _playerGoalActions;

        public void DispatchAction(string actionName, string actionValue, string peerId)
        {
            if (IsRightAction(actionName, actionValue))
            {
                _matchPointsCounter.IncreasePoints(PointsToWinPerRightAction);
            }
            else
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

        protected void Awake()
        {
            _playerGoalActions = new Dictionary<string, List<GoalAction>>();
        }

        private bool IsRightAction(string actionName, string actionValue)
        {
            var activeActions = _playerGoalActions.Select(a => a.Value.FirstOrDefault()).Where(a => a != null);
            return activeActions.Any(a => a.Name == actionName && a.Value == actionValue);
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
