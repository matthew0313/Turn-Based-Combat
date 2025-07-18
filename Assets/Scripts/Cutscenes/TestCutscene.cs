using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using System;
using AnimatorCutscene;

public class TestCutscene : Cutscene, ISavable
{
    [SerializeField] SaveID id;
    readonly int talkedID = Animator.StringToHash("Talked");
    public override void OnEnd()
    {
        base.OnEnd();
        anim.SetBool(talkedID, true);
    }

    public void Save(SaveData data)
    {
        data.stageElements[id.value] = new();
        data.stageElements[id.value].bools["talked"] = anim.GetBool(talkedID);
    }
    public void Load(SaveData data)
    {
        anim.SetBool(talkedID, data.stageElements[id.value].bools["talked"]);
    }
}