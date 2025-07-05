using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneSwitchButton : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] SceneTransition transition;
    Button button;
    private void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }
    void OnClick()
    {
        GlobalManager.Instance.SwitchScene(sceneName, transition);
    }
}