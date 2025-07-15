using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorCutscene;

[RequireComponent(typeof(Collider2D))]
public class Collider2DStayCutsceneTrigger : MonoBehaviour
{
    [SerializeField] Cutscene cutscene;
    [SerializeField] bool playOnce = true;
    bool played = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (played && playOnce) return;
        if(other.attachedRigidbody.CompareTag("Player"))
        {
            cutscene.Play();
            played = true;
        }
    }
}