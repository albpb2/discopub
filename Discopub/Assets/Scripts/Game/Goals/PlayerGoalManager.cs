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
            }
        }

        public void SetPlayer(Player.Player player)
        {
            _player = player;
        }

        protected void Awake()
        {
            if (isServer)
            {
                _goalProvider = FindObjectOfType<GoalProvider>();
            }
        }

        [TargetRpc]
        private void TargetSetGoalText(NetworkConnection connection, string goalText)
        {
            _goalText.text = goalText;
        }
    }
}
