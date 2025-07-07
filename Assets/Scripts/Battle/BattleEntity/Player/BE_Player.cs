using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

namespace Player_Battle
{
    public class BE_Player : BattleEntity
    {
        [Header("Ally Summon")]
        [SerializeField] UnitTime summonTime;
        [SerializeField] UnitTime summonEndTime;

        [Header("Mode Change")]
        [SerializeField] UnitTime modeChangeTime;
        [SerializeField] UnitTime modeChangeEndTime;

        [Header("Debug")]
        [SerializeField] PlayerBattleAction nextAction;
        [SerializeField] BattleEntity nextActionTarget;
        public override float maxHp => GlobalManager.Instance.playerStats.maxHp;
        public override float hp
        {
            get => GlobalManager.Instance.playerStats.hp;
            protected set => GlobalManager.Instance.playerStats.hp = value;
        }
        public PlayerElementMode elementMode { get; private set; } = PlayerElementMode.None;

        readonly int summonID = Animator.StringToHash("Summon");
        public override IEnumerator<float> OnBattleStart()
        {
            if (GlobalManager.Instance.playerStats.unlockedAllies.Count <= 0 || BattleManager.Instance.GetAllies(Alliance.Player).Count() >= BattleManager.maxParticipants) yield break;

            anim.SetTrigger(summonID);
            BattleManager.Instance.UI.ShowText("플레이어가 랜덤한 동료를 소환합니다.");
            yield return Timing.WaitForSeconds(summonTime);
            BattleManager.Instance.AddAlly(Alliance.Player, GlobalManager.Instance.playerStats.unlockedAllies.SelectRandom(item => true));
            yield return Timing.WaitForSeconds(summonEndTime - summonTime);
        }
        protected override IEnumerator<float> OnTurn()
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(base.OnTurn()));
            if (dead) yield break;

            bool finished = false;
            PlayerBattleAction nextAction = null;
            BattleManager.Instance.SelectPlayerAction(this, selected =>
            {
                nextAction = selected;
                finished = true;
            });
            while (!finished) yield return Timing.WaitForOneFrame;
            if(nextAction != null)
            {
                BattleManager.Instance.UI.ShowText($"플레이어가 {nextAction.actionName}를 사용합니다.");
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(nextAction.Act(this)));
            }
        }
        readonly int changeModeID = Animator.StringToHash("ChangeMode");
        readonly int modeID = Animator.StringToHash("Mode");
        internal IEnumerator<float> ChangeMode(PlayerElementMode mode)
        {
            if (elementMode == mode) yield break;

            anim.SetTrigger(changeModeID);
            yield return Timing.WaitForSeconds(modeChangeTime);
            elementMode = mode;
            anim.SetFloat(modeID, (float)mode);
            yield return Timing.WaitForSeconds(modeChangeEndTime - modeChangeTime);
        }
        readonly Dictionary<Element, Affinity> getBaseAffinitiesCache = new();
        protected override Dictionary<Element, Affinity> GetBaseAffinities()
        {
            getBaseAffinitiesCache.Clear();
            switch (elementMode)
            {
                case PlayerElementMode.Fire:
                    getBaseAffinitiesCache[Element.Fire] = Affinity.Resist;
                    getBaseAffinitiesCache[Element.Water] = Affinity.Weakness; break;
                case PlayerElementMode.Water:
                    getBaseAffinitiesCache[Element.Water] = Affinity.Resist;
                    getBaseAffinitiesCache[Element.Grass] = Affinity.Weakness; break;
                case PlayerElementMode.Grass:
                    getBaseAffinitiesCache[Element.Grass] = Affinity.Resist;
                    getBaseAffinitiesCache[Element.Fire] = Affinity.Weakness; break;
            }
            return getBaseAffinitiesCache;
        }
    }
    [System.Serializable]
    public enum PlayerElementMode
    {
        None,
        Fire,
        Water,
        Grass
    }

}