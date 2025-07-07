using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class PooledEffect : PooledPrefab<PooledEffect>
{
    [SerializeField] float duration = 5.0f;
    CoroutineHandle waiting;
    protected override void OnGet()
    {
        base.OnGet();
        waiting = Timing.RunCoroutine(CoroutineUtility.WaitThen(duration, () => Release()));
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Timing.KillCoroutines(waiting);
    }
}