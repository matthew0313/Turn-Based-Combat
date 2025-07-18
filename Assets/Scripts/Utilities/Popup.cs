using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Popup : MonoBehaviour
{
    [SerializeField] float duration = 0.7f;
    CoroutineHandle popping;
    public void Pop()
    {
        popping = Timing.RunCoroutine(Popping());
    }
    IEnumerator<float> Popping()
    {
        gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Timing.KillCoroutines(popping);
    }
}