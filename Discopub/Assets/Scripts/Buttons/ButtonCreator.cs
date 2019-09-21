using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public abstract class ButtonCreator
    {
        protected readonly ButtonInstantiator _buttonInstantiator;
        protected readonly GameObject _buttonPrefab;

        public ButtonCreator(ButtonInstantiator buttonInstantiator, GameObject buttonPrefab)
        {
            _buttonInstantiator = buttonInstantiator;
            _buttonPrefab = buttonPrefab;
        }

        public void CreateButton(Action action, GameObject layoutGameObject, string playerPeerId)
        {
            var controlGameObject = InstantiateButton(layoutGameObject);
            SetUpButton(controlGameObject, action, playerPeerId);
        }

        protected abstract void SetUpButton(GameObject button, Action action, string playerPeerId);

        private GameObject InstantiateButton(GameObject layoutGameObject)
        {
            return _buttonInstantiator.InstantiateButton(_buttonPrefab, layoutGameObject.transform);
        }
    }
}
