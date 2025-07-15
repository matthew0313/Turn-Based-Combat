using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace AnimatorCutscene
{
    public class ConversationChoiceBehaviour : StateMachineBehaviour
    {
        [SerializeField] ConversationSegment segment;
        [SerializeField] ConversationChoiceSegment choiceSegment;
        readonly int progressID = Animator.StringToHash("Progress");
        readonly int choiceID = Animator.StringToHash("Choice");

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CutsceneManager.Instance.OpenConversation(segment, () =>
            {
                CutsceneManager.Instance.OpenConversationChoice(choiceSegment, (index) =>
                {
                    animator.SetInteger(choiceID, index);
                    animator.SetTrigger(progressID);
                });
            }, true);
        }
    }
    [System.Serializable]
    public class ConversationChoiceSegment
    {
        [SerializeField] string[] m_options;
        public string[] options => m_options;
    }
}