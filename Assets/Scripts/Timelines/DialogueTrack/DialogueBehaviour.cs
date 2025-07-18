using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    [SerializeField][TextArea] string dialogue;
    [SerializeField] bool autoSpeed = true;

    [Header("Auto Speed")]
    [SerializeField] float endTime = 1.0f;
    [SerializeField] int pauseUnit = 5;

    [Header("Manual Speed")]
    [SerializeField] float talkSpeed = 0.05f;
    [SerializeField] float pauseTime = 0.25f;

    TMP_Text text;
    int displayed = 0;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        text = playerData as TMP_Text;
        if (text == null) return;

        float time = (float)playable.GetTime();
        float tot = 0;
        text.text = "";
        if(autoSpeed == false)
        {
            for (int i = 0; i < dialogue.Length; i++)
            {
                tot += (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseTime : talkSpeed;
                if (time < tot) break;
                text.text += dialogue[i];
            }
        }
        else
        {
            float totalTime = (float)playable.GetDuration() - endTime;
            int unitTotal = 0;
            for(int i = 0; i < dialogue.Length; i++)
            {
                unitTotal += (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseUnit : 1;
            }
            float timePerUnit = totalTime / unitTotal;
            for (int i = 0; i < dialogue.Length; i++)
            {
                tot += timePerUnit * ((dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseUnit : 1);
                text.text += dialogue[i];
                if (time < tot) break;
            }
        }
        if (text.text.Length != displayed)
        {
            displayed = text.text.Length;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (text == null) return;
        else
        {
            text.text = "";
        }
    }
}