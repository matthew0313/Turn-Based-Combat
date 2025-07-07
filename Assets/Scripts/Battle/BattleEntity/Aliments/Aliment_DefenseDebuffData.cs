using System;
using System.Collections.Generic;
using System.Linq;
using AudioManagement;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseDebuff", menuName = "Scriptables/Aliments/DefenseDebuff")]
public class Aliment_DefenseDebuffData : AlimentData
{
    [SerializeField] PooledEffect m_applyParticle;
    [SerializeField] Sound m_applySound;
    public PooledEffect applyParticle => m_applyParticle;
    public Sound applySound => m_applySound;
}
public class Aliment_DefenseDebuff : Aliment, IAlimentTextDisplayed
{
    new public readonly Aliment_DefenseDebuffData data;
    readonly float defenseMultiplierSubtract;
    public Aliment_DefenseDebuff(float defenseMultiplierSubtract, int durationLeft) : base(BattleManager.Instance.defenseDebuffData, durationLeft) 
    {
        this.data = BattleManager.Instance.defenseDebuffData;
        this.defenseMultiplierSubtract = defenseMultiplierSubtract;
    }

    public string alimentText => $"-{defenseMultiplierSubtract * 100.0f}%";

    public event Action onAlimentTextChange;

    public override void OnAlimentAdd(BattleEntity entity)
    {
        base.OnAlimentAdd(entity);
        entity.defenseMultiplier -= defenseMultiplierSubtract;
        data.applyParticle.Get(entity.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(data.applySound);
    }
    public override void OnAlimentRemove(BattleEntity entity)
    {
        base.OnAlimentRemove(entity);
        entity.defenseMultiplier += defenseMultiplierSubtract;
    }
}