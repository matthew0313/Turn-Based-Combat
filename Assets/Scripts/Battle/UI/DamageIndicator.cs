using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DamageIndicator : PooledPrefab<DamageIndicator>
{
    [SerializeField] CanvasGroup group;
    [SerializeField] Image elementIcon;
    [SerializeField] TMP_Text damageText;
    [SerializeField] GameObject weaknessHit, resistHit;

    [Header("Tween")]
    [SerializeField] float appearTweenDuration;
    [SerializeField] float disappearTweenDelay, disappearTweenDuration;
    Tween appearTween, disappearTween;
    public void Set(DamageReceivedInfo info)
    {
        elementIcon.sprite = BattleManager.Instance.UI.GetSprite(info.damageInfo.element);
        elementIcon.gameObject.SetActive(elementIcon.sprite != null);
        damageText.text = info.damageInfo.amount.ToString();
        weaknessHit.gameObject.SetActive(info.isWeaknessHit);
        resistHit.gameObject.SetActive(info.isResistHit);
        
        if (appearTween == null) appearTween = group.transform.DOScale(Vector2.one, appearTweenDuration).SetEase(Ease.OutElastic).SetAutoKill(false);
        appearTween.Restart();

        if (disappearTween == null) disappearTween = DOTween.To(() => group.alpha, value => group.alpha = value, 0.0f, disappearTweenDuration)
            .SetDelay(disappearTweenDelay)
            .SetAutoKill(false)
            .OnComplete(Release);
        disappearTween.Restart();
    }
}
