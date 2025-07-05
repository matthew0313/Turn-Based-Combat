using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;

public class AlimentListUI : MonoBehaviour
{
    [SerializeField] BattleEntity target;
    [SerializeField] AlimentListUIElement elementPrefab;
    [SerializeField] Transform elementAnchor;
    readonly List<AlimentListUIElement> elements = new();
    private void OnEnable()
    {
        target.onAlimentChange += OnAlimentChange;
        OnAlimentChange();
    }
    private void OnDisable()
    {
        target.onAlimentChange -= OnAlimentChange;
    }
    void OnAlimentChange()
    {
        int i = 0;
        foreach(var aliment in target.GetAliments())
        {
            if (elements.Count <= i) elements.Add(Instantiate(elementPrefab, elementAnchor));
            elements[i].gameObject.SetActive(true);
            elements[i].Set(aliment);
            i++;
        }
        for(; i < elements.Count; i++) elements[i].gameObject.SetActive(false);
    }
}
