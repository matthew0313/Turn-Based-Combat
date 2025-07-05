using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ActionCardSet : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2 showPos, hidePos;
    [SerializeField] float tweenTime = 0.35f;
    public void Show()
    {
        gameObject.SetActive(true);
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(showPos, tweenTime).SetEase(Ease.OutCirc);
    }
    public void Hide()
    {
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(hidePos, tweenTime).SetEase(Ease.OutCirc).OnComplete(() => gameObject.SetActive(false));
    }
}