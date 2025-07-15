using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using Player_Battle;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class BattleManager_UI : MonoBehaviour
{
    [Header("Element icons")]
    [SerializeField] Sprite physicalSprite;
    [SerializeField] Sprite fireSprite, waterSprite, grassSprite;

    [Header("Indicators")]
    [SerializeField] DamageIndicator damageIndicatorPrefab;
    [SerializeField] HealIndicator healIndicatorPrefab;
    [SerializeField] float offsetRange = 1.0f;

    [Header("Battle Text")]
    [SerializeField] BattleText battleText;

    [Header("Action Selection")]
    [SerializeField] ActionCardSet cardSet;
    [SerializeField] ActionCard[] cards = new ActionCard[BattleManager.cardCount];
    [SerializeField] TargetMark markPrefab;

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
    public void ShowCards()
    {
        cardSet.Show();
    }
    public void HideCards()
    {
        cardSet.Hide();
    }
    public void SetCard(int index, PlayerBattleAction battleAction)
    {
        cards[index].Set(battleAction);
    }
    ActionCard m_selectedCard = null;
    ActionCard selectedCard
    {
        set
        {
            if (m_selectedCard != value)
            {
                if (m_selectedCard != null) m_selectedCard.OnDeselect();
                m_selectedCard = value;
                if (m_selectedCard != null) m_selectedCard.OnSelect();
            }
        }
    }
    public void SelectCard(int index)
    {
        selectedCard = cards[index];
    }
    public void DeselectCard()
    {
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
