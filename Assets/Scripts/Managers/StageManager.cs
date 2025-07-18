using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;
using System.Linq;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] string stageName;
    [SerializeField] StageUI m_UI;
    public StageUI UI => m_UI;
    List<ISavable> m_savables;
    List<ISavable> savables
    {
        get
        {
            if (m_savables == null)
            {
                m_savables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISavable>().ToList();
                m_savables.Sort((a, b) => a.sortOrder.CompareTo(b.sortOrder));
            }
            return m_savables;
        }
    }
    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;
    }
    public void Save(SaveData data)
    {
        data.stageName = stageName;
        foreach (var i in savables) i.Save(data);
    }
    public void Load(SaveData data)
    {
        foreach (var i in savables) i.Load(data);
    }
}