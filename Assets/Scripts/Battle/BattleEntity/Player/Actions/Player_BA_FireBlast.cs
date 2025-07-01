using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    public class Player_BA_FireBlast : PlayerBattleAction
    {
        [Header("Fire Blast")]
        [SerializeField] string triggerName;
        [SerializeField] UnitTime projectileStartTime, projectileFlyTime, endDelayTime;
        public override TargetSelection targetSelection => TargetSelection.SingleEnemy;


        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override IEnumerator<float> Act(Action onFinish, Player_BattleEntity actor)
        {
            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(projectileStartTime);
            onFinish?.Invoke();
        }
    }
}