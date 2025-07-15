using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveStateSaver : MonoBehaviour, ISavable
{
    [SerializeField] GameObject saved;
    [SerializeField] SaveID id;

    public void Load(SaveData data)
    {
        data.stageElements[id.value] = new();
        data.stageElements[id.value].bools["active"] = saved.activeInHierarchy;
    }

    public void Save(SaveData data)
    {
        saved.SetActive(data.stageElements[id.value].bools["active"]);
    }
}