using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotUI : Tab
{
    [SerializeField] Animator anim;
    [SerializeField] int slotIndex = 0;
    [SerializeField] Image entityIcon;
    [SerializeField] TMP_Text entityText;
    private void OnEnable()
    {
        GlobalManager.Instance.onPartyChange += OnPartyChange;
        OnPartyChange();
    }
    private void OnDisable()
    {
        GlobalManager.Instance.onPartyChange -= OnPartyChange;
    }
    readonly int selectedID = Animator.StringToHash("Selected");
    public void OnSelect()
    {
        anim.SetBool(selectedID, true);
    }
    public void OnDeselect()
    {
        anim.SetBool(selectedID, false);
    }
    void OnPartyChange()
    {
        if (GlobalManager.Instance.party[slotIndex] != null)
        {
            entityIcon.sprite = GlobalManager.Instance.party[slotIndex].entityIcon;
            entityIcon.gameObject.SetActive(true);
            entityText.text = GlobalManager.Instance.party[slotIndex].entityName;
        }
        else
        {
            entityIcon.gameObject.SetActive(false);
            entityText.text = "없음";
        }
    }
}