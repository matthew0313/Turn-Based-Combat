using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedBarUI : MonoBehaviour
{
    [SerializeField] Image bar, reduceBar, refillBar;
    [SerializeField] float lerpRate = 3.0f;
    [SerializeField] float waitTime = 1.0f;
    [SerializeField] float m_targetScale = 1.0f;
    public float targetScale
    {
        get => m_targetScale;
        set
        {
            m_targetScale = value;
            UpdateBar();
        }
    }
    float currentScale = 1.0f;
    private void Update()
    {
        currentScale = Mathf.Lerp(currentScale, targetScale, lerpRate * Time.deltaTime);
        UpdateBar();
    }
    void UpdateBar()
    {
        if (currentScale < targetScale)
        {
            reduceBar.transform.localScale = new Vector2(0.0f, 1.0f);
            refillBar.transform.localScale = new Vector2(targetScale, 1.0f);
            bar.transform.localScale = new Vector2(currentScale, 1.0f);
        }
        else
        {
            refillBar.transform.localScale = new Vector2(0.0f, 1.0f);
            bar.transform.localScale = new Vector2(targetScale, 1.0f);
            reduceBar.transform.localScale = new Vector2(currentScale, 1.0f);
        }
    }
}