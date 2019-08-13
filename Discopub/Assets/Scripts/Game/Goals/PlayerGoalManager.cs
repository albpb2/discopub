using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game.Goals
{
    public class PlayerGoalManager : NetworkBehaviour
    {
        private TMPro.TMP_Text _goalText;
        private Player.Player _player;
        private GoalProvider _goalProvider;
        private Goal _activeGoal;
        private ActionCountdown _actionCountdown;
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
                _actionCountdown.StartCountdown();
                _actionDispatcher.SetPlayerGoalActions(_player.peerId, new List<GoalAction>(ActiveGoal.RequiredActions));
            }
        }

        public void SetPlayer(Player.Player player, ActionCountdown actionCountdown)
        {
            _player = player;
            _actionCountdown = actionCountdown;
            _actionCountdown.onActionCountdownFinished += FailGoal;
        }

        public void RemoveGoals()
        {
            _actionCountdown.StopCountdown();
            _actionDispatcher.SetPlayerGoalActions(_player.peerId, new List<GoalAction>());
        }

        protected void Awake()
        {
            _goalText = GameObject.FindWithTag(Tags.GoalText).GetComponent<TMPro.TMP_Text>();
        }

        protected void Start()
        {
            if (isServer)
            {
                _goalProvider = FindObjectOfType<GoalProvider>();
                _actionDispatcher = FindObjectOfType<ActionDispatcher>();
            }
        }

        protected void OnDisable()
        {
            if (_actionCountdown != null)
            {
                _actionCountdown.onActionCountdownFinished -= FailGoal;
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
