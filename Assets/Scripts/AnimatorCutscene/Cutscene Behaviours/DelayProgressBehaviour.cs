using System;
using UnityEngine;

namespace AnimatorCutscene
{
    public class DelayProgressBehaviour : StateMachineBehaviour
    {
        [SerializeField] float delay;
        readonly int progressID = Animator.StringToHash("Progress");
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            CutsceneManager.Instance.Delay(delay, () => animator.SetTrigger(progressID));
        }
    }
}
