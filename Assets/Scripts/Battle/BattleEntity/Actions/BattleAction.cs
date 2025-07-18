using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public abstract class BattleAction : ScriptableObject
{
    [SerializeField] string m_actionName;
    [SerializeField] ActionCard m_card;
    public string actionName => m_actionName;
    public ActionCard card => m_card;
    public abstract TargetSelection targetSelection { get; }

    [HideInInspector] public BattleEntity target;
    public abstract IEnumerator<float> Act(Alliance alliance);
    public virtual bool CheckRequirements(out string falseReason)
    {
        falseReason = null;
        return true;
    }
}
[System.Serializable]
public enum TargetSelection
{
    Enemy,
    SingleAlly,
    AllAlly,
    All,
}