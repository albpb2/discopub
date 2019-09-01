using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public class ActionButtonCreator : ButtonCreator
    {
        public ActionButtonCreator(ButtonInstantiator buttonInstantiator, GameObject actionButtonPrefab) : base(buttonInstantiator, actionButtonPrefab)
        {
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponentInChildren<ActionButtonController>();
            controller.SetUp(action.Name, action.Values[0], playerPeerId);
        }
    }
}
