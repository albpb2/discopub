using Assets.Scripts.Extensions;
using Assets.Scripts.Importers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Goals
{
    public class GoalProvider : MonoBehaviour
    {
        private const string GoalFilePath = "Config/Goals";
        private const int MaxGoalActions = 4;

        private List<Goal> _goals;
        private List<Goal> _availableGoals;
        private int _currentGoalIndex;

        public Goal GetNextGoal()
        {
            var nextGoal = _availableGoals[_currentGoalIndex];
            _currentGoalIndex = (_currentGoalIndex + 1) % _availableGoals.Count;
            return nextGoal;
        }

        public void SetAvailableGoals(List<Action> roundActions)
        {
            _goals.Shuffle();

            _availableGoals = new List<Goal>();
            foreach(var goal in _goals)
            {
                if (goal.RequiredActions.All(a => roundActions.Any(ra => ra.Name == a.Name)))
                {
                    _availableGoals.Add(CloneGoal(goal));
                }
            }
            _currentGoalIndex = 0;
        }

        protected void Awake()
        {
            _goals = GoalImporter.ImportGoals(GoalFilePath, true, MaxGoalActions);
        }

        private Goal CloneGoal(Goal goal)
        {
            var serializedGoal = JsonConvert.SerializeObject(goal);
            return JsonConvert.DeserializeObject<Goal>(serializedGoal);
        }
    }
}
