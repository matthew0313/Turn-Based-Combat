using System.Collections.Generic;
using UnityEngine;
using Player_Field;
using DG.Tweening;
using MEC;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.UI.Image;

public class JumpInteraction : Interaction
{
    [SerializeField] Animator anim;
    [SerializeField] float jumpDuration = 1.0f;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] Transform jumpTarget;
    private void OnDrawGizmos()
    {
        if(jumpCurve != null && jumpTarget != null)
        {
            Gizmos.color = Color.blue;
            Vector2 startPos = transform.position, targetPos = jumpTarget.position;
            for(int i = 0; i <= 10; i++)
            {
                Vector2 pos = Vector2.Lerp(startPos, targetPos, 0.1f * i);
                pos += Vector2.up * jumpCurve.Evaluate(0.1f * i);
                Gizmos.DrawWireSphere(pos, 0.1f);
            }
        }
    }
    public override void OnInteract()
    {
        player.movements.enabled = false;
        player.interactions.enabled = false;
        Timing.RunCoroutine(Jumping());
    }
    readonly int directionID = Animator.StringToHash("Direction");
    readonly int jumpID = Animator.StringToHash("Jump");
    readonly int isJumpingID = Animator.StringToHash("IsJumping");
    IEnumerator<float> Jumping()
    {
        float counter = 0.0f;
        Vector2 startPos = player.transform.position, targetPos = jumpTarget.position;

        Vector2 move = (targetPos - startPos).normalized;
        if (Mathf.Abs(move.x) >= Mathf.Abs(move.y))
        {
            if (move.x > 0) player.anim.SetInteger(directionID, 0);
            else player.anim.SetInteger(directionID, 2);
        }
        else
        {
            if (move.y > 0) player.anim.SetInteger(directionID, 1);
            else player.anim.SetInteger(directionID, 3);
        }
        player.anim.SetTrigger(jumpID);
        player.anim.SetBool(isJumpingID, true);

        while (counter < jumpDuration)
        {
            yield return Timing.WaitForOneFrame;
            counter = Mathf.Min(counter + Time.deltaTime, jumpDuration);
            Vector2 pos = Vector2.Lerp(startPos, targetPos, counter / jumpDuration);
            pos += Vector2.up * jumpCurve.Evaluate(counter / jumpDuration);
            player.transform.position = pos;
        }
        player.movements.enabled = true;
        player.interactions.enabled = true;
        player.anim.SetBool(isJumpingID, false);
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