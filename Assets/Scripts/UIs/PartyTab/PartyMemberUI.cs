using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Image entityIcon;
    [SerializeField] TMP_Text entityText;
    public BattleEntity entity { get; private set; }
    public void Set(BattleEntity entity)
    {
        this.entity = entity;
        entityIcon.sprite = entity.entityIcon;
        entityText.text = entity.entityName;
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
}