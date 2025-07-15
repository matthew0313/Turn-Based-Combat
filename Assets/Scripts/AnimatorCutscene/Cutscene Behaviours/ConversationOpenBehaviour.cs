using System;
using UnityEngine;
using UnityEngine.Timeline;
using static AnimatorCutscene.CutsceneManager;

namespace AnimatorCutscene
{
    public class ConversationOpenBehaviour : StateMachineBehaviour
    {
        [SerializeField] ConversationSegment segment;
        [SerializeField] bool progress = true;
        readonly int progressID = Animator.StringToHash("Progress");
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CutsceneManager.Instance.OpenConversation(segment, () => { if (progress) animator.SetTrigger(progressID); });
        }
    }
    [System.Serializable]
    public class ConversationSegment
    {
        [SerializeField] string m_talker;
        [SerializeField][TextArea] string m_text;
        [SerializeField] Sprite m_talkerImage;
        [SerializeField] TalkerDirection m_talkerDirection;
        [SerializeField] Sound m_talkSound;
        [SerializeField] bool m_skippable = true;
        public string talker => m_talker;
        public string text => m_text;
        public Sprite talkerImage => m_talkerImage;
        public TalkerDirection talkerDirection => m_talkerDirection;
        public Sound talkSound => m_talkSound;
        public bool skippable => m_skippable;
    }
    [System.Serializable]
    public enum TalkerDirection
    {
        Left,
        Right
    }
}