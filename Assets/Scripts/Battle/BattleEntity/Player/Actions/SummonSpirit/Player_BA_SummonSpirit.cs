using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Timeline;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Summon Spirit", menuName = "Scriptables/PlayerBattleActions/SummonSpirit")]
    public class Player_BA_SummonSpirit : PlayerBattleAction
    {
        [Header("Stats")]
        [SerializeField] BE_FireSpirit fireSpirit;
        [SerializeField] BE_WaterSpirit waterSpirit;
        [SerializeField] BE_GrassSpirit grassSpirit;

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] UnitTime summonTime, endTime;
        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override TargetSelection targetSelection => TargetSelection.Self;
        public override IEnumerator<float> Act(BE_Player actor)
        {
            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(summonTime);
            switch (actor.elementMode)
            {
                case PlayerElementMode.Fire: BattleManager.Instance.AddAlly(Alliance.Player, fireSpirit); break;
                case PlayerElementMode.Water: BattleManager.Instance.AddAlly(Alliance.Player, waterSpirit); break;
                case PlayerElementMode.Grass: BattleManager.Instance.AddAlly(Alliance.Player, grassSpirit); break;
            }
            yield return Timing.WaitForSeconds(endTime - summonTime);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(actor.ChangeMode(PlayerElementMode.None)));
        }
        public override bool CheckRequirements(BE_Player actor, out string falseReason)
        {
            if(actor.elementMode == PlayerElementMode.None)
            {
                falseReason = "사용 조건을 충족하지 못했습니다.";
                return false;
            }
            if(BattleManager.Instance.GetAllies(Alliance.Player).Count() >= BattleManager.maxParticipants)
            {
                falseReason = "소환할 공간이 없습니다.";
                return false;
            }
            falseReason = null;
            return true;
        }
    }
}