using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AnimatorCutscene
{
    public class TimelinePlayBehaviour : StateMachineBehaviour
    {
        [SerializeField] TimelineAsset timeline;
        [SerializeField] DirectorWrapMode wrapMode = DirectorWrapMode.None;
        [SerializeField] bool progressOnFinish = false;
        readonly int progressID = Animator.StringToHash("Progress");
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CutsceneManager.Instance.PlayTimeline(timeline, wrapMode, () =>
            {
                if (progressOnFinish) animator.SetTrigger(progressID);
            });
        }
    }
}