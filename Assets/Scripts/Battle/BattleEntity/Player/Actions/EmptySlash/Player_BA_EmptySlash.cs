using System;
using System.Collections.Generic;
using AudioManagement;
using DG.Tweening;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Empty Slash", menuName = "Scriptables/PlayerBattleActions/EmptySlash")]
    public class Player_BA_EmptySlash : PlayerBattleAction
    {
        [Header("Stats")]
        [SerializeField] float damagePerSlash = 5;
        [SerializeField] int slashCount = 3;
        [SerializeField] float noModeMultiplier = 1.2f;

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] PooledEffect slashEffect;
        [SerializeField] UnitTime startUp, slashInterval, slashDamageDelay, endTime;

        [Header("SFX")]
        [SerializeField] Sound slashSound;
        public override TargetSelection targetSelection => TargetSelection.SingleEnemy;
        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override IEnumerator<float> Act(BE_Player actor)
        {
            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(startUp);
            for(int i = 0; i < slashCount; i++)
            {
                Timing.RunCoroutine(Slash(actor));
                yield return Timing.WaitForSeconds(slashInterval);
            }
            yield return Timing.WaitForSeconds(endTime - slashInterval * slashCount - startUp);
        }
        IEnumerator<float> Slash(BE_Player actor)
        {
            slashEffect.Get(target.transform.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360.0f)));
            AudioManager.Instance.PlaySound(slashSound);
            yield return Timing.WaitForSeconds(slashDamageDelay);
            target.GetDamage(new()
            {
                attacker = actor,
                element = Element.Physical,
                amount = damagePerSlash * (actor.elementMode == PlayerElementMode.None ? noModeMultiplier : 1.0f) * actor.attackMultiplier
            });
        }
    }
}