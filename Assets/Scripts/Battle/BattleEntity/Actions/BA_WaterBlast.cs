using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "BA Water Blast", menuName = "Scriptables/BattleActions/WaterBlast")]
public class BA_WaterBlast : BattleAction
{
    [SerializeField] float damage = 10.0f;
    [SerializeField] int hitCount = 3;
    [SerializeField] UnitTime hitInterval, endTime;
    public override TargetSelection targetSelection => TargetSelection.Enemy;
    public override IEnumerator<float> Act(Alliance alliance)
    {
        var waterSpirit = BattleManager.Instance.GetAllies(alliance).Select(item => item is BE_WaterSpirit);
        var caster = (waterSpirit != null && !waterSpirit.dead) ? waterSpirit : BattleManager.Instance.GetAllies(alliance).First();
        var target = alliance == Alliance.Player ? this.target : BattleManager.Instance.GetEnemies(alliance).SelectRandom((entity) => !entity.dead);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(caster.CastAction(WaterBlast(caster, target))));
    }
    IEnumerator<float> WaterBlast(BattleEntity caster, BattleEntity target)
    {
        Timing.RunCoroutine(Burst(caster, target));
        yield return Timing.WaitForSeconds(endTime);
    }
    IEnumerator<float> Burst(BattleEntity caster, BattleEntity target)
    {
        for(int i = 0; i < hitCount; i++)
        {
            target.GetDamage(new()
            {
                attacker = caster,
                amount = damage + (caster is BE_WaterSpirit ? 5 : 0),
                element = Element.Water
            });
            yield return Timing.WaitForSeconds(hitInterval);
        }
    }
}