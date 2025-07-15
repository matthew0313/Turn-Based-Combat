using System;
using UnityEngine;

namespace AnimatorCutscene
{
    public class CutsceneFinishBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CutsceneManager.Instance.FinishCutscene();
        }
    }
}
