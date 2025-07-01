using System;
using System.Collections.Generic;
using System.Text;
using MEC;
using UnityEngine;

public static class CoroutineUtility
{
    public static IEnumerator<float> WaitThen(float time, Action action)
    {
        yield return Timing.WaitForSeconds(time); action?.Invoke();
    }
    public static IEnumerator<float> Typewrite(string text, float interval, float longInterval, Action<string> onStringUpdate, Action onFinish = null)
    {
        StringBuilder sb = new();
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                int tagStart = i;
                int tagEnd = text.Length;
                for (int k = tagStart + 1; k < text.Length; k++)
                {
                    if (text[k] == '>')
                    {
                        tagEnd = k;
                        break;
                    }
                }
                if (tagEnd < text.Length)
                {
                    sb.Append(text.Substring(tagStart, tagEnd - tagStart + 1));
                    i += tagEnd - tagStart + 1;
                }
            }

            sb.Append(text[i]);
            onStringUpdate.Invoke(sb.ToString());

            if (text[i] == '.' || text[i] == ',' || text[i] == '!' || text[i] == '?' || text[i] == '\n') yield return Timing.WaitForSeconds(longInterval);
            else yield return Timing.WaitForSeconds(interval);
            i++;
        }
        onFinish?.Invoke();
    }
}