using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using System;
using AnimatorCutscene;

public class StartCutscene : Cutscene, ISavable
{
    [SerializeField] SaveID id;
    bool played = false;

    private void Start()
    {
        if (!played)
        {
            Play(); played = true;
        }
    }
    public void Load(SaveData data)
    {
        played = data.stageElements[id.value].bools["played"];
    }

    public void Save(SaveData data)
    {
        data.stageElements[id.value] = new();
        data.stageElements[id.value].bools["played"] = played;
    }
}