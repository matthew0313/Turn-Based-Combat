using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

namespace Player_Battle
{
    public class BE_Player : BattleEntity
    {
        [Header("Stats")]
        [SerializeField] float m_maxHp = 200.0f;

        [Header("Mode Change")]
        [SerializeField] UnitTime modeChangeTime;
        [SerializeField] UnitTime modeChangeEndTime;

        [Header("Debug")]
        [SerializeField] PlayerBattleAction nextAction;
        [SerializeField] BattleEntity nextActionTarget;
        public override float maxHp => m_maxHp;
        public override float hp { get; protected set; }
        public PlayerElementMode elementMode { get; private set; } = PlayerElementMode.None;
        public readonly List<BattleEntity> alliesAvailable = new();
        private void Awake()
        {
            hp = maxHp;
        }
        public override IEnumerator<float> OnBattleStart()
        {
            /*foreach (var i in GlobalManager.Instance.unlockedAllies) alliesAvailable.Add(i);
            if (summonAllyAction.CheckRequirements(this, out string falseReason))
            {
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(summonAllyAction.Act(this)));
            }
            else
            {
                BattleManager.Instance.UI.ShowText(falseReason);
                yield return Timing.WaitForSeconds(1.0f);
            }*/
            yield break;
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