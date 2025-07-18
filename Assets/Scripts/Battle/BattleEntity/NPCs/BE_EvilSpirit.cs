using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

public class BE_EvilSpirit : NPCBattleEntity
{
    [SerializeField] BA_FireBlast fireBlast;
    [SerializeField] BA_GrassBlast grassBlast;
    [SerializeField] BA_WaterBlast waterBlast;
    public override IEnumerator<float> OnEnemyTurn()
    {
        BattleManager.Instance.UI.ShowText($"{entityName}의 상성이 바뀝니다.");
        int rand = UnityEngine.Random.Range(0, 3);
        switch (rand)
        {
            case 0: currentElement = Element.Water; break;
            case 1: currentElement = Element.Fire; break;
            case 2: currentElement = Element.Grass; break;
        }
        yield return Timing.WaitForSeconds(1.0f);
        switch (currentElement)
        {
            case Element.Fire: 
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(fireBlast.Act(Alliance.Enemy)));  break;
            case Element.Water:
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(waterBlast.Act(Alliance.Enemy))); break;
            case Element.Grass:
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(grassBlast.Act(Alliance.Enemy))); break;
        }
    }
    Element currentElement = Element.Fire;
    readonly Dictionary<Element, Affinity> cache = new();
    protected override Dictionary<Element, Affinity> GetBaseAffinities()
    {
        cache.Clear();
        switch (currentElement)
        {
            case Element.Fire:
                cache.Add(Element.Water, Affinity.Weakness);
                cache.Add(Element.Grass, Affinity.Resist); break;
            case Element.Water:
                cache.Add(Element.Electricity, Affinity.Weakness);
                cache.Add(Element.Fire, Affinity.Resist); break;
            case Element.Grass:
                cache.Add(Element.Fire, Affinity.Weakness);
                cache.Add(Element.Earth, Affinity.Resist); break;
        }
        return cache;
    }
}