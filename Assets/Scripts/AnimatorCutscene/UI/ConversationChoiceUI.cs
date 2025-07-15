using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AnimatorCutscene
{
    public class ConversationChoiceUI : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] Button button;
        [SerializeField] CanvasGroup group;
        [SerializeField] TMP_Text m_text;
        [SerializeField] float showTime = 0.25f;
        [SerializeField] float selectTime = 0.2f;
        [SerializeField] float selectScale = 1.2f;
        public TMP_Text text => m_text;

        Tween tween;
        float tweenCounter = 0.0f;
        public bool shown { get; private set; } = false;

        CutscenesUI UI;
        int choiceIndex;
        public void Set(CutscenesUI UI, int choiceIndex)
        {
            this.UI = UI;
            this.choiceIndex = choiceIndex;
        }
        public void Show()
        {
            if (shown) return;
            shown = true;

            gameObject.SetActive(true);
            if (tween == null)
            {
                tween = DOTween.To(() => tweenCounter, value => tweenCounter = value, 1.0f, showTime).OnUpdate(() =>
                {
                    group.alpha = tweenCounter;
                }).SetAutoKill(false);
            }
            tween.OnStepComplete(null);
            tween.PlayForward();
        }
        public void Hide()
        {
            if (!shown) return;
            shown = false;

            if (tween == null) return;
            tween.OnStepComplete(() => { gameObject.SetActive(false); });
            tween.PlayBackwards();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            UI.SelectChoiceOption(choiceIndex);
        }
        Tween selectTween;
        public void OnSelect()
        {
            if (selectTween == null) selectTween = button.transform.DOScale(new Vector2(selectScale, selectScale), selectTime)
                    .SetAutoKill(false);

            selectTween.SetEase(Ease.OutCirc);
            selectTween.PlayForward();
        }
        public void OnDeselect()
        {
            selectTween.SetEase(Ease.InCirc);
            selectTween.PlayBackwards();
        }
        public Action<int> onChoiceSelect;
        private void OnEnable()
        {
            button.onClick.AddListener(SelectChoice);
        }
        private void OnDisable()
        {
            button.onClick.RemoveListener(SelectChoice);
        }
        public void SelectChoice()
        {
            onChoiceSelect?.Invoke(choiceIndex);
        }
    }
}
