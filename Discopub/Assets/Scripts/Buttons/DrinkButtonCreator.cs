using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public class DrinkButtonCreator : ButtonCreator
    {
        private readonly GameObject _drinkPrefab;
        private readonly GameObject _beerPrefab;

        public DrinkButtonCreator(ButtonInstantiator buttonInstantiator, GameObject drinkPrefab, GameObject beerPrefab) : base(buttonInstantiator, drinkPrefab)
        {
            _drinkPrefab = drinkPrefab;
            _beerPrefab = beerPrefab;
        }

        protected override GameObject GetButtonPrefab(Action action)
        {
            return action.AdditionalProperties[DrinkAdditionalProperties.DrinkType] == DrinkType.Drink
                ? _drinkPrefab
                : _beerPrefab;
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponentInChildren<DrinkButtonController>();
            controller.SetUp(action.Name, action.Text, playerPeerId);
        }
    }
}
