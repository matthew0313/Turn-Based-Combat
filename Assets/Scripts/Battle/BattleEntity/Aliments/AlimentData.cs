using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public abstract class AlimentData : ScriptableObject
{
    [SerializeField] Sprite m_alimentIcon;
    public Sprite alimentIcon => m_alimentIcon;
}
public abstract class Aliment
{
    public readonly AlimentData data;
    int durationLeft = 0;
    public Aliment(AlimentData data, int durationLeft)
    {
        this.data = data;
        this.durationLeft = durationLeft;
    }
    public virtual void ApplyTo(BattleEntity entity)
    {
        entity.AddAliment(this);
    }
    public virtual void OnAlimentAdd(BattleEntity entity) { }
    public virtual IEnumerator<float> OnAlimentTurn(BattleEntity entity)
    {
        if(--durationLeft <= 0)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(OnDurationOver(entity)));
            entity.RemoveAliment(this);
        }
    }
    protected virtual IEnumerator<float> OnDurationOver(BattleEntity entity) { yield break; }
    public virtual void OnAlimentRemove(BattleEntity entity) { }
}