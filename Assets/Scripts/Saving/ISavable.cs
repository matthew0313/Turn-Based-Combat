using UnityEngine;
using UnityEngine.InputSystem;

public interface ISavable
{
    public int sortOrder => 0;
    public void Save(SaveData data);
    public void Load(SaveData data);
}