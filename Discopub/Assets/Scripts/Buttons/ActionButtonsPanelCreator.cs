using Assets.Scripts.Game;
using Assets.Scripts.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class ActionButtonsPanelCreator : NetworkBehaviour
    {
        private GameObject actionButtonsPanel;

        [SerializeField]
        private GameObject _actionButtonsPanelParent;
        [SerializeField]
        private GameObject _actionButtonPrefab;
        [SerializeField]
        private GameObject _emptyLayoutPrefab;

        [TargetRpc]
        public void TargetCreateActionButtonsPanels(NetworkConnection connection, string parsedActions, string playerPeerId)
        {
            Debug.Log($"Received rpc to create actions panel");
            var actions = JsonConvert.DeserializeObject<List<Action>>(parsedActions);
            var initialLayoutType = CalculateInitialLayoutType();
            actionButtonsPanel = InstantiateLayout(initialLayoutType, _actionButtonsPanelParent);
            FillLayout(actionButtonsPanel, initialLayoutType, actions.ToList(), playerPeerId);
            actionButtonsPanel.SetActive(false);
        }

        [TargetRpc]
        public void TargetEnableActionButtonsPannel(NetworkConnection connection)
        {
            Debug.Log($"Received rpc to enable actions panel");
            actionButtonsPanel.SetActive(true);
        }

        [ClientRpc]
        public void RpcDestroyPanel()
        {
            Debug.Log($"Received rpc to disable actions panel");
            Destroy(actionButtonsPanel.gameObject);
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
            var layoutGameObject = Instantiate(_emptyLayoutPrefab, parentObject.transform);

            HorizontalOrVerticalLayoutGroup layout;

            switch (layoutType)
            {
                case LayoutType.Horizontal:
                    layout = layoutGameObject.AddComponent<HorizontalLayoutGroup>();
                    break;
                default:
                    layout = layoutGameObject.AddComponent<VerticalLayoutGroup>();
                    break;
            }

            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;

            return layoutGameObject;
        }

        private void FillLayout(GameObject layoutGameObject, LayoutType layoutType, List<Action> actions, string playerPeerId)
        {
            if (actions.Count == 1)
            {
                var prefab = ResolveActionPrefab(actions[0].ControlType);
                var controlGameObject = Instantiate(prefab, layoutGameObject.transform);
                var controller = controlGameObject.GetComponentInChildren<ActionButtonController>();
                controller.SetUp(actions.Single().Name, actions.Single().Values.Single(), playerPeerId);
            }
            else
            {
                var halfActionsCount = Mathf.CeilToInt((float)actions.Count / 2);
                var firstHalfOfActions = actions.Take(halfActionsCount).ToList();
                var secondHalfOfActions = actions.Skip(halfActionsCount).ToList();

                var inverseLayoutType = CalculateInverseLayout(layoutType);

                var firstLayout = InstantiateLayout(inverseLayoutType, layoutGameObject);
                FillLayout(firstLayout, inverseLayoutType, firstHalfOfActions, playerPeerId);

                var secondLayout = InstantiateLayout(inverseLayoutType, layoutGameObject);
                FillLayout(secondLayout, inverseLayoutType, secondHalfOfActions, playerPeerId);
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
