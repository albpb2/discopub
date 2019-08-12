using Assets.Scripts.Extensions;
using Assets.Scripts.Importers;
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
        private LinkedList<Goal> _availableGoals;
        private LinkedListNode<Goal> _lastGoal;

        public Goal GetNextGoal()
        {
            var nextGoal = _lastGoal.Next;
            _lastGoal = nextGoal;
            return nextGoal.Value;
        }

        public void SetAvailableGoals(List<Action> roundActions)
        {
            _goals.Shuffle();

            _availableGoals = new LinkedList<Goal>();
            foreach(var goal in _goals)
            {
                if (goal.RequiredActions.All(a => roundActions.Any(ra => ra.Name == a.Name)))
                {
                    _availableGoals.AddLast(CloneGoal(goal));
                }
            }
            _lastGoal = _availableGoals.Last;
        }

        protected void Awake()
        {
            _goals = GoalImporter.ImportGoals(GoalFilePath, true, MaxGoalActions);
        }

        private Goal CloneGoal(Goal goal)
        {
            var clonedGoal = new Goal();
            clonedGoal.Name = goal.Name;
            clonedGoal.Text = goal.Text;
            clonedGoal.RequiredActions = new List<GoalAction>();
            foreach (var goalAction in goal.RequiredActions)
            {
                var clonedGoalAction = new GoalAction
                {
                    Name = goalAction.Name,
                    Value = goalAction.Value
                };
            }
            return clonedGoal;
        }
    }
}
