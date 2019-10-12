using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class EndOfRoundPanelAnimator : MonoBehaviour
    {
        private const string PanelEnterAnimationTrigger = "CurrentRoundEnding";
        private const string PanelExitAnimationTrigger = "NextRoundStarting";

        [SerializeField]
        private Animator _animator;

        public void AnimateEndOfRoundPanelEnter()
        {
            _animator.SetBool(PanelExitAnimationTrigger, false);
            _animator.SetBool(PanelEnterAnimationTrigger, true);
        }

        public void AnimateEndOfRoundPanelExit()
        {
            _animator.SetBool(PanelEnterAnimationTrigger, false);
            _animator.SetBool(PanelExitAnimationTrigger, true);
        }
    }
}
