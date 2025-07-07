using System;
using System.Collections.Generic;
using System.Linq;
using AudioManagement;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackBuff", menuName = "Scriptables/Aliments/AttackBuff")]
public class Aliment_AttackBuffData : AlimentData
{
    [SerializeField] PooledEffect m_applyParticle;
    [SerializeField] Sound m_applySound;
    public PooledEffect applyParticle => m_applyParticle;
    public Sound applySound => m_applySound;
}
public class Aliment_AttackBuff : Aliment, IAlimentTextDisplayed
{
    new public readonly Aliment_AttackBuffData data;
    public readonly float attackMultiplierAdd;
    public Aliment_AttackBuff(float attackMultiplierAdd, int durationLeft) : base(BattleManager.Instance.attackBuffData, durationLeft) 
    {
        this.data = BattleManager.Instance.attackBuffData;
        this.attackMultiplierAdd = attackMultiplierAdd;
    }

    public string alimentText => $"+{attackMultiplierAdd * 100.0f}%";

    public event Action onAlimentTextChange;

    public override void OnAlimentAdd(BattleEntity entity)
    {
        base.OnAlimentAdd(entity);
        entity.attackMultiplier += attackMultiplierAdd;
        data.applyParticle.Get(entity.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(data.applySound);
    }
    public override void OnAlimentRemove(BattleEntity entity)
    {
        base.OnAlimentRemove(entity);
        entity.attackMultiplier -= attackMultiplierAdd;
    }
}