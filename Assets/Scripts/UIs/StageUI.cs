using UnityEngine;
using MEC;
using UnityEngine;
using AnimatorCutscene;

public class StageUI : MonoBehaviour, ICutsceneCallbackReceiver
{
    Tab currentTab;
    [SerializeField] PartyTab partyTab;
    public bool isTabOpen => currentTab != null;
    private void Update()
    {
        if (InputManager.Instance.PartyTab.triggered) OpenTab(partyTab);
    }
    public void OpenTab(Tab tab)
    {
        if (currentTab == tab || tab == null)
        {
            CloseTab();
            return;
        }

        if (currentTab != null) currentTab.Close();
        currentTab = tab;
        currentTab.Open();
    }
    public void CloseTab()
    {
        if (currentTab != null) currentTab.Close();
        currentTab = null;
    }
    public void OnCutsceneEnd()
    {
        gameObject.SetActive(true);
    }

    public void OnCutsceneStart()
    {
        CloseTab();
        gameObject.SetActive(false);
    }
}