using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GlobalManager : MonoBehaviour
{
    static GlobalManager instance;
    public static GlobalManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Instantiate(Resources.Load<GlobalManager>("GlobalManager"));
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    
    public BattleEntity[] party = new BattleEntity[BattleManager.maxPlayerSide-1];
    public List<BattleAction> unlockedActions = new();
    public List<BattleEntity> unlockedAllies = new();

    [Header("Scene Switching")]
    [SerializeField] RenderTexture targetTexture;
    [SerializeField] RawImage circleOutImage;
    readonly int intensityID = Shader.PropertyToID("_Intensity");
    public void SwitchScene(string sceneName, SceneTransition transition, Action onSceneLoad = null)
    {
        UnityAction<Scene, LoadSceneMode> onFinish = null;
        onFinish += (scene, mode) =>
        {
            onSceneLoad?.Invoke();
            SceneManager.sceneLoaded -= onFinish;
        };
        SceneManager.sceneLoaded += onFinish;

        if (transition  == SceneTransition.None) SceneManager.LoadScene(sceneName);
        if (transition == SceneTransition.CircleOut)
        {
            Camera.main.targetTexture = targetTexture;
            Camera.main.Render();
            Camera.main.targetTexture = null;
            circleOutImage.material.DOKill();
            circleOutImage.gameObject.SetActive(true);
            circleOutImage.material.SetFloat(intensityID, 0.0f);
            SceneManager.LoadScene(sceneName);
            circleOutImage.material.DOFloat(10.0f, intensityID, 3.0f).OnComplete(() => circleOutImage.gameObject.SetActive(false)).SetEase(Ease.InCirc).SetUpdate(true);
        }
    }
    public void EnterBattle(BattleData data, SceneTransition transition, Action onSceneLoad = null)
    {
        SwitchScene("BattleScene", transition, () =>
        {
            onSceneLoad?.Invoke();
            BattleManager.Instance.InitiateBattle(data);
        });
    }
    public event Action onPartyChange;
    public event Action onAllyUnlock;
    public void ChangePartyMember(int partySlotIndex, BattleEntity entity)
    {
        if (partySlotIndex < 0 || partySlotIndex >= party.Length) return;
        for(int i = 0; i < party.Length; i++)
        {
            if (party[i] == entity) party[i] = null;
        }
        party[partySlotIndex] = entity;
        onPartyChange?.Invoke();
    }
    public void UnlockAlly(BattleEntity entity)
    {
        if (entity == null || unlockedAllies.Contains(entity)) return;
        unlockedAllies.Add(entity);
        onAllyUnlock?.Invoke();
    }
    public SaveData SaveGame()
    {
        if (StageManager.Instance == null) return null;
        SaveData data = new();
        data.unlockedActions = unlockedActions;
        data.unlockedAllies = unlockedAllies;
        data.unlockedActions = unlockedActions;
        StageManager.Instance.Save(data);
        return data;
    }
    public void LoadGame(SaveData data, SceneTransition transition)
    {
        unlockedActions = data.unlockedActions;
        unlockedAllies = data.unlockedAllies;
        unlockedActions = data.unlockedActions;
        SwitchScene(data.stageName, transition, () =>
        {
            StageManager.Instance.Load(data);
        });
    }
}
[System.Serializable]
public enum SceneTransition
{
    None,
    CircleOut
}
#if UNITY_EDITOR
[CustomEditor(typeof(GlobalManager))]
public class GlobalManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Test"))
        {
            (target as GlobalManager).SwitchScene("Victory", SceneTransition.CircleOut);
        }
    }
}
#endif