using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

public class BattleManager_UI : MonoBehaviour
{
    [Header("Element icons")]
    [SerializeField] Sprite physicalSprite;
    [SerializeField] Sprite fireSprite, waterSprite, grassSprite;

    [Header("Damage Indicator")]
    [SerializeField] DamageIndicator damageIndicatorPrefab;
    [SerializeField] float offsetRange = 1.0f;

    [Header("Battle Text")]
    [SerializeField] BattleText battleText;
    public void IndicateDamage(DamageReceivedInfo info)
    {
        float angle = UnityEngine.Random.Range(0, 360.0f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * UnityEngine.Random.Range(0, offsetRange);
        damageIndicatorPrefab.Get(info.receiver.transform.position + offset, Quaternion.identity).Set(info);
    }
    public Sprite GetSprite(Element element)
    {
        switch (element)
        {
            case Element.None: return null;
            case Element.Physical: return physicalSprite;
            case Element.Fire: return fireSprite;
            case Element.Water: return waterSprite;
            case Element.Grass: return grassSprite;
            default: return null;
        }
    }
    public void ShowText(string content)
    {
        battleText.ShowText(content);
    }
}
