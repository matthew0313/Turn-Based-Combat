using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using DG.Tweening;
using MEC;

public class JumpInteraction : Interaction
{
    public override string interactText => "도약하기";
    [SerializeField] Animator anim;
    [SerializeField] float jumpDuration = 1.0f;
    [SerializeField] Transform jumpTarget;
    public override void OnInteract()
    {
        player.movements.enabled = false;
        player.interactions.enabled = false;
        Timing.RunCoroutine(Jumping());
    }
    IEnumerator<float> Jumping()
    {
        float counter = 0.0f;
        Vector2 startPos = player.transform.position, targetPos = jumpTarget.position;
        while(counter < jumpDuration)
        {
            yield return Timing.WaitForOneFrame;
            counter = Mathf.Min(counter + Time.deltaTime, jumpDuration);
            player.transform.position = Vector2.Lerp(startPos, targetPos, counter / jumpDuration);
        }
        player.movements.enabled = true;
        player.interactions.enabled = true;
    }
    readonly int selectedID = Animator.StringToHash("Selected");
    public override void OnSelect()
    {
        base.OnSelect();
        anim.SetBool(selectedID, true);
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        anim.SetBool(selectedID, false);
    }
}