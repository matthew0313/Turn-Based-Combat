using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HealIndicator : PooledPrefab<HealIndicator>
{
    [SerializeField] CanvasGroup group;
    [SerializeField] TMP_Text healText;

    [Header("Tween")]
    [SerializeField] float appearTweenDuration;
    [SerializeField] float disappearTweenDelay, disappearTweenDuration;
    Tween appearTween, disappearTween;
    public void Set(HealReceivedInfo info)
    {
        healText.text = $"+{info.healInfo.amount.ToString()}";

        if (appearTween == null) appearTween = group.transform.DOScale(Vector2.one, appearTweenDuration).SetEase(Ease.OutElastic).SetAutoKill(false);
        appearTween.Restart();

        if (disappearTween == null) disappearTween = DOTween.To(() => group.alpha, value => group.alpha = value, 0.0f, disappearTweenDuration)
            .SetDelay(disappearTweenDelay)
            .SetAutoKill(false)
            .OnComplete(Release);
        disappearTween.Restart();
    }
}
