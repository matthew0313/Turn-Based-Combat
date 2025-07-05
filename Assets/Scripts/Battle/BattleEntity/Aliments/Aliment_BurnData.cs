using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Burn", menuName = "Scriptables/Aliments/Burn")]
public class Aliment_BurnData : AlimentData
{

}
public class Aliment_Burn : Aliment, IAlimentTextDisplayed
{
    new public readonly Aliment_BurnData data;
    readonly float damage;
    public Aliment_Burn(float damage, int durationLeft) : base(BattleManager.Instance.burnData, durationLeft) 
    {
        this.data = BattleManager.Instance.burnData;
        this.damage = damage;
    }

    public string alimentText => $"{damage}";

    public event Action onAlimentTextChange;

    public override IEnumerator<float> OnAlimentTurn(BattleEntity entity)
    {
        BattleManager.Instance.UI.ShowText($"{entity.entityName}가 불탑니다.");
        entity.GetDamage(new() { element = Element.Fire, amount = damage });
        yield return Timing.WaitForSeconds(1.0f);

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(base.OnAlimentTurn(entity)));
    }
}