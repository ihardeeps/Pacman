using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum eState
    {
        NONE,
        INPROGRESS,
        TARGET_ACHIEVED,
        WIN,
        LOSE
    }
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }
    private Node[,] mainBoard;
    private int GridSizeX, GridSizeY;
    public static System.Action<UnitController.eDirection> OnPlayerInputEvent;
    private List<Node> allNodes;
    private Node startNode, endNode;
    private UnitController playerUnit;
    private eState state;
    public eState State { get { return state; } }
    public PlayerUnit Player { get { return playerUnit as PlayerUnit; } }

    private void Awake()
    {
        GameManager.PlayerData.Reset();
        OnPlayerInputEvent += PlayerInput;
        instance = this;
        GridSizeX = GameManager.PersistentData.GameConfiguration.GridSizeX;
        GridSizeY = GameManager.PersistentData.GameConfiguration.GridSizeY;
        LoadGamePlay();

        CreateBoard();
        SetStart();
        SetEnd();
        FillRandomObstacles();
        SetEnemies();
        FillCoins();
        SetPlayer();
    }

    private void LoadGamePlay()
    {
        GameObject gameCanvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject gamePlayRef = Resources.Load<GameObject>("UI/Game/GamePlay");
        Instantiate<GameObject>(gamePlayRef, gameCanvas.transform);
    }
    private void CreateBoard()
    {
        allNodes = new List<Node>();
        GameObject gameArea = GameObject.FindGameObjectWithTag("GameArea");
        GameObject container = GameObject.FindGameObjectWithTag("TilesContainer");
        RectTransform gameAreaTrans = gameArea.GetComponent<RectTransform>();
        GameObject tileRef = Resources.Load<GameObject>("UI/Game/Tile");
        float tileW = gameAreaTrans.sizeDelta.x / GridSizeX, tileH = gameAreaTrans.sizeDelta.x / GridSizeY;
        float tilePosX = -gameAreaTrans.sizeDelta.x * 0.5f + tileW * 0.5f, tilePosY = -gameAreaTrans.sizeDelta.x * 0.5f + tileH * 0.5f;
        mainBoard = new Node[GridSizeX, GridSizeY];
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                Node node = new Node();
                node.X = i;
                node.Y = j;
                GameObject tile = Instantiate<GameObject>(tileRef, container.transform);
                node.tileRef = tile.GetComponent<Tile>();
                node.tileRef.rectTransform.sizeDelta = new Vector2(tileW, tileH);
                
                node.tileRef.rectTransform.localPosition = new Vector3(tilePosX, tilePosY, 0);
                tilePosY += tileH;
                mainBoard[i, j] = node;

                if (i > 0 && j < GridSizeY)
                {
                    mainBoard[i - 1, j].CreateConnection(UnitController.eDirection.RIGHT, node);
                    node.CreateConnection(UnitController.eDirection.LEFT, mainBoard[i - 1, j]);
                }
                if (i < GridSizeX && j > 0)
                {
                    mainBoard[i, j - 1].CreateConnection(UnitController.eDirection.UP, node);
                    node.CreateConnection(UnitController.eDirection.DOWN, mainBoard[i, j - 1]);

                }
                allNodes.Add(node);
            }
            tilePosX += tileW;
            tilePosY = -gameAreaTrans.sizeDelta.x * 0.5f + tileH * 0.5f;
        }
    }
    private void SetStart()
    {
        //for now and simplicity, I have just taken the edge nodes as start or end.... this can be changed however we'd like.
        int indexX, indexY;
        indexX = Random.Range(0, GridSizeX);
        if (indexX > 0)
            indexY = 0;
        else
            indexY = Random.Range(0, GridSizeY);
        Node node = mainBoard[indexX, indexY];

        startNode = node;
        node.SetStart();
    }
    private void SetEnd()
    {
        int indexX, indexY;
        indexY = Random.Range(0, GridSizeY);
        if (indexY == GridSizeY - 1)
            indexX = Random.Range(0, GridSizeX);
        else
            indexX = GridSizeX - 1;
        Node node = mainBoard[indexX, indexY];
        endNode = node;
        node.SetEnd(true);
    }
    private void FillRandomObstacles()
    {
        int obstacleCount = Random.Range(1, 7);
        int count = 0, indexX = (int)((float)GridSizeX * 0.5f), indexY = (int)((float)GridSizeY * 0.5f);
        Node node = mainBoard[indexX, indexY];
        while (count < obstacleCount)
        {
            node = mainBoard[indexX, indexY];
            node.SetBlocker();
            node = node.GetNieghbors();
            indexX = node.X;
            indexY = node.Y;
            count++;
        }
    }
    private void FillCoins()
    {
        GameObject container = GameObject.FindGameObjectWithTag("CoinsContainer");
        int coinsToCollect = 0;
        GameObject coinRef = Resources.Load<GameObject>("UI/Game/Coin");
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                if (!mainBoard[i, j].IsBlocked && !mainBoard[i, j].IsStart && !mainBoard[i, j].IsEnd && !mainBoard[i, j].IsEnemy)
                {
                    Collectible coin = Instantiate<GameObject>(coinRef, container.transform).GetComponent<Collectible>();
                    mainBoard[i, j].SetCollectible(coin);
                    coinsToCollect += coin.value;
                }
            }
        }
        GameManager.PlayerData.SetObjective(coinsToCollect);
    }
    private void SetPlayer()
    {
        GameObject container = GameObject.FindGameObjectWithTag("UnitsContainer");
        GameObject playerRef = Resources.Load<GameObject>("UI/Game/Player/Player");
        playerUnit = Instantiate<GameObject>(playerRef, container.transform).GetComponent<UnitController>();
        playerUnit.Init(mainBoard, startNode.X, startNode.Y);
    }
    private void SetEnemies()
    {
        //Load Active enemy
        GameObject container = GameObject.FindGameObjectWithTag("UnitsContainer");
        //This can be made better for n number of enemies. this just for 3 different enemeis for now.
        GameObject enemyRef = Resources.Load<GameObject>("UI/Game/Enemies/EnemyActive");
        List<Node> withoutStartAndEnd = null;
        Node enemyNode = null;
        UnitController enemy = null;

        withoutStartAndEnd = allNodes.FindAll(node => (!node.IsBlocked && !node.IsEnd && !node.IsStart));

        enemyNode = withoutStartAndEnd[Random.Range(0, withoutStartAndEnd.Count)];
        enemy = Instantiate<GameObject>(enemyRef, container.transform).GetComponent<UnitController>();
        enemy.Init(mainBoard, enemyNode.X, enemyNode.Y);
        withoutStartAndEnd.Remove(enemyNode);
        
        //Load Lazy enemy
        enemyRef = Resources.Load<GameObject>("UI/Game/Enemies/EnemyLazy");
        enemyNode = withoutStartAndEnd[Random.Range(0, withoutStartAndEnd.Count)];
        enemy = Instantiate<GameObject>(enemyRef, container.transform).GetComponent<UnitController>();
        enemy.Init(mainBoard, enemyNode.X, enemyNode.Y);
        withoutStartAndEnd.Remove(enemyNode);

        //Load Patrolling enemy
        enemyRef = Resources.Load<GameObject>("UI/Game/Enemies/EnemyPatroller");
        enemyNode = withoutStartAndEnd[Random.Range(0, withoutStartAndEnd.Count)];
        enemy = Instantiate<GameObject>(enemyRef, container.transform).GetComponent<UnitController>();
        enemy.Init(mainBoard, enemyNode.X, enemyNode.Y);

    }

    public void OnChangeState(eState newState)
    {
        if (state == newState || state == eState.WIN || state == eState.LOSE) return;
        state = newState;
        switch (newState)
        {
            case eState.TARGET_ACHIEVED:
                endNode.SetEnd(false);
                break;
            case eState.WIN:
            case eState.LOSE:
                UIManager.Instance.OnMenuUITransition(UIManager.eMenuState.RESULTS);
                break;
        }
    }
    private void PlayerInput(UnitController.eDirection direction)
    {
        //On user input attempt move in the given direction.
        playerUnit.AttemptMove(direction);
    }
    public static void OnPlayerInput(UnitController.eDirection direction)
    {
        if (OnPlayerInputEvent != null)
            OnPlayerInputEvent(direction);
    }
    private void OnDestroy()
    {
        instance = null;
        OnPlayerInputEvent -= PlayerInput;
    }

#if UNITY_EDITOR
    //Arrow input for movement.
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            PlayerInput(UnitController.eDirection.UP);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            PlayerInput(UnitController.eDirection.DOWN);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            PlayerInput(UnitController.eDirection.LEFT);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            PlayerInput(UnitController.eDirection.RIGHT);
        }

    }
#endif
}
