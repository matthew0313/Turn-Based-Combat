using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

public class BE_FireSpirit : NPCBattleEntity
{
    [SerializeField] BA_FireBlast fireBlast;
    public override IEnumerator<float> OnEnemyTurn()
    {
        BattleManager.Instance.UI.ShowText($"{entityName}이 {fireBlast.actionName}을 사용합니다.");
        return fireBlast.Act(Alliance.Enemy);
    }
}