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

        private HashSet<string> _multiValueButtonTypes = new HashSet<string>
        {
            ActionControlType.OnOffButton,
            ActionControlType.MultiValueButton,
            ActionControlType.MultiValueSlider
        };

        public void StartNextGoal()
        {
            if (isServer)
            {
                var nextGoal = _goalProvider.GetNextGoal();
                var (goalValue, goalText) = SelectUnusedControlValueForRequiredAction(nextGoal);
                nextGoal.RequiredActions[0].Value = goalValue;

                _activeGoal = nextGoal;

                goalText = GetGoalText(_activeGoal, goalText);
                TargetSetGoalText(_player.connectionToClient, goalText);

                _actionCountdown.StartCountdown();
                _actionDispatcher.SetPlayerGoalActions(_player.peerId, new List<GoalAction>(_activeGoal.RequiredActions));
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

        private string GetGoalText(Goal goal, string currentGoalText)
        {
            if (goal.ControlType == ActionControlType.OnOffButton)
            {
                if (goal.RequiredActions[0].Value == OnOffButtonController.OnValue)
                {
                    return $"Encender {goal.Text}";
                }

                return $"Apagar {goal.Text}";
            }
            else if (goal.ControlType == ActionControlType.MultiValueButton)
            {
                return $"{goal.Text} ({currentGoalText})";
            }
            else if (goal.ControlType == ActionControlType.MultiValueSlider)
            {
                return $"{goal.Text} {currentGoalText}";
            }

            return goal.Text;
        }

        [TargetRpc]
        private void TargetSetGoalText(NetworkConnection connection, string goalText)
        {
            if (_goalText == null)
            {
                _goalText = GameObject.FindWithTag(Tags.GoalText).GetComponent<TMPro.TMP_Text>();
            }
            
            _goalText.text = goalText;
        }

        private void FailGoal()
        {
            _actionDispatcher.FailAction(_player.peerId);
            StartNextGoal();
        }

        private (string value, string text) SelectUnusedControlValueForRequiredAction(Goal nextGoal)
        {
            if (_multiValueButtonTypes.Contains(nextGoal.ControlType))
            {
                return _multiValueControlsManager.GetRandomControlValue(nextGoal.RequiredActions[0].Name);
            }

            return (nextGoal.RequiredActions[0].Value, nextGoal.Text);
        }
    }
}
