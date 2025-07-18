using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Player_Battle;

public class ActionCard : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2 tweenPivot = new Vector2(0.5f, -0.5f);
    [SerializeField] float tweenTime = 0.25f;
    [SerializeField] int siblingIndex = 0;

    [Header("Action Desc")]
    [SerializeField] Image actionIcon;
    [SerializeField] TMP_Text actionText, actionDescText;
    Tween tween;

    readonly int elementID = Animator.StringToHash("Element");
    public void Set(PlayerBattleAction battleAction)
    {
        anim.SetInteger(elementID, (int)battleAction.abilityElement);
        actionIcon.sprite = battleAction.actionIcon;
        actionText.text = battleAction.actionName;
        actionDescText.text = battleAction.actionDesc;
    }
    public void OnSelect()
    {
        if (tween == null) tween = rectTransform.DOPivot(tweenPivot, tweenTime)
                .SetAutoKill(false);
        tween.PlayForward();
        transform.SetAsLastSibling();
    }
    public void OnDeselect()
    {
        tween.PlayBackwards();
        transform.SetSiblingIndex(siblingIndex);
    }
}