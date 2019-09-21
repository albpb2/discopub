using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class OnOffButtonCreator : ButtonCreator
    {
        public OnOffButtonCreator(ButtonInstantiator buttonInstantiator, GameObject onOffButtonPrefab) : base(buttonInstantiator, onOffButtonPrefab)
        {
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponent<OnOffButtonController>();
            controller.SetUp(action.Name, action.Text, playerPeerId);

            var parentLayout = button.GetComponentInParent<HorizontalOrVerticalLayoutGroup>();
            var onOffButtonsPanel = button.GetComponentsInChildren<LayoutElement>()[2].gameObject;
            HorizontalOrVerticalLayoutGroup finalChildLayout;

            if (parentLayout is VerticalLayoutGroup)
            {
                finalChildLayout = onOffButtonsPanel.AddComponent<HorizontalLayoutGroup>();
            }
            else
            {
                finalChildLayout = onOffButtonsPanel.AddComponent<VerticalLayoutGroup>();
            }

            var buttons = button.GetComponentsInChildren<Button>();
            foreach(var b in buttons)
            {
                button.transform.parent = button.transform;
            }
        }
    }
}
