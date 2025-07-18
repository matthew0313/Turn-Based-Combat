using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<InputManager>("InputManager"));
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    PlayerInputActions inputActions;
    //All
    public InputAction SelectRight => inputActions.All.SelectRight;
    public InputAction SelectLeft => inputActions.All.SelectLeft;
    public InputAction SelectUp => inputActions.All.SelectUp;
    public InputAction SelectDown => inputActions.All.SelectDown;
    public InputAction Select => inputActions.All.Select;
    public InputAction Cancel => inputActions.All.Cancel;

    //Field
    public InputAction Move => inputActions.Field.Move;
    public InputAction Interact => inputActions.Field.Interact;
    public InputAction CutsceneFastPlay => inputActions.Field.CutsceneFastPlay;
    public InputAction PassDialogue => inputActions.Field.PassDialogue;
    public InputAction PartyTab => inputActions.Field.PartyTab;

    //Battle
    public InputAction PassTurn => inputActions.Battle.PassTurn;
    public InputAction BattleFastPlay => inputActions.Battle.FastPlay;
    private void OnEnable()
    {
        if(inputActions == null) inputActions = new();
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}