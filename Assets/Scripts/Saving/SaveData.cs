using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    public PlayerStats playerStats;

    public int stageIndex = 0;
    public SerializableDictionary<string, DataUnit> stageElements = new();

    public Vector2 playerPos;
}