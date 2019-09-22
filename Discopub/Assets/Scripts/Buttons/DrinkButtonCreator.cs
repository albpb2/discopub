using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public class DrinkButtonCreator : ButtonCreator
    {
        private readonly GameObject _drinkPrefab;
        private readonly GameObject _beerPrefab;
        private readonly ButtonColorManager _buttonColorManager;

        public DrinkButtonCreator(ButtonInstantiator buttonInstantiator, GameObject drinkPrefab, GameObject beerPrefab) : base(buttonInstantiator, drinkPrefab)
        {
            _drinkPrefab = drinkPrefab;
            _beerPrefab = beerPrefab;
            _buttonColorManager = GameObject.FindObjectOfType<ButtonColorManager>();
        }

        protected override GameObject GetButtonPrefab(Action action)
        {
            return action.AdditionalProperties[DrinkAdditionalProperties.DrinkType] == DrinkType.Drink
                ? _drinkPrefab
                : _beerPrefab;
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            if (action.AdditionalProperties[DrinkAdditionalProperties.DrinkType] == DrinkType.Drink)
            {
                ColorDrinkSprite(button);
            }

            var controller = button.GetComponentInChildren<DrinkButtonController>();
            controller.SetUp(action.Name, action.Text, playerPeerId);
        }

        private void ColorDrinkSprite(GameObject button)
        {
            const string ColorizableChildName = "Liquid";
            var colorizableChild = button.transform.Find(ColorizableChildName);
            var colorizableSpriteRenderer = colorizableChild.GetComponent<SpriteRenderer>();
            colorizableSpriteRenderer.color = _buttonColorManager.GetRandomColorBlock().normalColor;
        }
    }
}
