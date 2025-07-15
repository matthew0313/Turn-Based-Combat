using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using System;
using AnimatorCutscene;

public class CutsceneInteraction : Interaction
{
    [SerializeField] Cutscene cutscene;
    public override string interactText => "대화하기";

    public override void OnInteract()
    {
        cutscene.Play();
    }
}