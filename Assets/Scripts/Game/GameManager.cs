using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum eGameState
    {
        NONE = -1,
        INIT,
        MENU,
        GAME,
        MAX
    }
    private static GameManager instance;
    private static PersistentData persistentData;
    private static PlayerData playerData;

    public static GameManager Instance { get { return instance; } }
    public static PersistentData PersistentData { get { return persistentData; } }
    public static PlayerData PlayerData { get { return playerData; } }

    private eGameState prevState, state;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
        prevState = state = eGameState.NONE;
    }
    private void Start()
    {
        //This is the beginning of all.
        //The flow begins from here.
        AttemptChangeState(eGameState.INIT);
    }
    public void AttemptChangeState(eGameState newState, bool force = false)
    {
        if (state == newState && !force) return;

        prevState = state;
        state = newState;

        switch (newState)
        {
            case eGameState.INIT:
                InitGame();
                break;
            case eGameState.MENU:
                LoadMenu();
                break;
            case eGameState.GAME:
                LoadGame();
                break;
        }
    }
    private IEnumerator DelayedLoadPlayerData()
    {
        playerData = new PlayerData();
        yield return null;
    }
    private IEnumerator DelayedLoadPersistentData()
    {
        ResourceRequest req = null;

        // Init persistent data
        persistentData = new PersistentData();

        // Load GameConfiguration
        req = Resources.LoadAsync<GameConfiguration>(GameConfiguration.AssetPath);
        yield return req;
        persistentData.GameConfiguration = req.asset as GameConfiguration;

        // Load GameBalance
        req = Resources.LoadAsync<GameBalance>(GameBalance.AssetPath);
        yield return req;
        persistentData.GameBalance = req.asset as GameBalance;

        yield return null;

    }
    private IEnumerator DelayedLoad()
    {
        yield return StartCoroutine("DelayedLoadPersistentData");
        yield return StartCoroutine("DelayedLoadPlayerData");
        AttemptChangeState(eGameState.MENU);
    }
    private void InitGame()
    {
        StartCoroutine("DelayedLoad");
    }
    private void LoadMenu()
    {
        UIManager.Instance.LoadLevel("Menu");
    }
    private void LoadGame()
    {
        UIManager.Instance.LoadLevel("Game");
    }
}
