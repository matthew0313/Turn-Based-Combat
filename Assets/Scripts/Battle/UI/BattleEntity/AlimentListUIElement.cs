using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
using TMPro;

public class AlimentListUIElement : MonoBehaviour
{
    [SerializeField] Image alimentIcon;
    [SerializeField] TMP_Text alimentText;
    Aliment displaying;
    public void Set(Aliment aliment)
    {
        if(displaying is IAlimentTextDisplayed)
        {
            (displaying as IAlimentTextDisplayed).onAlimentTextChange -= OnAlimentTextChange;
        }

        displaying = aliment;
        alimentIcon.sprite = aliment.data.alimentIcon;

        if(displaying is IAlimentTextDisplayed)
        {
            alimentText.gameObject.SetActive(true);
            (displaying as IAlimentTextDisplayed).onAlimentTextChange += OnAlimentTextChange;
            OnAlimentTextChange();
        }
        else alimentText.gameObject.SetActive(false);
    }

    void OnAlimentTextChange()
    {
        alimentText.text = (displaying as IAlimentTextDisplayed).alimentText;
    }
}
