using System.Collections.Generic;
using UnityEngine;
using MEC;

public class CanvasGroupTweener : MonoBehaviour
{
    [SerializeField] CanvasGroup group;
    private void Awake()
    {
        group.alpha = 0.0f;
    }
}