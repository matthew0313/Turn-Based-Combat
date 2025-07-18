using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BackgroundLayer : MonoBehaviour
{
    [SerializeField] float multiplier = 1.0f;
    [SerializeField] int segments = 2;
    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        int divisor = Mathf.FloorToInt(rectTransform.rect.width) * (segments - 1);
        transform.localPosition = new Vector2(Mathf.Repeat((Mathf.Repeat(Camera.main.transform.position.x * multiplier, divisor) + divisor), divisor), 0.0f);
    }
}