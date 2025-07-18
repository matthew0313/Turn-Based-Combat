using System.Collections.Generic;
using System.Linq;
using AudioManagement;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PartyTab : Tab
{
    [SerializeField] PartySlotUI[] slotUIs = new PartySlotUI[BattleManager.maxPlayerSide];
    [SerializeField] PartyMemberUI memberUIPrefab;
    [SerializeField] Transform memberUIAnchor;
    readonly List<PartyMemberUI> memberUIs = new();

    [Header("Tweening")]
    [SerializeField] RectTransform tweenTarget;
    [SerializeField] float tweenTime = 0.5f;

    Tween tween;
    float tweenCounter = 0.0f;
    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        if (tween == null) tween = DOTween.To(() => tweenCounter, value => tweenCounter = value, 1.0f, tweenTime)
                .SetAutoKill(false)
                .SetEase(Ease.OutCirc)
                .OnUpdate(() =>
                {
                    tweenTarget.anchorMin = new Vector2(0.0f, 1.0f - tweenCounter);
                    tweenTarget.anchorMax = new Vector2(1.0f, 2.0f - tweenCounter);
                });
        tween.OnStepComplete(null);
        tween.PlayForward();
    }
    public override void Close()
    {
        base.Close();
        tween.OnStepComplete(() => gameObject.SetActive(false));
        tween.PlayBackwards();
    }
    private void OnEnable()
    {
        GlobalManager.Instance.onAllyUnlock += Refresh;
        GlobalManager.Instance.onPartyChange += Refresh;
        SelectSlot(0);
        Refresh();
    }
    private void OnDisable()
    {
        GlobalManager.Instance.onAllyUnlock -= Refresh;
        GlobalManager.Instance.onPartyChange -= Refresh;

        DeselectSlot();
        DeselectMember();
    }
    bool selectingMember = false;

    int selectedSlotIndex = 0;
    PartySlotUI selectedSlot = null;

    int selectedMemberIndex = 0;
    PartyMemberUI selectedMember = null;
    private void Update()
    {
        if (selectingMember)
        {
            if (InputManager.Instance.SelectRight.triggered) SelectMember(selectedMemberIndex + 1);
            if (InputManager.Instance.SelectLeft.triggered) SelectMember(selectedMemberIndex - 1);
            if (InputManager.Instance.Cancel.triggered)
            {
                AudioManager.Instance.PlaySound("Cancel");
                DeselectMember();
                selectingMember = false;
                return;
            }
            if (InputManager.Instance.Select.triggered)
            {
                GlobalManager.Instance.ChangePartyMember(selectedSlotIndex, selectedMember.entity);
                AudioManager.Instance.PlaySound("Confirm");
                DeselectMember();
                selectingMember = false;
            }
        }
        else
        {
            if (InputManager.Instance.SelectRight.triggered) SelectSlot(selectedSlotIndex + 1);
            if (InputManager.Instance.SelectLeft.triggered) SelectSlot(selectedSlotIndex - 1);
            if (InputManager.Instance.Select.triggered)
            {
                if(GlobalManager.Instance.unlockedAllies.Count > 0)
                {
                    SelectMember(0);
                    AudioManager.Instance.PlaySound("Confirm");
                    selectingMember = true;
                }
                else
                {
                    AudioManager.Instance.PlaySound("Invalid");
                }
            }
        }
    }
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slotUIs.Length || selectedSlot == slotUIs[index]) return;
        if (selectedSlot != null) selectedSlot.OnDeselect();
        selectedSlotIndex = index;
        selectedSlot = slotUIs[index];
        selectedSlot.OnSelect();
    }
    public void DeselectSlot()
    {
        if (selectedSlot != null) selectedSlot.OnDeselect();
        selectedSlot = null;
    }
    public void SelectMember(int index)
    {
        if (index < 0 || index >= memberUIs.Count || selectedMember == memberUIs[index]) return;
        if (selectedMember != null) selectedMember.OnDeselect();
        selectedMember = memberUIs[index];
        selectedMember.OnSelect();
    }
    public void DeselectMember()
    {
        if (selectedMember != null) selectedMember.OnDeselect();
        selectedMember = null;
    }
    void Refresh()
    {
        foreach (var entity in GlobalManager.Instance.unlockedAllies)
        {
            PartyMemberUI found = memberUIs.Find(item => item.entity == entity);
            if (found == null)
            {
                var tmp = Instantiate(memberUIPrefab, memberUIAnchor);
                tmp.Set(entity);
                memberUIs.Add(tmp);

                found = tmp;
            }
            found.gameObject.SetActive(true);
        }
    }
}