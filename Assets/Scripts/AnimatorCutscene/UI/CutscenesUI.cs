using AudioManagement;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimatorCutscene
{
    public class CutscenesUI : MonoBehaviour
    {
        [Header("Edge")]
        [SerializeField] float edgeMoveTime = 0.5f;
        [SerializeField] RectTransform upEdge, downEdge;

        [Header("Conversation")]
        [SerializeField] float conversationUIAppearTime = 0.25f;
        [SerializeField] float conversationUIAppearPivot = -0.35f;
        [SerializeField] RectTransform conversationUI;
        [SerializeField] CanvasGroup conversationUIGroup;
        [SerializeField] TMP_Text talkerText, talkContentText;
        [SerializeField] Image leftTalker, rightTalker;
        [SerializeField] Color talkingColor, notTalkingColor;

        [Header("Conversation Choice")]
        [SerializeField] Transform conversationChoiceAnchor;
        [SerializeField] ConversationChoiceUI conversationChoicePrefab;
        List<ConversationChoiceUI> conversationChoices = new();

        [Header("Fast Play")]
        [SerializeField] GameObject fastPlayEffect;
        [SerializeField] Image fastPlayEffectBottom, fastPlayEffectTop;
        [SerializeField] float scrollSpeed = 2.0f;
        readonly int scrollID = Shader.PropertyToID("_Scroll");
        float scroll = 0.0f;

        bool isFastPlaying = false;

        bool isChoosing = false;
        ConversationChoiceUI selected = null;
        int selectedIndex = 0;
        int choiceCount = 0;
        private void Update()
        {
            if (isFastPlaying)
            {
                scroll += Time.unscaledDeltaTime * scrollSpeed;
                fastPlayEffectTop.material.SetFloat(scrollID, scroll);
                fastPlayEffectBottom.material.SetFloat(scrollID, scroll);
            }
            if (isChoosing)
            {
                if (InputManager.Instance.SelectDown.triggered && selectedIndex < choiceCount - 1) SelectChoiceOption(selectedIndex + 1);
                if (InputManager.Instance.SelectUp.triggered && selectedIndex > 0) SelectChoiceOption(selectedIndex - 1);
                if (InputManager.Instance.Select.triggered && selected != null)
                {
                    AudioManager.Instance.PlaySound("Confirm");
                    selected.SelectChoice();
                }
            }
        }
        public void SelectChoiceOption(int index)
        {
            if (index < 0 || index >= choiceCount || selected == conversationChoices[index]) return;
            AudioManager.Instance.PlaySound("Switch");
            if (selected != null) selected.OnDeselect();
            selectedIndex = index;
            selected = conversationChoices[index];
            if(selected != null) selected.OnSelect();
        }
        void DeselectChoiceOption()
        {
            if(selected != null) selected.OnDeselect();
            selected = null;
        }

        Tween edgeUITween;
        float edgeUIAppear = 0.0f;
        internal void ShowEdgeUI()
        {
            if (edgeUITween == null) edgeUITween = DOTween.To(() => edgeUIAppear, value => edgeUIAppear = value, 1.0f, edgeMoveTime).OnUpdate(() =>
            {
                upEdge.pivot = new Vector2(upEdge.pivot.x, edgeUIAppear);
                downEdge.pivot = new Vector2(downEdge.pivot.x, 1.0f - edgeUIAppear);
            }).SetAutoKill(false);

            edgeUITween.SetEase(Ease.OutCirc).PlayForward();
        }
        internal void HideEdgeUI()
        {
            edgeUITween.SetEase(Ease.InCirc).PlayBackwards();
        }

        Tween conversationUITween;
        float conversationUIAppear = 0.0f;
        internal void ShowConversationUI()
        {
            if (conversationUITween == null) conversationUITween = DOTween.To(() => conversationUIAppear, value => conversationUIAppear = value, 1.0f, conversationUIAppearTime).OnUpdate(() =>
            {
                conversationUI.pivot = new Vector2(0.5f, Mathf.Lerp(1.0f, conversationUIAppearPivot, conversationUIAppear));
                conversationUIGroup.alpha = conversationUIAppear;
            }).SetEase(Ease.OutCirc).SetAutoKill(false);

            conversationUITween.SetEase(Ease.OutCirc).PlayForward();
        }
        internal void HideConversationUI()
        {
            conversationUITween.SetEase(Ease.InCirc).PlayBackwards();
            leftTalker.gameObject.SetActive(false);
            rightTalker.gameObject.SetActive(false);
        }
        internal void SetTalker(ConversationSegment segment)
        {
            talkerText.text = segment.talker;
            TalkerDirection direction = segment.talkerDirection;
            Sprite talkerImage = segment.talkerImage;
            if (direction == TalkerDirection.Left)
            {
                leftTalker.sprite = talkerImage;
                leftTalker.color = talkingColor;
                rightTalker.color = notTalkingColor;
                leftTalker.gameObject.SetActive(talkerImage != null);
            }
            if (direction == TalkerDirection.Right)
            {
                rightTalker.sprite = talkerImage;
                rightTalker.color = talkingColor;
                leftTalker.color = notTalkingColor;
                rightTalker.gameObject.SetActive(talkerImage != null);
            }
        }
        internal void SetTalkContentText(string talkContent)
        {
            talkContentText.text = talkContent;
        }
        internal void ShowChoice(ConversationChoiceSegment segment, Action<int> onSelect)
        {
            isChoosing = true;
            choiceCount = segment.options.Length;
            for (int i = 0; i < segment.options.Length; i++)
            {
                if (conversationChoices.Count <= i)
                {
                    var tmp = Instantiate(conversationChoicePrefab, conversationChoiceAnchor);
                    tmp.Set(this, i);
                    conversationChoices.Add(tmp);
                }

                conversationChoices[i].Show();
                conversationChoices[i].text.text = segment.options[i];

                conversationChoices[i].onChoiceSelect = index =>
                {
                    if (!isChoosing) return;
                    isChoosing = false;
                    onSelect?.Invoke(index);
                    HideChoice();
                };
            }
            SelectChoiceOption(0);
        }
        void HideChoice()
        {
            DeselectChoiceOption();
            for (int i = 0; i < conversationChoices.Count; i++)
            {
                if (conversationChoices[i].shown) conversationChoices[i].Hide();
            }
        }
        internal void SetFastPlay(bool fastPlay)
        {
            fastPlayEffect.gameObject.SetActive(fastPlay);
            isFastPlaying = fastPlay;
        }
    }
}