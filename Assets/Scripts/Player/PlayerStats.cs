using System.Collections.Generic;
using UnityEngine;
using MEC;
using Player_Battle;
[System.Serializable]
public class PlayerStats
{
    public float maxHp;
    public float hp;
    public List<PlayerBattleAction> unlockedActions = new();
    public List<BattleEntity> unlockedAllies = new();
}