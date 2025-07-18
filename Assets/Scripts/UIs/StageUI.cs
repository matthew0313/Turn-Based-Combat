using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine;
using AnimatorCutscene;

public class StageUI : TabOpener, ICutsceneCallbackReceiver
{
    public void OnCutsceneEnd()
    {
        
    }

    public void OnCutsceneStart()
    {
        CloseTab();
    }
}