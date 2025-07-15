using System.Collections.Generic;
using UnityEngine;
using Player_Field;

public class TestInteraction : Interaction
{
    public override string interactText => "테스트 상호작용";
    public override void OnInteract()
    {
        Debug.Log("Interacted");
    }
}