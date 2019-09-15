using Assets.Scripts.Buttons;
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
        private MultiValueControlsManager _multiValueControlsManager;
        private ActionsManager _actionsManager;

        public Goal ActiveGoal
        {
            get
            {
                return _activeGoal;
            }
            private set
            {
                _activeGoal = value;
                var goalText = GetGoalText(_activeGoal);
                TargetSetGoalText(_player.connectionToClient, goalText);
            }
        }

        public void StartNextGoal()
        {
            if (isServer)
            {
                var nextGoal = _goalProvider.GetNextGoal();
                nextGoal.RequiredActions[0].Value = SelectUnusedControlValueForRequiredAction(nextGoal);

                ActiveGoal = nextGoal;

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
            _actionsManager = ActionsManager.Instance;
        }

        protected void Start()
        {
            if (isServer)
            {
                _goalProvider = FindObjectOfType<GoalProvider>();
                _actionDispatcher = FindObjectOfType<ActionDispatcher>();
                _multiValueControlsManager = FindObjectOfType<MultiValueControlsManager>();

                _actionDispatcher.SetPlayerGoalManager(_player.peerId, this);
            }
        }

        protected void OnDisable()
        {
            if (_actionCountdown != null)
            {
                _actionCountdown.onActionCountdownFinished -= FailGoal;
            }
        }

        private string GetGoalText(Goal goal)
        {
            if (goal.ControlType == ActionControlType.OnOffButton)
            {
                if (goal.RequiredActions[0].Value == OnOffButtonController.OnValue)
                {
                    return $"Encender {goal.Text}";
                }

                return $"Apagar {goal.Text}";
            }

            return goal.Text;
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

        private string SelectUnusedControlValueForRequiredAction(Goal nextGoal)
        {
            if (nextGoal.ControlType == ActionControlType.OnOffButton)
            {
                return _multiValueControlsManager.GetDifferentOnOffControlValue(nextGoal.RequiredActions[0].Name);
            }

            return nextGoal.RequiredActions[0].Value;
        }
    }
}
