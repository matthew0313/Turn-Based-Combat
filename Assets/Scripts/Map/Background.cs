using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Background : MonoBehaviour
{
    [SerializeField] float scale = 100.0f;
    [SerializeField] RectTransform self;
    private void Update()
    {
        self.anchoredPosition = new Vector2(Camera.main.transform.position.x * scale, 0.0f);
    }
}