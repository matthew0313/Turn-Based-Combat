using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using System;
using AnimatorCutscene;

public class Interaction_Battle1 : Interaction, ISavable
{
    [SerializeField] Cutscene cutscene;
    [SerializeField] Animator anim;

    [Header("Battle1")]
    [SerializeField] BattleEntity enemy;
    [SerializeField] Sound music;
    [SerializeField] SceneTransition transition = SceneTransition.CircleOut;
    [SerializeField] BattleEntity unlockedAlly;

    [SerializeField] SaveID id;

    bool completed = false, recruited = false, playOnStart = false;
    protected override bool canInteract => !recruited;
    private void Start()
    {
        if (playOnStart)
        {
            cutscene.Play();
            playOnStart = false;
        }
    }
    public override void OnInteract()
    {
        cutscene.Play();
    }
    public void InitiateBattle()
    {
        BattleData battleData = new();
        for (int i = 0; i < BattleManager.maxPlayerSide; i++)
        {
            battleData.playerSide[i] = GlobalManager.Instance.party[i];
        }
        battleData.enemy = enemy;
        battleData.music = music;

        SaveData data = new();
        StageManager.Instance.Save(data);
        data.stageElements[id.value].bools["playOnStart"] = true;
        data.stageElements[id.value].bools["completed"] = true;

        battleData.onVictory = () =>
        {
            GlobalManager.Instance.SwitchScene(data.stageName, SceneTransition.CircleOut, () =>
            {
                StageManager.Instance.Load(data);
            });
        };
        GlobalManager.Instance.EnterBattle(battleData, transition);
    }
    readonly int completedID = Animator.StringToHash("Completed");
    readonly int recruitedID = Animator.StringToHash("Recruited");
    readonly int recruitID = Animator.StringToHash("Recruit");
    public void Recruit()
    {
        recruited = true;
        anim.SetTrigger(recruitID);
        anim.SetBool(recruitedID, true);

        GlobalManager.Instance.unlockedAllies.Add(unlockedAlly);
    }
    public void Load(SaveData data)
    {
        completed = data.stageElements[id.value].bools["completed"];
        anim.SetBool(completedID, completed);
        cutscene.anim.SetBool(completedID, completed);

        recruited = data.stageElements[id.value].bools["recruited"];
        anim.SetBool(recruitedID, recruited);

        playOnStart = data.stageElements[id.value].bools["playOnStart"];
    }

    public void Save(SaveData data)
    {
        data.stageElements[id.value] = new();
        data.stageElements[id.value].bools["completed"] = completed;
        data.stageElements[id.value].bools["recruited"] = recruited;
        data.stageElements[id.value].bools["playOnStart"] = playOnStart;
    }
}