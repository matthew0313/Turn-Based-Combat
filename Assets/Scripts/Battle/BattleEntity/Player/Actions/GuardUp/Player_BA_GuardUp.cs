using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Timeline;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Guard Up", menuName = "Scriptables/PlayerBattleActions/GuardUp")]
    public class Player_BA_GuardUp : PlayerBattleAction
    {
        [Header("Stats")]
        [SerializeField] float defenseMultiplierAdd = 0.2f;
        [SerializeField] int defenseBuffTurns = 3;

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] UnitTime buffApplyTime, endTime;
        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override Element abilityElement => Element.Fire;
        public override TargetSelection targetSelection => TargetSelection.AllAlly;
        public override IEnumerator<float> Act(BE_Player actor)
        {
            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(buffApplyTime);
            foreach (var i in BattleManager.Instance.GetAllies(Alliance.Player))
                if (!i.dead) i.ApplyAliment(new Aliment_DefenseBuff(defenseMultiplierAdd, defenseBuffTurns));
            yield return Timing.WaitForSeconds(endTime - buffApplyTime);
        }
    }
}