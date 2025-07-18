using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "BA Fire Blast", menuName = "Scriptables/BattleActions/FireBlast")]
public class BA_FireBlast : BattleAction
{
    [SerializeField] float burnDamage = 10.0f;
    [SerializeField] UnitTime endTime;
    public override TargetSelection targetSelection => TargetSelection.Enemy;
    public override IEnumerator<float> Act(Alliance alliance)
    {
        var fireSpirit = BattleManager.Instance.GetAllies(alliance).Select(item => item is BE_FireSpirit);
        var caster = fireSpirit != null ? fireSpirit : BattleManager.Instance.GetAllies(alliance).First();
        var target = alliance == Alliance.Player ? this.target : BattleManager.Instance.GetEnemies(alliance).SelectRandom();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(caster.CastAction(FireBlast(caster, target))));
    }
    IEnumerator<float> FireBlast(BattleEntity caster, BattleEntity target)
    {
        target.AddAliment(new Aliment_Burn(burnDamage + (caster is BE_FireSpirit ? 5 : 0), 99999));
        yield return Timing.WaitForSeconds(endTime);
    }
}