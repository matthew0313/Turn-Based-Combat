using System;
using System.Collections.Generic;
using System.Linq;
using AudioManagement;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDebuff", menuName = "Scriptables/Aliments/AttackDebuff")]
public class Aliment_AttackDebuffData : AlimentData
{
    [SerializeField] PooledEffect m_applyParticle;
    [SerializeField] Sound m_applySound;
    public PooledEffect applyParticle => m_applyParticle;
    public Sound applySound => m_applySound;
}
public class Aliment_AttackDebuff : Aliment, IAlimentTextDisplayed
{
    new public readonly Aliment_AttackDebuffData data;
    readonly float attackMultiplierSubtract;
    public Aliment_AttackDebuff(float attackMultiplierSubtract, int durationLeft) : base(BattleManager.Instance.attackDebuffData, durationLeft) 
    {
        this.data = BattleManager.Instance.attackDebuffData;
        this.attackMultiplierSubtract = attackMultiplierSubtract;
    }

    public string alimentText => $"-{attackMultiplierSubtract * 100.0f}%";

    public event Action onAlimentTextChange;

    public override void OnAlimentAdd(BattleEntity entity)
    {
        base.OnAlimentAdd(entity);
        entity.attackMultiplier -= attackMultiplierSubtract;
        data.applyParticle.Get(entity.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(data.applySound);
    }
    public override void OnAlimentRemove(BattleEntity entity)
    {
        base.OnAlimentRemove(entity);
        entity.attackMultiplier += attackMultiplierSubtract;
    }
}