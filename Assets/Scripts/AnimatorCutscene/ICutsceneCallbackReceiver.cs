using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorCutscene
{
    public interface ICutsceneCallbackReceiver
    {
        public int executionOrder => 0;
        public void OnCutsceneStart();
        public void OnCutsceneEnd();
    }
}