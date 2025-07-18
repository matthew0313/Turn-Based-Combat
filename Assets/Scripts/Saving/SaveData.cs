using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    public string stageName;
    public SerializableDictionary<string, DataUnit> stageElements = new();

    public Vector2 playerPos;

    public BattleEntity[] party = new BattleEntity[3];
    public List<BattleAction> unlockedActions = new();
    public List<BattleEntity> unlockedAllies = new();
}