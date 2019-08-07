using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ActionDispatcher : MonoBehaviour
    {
        private const int PointsToWinPerRightAction = 10;
        private const int PointsToLosePerWrongAction = 5;

        [SerializeField]
        private PlayerActionsManager _playerActionsManager;
        [SerializeField]
        private MatchPointsCounter _matchPointsCounter;

        public void DispatchAction(string actionName)
        {
            if (_playerActionsManager.IsRightAction(actionName))
            {
                _matchPointsCounter.IncreasePoints(PointsToWinPerRightAction);
            }
            else
            {
                _matchPointsCounter.DecreasePoints(PointsToLosePerWrongAction);
            }
        }
    }
}
