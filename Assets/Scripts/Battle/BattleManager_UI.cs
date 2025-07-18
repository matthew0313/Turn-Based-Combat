using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager_UI : MonoBehaviour
{
    [Header("Element icons")]
    [SerializeField] Sprite waterSprite;
    [SerializeField] Sprite fireSprite, grassSprite, earthSprite, electricitySprite;

    [Header("Indicators")]
    [SerializeField] DamageIndicator damageIndicatorPrefab;
    [SerializeField] HealIndicator healIndicatorPrefab;
    [SerializeField] float offsetRange = 1.0f;

    [Header("Battle Text")]
    [SerializeField] BattleText battleText;

    [Header("Action Selection")]
    [SerializeField] ActionCardSet cardSet;
    [SerializeField] Transform[] cardAnchors = new Transform[BattleManager.cardCount];
    [SerializeField] TargetMark markPrefab;
    ActionCard[] cards = new ActionCard[BattleManager.cardCount];

    [Header("Fast Play")]
    [SerializeField] GameObject fastPlayEffect;
    [SerializeField] Image fastPlayEffectTop, fastPlayEffectBottom;
    [SerializeField] float scrollSpeed = 2.0f;
    readonly int scrollID = Shader.PropertyToID("_Scroll");
    float scroll = 0.0f;
    private void Update()
    {
        if (isFastPlaying)
        {
            scroll += Time.unscaledDeltaTime * scrollSpeed;
            fastPlayEffectTop.material.SetFloat(scrollID, scroll);
            fastPlayEffectBottom.material.SetFloat(scrollID, scroll);
        }
    }
    public void IndicateDamage(DamageReceivedInfo info)
    {
        float angle = UnityEngine.Random.Range(0, 360.0f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * UnityEngine.Random.Range(0, offsetRange);
        damageIndicatorPrefab.Get(info.receiver.transform.position + offset, Quaternion.identity).Set(info);
    }
    public void IndicateHeal(HealReceivedInfo info)
    {
        float angle = UnityEngine.Random.Range(0, 360.0f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * UnityEngine.Random.Range(0, offsetRange);
        healIndicatorPrefab.Get(info.receiver.transform.position + offset, Quaternion.identity).Set(info);
    }
    public Sprite GetSprite(Element element)
    {
        switch (element)
        {
            case Element.Water: return waterSprite;
            case Element.Fire: return fireSprite;
            case Element.Grass: return grassSprite;
            case Element.Earth: return earthSprite;
            case Element.Electricity: return electricitySprite;
        }
        return null;
    }
    public void ShowText(string content)
    {
        battleText.ShowText(content);
    }
    public void ShowCards()
    {
        cardSet.Show();
    }
    public void HideCards()
    {
        cardSet.Hide();
    }
    public void SetCard(int index, BattleAction battleAction)
    {
        ActionCard tmp = battleAction.card.Get();
        tmp.transform.SetParent(cardAnchors[index]);
        tmp.transform.localPosition = Vector2.zero;
        tmp.transform.localRotation = Quaternion.identity;
        tmp.transform.localScale = Vector3.one;
        tmp.Set(battleAction, index);

        if (cards[index] != null) cards[index].Release();
        cards[index] = tmp;
    }
    ActionCard selectedCard;
    public void SelectCard(int index)
    {
        if (selectedCard != null) selectedCard.OnDeselect();
        selectedCard = cards[index];
        selectedCard.OnSelect();
    }
    public void DeselectCard()
    {
        if (selectedCard != null) selectedCard.OnDeselect();
        selectedCard = null;
    }
    List<TargetMark> marks = new();
    public void ShowMark(BattleEntity entity)
    {
        marks.Add(markPrefab.Get(entity.transform.position, Quaternion.identity));
    }
    public void HideMarks()
    {
        foreach (var i in marks) i.Release(); marks.Clear();
    }
    bool isFastPlaying = false;
    public void ToggleFastPlay(bool fastPlay)
    {
        fastPlayEffect.SetActive(fastPlay);
        isFastPlaying = fastPlay;
    }
}
