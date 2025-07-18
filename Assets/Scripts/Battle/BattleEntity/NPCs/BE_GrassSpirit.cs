using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

public class BE_GrassSpirit : NPCBattleEntity
{
    [SerializeField] BA_GrassBlast grassBlast;
    public override IEnumerator<float> OnEnemyTurn()
    {
        BattleManager.Instance.UI.ShowText($"{entityName}이 {grassBlast.actionName}을 사용합니다.");
        return grassBlast.Act(Alliance.Enemy);
    }
}