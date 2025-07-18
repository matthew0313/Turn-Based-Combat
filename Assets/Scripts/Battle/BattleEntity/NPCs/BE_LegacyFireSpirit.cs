using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

public class BE_LegacyFireSpirit : NPCBattleEntity
{
    [Header("Attack")]
    [SerializeField] float attackDamage = 3;
    [SerializeField] float burnDamage = 3;
    [SerializeField] int burnTurns = 2;
    [SerializeField] UnitTime attackTime, attackEndTime;

    [Header("Bless")]
    [SerializeField] float attackMultiplierAdd = 0.1f;
    [SerializeField] int attackBuffTurns = 5;
    [SerializeField] UnitTime buffApplyTime, blessEndTime;
    protected override IEnumerator<float> OnTurn()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(base.OnTurn()));
        var allies = BattleManager.Instance.GetAllies(alliance).ToList();
        allies.RemoveAll(ally => ally.dead);
        allies.Sort((a, b) => a.attackMultiplier.CompareTo(b.attackMultiplier));
        if (allies[0].attackMultiplier < 1.1f)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(Bless(allies[0])));
            yield break;
        }
        var target = BattleManager.Instance.GetEnemies(alliance).SelectRandom(entity => entity != null && entity.dead == false);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Attack(target)));
    }
    readonly int attackID = Animator.StringToHash("Attack");
    readonly int blessID = Animator.StringToHash("Bless");
    IEnumerator<float> Attack(BattleEntity target)
    {
        anim.SetTrigger(attackID);
        BattleManager.Instance.UI.ShowText($"{entityName}가 공격합니다.");
        yield return Timing.WaitForSeconds(attackTime);
        target.GetDamage(new() { attacker = this, element = Element.Fire, amount = attackDamage * attackMultiplier,
        aliment = new Aliment_Burn(burnDamage, burnTurns)});
        yield return Timing.WaitForSeconds(attackEndTime - attackTime);
    }
    IEnumerator<float> Bless(BattleEntity target)
    {
        anim.SetTrigger(blessID);
        BattleManager.Instance.UI.ShowText($"{entityName}가 {target.entityName}를 축복합니다.");
        yield return Timing.WaitForSeconds(buffApplyTime);
        target.ApplyAliment(new Aliment_AttackBuff(attackMultiplierAdd, attackBuffTurns));
        yield return Timing.WaitForSeconds(blessEndTime - buffApplyTime);
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        BattleManager.Instance.RemoveEntity(this);
        Destroy(gameObject);
    }
}