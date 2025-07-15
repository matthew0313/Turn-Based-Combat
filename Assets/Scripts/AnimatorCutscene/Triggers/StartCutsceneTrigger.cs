using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorCutscene;

public class StartCutsceneTrigger : MonoBehaviour
{
    [SerializeField] Cutscene cutscene;
    private void Start()
    {
        cutscene.Play();
    }
}