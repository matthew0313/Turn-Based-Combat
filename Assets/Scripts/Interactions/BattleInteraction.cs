using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using System;

public class BattleInteraction : Interaction, ISavable
{
    [SerializeField] Animator anim;
    [SerializeField] BattleEntity[] enemies = new BattleEntity[BattleManager.maxEnemySide];
    [SerializeField] Sound music;
    [SerializeField] SceneTransition transition = SceneTransition.CircleOut;
    [SerializeField] SaveID id;
    bool completed = false;
    protected override bool canInteract => !completed;
    public override void OnInteract()
    {
        BattleData battleData = new();
        battleData.playerSide[0] = player.battleEntity;
        for(int i = 0; i < BattleManager.maxPlayerSide - 1; i++)
        {
            battleData.playerSide[i + 1] = GlobalManager.Instance.party[i];
        }
        for(int i = 0; i < BattleManager.maxEnemySide; i++)
        {
            battleData.enemySide[i] = enemies[i];
        }
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
            GlobalManager.Instance.EnterStage(StageManager.Instance.stageIndex, SceneTransition.CircleOut, () =>
            {
                StageManager.Instance.Load(data);
            });
        };
    }
    protected virtual Action GetOnLoss() => null;
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