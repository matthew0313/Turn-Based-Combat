using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Text;
using AudioManagement;
using static UnityEngine.Rendering.DebugUI;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnimatorCutscene
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager Instance { get; private set; }
        public CutsceneManager() => Instance = this;
        PlayableDirector director;

        [SerializeField] CutscenesUI UI;

        [Header("FastPlay")]
        [SerializeField] float fastPlaySpeed = 10.0f;

        [Header("Talk")]
        [SerializeField] float talkInterval = 0.05f;
        [SerializeField] float talkLongInterval = 0.25f;

        Cutscene playing;
        List<ICutsceneCallbackReceiver> callbackReceivers;
        private void Awake()
        {
            director = GetComponent<PlayableDirector>();

            callbackReceivers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>().ToList();
            callbackReceivers.Sort((a, b) => a.executionOrder.CompareTo(b.executionOrder));
        }
        bool GetConversationPassInput()
        {
            return InputManager.Instance.PassDialogue.triggered;
        }
        private void Update()
        {
            if(playing != null)
            {
                if (InputManager.Instance.CutsceneFastPlay.triggered) SetFastPlay(!isFastPlaying);
            }
        }
        void PlaySound(Sound sound)
        {
            AudioManager.Instance.PlaySound(sound);
        }
        public void StartCutscene(Cutscene cutscene)
        {
            if (playing != null) return;
            playing = cutscene;
            foreach (var i in callbackReceivers)
            {
                i.OnCutsceneStart();
            }

            UI.ShowEdgeUI();
            playing.OnPlay();
        }
        public void FinishCutscene()
        {
            if (playing == null) return;

            playing.OnEnd();
            playing = null;

            Timing.KillCoroutines(playingTimeline);
            director.Stop();

            foreach (var i in callbackReceivers)
            {
                i.OnCutsceneEnd();
            }

            if (conversationOpen) CloseConversation();
            isFastPlaying = false;
            UI.HideEdgeUI();
            UI.SetFastPlay(false);
        }
        CoroutineHandle playingTimeline;
        public void PlayTimeline(TimelineAsset timeline, DirectorWrapMode wrapMode, Action onFinish = null)
        {
            Timing.KillCoroutines(playingTimeline);

            director.Stop();
            director.extrapolationMode = wrapMode;

            playingTimeline = Timing.RunCoroutine(PlayingTimeline(timeline, onFinish));
        }
        public bool isFastPlaying { get; private set; } = false;
        public void SetFastPlay(bool fastPlay)
        {
            if (isFastPlaying == fastPlay) return;

            isFastPlaying = fastPlay;
            if (director.playableGraph.IsValid() && director.playableGraph.GetRootPlayable(0).IsValid())
            {
                director.playableGraph.GetRootPlayable(0).SetSpeed(isFastPlaying ? 10.0f : 1.0f);
            }

            UI.SetFastPlay(isFastPlaying);
        }
        IEnumerator<float> PlayingTimeline(TimelineAsset timeline, Action onFinish)
        {
            director.Play(timeline);
            director.playableGraph.GetRootPlayable(0).SetSpeed(isFastPlaying ? 10.0f : 1.0f);
            while (director.time < director.playableAsset.duration - 0.1f) yield return Timing.WaitForOneFrame;
            onFinish?.Invoke();
        }

        CoroutineHandle conversation;
        CoroutineHandle conversationTyping;
        bool conversationOpen = false;
        public void OpenConversation(ConversationSegment segment, Action onProgress, bool autoPass = false)
        {
            if (!conversationOpen)
            {
                conversationOpen = true;
                UI.ShowConversationUI();
            }
            Timing.KillCoroutines(conversation);
            conversation = Timing.RunCoroutine(Conversation(segment, onProgress, autoPass));
        }
        IEnumerator<float> Conversation(ConversationSegment segment, Action onProgress, bool autoPass)
        {
            UI.SetTalker(segment);
            UI.SetTalkContentText(string.Empty);

            bool typing = true;
            conversationTyping = Timing.RunCoroutine(CoroutineUtility.Typewrite(segment.text,
                talkInterval, talkLongInterval,
                (content) =>
                {
                    UI.SetTalkContentText(content);
                    PlaySound(segment.talkSound);
                },
                () => typing = false));

            while (typing)
            {
                yield return Timing.WaitForOneFrame;
                if ((GetConversationPassInput() || isFastPlaying) && segment.skippable)
                {
                    yield return Timing.WaitForOneFrame;
                    UI.SetTalkContentText(segment.text);
                    break;
                }
            }
            Timing.KillCoroutines(conversationTyping);
            yield return Timing.WaitForOneFrame;
            if (!autoPass) while (!GetConversationPassInput() && !isFastPlaying) yield return Timing.WaitForOneFrame;
            onProgress?.Invoke();
        }
        CoroutineHandle delaying;
        public void Delay(float delay, Action onFinish)
        {
            Timing.KillCoroutines(delaying);
            delaying = Timing.RunCoroutine(Delaying(delay, onFinish));
        }
        IEnumerator<float> Delaying(float delay, Action onFinish)
        {
            float counter = 0.0f;
            while (counter < delay)
            {
                yield return Timing.WaitForOneFrame;
                counter += Time.deltaTime * (isFastPlaying ? 10.0f : 1.0f);
            }
            onFinish?.Invoke();
        }
        public void CloseConversation()
        {
            if (!conversationOpen) return;
            conversationOpen = false;
            UI.HideConversationUI();
        }
        public void OpenConversationChoice(ConversationChoiceSegment segment, Action<int> onSelect)
        {
            if (!conversationOpen)
            {
                onSelect?.Invoke(0);
                return;
            }

            UI.ShowChoice(segment, onSelect);
        }
        private void OnDestroy()
        {
            Timing.KillCoroutines(playingTimeline);
            Timing.KillCoroutines(conversation);
            Timing.KillCoroutines(conversationTyping);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(CutsceneManager))]
    public class CutsceneManager_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("End Cutscene")) (target as CutsceneManager).FinishCutscene();
        }
    }
#endif
}