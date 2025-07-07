using System;
using System.Collections.Generic;
using AudioManagement;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Timeline;

namespace Player_Battle
{
    [CreateAssetMenu(fileName = "Grass Blast", menuName = "Scriptables/PlayerBattleActions/GrassBlast")]
    public class Player_BA_GrassBlast : PlayerBattleAction
    {
        [Header("Stats")]
        [SerializeField] float damage;
        [SerializeField] float grassModeMultiplier = 1.5f;
        [SerializeField] float fireModeMultiplier = 0.5f;
        [SerializeField] float grassModeHeal = 10.0f;

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] GrassBlast_Projectile projectile;
        [SerializeField] UnitTime projectileStartTime, projectileFlyTime, endTime;
        [SerializeField] Vector2 projectileOffset;
        [SerializeField] AnimationCurve projectileTravelCurve;

        [Header("SFX")]
        [SerializeField] Sound blastFireSound;
        [SerializeField] Sound blastExplodeSound;
        public override TargetSelection targetSelection => TargetSelection.SingleEnemy;


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

                DamageInfo damage = new() { amount = this.damage * actor.attackMultiplier, element = Element.Grass, attacker = actor };
                if (actor.elementMode == PlayerElementMode.Grass)
                {
                    damage.amount *= grassModeMultiplier;
                    actor.GetHeal(new() { healer = actor, amount = grassModeHeal });
                }
                if (actor.elementMode == PlayerElementMode.Fire) damage.amount *= fireModeMultiplier;
                target.GetDamage(damage);
            }).SetEase(projectileTravelCurve);
            while (!finished) yield return Timing.WaitForOneFrame;

            yield return Timing.WaitForSeconds(endTime - projectileStartTime - projectileFlyTime);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(actor.ChangeMode(PlayerElementMode.Grass)));
        }
    }
}