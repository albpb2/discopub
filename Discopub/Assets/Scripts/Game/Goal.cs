using System.Collections.Generic;

namespace Assets.Scripts.Game
{
    public class Goal
    {
        public string Name { get; set; }

        public string ControlType { get; set; }

        public string Text { get; set; }

        public List<GoalAction> RequiredActions { get; set; }
    }
}
