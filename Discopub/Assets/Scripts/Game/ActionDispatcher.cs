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

        public void DispatchAction(string actionName, int peerId)
        {
            if (_playerActionsManager.IsRightAction(actionName))
            {
                _matchPointsCounter.IncreasePoints(PointsToWinPerRightAction);
            }
            else
            {
                FailAction(peerId);
            }
        }

        public void FailAction(int playerPeerId)
        {
            _matchPointsCounter.DecreasePoints(PointsToLosePerWrongAction);
        }

        protected void Awake()
        {
            ActionCountdown.onActionCountdownFinished += FailAction;
        }

        protected void OnDisable()
        {
            ActionCountdown.onActionCountdownFinished -= FailAction;
        }
    }
}
