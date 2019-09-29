using Assets.Scripts.Actions;
using System;
using UnityEngine;
using UnityEngine.UI;
using Action = Assets.Scripts.Actions.Action;

namespace Assets.Scripts.Buttons
{
    public class MultiValueSliderCreator : ButtonCreator
    {
        private readonly GameObject _multiValueSliderHorizontal5Prefab;
        private readonly GameObject _multiValueSliderVertical5Prefab;

        public MultiValueSliderCreator(
            ButtonInstantiator buttonInstantiator, 
            GameObject multiValueSliderHorizontal5Prefab,
            GameObject multiValueSliderVertical5Prefab) : base(buttonInstantiator, multiValueSliderHorizontal5Prefab)
        {
            _multiValueSliderHorizontal5Prefab = multiValueSliderHorizontal5Prefab;
            _multiValueSliderVertical5Prefab = multiValueSliderVertical5Prefab;
        }

        protected override GameObject GetButtonPrefab(Action action, GameObject layoutGameObject)
        {
            var horizontalParentLayout = layoutGameObject.GetComponent<HorizontalLayoutGroup>();

            return horizontalParentLayout != null
                ? GetHorizontalButtonPrefab(action)
                : GetVerticalButtonPrefab(action);
        }

        private GameObject GetVerticalButtonPrefab(Action action)
        {
            if (action.Values.Length == 5)
            {
                return _multiValueSliderVertical5Prefab;
            }
            
            throw GetInvalidNumberOfValuesException(action.Values.Length);
        }

        private GameObject GetHorizontalButtonPrefab(Action action)
        {
            if (action.Values.Length == 5)
            {
                return _multiValueSliderHorizontal5Prefab;
            }

            throw GetInvalidNumberOfValuesException(action.Values.Length);
        }

        private Exception GetInvalidNumberOfValuesException(int valuesCount) => 
            new InvalidOperationException($"Trying to initialize a multivalue slider with an invalid number of values: {valuesCount}");

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponent<MultiValueSliderController>();
            controller.SetUp(action.Name, action.Text, playerPeerId, action.Values, action.ValuesTexts);
        }
    }
}
