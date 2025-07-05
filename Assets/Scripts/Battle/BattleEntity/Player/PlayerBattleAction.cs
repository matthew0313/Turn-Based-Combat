using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    public abstract class PlayerBattleAction : ScriptableObject
    {
        [Header("Player Battle Action")]
        [SerializeField] Sprite m_actionIcon;
        [SerializeField] string m_actionName;
        [SerializeField][TextArea] string m_actionDesc;
        public Sprite actionIcon => m_actionIcon;
        public string actionName => m_actionName;
        public string actionDesc => m_actionDesc;
        public abstract TargetSelection targetSelection { get; }
        [HideInInspector] public BattleEntity target;
        public abstract IEnumerator<float> Act(BE_Player actor);
        public virtual bool CheckRequirements(BE_Player actor, out string falseReason)
        {
            falseReason = null;
            return true;
        }
    }
    [System.Serializable]
    public enum TargetSelection
    {
        Self,
        SingleEnemy,
        SingleAlly,
        SingleAllyExceptSelf,
        AllEnemy,
        AllAlly,
        AllAllyExceptSelf,
        All,
    }
}