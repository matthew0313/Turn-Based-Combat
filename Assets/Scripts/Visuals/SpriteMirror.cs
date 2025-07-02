using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMirror : MonoBehaviour
{
    [SerializeField] SpriteRenderer target;
    SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (target != null) spriteRenderer.sprite = target.sprite;
    }
}