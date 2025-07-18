using UnityEngine;
using MEC;
using UnityEngine;
using AnimatorCutscene;

public class StageUI : MonoBehaviour, ICutsceneCallbackReceiver
{
    [SerializeField] Tab currentTab;
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