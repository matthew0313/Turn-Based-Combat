using System.Collections.Generic;
using UnityEngine;
using MEC;
using Player_Battle;
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
    public PlayerStats playerStats;

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
    public void EnterStage(int stageIndex, SceneTransition transition, Action onSceneLoad = null)
    {
        SwitchScene($"Stage{stageIndex}", transition, onSceneLoad);
    }
    public SaveData SaveGame()
    {
        if (StageManager.Instance == null) return null;
        SaveData data = new();
        data.playerStats = playerStats;
        StageManager.Instance.Save(data);
        return data;
    }
    public void LoadGame(SaveData data, SceneTransition transition)
    {
        playerStats = data.playerStats;
        EnterStage(data.stageIndex, transition, () =>
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