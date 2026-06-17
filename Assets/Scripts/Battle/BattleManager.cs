using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;
using System;
using AudioManagement;

[RequireComponent(typeof(BattleManager_UI))]
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    public BattleManager() => Instance = this;
    public BattleManager_UI UI { get; private set; }

    public const int maxPlayerSide = 3;

    [Header("Battle")]
    [SerializeField] Transform[] playerSidePos = new Transform[maxPlayerSide];
    [SerializeField] Transform enemyPos;
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


    private void Awake()
    {
        UI = GetComponent<BattleManager_UI>();
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

    readonly BattleEntity[] playerSide = new BattleEntity[maxPlayerSide];
    BattleEntity enemy;
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
        SetEnemy(data.enemy);
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
    public void SetEnemy(BattleEntity entityPrefab)
    {
        BattleEntity entity = Instantiate(entityPrefab);
        entity.transform.SetParent(enemyPos, false);
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localRotation = Quaternion.identity;
        entity.rotator.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        enemy = entity;
        entity.Set(0, Alliance.Enemy);
    }
    IEnumerator<float> ProgressBattle()
    {
        yield return Timing.WaitForSeconds(battleStartTime);
        for(int i = 0; i < maxPlayerSide; i++)
        {
            if (playerSide[i] == null) continue;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(playerSide[i].OnBattleStart()));
        }
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy.OnBattleStart()));
        while (true)
        {
            for(int i = 0; i < maxPlayerSide; i++)
            {
                if (playerSide[i] == null || playerSide[i].dead) continue;
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(playerSide[i].OnTurnStart()));
                CheckDefeat();
                yield return Timing.WaitForOneFrame;
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy.OnTurnStart()));
            CheckDefeat();
            yield return Timing.WaitForOneFrame;

            bool finished = false;
            BattleAction selectedAction = null;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(SetCards(action =>
            {
                finished = true;
                selectedAction = action;
            })));
            while (!finished) yield return Timing.WaitForOneFrame;
            if(selectedAction != null)
            {
                UI.ShowText($"플레이어가 {selectedAction.actionName}을 사용합니다.");
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(selectedAction.Act(Alliance.Player)));
            }
            CheckDefeat();
            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForSeconds(turnInterval);

            for (int i = 0; i < maxPlayerSide; i++)
            {
                if (playerSide[i] == null || playerSide[i].dead) continue;
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(playerSide[i].OnTurnStart()));
                CheckDefeat();
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy.OnTurnStart()));
            CheckDefeat();
            yield return Timing.WaitForOneFrame;

            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy.OnEnemyTurn()));
            CheckDefeat();
            yield return Timing.WaitForOneFrame;
        }
    }
    void CheckDefeat()
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
        if (enemy != null && !enemy.dead) playerWon = false;
        if (playerWon)
        {
            Victory(); return;
        }
    }
    public IEnumerable<BattleEntity> GetEnemies(Alliance alliance)
    {
        if (alliance == Alliance.Enemy) foreach (var i in playerSide) if (i != null) yield return i;
        if (alliance == Alliance.Player) yield return enemy;
    }
    public IEnumerable<BattleEntity> GetAllies(Alliance alliance)
    {
        if (alliance == Alliance.Enemy) yield return enemy;
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
            enemy = null;
        }
    }
    public void AddAlly(BattleEntity entity)
    {
        if (GetAllies(Alliance.Player).Count() >= maxPlayerSide) return;
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
    readonly BattleAction[] battleActions = new BattleAction[cardCount];
    public void SelectPlayerAction(Action<BattleAction> onFinish)
    {
        Timing.RunCoroutine(SetCards(onFinish));
    }
    void ShuffleAction(int index)
    {
        if (index < 0 || index >= cardCount) return;
        battleActions[index] = GlobalManager.Instance.unlockedActions.SelectRandom(item => true);
        UI.SetCard(index, battleActions[index]);
    }
    IEnumerator<float> SetCards(Action<BattleAction> onFinish)
    {
        if (GlobalManager.Instance.unlockedActions.Count <= 0)
        {
            UI.ShowText("액션이 없어 턴을 스킵합니다.");
            yield return Timing.WaitForSeconds(1.0f);
            onFinish?.Invoke(null); yield break;
        }
        for (int i = 0; i < cardCount; i++)
        {
            if (battleActions[i] == null)
            {
                ShuffleAction(i);
            }
        }
        UI.ShowCards();
        UI.SelectCard(0);
        selectedCardIndex = 0;
        Timing.RunCoroutine(SelectingCard(onFinish));
    }
    IEnumerator<float> SelectingCard(Action<BattleAction> onFinish)
    {
        while (true)
        {
            if(Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.Q))
            {
                Victory(); yield break;
            }
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
                Timing.RunCoroutine(SelectingTarget(onFinish)); yield break;
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
    IEnumerator<float> SelectingTarget(Action<BattleAction> onFinish)
    {
        List<BattleEntity> targets = new();
        BattleAction selectedAction = battleActions[selectedCardIndex];
        if (!selectedAction.CheckRequirements(out string falseReason))
        {
            UI.ShowText(falseReason);
            AudioManager.Instance.PlaySound("Invalid");
            Timing.RunCoroutine(SelectingCard(onFinish)); yield break;
        }
        switch (selectedAction.targetSelection)
        {
            case TargetSelection.Enemy:
                UI.ShowMark(enemy);
                while (true)
                {
                    yield return Timing.WaitForOneFrame;
                    if (InputManager.Instance.Select.triggered)
                    {
                        UI.HideMarks();
                        UI.HideCards();
                        UI.DeselectCard();
                        selectedAction.target = enemy;
                        AudioManager.Instance.PlaySound("Confirm");
                        battleActions[selectedCardIndex] = null;
                        onFinish?.Invoke(selectedAction); yield break;
                    }
                    if (InputManager.Instance.Cancel.triggered)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Cancel");
                        Timing.RunCoroutine(SelectingCard(onFinish)); yield break;
                    }
                }
            case TargetSelection.AllAlly:
            case TargetSelection.All:
                switch (selectedAction.targetSelection)
                {
                    case TargetSelection.AllAlly:  foreach(var i in GetAllies(Alliance.Player)) if (!i.dead) targets.Add(i); break;
                    case TargetSelection.All:
                        foreach (var i in GetEnemies(Alliance.Player)) if (!i.dead) targets.Add(i);
                        foreach (var i in GetAllies(Alliance.Player)) if (!i.dead) targets.Add(i);
                        break;
                }
                if (targets.Count == 0)
                {
                    UI.ShowText("타깃이 없습니다.");
                    AudioManager.Instance.PlaySound("Invalid");
                    Timing.RunCoroutine(SelectingCard(onFinish)); yield break;
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
                        battleActions[selectedCardIndex] = null;
                        onFinish?.Invoke(selectedAction); yield break;
                    }
                    if (InputManager.Instance.Cancel.triggered)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Cancel");
                        Timing.RunCoroutine(SelectingCard(onFinish)); yield break;
                    }
                }
            case TargetSelection.SingleAlly:
                switch (selectedAction.targetSelection)
                {
                    case TargetSelection.SingleAlly: foreach (var i in GetAllies(Alliance.Player)) if (!i.dead) targets.Add(i); break;
                }
                if(targets.Count == 0)
                {
                    UI.ShowText("타깃이 없습니다.");
                    AudioManager.Instance.PlaySound("Invalid");
                    Timing.RunCoroutine(SelectingCard(onFinish)); yield break;
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
                        battleActions[selectedCardIndex] = null;
                        onFinish?.Invoke(selectedAction); yield break;
                    }
                    if (InputManager.Instance.Cancel.triggered)
                    {
                        UI.HideMarks();
                        AudioManager.Instance.PlaySound("Cancel");
                        Timing.RunCoroutine(SelectingCard(onFinish)); yield break;
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
        GlobalManager.Instance.SwitchScene("GameOver", SceneTransition.CircleOut);
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
    public BattleEntity enemy;
    public Sound music;

    public Action onVictory;
    public Action onLoss;
}