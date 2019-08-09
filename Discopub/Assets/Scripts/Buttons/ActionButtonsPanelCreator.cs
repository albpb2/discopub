using Assets.Scripts.Game;
using Assets.Scripts.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class ActionButtonsPanelCreator : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _actionButtonsPanelParent;
        [SerializeField]
        private GameObject _actionButtonPrefab;

        [TargetRpc]
        public void TargetCreateActionButtonsPanels(NetworkConnection connection, Action[] actions)
        {
            var initialLayoutType = CalculateInitialLayoutType();
            var initialLayout = InstantiateLayout(initialLayoutType, _actionButtonsPanelParent);
            FillLayout(initialLayout, initialLayoutType, actions.ToList());
        }

        private LayoutType CalculateInitialLayoutType()
        {
            var random = new System.Random();
            var randomNumber = random.Next(0, 2);

            var layoutType = (LayoutType)randomNumber;

            Debug.Log($"Creating layout of type {layoutType}");

            return layoutType;
        }

        private GameObject InstantiateLayout(LayoutType layoutType, GameObject parentObject)
        {
            var layoutGameObject = new GameObject();
            layoutGameObject.transform.parent = parentObject.transform;

            HorizontalOrVerticalLayoutGroup layout;

            switch (layoutType)
            {
                case LayoutType.Horizontal:
                    layout = gameObject.AddComponent<HorizontalLayoutGroup>();
                    break;
                default:
                    layout = gameObject.AddComponent<VerticalLayoutGroup>();
                    break;
            }

            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;

            return layoutGameObject;
        }

        private void FillLayout(GameObject layoutGameObject, LayoutType layoutType, List<Action> actions)
        {
            if (actions.Count == 1)
            {
                var prefab = ResolveActionPrefab(actions[0].ControlType);
                var controlGameObject = Instantiate(prefab);
                controlGameObject.transform.parent = layoutGameObject.transform;
            }
            else
            {
                var halfActionsCount = Mathf.CeilToInt(actions.Count / 2);
                var firstHalfOfActions = actions.Take(halfActionsCount).ToList();
                var secondHalfOfActions = actions.Skip(halfActionsCount).ToList();

                var inverseLayoutType = CalculateInverseLayout(layoutType);

                var firstLayout = InstantiateLayout(layoutType, layoutGameObject);
                FillLayout(firstLayout, inverseLayoutType, firstHalfOfActions);

                var secondLayout = InstantiateLayout(layoutType, layoutGameObject);
                FillLayout(secondLayout, inverseLayoutType, secondHalfOfActions);
            }
        }

        private GameObject ResolveActionPrefab(string actionControlType)
        {
            switch (actionControlType)
            {
                case ActionControlType.ActionButton:
                    return _actionButtonPrefab;
                default:
                    throw new System.Exception($"Trying to instantiate invalid control type: {actionControlType}");
            }
        }

        private LayoutType CalculateInverseLayout(LayoutType layoutType)
        {
            if (layoutType == LayoutType.Horizontal)
            {
                return LayoutType.Vertical;
            }

            return LayoutType.Horizontal;
        }
    }
}
