using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace AnimatorCutscene
{
    public class ConversationCloseBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CutsceneManager.Instance.CloseConversation();
        }
    }
}