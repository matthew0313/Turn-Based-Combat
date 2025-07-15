using System;
using System.Collections.Generic;
using System.Linq;
using AudioManagement;
using DG.Tweening;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Summon Ally", menuName = "Scriptables/PlayerBattleActions/SummonAlly")]
    public class Player_BA_SummonAlly : PlayerBattleAction
    {
        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] UnitTime summonTime;
        [SerializeField] UnitTime summonEndTime;
        public override TargetSelection targetSelection => TargetSelection.Self;
        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override IEnumerator<float> Act(BE_Player actor)
        {
            actor.anim.SetTrigger(triggerID);
            BattleManager.Instance.UI.ShowText("플레이어가 랜덤한 동료를 소환합니다.");
            yield return Timing.WaitForSeconds(summonTime);
            BattleEntity selected = actor.alliesAvailable.SelectRandom(item => true);
            BattleManager.Instance.AddAlly(Alliance.Player, selected);
            actor.alliesAvailable.Remove(selected);
            yield return Timing.WaitForSeconds(summonEndTime - summonTime);
        }
        public override bool CheckRequirements(BE_Player actor, out string falseReason)
        {
            if(actor.alliesAvailable.Count <= 0)
            {
                falseReason = "소환할 수 있는 동료가 없습니다.";
                return false;
            }
            if(BattleManager.Instance.GetAllies(Alliance.Player).Count() >= BattleManager.maxPlayerSide)
            {
                falseReason = "소환할 공간이 없습니다.";
                return false;
            }
            falseReason = null; return true;
        }
    }
}