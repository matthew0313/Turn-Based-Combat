using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class BE_TestEnemy : NPCBattleEntity
{
    [Header("Attack")]
    [SerializeField] DamageInfo attackDamage;
    [SerializeField] UnitTime attackTime;
    [SerializeField] UnitTime attackEndTime;

    [Header("Check")]
    [SerializeField] UnitTime checkEndTime;
    protected override IEnumerator<float> OnTurn()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(base.OnTurn()));
        if (dead) yield break;

        float rand = UnityEngine.Random.Range(0, 100.0f);
        if(rand <= 80.0f)
        {
            var target = BattleManager.Instance.GetEnemies(alliance).SelectRandom(entity => entity != null && entity.dead == false);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(Attack(target)));
            yield break;
        }
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Check()));
    }
    readonly int attackID = Animator.StringToHash("Attack");
    readonly int checkID = Animator.StringToHash("Check");
    IEnumerator<float> Attack(BattleEntity target)
    {
        anim.SetTrigger(attackID);
        BattleManager.Instance.UI.ShowText($"{entityName}가 공격합니다.");
        yield return Timing.WaitForSeconds(attackTime);
        DamageInfo attackDamage = this.attackDamage;
        attackDamage.attacker = this;
        attackDamage.amount *= attackMultiplier;
        target.GetDamage(attackDamage);
        yield return Timing.WaitForSeconds(attackEndTime - attackTime);
    }
    IEnumerator<float> Check()
    {
        anim.SetTrigger(checkID);
        BattleManager.Instance.UI.ShowText($"{entityName}가 상황을 살핍니다.");
        yield return Timing.WaitForSeconds(checkEndTime);
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        BattleManager.Instance.RemoveEntity(this);
        Destroy(gameObject);
    }
}
