using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "BA Grass Blast", menuName = "Scriptables/BattleActions/GrassBlast")]
public class BA_GrassBlast : BattleAction
{
    [SerializeField] float damage = 30.0f, heal = 10.0f;
    [SerializeField] UnitTime endTime;
    public override TargetSelection targetSelection => TargetSelection.Enemy;
    public override IEnumerator<float> Act(Alliance alliance)
    {
        var grassSpirit = BattleManager.Instance.GetAllies(alliance).Select(item => item is BE_GrassSpirit);
        var caster = (grassSpirit != null && !grassSpirit.dead) ? grassSpirit : BattleManager.Instance.GetAllies(alliance).First();
        var target = alliance == Alliance.Player ? this.target : BattleManager.Instance.GetEnemies(alliance).SelectRandom((entity) => !entity.dead);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(caster.CastAction(GrassBlast(caster, target))));
    }
    IEnumerator<float> GrassBlast(BattleEntity caster, BattleEntity target)
    {
        target.GetDamage(new()
        {
            attacker = caster,
            element = Element.Grass,
            amount = damage
        });
        if (caster is BE_GrassSpirit) caster.GetHeal(new()
        {
            healer = caster,
            amount = heal
        });
        yield return Timing.WaitForSeconds(endTime);
    }
}