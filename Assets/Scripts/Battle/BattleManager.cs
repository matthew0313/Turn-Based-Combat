using System.Collections.Generic;
using UnityEngine;
using MEC;

[RequireComponent(typeof(BattleManager_UI))]
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    public BattleManager() => Instance = this;
    public BattleManager_UI UI { get; private set; }

    public const int maxParticipants = 4;

    [SerializeField] Transform[] playerSidePos = new Transform[maxParticipants];
    [SerializeField] Transform[] enemySidePos = new Transform[maxParticipants];
    [SerializeField] UnitTime battleStartTime;
    [SerializeField] float turnInterval = 0.5f;

    [Header("Debug")]
    [SerializeField] bool debug = false;
    [SerializeField] List<BattleEntity> debugBattlePlayerSide, debugBattleEnemySide;

    private void Awake()
    {
        UI = GetComponent<BattleManager_UI>();
        if (debug) InitiateBattle(debugBattlePlayerSide, debugBattleEnemySide);
    }

    public readonly BattleEntity[] playerSide = new BattleEntity[maxParticipants], enemySide = new BattleEntity[maxParticipants];
    CoroutineHandle progressBattle;
    public void InitiateBattle(List<BattleEntity> playerSide, List<BattleEntity> enemySide)
    {
        for(int i = 0; i < Mathf.Min(playerSide.Count, maxParticipants); i++)
        {
            var tmp = Instantiate(playerSide[i]);
            SetPlayerSide(i, tmp);
        }
        for(int i = 0; i < Mathf.Min(enemySide.Count, maxParticipants); i++)
        {
            var tmp = Instantiate(enemySide[i]);
            SetEnemySide(i, tmp);
        }
        progressBattle = Timing.RunCoroutine(ProgressBattle());
    }
    public void SetPlayerSide(int index, BattleEntity entity)
    {
        entity.transform.SetParent(playerSidePos[index], false);
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localRotation = Quaternion.identity;
        entity.rotator.localScale = Vector3.one;
        playerSide[index] = entity;
        entity.Set(index, Alliance.Player);
    }
    public void SetEnemySide(int index, BattleEntity entity)
    {
        entity.transform.SetParent(enemySidePos[index], false);
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localRotation = Quaternion.identity;
        entity.rotator.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        enemySide[index] = entity;
        entity.Set(index, Alliance.Enemy);
    }

    bool turnWaiting = false;
    BattleEntity currentTurnEntity = null;
    IEnumerator<float> ProgressBattle()
    {
        yield return Timing.WaitForSeconds(battleStartTime);
        for(int i = 0; i < maxParticipants; i++)
        {
            if (playerSide[i] == null) continue;

            bool finished = false;
            playerSide[i].OnBattleStart(() => finished = true);
            while (!finished) yield return Timing.WaitForOneFrame;
        }
        for(int i = 0; i < maxParticipants; i++)
        {
            if (enemySide[i] == null) continue;

            bool finished = false;
            playerSide[i].OnBattleStart(() => finished = true);
            while (!finished) yield return Timing.WaitForOneFrame;
        }
        while (true)
        {
            for(int i = 0; i < maxParticipants; i++)
            {
                if (playerSide[i] == null || playerSide[i].dead) continue;
                currentTurnEntity = playerSide[i];
                turnWaiting = true;
                playerSide[i].StartTurn();
                while (turnWaiting) yield return Timing.WaitForOneFrame;
                yield return Timing.WaitForSeconds(turnInterval);
            }
            for (int i = 0; i < maxParticipants; i++)
            {
                if (enemySide[i] == null || enemySide[i].dead) continue;
                currentTurnEntity = enemySide[i];
                turnWaiting = true;
                enemySide[i].StartTurn();
                while (turnWaiting) yield return Timing.WaitForOneFrame;
                yield return Timing.WaitForSeconds(turnInterval);
            }
        }
    }
    public void FinishTurn()
    {
        currentTurnEntity = null;

        bool playerLost = true;
        for(int i = 0; i < maxParticipants; i++)
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
        for (int i = 0; i < maxParticipants; i++)
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

        turnWaiting = false;
    }
    public IEnumerable<BattleEntity> GetEnemies(Alliance alliance) => alliance == Alliance.Enemy ? playerSide : enemySide;
    public IEnumerable<BattleEntity> GetAllies(Alliance alliance) => alliance == Alliance.Enemy ? enemySide : playerSide;
    void Victory()
    {
        Timing.KillCoroutines(progressBattle);
    }
    void Lost()
    {
        Timing.KillCoroutines(progressBattle);
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
