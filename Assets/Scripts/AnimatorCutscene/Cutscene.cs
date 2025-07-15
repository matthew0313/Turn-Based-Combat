using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnimatorCutscene
{
    [RequireComponent(typeof(Animator))]
    public class Cutscene : MonoBehaviour
    {
        protected Animator anim;
        readonly int playID = Animator.StringToHash("Play");

        [SerializeField] CinemachineCamera[] cinemachineCameras;
        [SerializeField] UnityEvent[] events;
        [SerializeField] UnityEvent onCutsceneStart, onCutsceneFinish;
        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
        }
        public void AddCameraPriority(int cameraIndex, int priority)
        {
            if (cameraIndex < 0 || cameraIndex >= cinemachineCameras.Length) return;
            cinemachineCameras[cameraIndex].Priority += priority;
        }
        public void SetCameraActive(int cameraIndex, bool active)
        {
            if(cameraIndex < 0 || cameraIndex >= cinemachineCameras.Length) return;
            cinemachineCameras[cameraIndex].gameObject.SetActive(active);
        }
        public void InvokeEvent(int eventIndex)
        {
            if (eventIndex < 0 || eventIndex >= events.Length) return;
            events[eventIndex].Invoke();
        }
        public void Play()
        {
            CutsceneManager.Instance.StartCutscene(this);
        }
        public virtual void OnPlay()
        {
            onCutsceneStart?.Invoke();
            anim.SetTrigger(playID);
        }
        public virtual void OnEnd()
        {
            onCutsceneFinish?.Invoke();
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(Cutscene))]
    public class Cutscene_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Start Cutscene")) CutsceneManager.Instance.StartCutscene(target as Cutscene);
        }
    }
#endif
}
