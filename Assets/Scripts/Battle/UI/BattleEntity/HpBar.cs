using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;

public class HpBar : AnimatedBarUI
{
    [SerializeField] BattleEntity target;
    private void OnEnable()
    {
        target.onHpChange += OnHpChange;
        OnHpChange();
    }
    private void OnDisable()
    {
        target.onHpChange -= OnHpChange;
    }
    void OnHpChange()
    {
        targetScale = target.hp / target.maxHp;
    }
}
