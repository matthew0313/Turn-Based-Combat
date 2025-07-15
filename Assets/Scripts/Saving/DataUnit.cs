using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DataUnit
{
    public SerializableDictionary<string, int> integers = new();
    public SerializableDictionary<string, float> floats = new();
    public SerializableDictionary<string, string> strings = new();
    public SerializableDictionary<string, bool> bools = new();
}