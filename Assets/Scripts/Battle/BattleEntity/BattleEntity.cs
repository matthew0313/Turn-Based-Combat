using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    [SerializeField] Animator m_anim;
    [SerializeField] string m_entityName;
    [SerializeField] Transform m_rotator;
    public Animator anim => m_anim;
    public string entityName => m_entityName;
    public Transform rotator => m_rotator;
    public abstract float maxHp { get; }
    public abstract float hp { get; protected set; }
    public bool dead { get; protected set; } = false;
    public int battlePosition { get; private set; }
    public Alliance alliance { get; private set; }
    public virtual void Set(int battlePosition, Alliance alliance)
    {
        this.battlePosition = battlePosition;
        this.alliance = alliance;
    }
    public virtual void OnBattleStart(Action onFinish) => onFinish?.Invoke();

    bool isMyTurn = false;
    public void StartTurn()
    {
        isMyTurn = true;
        Timing.RunCoroutine(OnTurn(FinishTurn));
    }
    protected virtual IEnumerator<float> OnTurn(Action onFinish)
    {
        onFinish?.Invoke();
        yield break;
    }
    protected void FinishTurn()
    {
        if (!isMyTurn) return;
        isMyTurn = false;
        BattleManager.Instance.FinishTurn();
    }


    //Affinities
    protected abstract Dictionary<Element, Affinity> GetBaseAffinities();
    public readonly Dictionary<Element, Affinity> affinityOverrides = new();

    readonly Dictionary<Element, Affinity> getAffinitiesCache = new();
    Dictionary<Element, Affinity> GetAffinities()
    {
        getAffinitiesCache.Clear();
        foreach (var i in GetBaseAffinities()) getAffinitiesCache[i.Key] = i.Value;
        foreach (var i in affinityOverrides) getAffinitiesCache[i.Key] = i.Value;
        return getAffinitiesCache;
    }


    //Damage, Hp
    public event Action<DamageReceivedInfo> onDamage;
    public event Action onHpChange;

    protected void SetHP(float hp)
    {
        this.hp = hp;
        onHpChange?.Invoke();
    }
    public DamageReceivedInfo GetDamage(DamageInfo damageInfo)
    {
        DamageReceivedInfo receivedInfo = ProcessDamage(damageInfo);
        onDamage?.Invoke(receivedInfo);
        BattleManager.Instance.UI.IndicateDamage(receivedInfo);
        return receivedInfo;
    }

    const float weaknessMultiplier = 1.5f, resistMultiplier = 0.5f;
    protected virtual DamageReceivedInfo ProcessDamage(DamageInfo damageInfo)
    {
        DamageReceivedInfo tmp = new() { receiver = this, damageInfo = damageInfo };
        if (dead)
        {
            tmp.isNullified = true;
            return tmp;
        }

        if(GetAffinities().TryGetValue(damageInfo.element, out Affinity affinity))
        {
            switch (affinity)
            {
                case Affinity.Weakness:
                    damageInfo.amount *= weaknessMultiplier;
                    tmp.isWeaknessHit = true; break;
                case Affinity.Resist:
                    damageInfo.amount *= resistMultiplier;
                    tmp.isResistHit = true; break;
            }
        }
        
        SetHP(Mathf.Max(hp - damageInfo.amount, 0));
        if (hp <= 0)
        {
            tmp.isFatal = true;
            OnDeath();
        }
        return tmp;
    }
    readonly int deathID = Animator.StringToHash("Death");
    protected virtual void OnDeath()
    {
        dead = true;
        anim.SetTrigger(deathID);
    }
}
[System.Serializable]
public enum Element
{
    None = 0,
    Physical = 1,
    Fire = 2,
    Water = 3,
    Grass = 4
}
[System.Serializable]
public struct DamageInfo
{
    public Element element;
    public float amount;
}
[System.Serializable]
public struct DamageReceivedInfo
{
    public BattleEntity receiver;
    public DamageInfo damageInfo;

    public bool isNullified;
    public bool isCritical, isWeaknessHit, isResistHit, isBlockHit;
    public bool isFatal;
}
[System.Serializable]
public enum Affinity
{
    None = 0,
    Weakness = 1,
    Resist = 2,
    Block = 3
}
[System.Serializable]
public class ElementAffinityPair
{
    public Element element;
    public Affinity affinity;
}
