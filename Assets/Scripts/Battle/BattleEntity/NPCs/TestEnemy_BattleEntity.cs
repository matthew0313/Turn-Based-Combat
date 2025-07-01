using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class TestEnemy_BattleEntity : NPCBattleEntity
{
    [Header("Attack")]
    [SerializeField] DamageInfo attackDamage;
    [SerializeField] UnitTime attackTime;
    [SerializeField] UnitTime attackEndTime;

    [Header("Check")]
    [SerializeField] UnitTime checkTime;
    protected override IEnumerator<float> OnTurn(Action onFinish)
    {
        bool parentDone = false; Timing.RunCoroutine(base.OnTurn(() => parentDone = true));
        while (!parentDone) yield return Timing.WaitForOneFrame;

        bool finished = false;
        float rand = UnityEngine.Random.Range(0, 100.0f);
        if(rand <= 80.0f)
        {
            var target = BattleManager.Instance.GetEnemies(alliance).SelectRandom(entity => entity != null && entity.dead == false);
            if (target != null) Timing.RunCoroutine(Attack(() => finished = true, target));
            else Timing.RunCoroutine(Check(() => finished = true));
        }
        else
        {
            Timing.RunCoroutine(Check(() => finished = true));
        }
        while(!finished) yield return Timing.WaitForOneFrame;
        onFinish?.Invoke();
    }
    IEnumerator<float> Attack(Action onFinish, BattleEntity target)
    {
        anim.SetTrigger("Attack");
        BattleManager.Instance.UI.ShowText("테스트 적이 공격합니다.");
        yield return Timing.WaitForSeconds(attackTime);
        target.GetDamage(attackDamage);
        yield return Timing.WaitForSeconds(attackEndTime - attackTime);
        onFinish?.Invoke();
    }
    IEnumerator<float> Check(Action onFinish)
    {
        anim.SetTrigger("Check");
        BattleManager.Instance.UI.ShowText("테스트 적이 상황을 살핍니다.");
        yield return Timing.WaitForSeconds(checkTime);
        onFinish?.Invoke();
    }
}
