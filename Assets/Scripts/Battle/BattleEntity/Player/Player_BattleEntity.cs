using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Player_Battle
{
    public class Player_BattleEntity : BattleEntity
    {
        [Header("Mode Change")]
        [SerializeField] UnitTime modeChangeTime;
        [SerializeField] UnitTime modeChangeEndTime;
        public override float maxHp => PlayerStats.maxHp;
        public override float hp
        {
            get => PlayerStats.hp;
            protected set => PlayerStats.hp = value;
        }
        public PlayerElementMode elementMode { get; private set; } = PlayerElementMode.None;
        protected override IEnumerator<float> OnTurn(Action onFinish)
        {
            bool parentDone = false;  Timing.RunCoroutine(base.OnTurn(() => parentDone = true));
            while (!parentDone) yield return Timing.WaitForOneFrame;

            while (true)
            {
                yield return Timing.WaitForOneFrame;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    BattleManager.Instance.UI.ShowText("플레이어가 턴을 스킵합니다.");
                    yield return Timing.WaitForSeconds(1.0f);
                    break;
                }
            }
            onFinish?.Invoke();
        }
        readonly int changeModeID = Animator.StringToHash("ChangeMode");
        readonly int modeID = Animator.StringToHash("Mode");
        internal IEnumerator<float> ChangeMode(Action onFinish, PlayerElementMode mode)
        {
            if (elementMode == mode)
            {
                onFinish?.Invoke(); yield break;
            }

            anim.SetTrigger(changeModeID);
            yield return Timing.WaitForSeconds(modeChangeTime);
            elementMode = mode;
            anim.SetFloat(modeID, (float)mode);
            yield return Timing.WaitForSeconds(modeChangeEndTime - modeChangeTime);
            onFinish?.Invoke();
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