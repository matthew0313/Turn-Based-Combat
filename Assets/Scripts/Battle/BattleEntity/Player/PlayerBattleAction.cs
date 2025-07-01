using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    public abstract class PlayerBattleAction : ScriptableObject
    {
        [Header("Player Battle Action")]
        [SerializeField] string m_actionName;
        [SerializeField][TextArea] string m_actionDesc;
        public abstract TargetSelection targetSelection { get; }
        public BattleEntity target;
        public abstract IEnumerator<float> Act(Action onFinish, Player_BattleEntity actor);
    }
    [System.Serializable]
    public enum TargetSelection
    {
        None,
        SingleEnemy,
        SingleAlly,
        AllEnemy,
        AllAlly,
        All
    }
}