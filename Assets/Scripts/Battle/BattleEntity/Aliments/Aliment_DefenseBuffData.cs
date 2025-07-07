using System;
using System.Collections.Generic;
using System.Linq;
using AudioManagement;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseBuff", menuName = "Scriptables/Aliments/DefenseBuff")]
public class Aliment_DefenseBuffData : AlimentData
{
    [SerializeField] PooledEffect m_applyParticle;
    [SerializeField] Sound m_applySound;
    public PooledEffect applyParticle => m_applyParticle;
    public Sound applySound => m_applySound;
}
public class Aliment_DefenseBuff : Aliment, IAlimentTextDisplayed
{
    new public readonly Aliment_DefenseBuffData data;
    public readonly float defenseMultiplierAdd;
    public Aliment_DefenseBuff(float defenseMultiplierAdd, int durationLeft) : base(BattleManager.Instance.defenseBuffData, durationLeft) 
    {
        this.data = BattleManager.Instance.defenseBuffData;
        this.defenseMultiplierAdd = defenseMultiplierAdd;
    }

    public string alimentText => $"+{defenseMultiplierAdd * 100.0f}%";

    public event Action onAlimentTextChange;

    public override void OnAlimentAdd(BattleEntity entity)
    {
        base.OnAlimentAdd(entity);
        entity.defenseMultiplier += defenseMultiplierAdd;
        data.applyParticle.Get(entity.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(data.applySound);
    }
    public override void OnAlimentRemove(BattleEntity entity)
    {
        base.OnAlimentRemove(entity);
        entity.defenseMultiplier -= defenseMultiplierAdd;
    }
}