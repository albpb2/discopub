using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public abstract class ButtonCreator
    {
        protected readonly ButtonInstantiator _buttonInstantiator;
        protected readonly GameObject _defaultButtonPrefab;

        public ButtonCreator(ButtonInstantiator buttonInstantiator, GameObject defaultButtonPrefab)
        {
            _buttonInstantiator = buttonInstantiator;
            _defaultButtonPrefab = defaultButtonPrefab;
        }

        public void CreateButton(Action action, GameObject layoutGameObject, string playerPeerId)
        {
            var controlGameObject = InstantiateButton(layoutGameObject, action);
            SetUpButton(controlGameObject, action, playerPeerId);
        }

        protected abstract void SetUpButton(GameObject button, Action action, string playerPeerId);

        protected virtual GameObject GetButtonPrefab(Action action) => _defaultButtonPrefab;

        private GameObject InstantiateButton(GameObject layoutGameObject, Action action)
        {
            return _buttonInstantiator.InstantiateButton(GetButtonPrefab(action), layoutGameObject.transform);
        }
    }
}
