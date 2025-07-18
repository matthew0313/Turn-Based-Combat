using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

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

    //modified stats
    public float attackMultiplier = 1.0f;
    public float defenseMultiplier = 1.0f;
    public bool isStunned = false;
    public virtual void Set(int battlePosition, Alliance alliance)
    {
        this.battlePosition = battlePosition;
        this.alliance = alliance;
    }
    public virtual IEnumerator<float> OnBattleStart() { yield break; }

    bool isMyTurn = false;
    CoroutineHandle currentTurn;
    public IEnumerator<float> Turn()
    {
        if (isMyTurn) yield break;

        isMyTurn = true;
        currentTurn = Timing.RunCoroutine(OnTurn());
        yield return Timing.WaitUntilDone(currentTurn);
        FinishTurn();
    }
    protected virtual IEnumerator<float> OnTurn()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(IterateAliments()));
        yield break;
    }
    protected void FinishTurn()
    {
        if (!isMyTurn) return;
        isMyTurn = false;
        Timing.KillCoroutines(currentTurn);
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
    public event Action<HealReceivedInfo> onHeal;
    public event Action onHpChange;

    protected void SetHP(float hp)
    {
        this.hp = hp;
        onHpChange?.Invoke();
    }
    public void GetDamage(DamageInfo damageInfo)
    {
        if (dead) return;
        DamageReceivedInfo receivedInfo = ProcessDamage(damageInfo);
        onDamage?.Invoke(receivedInfo);
        BattleManager.Instance.UI.IndicateDamage(receivedInfo);
    }

    const float weaknessMultiplier = 1.5f, resistMultiplier = 0.5f;
    protected virtual DamageReceivedInfo ProcessDamage(DamageInfo damageInfo)
    {
        DamageReceivedInfo tmp = new() { receiver = this };

        damageInfo.amount /= defenseMultiplier;
        if (GetAffinities().TryGetValue(damageInfo.element, out Affinity affinity))
        {
            switch (affinity)
            {
                case Affinity.Weakness:
                    damageInfo.amount *= weaknessMultiplier;
                    tmp.isWeaknessHit = true; break;
                case Affinity.Resist:
                    damageInfo.amount *= resistMultiplier;
                    tmp.isResistHit = true; break;
                case Affinity.Block:
                    tmp.isBlocked = true;
                    tmp.damageInfo = damageInfo;
                    return tmp;
                case Affinity.Reflect:
                    if (damageInfo.attacker != null || damageInfo.attacker == this)
                    {
                        tmp.isReflected = true;
                        damageInfo.attacker.GetDamage(damageInfo);
                    }
                    else tmp.isBlocked = true;
                    tmp.damageInfo = damageInfo;
                    return tmp;
                case Affinity.Absorb:
                    tmp.isAbsorbed = true;
                    GetHeal(new() { healer = damageInfo.attacker, amount = damageInfo.amount });
                    tmp.damageInfo = damageInfo;
                    return tmp;
            }
        }

        SetHP(Mathf.Max(hp - damageInfo.amount, 0));
        if (hp <= 0)
        {
            tmp.isFatal = true;
            OnDeath();
        }
        else if (damageInfo.aliment != null) ApplyAliment(damageInfo.aliment);
        tmp.damageInfo = damageInfo;
        return tmp;
    }
    public void GetHeal(HealInfo healInfo)
    {
        if (dead) return;
        HealReceivedInfo receivedInfo = ProcessHeal(healInfo);
        onHeal?.Invoke(receivedInfo);
        BattleManager.Instance.UI.IndicateHeal(receivedInfo);
    }
    protected virtual HealReceivedInfo ProcessHeal(HealInfo healInfo)
    {
        HealReceivedInfo tmp = new() { receiver = this };
        SetHP(Mathf.Min(hp + healInfo.amount, maxHp));
        tmp.healInfo = healInfo;
        return tmp;
    }
    readonly int deathID = Animator.StringToHash("Death");
    protected virtual void OnDeath()
    {
        dead = true;
        anim.SetTrigger(deathID);
        Aliment[] aliments = new Aliment[this.aliments.Count];
        this.aliments.CopyTo(aliments);
        foreach (var i in aliments) RemoveAliment(i);
    }

    readonly List<Aliment> aliments = new();
    public event Action onAlimentChange;
    IEnumerator<float> IterateAliments()
    {
        Aliment[] aliments = new Aliment[this.aliments.Count];
        this.aliments.CopyTo(aliments);
        foreach (var i in aliments)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(i.OnAlimentTurn(this)));
            if (dead) yield break;
        }
    }
    public IEnumerable<Aliment> GetAliments() => aliments;
    public IEnumerable<Aliment> GetAliments(Func<Aliment, bool> predicate)
    {
        foreach(var i in GetAliments()) if(predicate(i)) yield return i;
    }
    public IEnumerable<T> GetAliments<T>() where T : Aliment
    {
        foreach (var i in GetAliments(aliment => aliment is T)) yield return i as T;
    }
    public void ApplyAliment(Aliment aliment)
    {
        aliment.ApplyTo(this);
    }
    public void AddAliment(Aliment aliment)
    {
        aliments.Add(aliment);
        aliment.OnAlimentAdd(this);
        onAlimentChange?.Invoke();
    }
    public void RemoveAliment(Aliment aliment)
    {
        aliments.Remove(aliment);
        aliment.OnAlimentRemove(this);
        onAlimentChange?.Invoke();
    }
}
[System.Serializable]
public enum Element
{
    Water = 0,
    Fire = 1,
    Grass = 2,
    Earth = 3,
    Electricity = 4,
}
[System.Serializable]
public struct DamageInfo
{
    [HideInInspector] public BattleEntity attacker;
    public Element element;
    public float amount;
    public Aliment aliment;
}
[System.Serializable]
public struct DamageReceivedInfo
{
    public BattleEntity receiver;
    public DamageInfo damageInfo;

    public bool isNullified;
    public bool isWeaknessHit, isResistHit, isBlocked, isReflected, isAbsorbed;
    public bool isFatal;
}
[System.Serializable]
public struct HealInfo
{
    [HideInInspector] public BattleEntity healer;
    public float amount;
}
[System.Serializable]
public struct HealReceivedInfo
{
    public BattleEntity receiver;
    public HealInfo healInfo;

    public bool isNullified;
}
[System.Serializable]
public enum Affinity
{
    None = 0,
    Weakness = 1,
    Resist = 2,
    Block = 3,
    Reflect = 4,
    Absorb = 5
}
[System.Serializable]
public class ElementAffinityPair
{
    public Element element;
    public Affinity affinity;
}
