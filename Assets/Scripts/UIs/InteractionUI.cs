using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Player_Field;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    Player player;

    [SerializeField] CanvasGroup group;
    [SerializeField] Image back;
    [SerializeField] float tweenDuration = 0.5f;
    [SerializeField] float targetSizeX = 200.0f;

    [SerializeField] TMP_Text interactText;
    [SerializeField] float typewriteInterval = 0.1f;
    private void OnEnable()
    {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.interactions.onInteractionSelect += OnInteractionSelect;
    }
    private void OnDisable()
    {
        player.interactions.onInteractionSelect -= OnInteractionSelect;
    }
    Tween tween;
    CoroutineHandle typeWriting;
    float counter = 0.0f;
    void OnInteractionSelect(Interaction interaction)
    {
        Timing.KillCoroutines(typeWriting);

        if (tween == null) tween = DOTween.To(() => counter, value => counter = value, 1.0f, tweenDuration).OnUpdate(() =>
        {
            group.alpha = counter;
            back.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSizeX * counter);
        }).SetAutoKill(false).SetEase(Ease.OutCirc);

        if (interaction != null)
        {
            transform.position = interaction.interactPosition;
            tween.Rewind();
            tween.PlayForward();
            typeWriting = Timing.RunCoroutine(CoroutineUtility.Typewrite(interaction.interactText, typewriteInterval, typewriteInterval, value => interactText.text = value));
        }
        else
        {
            tween.PlayBackwards();
        }
    }
}