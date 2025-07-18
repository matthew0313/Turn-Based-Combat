using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

public class BE_WaterSpirit : NPCBattleEntity
{
    [Header("Water Splash")]
    [SerializeField] float WSDamage = 10;
    [SerializeField] int WSCount = 3;
    [SerializeField] UnitTime WSStartTime, WSInterval, WSEndTime;
    [SerializeField] Transform WSBlastFirePoint;
    [SerializeField] PooledEffect WSBlast, WSExplosion;
    [SerializeField] float WSBlastFlyTime = 1.0f;
    protected override IEnumerator<float> OnTurn()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(base.OnTurn()));
        yield return Timing.WaitUntilDone(
            Timing.RunCoroutine(WaterSplash(BattleManager.Instance.GetEnemies(alliance).SelectRandom(item => item != null))));
    }
    IEnumerator<float> WaterSplash(BattleEntity target)
    {
        yield return Timing.WaitForSeconds(WSStartTime);
        Timing.RunCoroutine(WaterSplashBurst(target));
        yield return Timing.WaitForSeconds(WSEndTime - WSStartTime);
    }
    IEnumerator<float> WaterSplashBurst(BattleEntity target)
    {
        for (int i = 0; i < WSCount; i++)
        {
            WaterSplashFire(target);
            yield return Timing.WaitForSeconds(WSInterval);
        }
    }
    void WaterSplashFire(BattleEntity target)
    {
        PooledEffect blast = WSBlast.Get(WSBlastFirePoint.position,
            MathUtility.LookAt(WSBlastFirePoint.position, target.transform.position));

        blast.transform.DOMove(target.transform.position, WSBlastFlyTime).SetEase(Ease.InCirc).OnComplete(() =>
        {
            WSExplosion.Get(target.transform.position, Quaternion.identity);
            target.GetDamage(new()
            {
                element = Element.Water,
                amount = WSDamage,
                attacker = this
            });
        });
    }
}