using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Timeline;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Guard Down", menuName = "Scriptables/PlayerBattleActions/GuardDown")]
    public class Player_BA_GuardDown : PlayerBattleAction
    {
        [Header("Stats")]
        [SerializeField] float defenseMultiplierSubtract = 0.2f;
        [SerializeField] int defenseDebuffTurns = 3;

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] UnitTime debuffApplyTime, endTime;
        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override TargetSelection targetSelection => TargetSelection.AllEnemy;
        public override IEnumerator<float> Act(BE_Player actor)
        {
            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(debuffApplyTime);
            foreach (var i in BattleManager.Instance.GetEnemies(Alliance.Player))
                if (!i.dead) i.ApplyAliment(new Aliment_DefenseDebuff(defenseMultiplierSubtract, defenseDebuffTurns));
            yield return Timing.WaitForSeconds(endTime - debuffApplyTime);
        }
    }
}