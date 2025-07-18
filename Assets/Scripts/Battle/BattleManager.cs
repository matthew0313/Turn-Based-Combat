using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;
using System;
using Player_Battle;
using AudioManagement;
using Unity.VisualScripting;

[RequireComponent(typeof(BattleManager_UI))]
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    public BattleManager() => Instance = this;
    public BattleManager_UI UI { get; private set; }

    public const int maxPlayerSide = 4, maxEnemySide = 3;

    [Header("Battle")]
    [SerializeField] Transform[] playerSidePos = new Transform[maxPlayerSide];
    [SerializeField] Transform[] enemySidePos = new Transform[maxEnemySide];
    [SerializeField] UnitTime battleStartTime;
    [SerializeField] float turnInterval = 0.5f;

    [Header("Aliments")]
    [SerializeField] Aliment_BurnData m_burnData;
    [SerializeField] Aliment_DefenseBuffData m_defenseBuffData;
    [SerializeField] Aliment_DefenseDebuffData m_defenseDebuffData;
    [SerializeField] Aliment_AttackBuffData m_attackBuffData;
    [SerializeField] Aliment_AttackDebuffData m_attackDebuffData;
    public Aliment_BurnData burnData => m_burnData;
    public Aliment_DefenseBuffData defenseBuffData => m_defenseBuffData;
    public Aliment_DefenseDebuffData defenseDebuffData => m_defenseDebuffData;
    public Aliment_AttackBuffData attackBuffData => m_attackBuffData;
    public Aliment_AttackDebuffData attackDebuffData => m_attackDebuffData;

    [Header("SFX")]
    [SerializeField] Sound cardSwitchSound;

    [Header("Fast Play")]
    [SerializeField] float fastPlaySpeed = 10.0f;

    [Header("Debug")]
    [SerializeField] bool debug = false;
    [SerializeField] BattleData debugBattleData;

    ControlMachine<float> timeScaleControl;
    ControlElement<float> fastPlayControl;

    private void Awake()
    {
        UI = GetComponent<BattleManager_UI>();
        timeScaleControl = new(value => Time.timeScale = value, 1.0f);
        fastPlayControl = new(() => fastPlaySpeed, 0, false);
    }
    private void Start()
    {
        if (!battleInitiated && debug)
        {
            InitiateBattle(debugBattleData);
        }
    }
    private void Update()
    {
        if (InputManager.Instance.BattleFastPlay.triggered) ToggleFastPlay();
    }

    readonly BattleEntity[] playerSide = new BattleEntity[maxPlayerSide], enemySide = new BattleEntity[maxEnemySide];
    CoroutineHandle progressBattle;
    bool battleInitiated = false;

    BattleData data;
    public void InitiateBattle(BattleData data)
    {
        this.data = data;

        battleInitiated = true;
        AudioManager.Instance.ChangeMusic(data.music);
        for(int i = 0; i < maxPlayerSide; i++)
        {
            if (data.playerSide[i] == null) continue;
            SetPlayerSide(i, data.playerSide[i]);
        }
        for(int i = 0; i < maxEnemySide; i++)
        {
            if (data.enemySide[i] == null) continue;
            SetEnemySide(i, data.enemySide[i]);
        }
        progressBattle = Timing.RunCoroutine(ProgressBattle());
    }
    public void SetPlayerSide(int index, BattleEntity entityPrefab)
    {
        BattleEntity entity = Instantiate(entityPrefab);
        entity.transform.SetParent(playerSidePos[index], false);
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localRotation = Quaternion.identity;
        entity.rotator.localScale = Vector3.one;
        playerSide[index] = entity;
        entity.Set(index, Alliance.Player);
    }
    public void SetEnemySide(int index, BattleEntity entityPrefab)
    {
        BattleEntity entity = Instantiate(entityPrefab);
        entity.transform.SetParent(enemySidePos[index], false);
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localRotation = Quaternion.identity;
        entity.rotator.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        enemySide[index] = entity;
        entity.Set(index, Alliance.Enemy);
    }
    IEnumerator<float> ProgressBattle()
    {
        yield return Timing.WaitForSeconds(battleStartTime);
        for(int i = 0; i < maxPlayerSide; i++)
        {
            if (playerSide[i] == null) continue;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(playerSide[i].OnBattleStart()));
        }
        for(int i = 0; i < maxEnemySide; i++)
        {
            if (enemySide[i] == null) continue;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemySide[i].OnBattleStart()));
        }
        while (true)
        {
            for(int i = 0; i < maxPlayerSide; i++)
            {
                if (playerSide[i] == null || playerSide[i].dead) continue;
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(playerSide[i].Turn()));
                OnTurnFinish();
                yield return Timing.WaitForSeconds(turnInterval);
            }
            for (int i = 0; i < maxEnemySide; i++)
            {
                if (enemySide[i] == null || enemySide[i].dead) continue;
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemySide[i].Turn()));
                OnTurnFinish();
                yield return Timing.WaitForSeconds(turnInterval);
            }
        }
    }
    void OnTurnFinish()
    {
        bool playerLost = true;
        for(int i = 0; i < maxPlayerSide; i++)
        {
            if (playerSide[i] != null && !playerSide[i].dead)
            {
                playerLost = false; break;
            }
        }
        if (playerLost)
        {
            Lost(); return;
        }

        bool playerWon = true;
        for (int i = 0; i < maxEnemySide; i++)
        {
            if (enemySide[i] != null && !enemySide[i].dead)
            {
                playerWon = false; break;
            }
        }
        if (playerWon)
        {
            Victory(); return;
        }
    }
    public IEnumerable<BattleEntity> GetEnemies(Alliance alliance)
    {
        if (alliance == Alliance.Enemy) foreach (var i in playerSide) if (i != null) yield return i;
        if (alliance == Alliance.Player) foreach(var i in enemySide) if(i != null) yield return i;
    }
    public IEnumerable<BattleEntity> GetAllies(Alliance alliance)
    {
        if (alliance == Alliance.Enemy) foreach (var i in enemySide) if (i != null) yield return i;
        if (alliance == Alliance.Player) foreach (var i in playerSide) if (i != null) yield return i;
    }
    public void RemoveEntity(BattleEntity entity)
    {
        if (entity == null) return;
        if(entity.alliance == Alliance.Player)
        {
            playerSide[entity.battlePosition] = null;
        }
        if(entity.alliance == Alliance.Enemy)
        {
            enemySide[entity.battlePosition] = null;
        }
    }
    public void AddAlly(Alliance alliance, BattleEntity entity)
    {
        if (GetAllies(alliance).Count() >= (alliance == Alliance.Player ? maxPlayerSide : maxEnemySide)) return;
        if(alliance == Alliance.Enemy)
        {
            for(int i = 0; i < maxEnemySide; i++)
            {
                if (enemySide[i] == null)
                {
                    SetEnemySide(i, entity); return;
                }
            }
        }
        else
        {
            for (int i = 0; i < maxPlayerSide; i++)
            {
                if (playerSide[i] == null)
                {
                    SetPlayerSide(i, entity); return;
                }
            }
        }
    }

    public const int cardCount = 3;
    int selectedCardIndex = 0;
    readonly PlayerBattleAction[] battleActions = new PlayerBattleAction[cardCount];
    public void SelectPlayerAction(BE_Player actor, Action<PlayerBattleAction> onFinish)
    {
        Timing.RunCoroutine(SetCards(actor, onFinish));
    }
    IEnumerator<float> SetCards(BE_Player actor, Action<PlayerBattleAction> onFinish)
    {
        if(GlobalManager.Instance.unlockedActions.Count <= 0)
        {
            UI.ShowText("액션이 없어 턴을 스킵합니다.");
            yield return Timing.WaitForSeconds(1.0f);
            onFinish?.Invoke(null); yield break;
        }
        UI.ShowCards();
        for (int i = 0; i < cardCount; i++)
        {
            float rand = UnityEngine.Random.Range(0, 100.0f);
            battleActions[i] = GlobalManager.Instance.unlockedActions.SelectRandom(item => true);
            UI.SetCard(i, battleActions[i]);
        }
        selectedCardIndex = 0;
        UI.SelectCard(0);
        Timing.RunCoroutine(SelectingCard(actor, onFinish));
    }
    IEnumerator<float> SelectingCard(BE_Player actor, Action<PlayerBattleAction> onFinish)
    {
        while (true)
        {
            yield return Timing.WaitForOneFrame;
            if (InputManager.Instance.SelectRight.triggered && selectedCardIndex < cardCount - 1)
            {
                selectedCardIndex++;
                AudioManager.Instance.PlaySound(cardSwitchSound);
                UI.SelectCard(selectedCardIndex);
            }
            if (InputManager.Instance.SelectLeft.triggered && selectedCardIndex > 0)
            {
                selectedCardIndex--;
                AudioManager.Instance.PlaySound(cardSwitchSound);
                UI.SelectCard(selectedCardIndex);
            }
            if (InputManager.Instance.Select.triggered && selectedCardIndex != -1)
            {
                AudioManager.Instance.PlaySound("Confirm");
                Timing.RunCoroutine(SelectingTarget(actor, battleActions[selectedCardIndex], onFinish)); yield break;
            }
            if (InputManager.Instance.PassTurn.triggered)
            {
                AudioManager.Instance.PlaySound("Confirm");
                UI.HideCards();
                UI.DeselectCard();
                UI.ShowText("플레이어가 턴을 스킵합니다.");
                yield return Timing.WaitForSeconds(1.0f);
                onFinish?.Invoke(null);
                yield break;
            }
        }
    }
    IEnumerator<float> SelectingTarget(BE_Player actor, PlayerBattleAction selectedAction, Action<PlayerBattleAction> onFinish)
    {
        List<BattleEntity> targets = new();
        if (!selectedAction.CheckRequirements(actor, out string falseReason))
        {
            UI.ShowText(falseReason);
            AudioManager.Instance.PlaySound("Invalid");
            Timing.RunCoroutine(SelectingCard(actor, onFinish)); yield break;
        }
        switch (selectedAction.targetSelection)
        {
            case TargetSelection.Self:
            case TargetSelection.AllEnemy:
            case TargetSelection.AllAlly:
            case TargetSelection.AllAllyExceptSelf:
            case TargetSelection.All:
                switch (selectedAction.targetSelection)
                {
                    case TargetSelection.Self: targets.Add(actor); break;
                    case TargetSelection.AllEnemy: foreach(var i in GetEnemies(Alliance.Player)) if(!i.dead) targets.Add(i); break;
                    case TargetSelection.AllAlly:  foreach(var i in GetAllies(Alliance.Player)) if (!i.dead) targets.Add(i); break;
                    case TargetSelection.AllAllyExceptSelf: foreach (var i in GetAllies(Alliance.Player)) if (!i.dead && i != actor) targets.Add(i); break;
                    case TargetSelection.All:
                        foreach (var i in GetEnemies(Alliance.Player)) if (!i.dead) targets.Add(i);
                        foreach (var i in GetAllies(Alliance.Player)) if (!i.dead) targets.Add(i);
                        break;
                }
                if (targets.Count == 0)
                {
                    UI.ShowText("타깃이 없습니다.");
                    AudioManager.Instance.PlaySound("Invalid");
                    Timing.RunCoroutine(SelectingCard(actor, onFinish)); yield break;
                }
                foreach (var i in targets) UI.ShowMark(i);
                while (true)
                {
                    yield return Timing.WaitForOneFrame;
                    if (InputManager.Instance.Select.triggered)
                    {
                        UI.HideMarks();
                        UI.HideCards();
                        UI.DeselectCard();
                        AudioManager.Instance.PlaySound("Confirm");
                        onFinish?.Invoke(selectedAction); yield break;
                    }
                    if (InputManager.Instance.Cancel.triggered)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Cancel");
                        Timing.RunCoroutine(SelectingCard(actor, onFinish)); yield break;
                    }
                }
            case TargetSelection.SingleEnemy:
            case TargetSelection.SingleAlly:
            case TargetSelection.SingleAllyExceptSelf:
                switch (selectedAction.targetSelection)
                {
                    case TargetSelection.SingleEnemy: foreach (var i in GetEnemies(Alliance.Player)) if (!i.dead) targets.Add(i); break;
                    case TargetSelection.SingleAlly: foreach (var i in GetAllies(Alliance.Player)) if (!i.dead) targets.Add(i); break;
                    case TargetSelection.SingleAllyExceptSelf: foreach (var i in GetAllies(Alliance.Player)) if (!i.dead && i != actor) targets.Add(i); break;
                }
                if(targets.Count == 0)
                {
                    UI.ShowText("타깃이 없습니다.");
                    AudioManager.Instance.PlaySound("Invalid");
                    Timing.RunCoroutine(SelectingCard(actor, onFinish)); yield break;
                }
                int selected = 0;
                UI.ShowMark(targets[selected]);
                while (true)
                {
                    yield return Timing.WaitForOneFrame;
                    if(InputManager.Instance.SelectUp.triggered && selected < targets.Count - 1)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Switch");
                        UI.ShowMark(targets[++selected]);
                    }
                    if (InputManager.Instance.SelectDown.triggered && selected > 0)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Switch");
                        UI.ShowMark(targets[--selected]);
                    }
                    if (InputManager.Instance.Select.triggered)
                    {
                        UI.HideMarks();
                        UI.HideCards();
                        UI.DeselectCard();
                        selectedAction.target = targets[selected];
                        AudioManager.Instance.PlaySound("Confirm");
                        onFinish?.Invoke(selectedAction); yield break;
                    }
                    if (InputManager.Instance.Cancel.triggered)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Cancel");
                        Timing.RunCoroutine(SelectingCard(actor, onFinish)); yield break;
                    }
                }
        }
    }
    public bool battleEnded { get; private set; } = false;
    public bool isFastPlaying { get; private set; } = false;
    public void ToggleFastPlay()
    {
        if (battleEnded) return;

        isFastPlaying = !isFastPlaying;
        Time.timeScale = isFastPlaying ? fastPlaySpeed : 1.0f;
        UI.ToggleFastPlay(isFastPlaying);
    }
    void OnBattleEnd()
    {
        if (isFastPlaying) ToggleFastPlay();

        battleEnded = true;
        Timing.KillCoroutines(progressBattle);
        AudioManager.Instance.ChangeMusic(null);
    }
    void Victory()
    {
        OnBattleEnd();
        data.onVictory?.Invoke();
    }
    void Lost()
    {
        OnBattleEnd();
        data.onLoss?.Invoke();
    }
    private void OnDestroy()
    {
        Timing.KillCoroutines(progressBattle);
    }
}
[System.Serializable]
public enum Alliance
{
    Player,
    Enemy
}
[System.Serializable]
public class BattleData
{
    public BattleEntity[] playerSide = new BattleEntity[BattleManager.maxPlayerSide];
    public BattleEntity[] enemySide = new BattleEntity[BattleManager.maxEnemySide];
    public Sound music;

    public Action onVictory, onLoss;
}