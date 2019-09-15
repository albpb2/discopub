using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public class DrinkButtonCreator : ButtonCreator
    {
        public DrinkButtonCreator(ButtonInstantiator buttonInstantiator, GameObject actionButtonPrefab) : base(buttonInstantiator, actionButtonPrefab)
        {
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponentInChildren<DrinkButtonController>();
            controller.SetUp(action.Name, action.Values[0], playerPeerId);
        }
    }
}
