using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace AnimatorCutscene
{
    public class CutsceneEventBehaviour : StateMachineBehaviour
    {
        [SerializeField] int eventIndex;
        Cutscene cutscene;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(cutscene == null) cutscene = animator.GetComponent<Cutscene>();
            cutscene.InvokeEvent(eventIndex);
        }
    }
}
