using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using System;

public class BattleInteraction : Interaction, ISavable
{
    [SerializeField] Animator anim;
    [SerializeField] BattleEntity enemy;
    [SerializeField] Sound music;
    [SerializeField] SceneTransition transition = SceneTransition.CircleOut;
    [SerializeField] SaveID id;
    bool completed = false;
    protected override bool canInteract => !completed;
    public override void OnInteract()
    {
        BattleData battleData = new();
        for(int i = 0; i < BattleManager.maxPlayerSide; i++)
        {
            battleData.playerSide[i] = GlobalManager.Instance.party[i];
        }
        battleData.enemy = enemy;
        battleData.music = music;
        battleData.onVictory = GetOnVictory();
        battleData.onLoss = GetOnLoss();
        GlobalManager.Instance.EnterBattle(battleData, transition);
    }
    protected virtual Action GetOnVictory()
    {
        SaveData data = new();
        completed = true;
        StageManager.Instance.Save(data);
        return () =>
        {
            GlobalManager.Instance.SwitchScene(data.stageName, SceneTransition.CircleOut, () =>
            {
                StageManager.Instance.Load(data);
            });
        };
    }
    protected virtual Action GetOnLoss()
    {
        return () => { Debug.Log("OnLoss"); GlobalManager.Instance.SwitchScene("GameOver", SceneTransition.CircleOut); };
    }
    readonly int completedID = Animator.StringToHash("Completed");
    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.bools["completed"] = completed;
        data.stageElements[id.value] = tmp;
    }

    public void Load(SaveData data)
    {
        completed = data.stageElements[id.value].bools["completed"];
        anim.SetBool(completedID, completed);
    }
}