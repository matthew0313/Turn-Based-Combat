using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimatorCutscene
{
    public class FastPlayToggleUI : MonoBehaviour
    {
        [SerializeField] Toggle toggle;
        private void OnEnable()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }
        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }
        private void Update()
        {
            toggle.isOn = CutsceneManager.Instance.isFastPlaying;
        }
        void OnValueChanged(bool value)
        {
            CutsceneManager.Instance.SetFastPlay(value);
        }
    }
}
