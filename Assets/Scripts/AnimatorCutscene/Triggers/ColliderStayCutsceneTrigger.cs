using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorCutscene;

[RequireComponent(typeof(Collider))]
public class ColliderStayCutsceneTrigger : MonoBehaviour
{
    [SerializeField] Cutscene cutscene;
    [SerializeField] bool playOnce = true;
    bool played = false;
    private void OnTriggerEnter(Collider other)
    {
        if (played && playOnce) return;
        if(other.CompareTag("Player"))
        {
            cutscene.Play();
            played = true;
        }
    }
}