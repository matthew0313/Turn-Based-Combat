using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ActionCard : PooledPrefab<ActionCard>
{
    [SerializeField] Animator anim;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2 tweenPivot = new Vector2(0.5f, -0.5f);
    [SerializeField] float tweenTime = 0.25f;

    [Header("Action Desc")]
    [SerializeField] Image actionIcon;
    [SerializeField] TMP_Text actionText, actionDescText;
    Tween tween;
    public BattleAction action { get; private set; }
    public int index { get; private set; }
    public void Set(BattleAction action, int index)
    {
        this.action = action;
        this.index = index;
    }
    public void OnSelect()
    {
        if (tween == null) tween = rectTransform.DOPivot(tweenPivot, tweenTime)
                .SetAutoKill(false);
        tween.PlayForward();
        transform.parent.SetAsLastSibling();
    }
    public void OnDeselect()
    {
        tween.PlayBackwards();
        transform.parent.SetSiblingIndex(BattleManager.cardCount - index);
    }
}