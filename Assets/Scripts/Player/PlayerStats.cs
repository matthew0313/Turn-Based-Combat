using System.Collections.Generic;
using UnityEngine;
using MEC;
using Player_Battle;
public static class PlayerStats
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        maxHp = 100.0f;
        hp = 100.0f;
        actionsUnlocked = new();
    }
    public static float maxHp;
    public static float hp;

    public static List<PlayerBattleAction> actionsUnlocked;
}