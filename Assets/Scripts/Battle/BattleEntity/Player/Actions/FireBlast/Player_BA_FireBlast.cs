using System;
using System.Collections.Generic;
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

        [Header("Visuals")]
        [SerializeField] string triggerName;
        [SerializeField] FireBlast_Projectile projectile;
        [SerializeField] UnitTime projectileStartTime, projectileFlyTime, endTime;
        [SerializeField] Vector2 projectileOffset;
        [SerializeField] AnimationCurve projectileTravelCurve;
        public override TargetSelection targetSelection => TargetSelection.SingleEnemy;


        int triggerID;
        private void OnEnable()
        {
            triggerID = Animator.StringToHash(triggerName);
        }
        public override IEnumerator<float> Act(Action onFinish, Player_BattleEntity actor)
        {
            if (target == null)
            {
                onFinish?.Invoke(); yield break;
            }

            actor.anim.SetTrigger(triggerID);
            yield return Timing.WaitForSeconds(projectileStartTime);

            Vector2 start = (Vector2)actor.transform.position + projectileOffset;
            var tmp = projectile.Get(start, MathUtility.LookAt(start, target.transform.position));
            bool finished = false;
            tmp.transform.DOMove(target.transform.position, projectileFlyTime).OnComplete(() =>
            {
                finished = true;
                tmp.Explode();

                DamageInfo damage = new() { amount = this.damage, element = Element.Fire };
                if (actor.elementMode == PlayerElementMode.Fire) damage.amount *= fireModeMultiplier;
                if (actor.elementMode == PlayerElementMode.Water) damage.amount *= waterModeMultiplier;
                target.GetDamage(damage);
            }).SetEase(projectileTravelCurve);
            while (!finished) yield return Timing.WaitForOneFrame;

            yield return Timing.WaitForSeconds(endTime - projectileStartTime - projectileFlyTime);

            finished = false;
            Timing.RunCoroutine(actor.ChangeMode(() => finished = true, PlayerElementMode.Fire));
            while(!finished) yield return Timing.WaitForOneFrame;

            onFinish?.Invoke();
        }
    }
}