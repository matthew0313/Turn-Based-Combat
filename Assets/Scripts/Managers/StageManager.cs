using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;
using System.Linq;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public StageManager() => Instance = this;

    [SerializeField] int m_stageIndex = 0;
    public int stageIndex => m_stageIndex;

    List<ISavable> savables;
    private void Awake()
    {
        savables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISavable>().ToList();
        savables.Sort((a, b) => a.sortOrder.CompareTo(b.sortOrder));
    }
    public void Save(SaveData data)
    {
        foreach (var i in savables) i.Save(data);
    }
    public void Load(SaveData data)
    {
        foreach (var i in savables) i.Load(data);
    }
}