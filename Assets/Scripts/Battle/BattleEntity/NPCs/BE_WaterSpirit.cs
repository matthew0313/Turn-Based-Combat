using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

public class BE_WaterSpirit : NPCBattleEntity
{
    [SerializeField] BA_WaterBlast waterBlast;
    public override IEnumerator<float> OnEnemyTurn()
    {
        BattleManager.Instance.UI.ShowText($"물 정령이 {waterBlast.actionName}을 사용합니다.");
        return waterBlast.Act(Alliance.Enemy);
    }
}