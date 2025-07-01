using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleText : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text text;
    readonly int showID = Animator.StringToHash("Show");
    public void ShowText(string text)
    {
        this.text.text = text;
        anim.SetTrigger(showID);
    }
}