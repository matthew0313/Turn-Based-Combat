using System;
using System.Collections.Generic;
using AudioManagement;
using DG.Tweening;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Fire Blast", menuName = "Scriptables/PlayerBattleActions/FireBlast")]
    public class Player_BA_FireBlast : PlayerBattleAction
    {
        [Header("Stats")]
        [SerializeField] float damage;
        [SerializeField] float fireModeMultiplier = 1.5f;
        [SerializeField] float waterModeMultiplier = 0.5f;
        [SerializeField] float burnDamage = 5.0f;
        [SerializeField] int burnTurns = 3;

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] FireBlast_Projectile projectile;
        [SerializeField] UnitTime projectileStartTime, projectileFlyTime, endTime;
        [SerializeField] Vector2 projectileOffset;
        [SerializeField] AnimationCurve projectileTravelCurve;

        [Header("SFX")]
        [SerializeField] Sound blastFireSound;
        [SerializeField] Sound blastExplodeSound;
        public override TargetSelection targetSelection => TargetSelection.SingleEnemy;
        public override Element abilityElement => Element.Fire;

        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override IEnumerator<float> Act(BE_Player actor)
        {
            if (target == null) yield break;

            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(projectileStartTime);

            Vector2 start = (Vector2)actor.transform.position + projectileOffset;
            var tmp = projectile.Get(start, MathUtility.LookAt(start, target.transform.position));
            AudioManager.Instance.PlaySound(blastFireSound);
            bool finished = false;
            tmp.transform.DOMove(target.transform.position, projectileFlyTime).OnComplete(() =>
            {
                finished = true;
                tmp.Explode();
                AudioManager.Instance.PlaySound(blastExplodeSound);

                DamageInfo damage = new() { amount = this.damage * actor.attackMultiplier, element = Element.Fire, attacker = actor };
                if (actor.elementMode == PlayerElementMode.Fire)
                {
                    damage.amount *= fireModeMultiplier;
                    damage.aliment = new Aliment_Burn(burnDamage, burnTurns);
                }
                if (actor.elementMode == PlayerElementMode.Water) damage.amount *= waterModeMultiplier;
                target.GetDamage(damage);
            }).SetEase(projectileTravelCurve);
            while (!finished) yield return Timing.WaitForOneFrame;

            yield return Timing.WaitForSeconds(endTime - projectileStartTime - projectileFlyTime);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(actor.ChangeMode(PlayerElementMode.Fire)));
        }
    }
}