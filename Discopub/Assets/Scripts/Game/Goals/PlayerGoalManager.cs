using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game.Goals
{
    public class PlayerGoalManager : NetworkBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _goalText;

        private Player.Player _player;
        private GoalProvider _goalProvider;
        private Goal _activeGoal;
        private Timer _timer;
        private ActionDispatcher _actionDispatcher;

        public Goal ActiveGoal
        {
            get
            {
                return _activeGoal;
            }
            private set
            {
                _activeGoal = value;
                TargetSetGoalText(_player.connectionToClient, _activeGoal.Text);
            }
        }

        public void StartNextGoal()
        {
            if (isServer)
            {
                ActiveGoal = _goalProvider.GetNextGoal();
                _timer.StartTimer();
            }
        }

        public void SetPlayer(Player.Player player, Timer timer)
        {
            _player = player;
            _timer = timer;
            _timer.onTimerEnded += FailGoal;
        }

        protected void Awake()
        {
            if (isServer)
            {
                _goalProvider = FindObjectOfType<GoalProvider>();
                _actionDispatcher = FindObjectOfType<ActionDispatcher>();
            }
        }

        protected void OnDisable()
        {
            if (_timer != null)
            {
                _timer.onTimerEnded -= FailGoal;
            }
        }

        [TargetRpc]
        private void TargetSetGoalText(NetworkConnection connection, string goalText)
        {
            _goalText.text = goalText;
        }

        private void FailGoal()
        {
            _actionDispatcher.FailAction(_player.peerId);
            StartNextGoal();
        }
    }
}
