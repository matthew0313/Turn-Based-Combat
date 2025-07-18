using System.Collections.Generic;
using Player_Battle;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    public int stageIndex = 0;
    public SerializableDictionary<string, DataUnit> stageElements = new();

    public Vector2 playerPos;

    public BattleEntity[] party = new BattleEntity[3];
    public List<PlayerBattleAction> unlockedActions = new();
    public List<BattleEntity> unlockedAllies = new();
}