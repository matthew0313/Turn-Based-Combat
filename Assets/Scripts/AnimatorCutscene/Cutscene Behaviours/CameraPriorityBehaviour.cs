using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace AnimatorCutscene
{
    public class CameraPriorityBehaviour : StateMachineBehaviour
    {
        [SerializeField] int cameraIndex;
        [SerializeField] int priorityAdd = 10;
        Cutscene cutscene;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(cutscene == null) cutscene = animator.GetComponent<Cutscene>();
            cutscene.AddCameraPriority(cameraIndex, priorityAdd);
        }
    }
}