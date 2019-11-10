using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//All UI Transition is handled here. Goin from Home to Game and Result.
public class UIManager : MonoBehaviour
{
    public enum eMenuState
    {
        NONE = -1,
        HOME,
        GAME,
        RESULTS
    }
    public enum eGameState
    {
        NONE = -1,
        PAUSE,
        WIN,
        LOSE
    };

    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    private bool loaded = false;

    private eMenuState menuState;
    //All menus that will be displayed.
    private PanelBase panelMenuHome, panelGameHud, panelGameResult;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        loaded = false;
        menuState = eMenuState.NONE;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string level)
    {
        //Making sure we focus onloading level and stop all other things
        StopAllCoroutines();
        StartCoroutine(DelayedLoadLevel(level));
    }
   
    public void OnMenuUITransition(eMenuState newState)
    {
        if (menuState == newState)
            return;

        TransitionMenu(menuState, newState);
        menuState = newState;
    }
    private void TransitionMenu(eMenuState prevState, eMenuState newState)
    {
        switch (newState)
        {
            case eMenuState.HOME:
                panelMenuHome.Enable(true);
                break;
            case eMenuState.GAME:
                panelGameHud.Enable(true);
                break;
            case eMenuState.RESULTS:
                panelGameResult.Enable(true);
                break;
        }
    }

    private PanelBase LoadPanel(string path, Transform parent)
    {
        PanelBase panel = null;
        GameObject baseObj = (GameObject)Resources.Load(path, typeof(GameObject));
        GameObject newObj = Instantiate<GameObject>(baseObj, parent);

        panel = newObj.GetComponent<PanelBase>();
        if (panel != null) panel.Initialize(true);

        return panel;
    }

    private IEnumerator DelayedLoadLevel(string level)
    {
        WaitForEndOfFrame endofframe = new WaitForEndOfFrame();
        yield return endofframe;
        //Unload all panels
        yield return StartCoroutine("DelayedUnloadPanels");

        yield return endofframe;
        //Load respective level async
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        yield return new WaitUntil(() => operation.isDone);
        yield return new WaitForEndOfFrame();
        //Load Game or Menu respectively
        switch (level)
        {
            case "Menu":
                yield return StartCoroutine("DelayedLoadMenu");
                break;
            case "Game":
                yield return StartCoroutine("DelayedLoadGame");
                break;
        }
    }
    private IEnumerator DelayedUnloadPanels()
    {
        //All the cleaning is done here
        GameObject obj = null;
        GameObject parent = GameObject.FindGameObjectWithTag("Canvas");
        if (parent != null)
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                obj = parent.transform.GetChild(i).gameObject;
            }

        yield return null;

        Resources.UnloadUnusedAssets();

        yield return null;

        System.GC.Collect();
    }
    private IEnumerator DelayedLoadMenu()
    {
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        Transform parent = GameObject.FindGameObjectWithTag("Canvas").transform;
        yield return endOfFrame;
        panelMenuHome = LoadPanel("UI/Menu/PanelMenuHome", parent);
        loaded = true;
        yield return endOfFrame;
        OnMenuUITransition(eMenuState.HOME);
    }
    private IEnumerator DelayedLoadGame()
    {
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        Transform parent = GameObject.FindGameObjectWithTag("Canvas").transform;
        yield return endOfFrame;
        panelGameHud = LoadPanel("UI/Game/PanelGameHUD", parent);
        panelGameResult = LoadPanel("UI/Game/PanelGameResult", parent);
        loaded = true;
        yield return endOfFrame;
        OnMenuUITransition(eMenuState.GAME);
    }
}
