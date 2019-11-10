using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitController
{
    public enum eBehaviour
    {
        NONE,
        LAZY,
        ACTIVE,
        PATROLLER
    }
    public eBehaviour behaviourType;
    public int Range;
    private eDirection moveDir;
    public bool inRange = true;
    public override void Init(Node[,] boardRef, int x, int y)
    {
        base.Init(boardRef, x, y);
        CurrentNode.AttempChangeState(Node.eState.ENEMY);
        StartCoroutine("DelayedLoad");
        inRange = behaviourType != eBehaviour.LAZY;
    }
    private IEnumerator DelayedLoad()
    {
        yield return new WaitForEndOfFrame();
        float refreshRate = GameManager.PersistentData.GameBalance.GetPathFindRefreshRate(GameManager.PlayerData.Level), currTime = 0;
        InitializeBehaviour();
        //Update function
        while (true)
        {
            yield return null;
            currTime += Time.deltaTime;
            if (currTime >= refreshRate)
            {
                currTime = 0;
                requirePathUpdate = false;
            }
        }
    }
    private void InitializeBehaviour()
    {
        //Initializing behaviour baed on type.
        switch (behaviourType)
        {
            case eBehaviour.PATROLLER:
                IntitializePatrollingEnemy();
                break;
            case eBehaviour.ACTIVE:
                speed = GameManager.PersistentData.GameBalance.GetActiveEnemySpeed(GameManager.PlayerData.Level);
                IntitializeActiveEnemy();
                break;
            case eBehaviour.LAZY:
                speed = GameManager.PersistentData.GameBalance.GetLazyEnemySpeed(GameManager.PlayerData.Level);
                InitializeLazyEnemy();
                break;
        }
    }

    #region PATROLLING BEHAVIOUR
    private bool upDownMovement = true;
    private void IntitializePatrollingEnemy()
    {
        upDownMovement = Random.Range(0, 2) == 1 ? true : false;
        speed = GameManager.PersistentData.GameBalance.GetPatrolEnemySpeed(GameManager.PlayerData.Level);
        moveDir = upDownMovement ? eDirection.DOWN : eDirection.LEFT;
        StartCoroutine("DelayedPatrollerUpdate");
    }
    private IEnumerator DelayedPatrollerUpdate()
    {
        if (!AttemptMove(moveDir))
        {
            if (upDownMovement)
                moveDir = moveDir == eDirection.DOWN ? eDirection.UP : eDirection.DOWN;
            else
                moveDir = moveDir == eDirection.LEFT ? eDirection.RIGHT : eDirection.LEFT;

            AttemptMove(moveDir);
        }
        yield return null;
        while (isMoving) yield return null;
        StartCoroutine("DelayedPatrollerUpdate");
    }
    #endregion

    #region ACTIVE BEHAVIOUR
    public void IntitializeActiveEnemy()
    {
        if (LevelManager.Instance.State == LevelManager.eState.WIN || LevelManager.Instance.State == LevelManager.eState.LOSE)
        {
            StopAllCoroutines();
            return;
        }
        requirePathUpdate = false;
        List<UnitController.eDirection> directions = GetDirections(CurrentNode, LevelManager.Instance.Player.CurrentNode);
        List<Node> path = GetPath(CurrentNode, LevelManager.Instance.Player.CurrentNode);
        StartCoroutine("DelayedFollowPlayer", directions);
    }
    private IEnumerator DelayedFollowPlayer(List<UnitController.eDirection> directions)
    {
        //Making sure the player has moved out of home.
        while (!LevelManager.Instance.Player.hasMoved || !inRange) yield return null;

        foreach (UnitController.eDirection direction in directions)
        {
            AttemptMove(direction);
            yield return null;
            while (isMoving) yield return null;
            if (requirePathUpdate) break;
        }
        IntitializeActiveEnemy();
    }
    #endregion

    #region LAZY BEHAVIOUR
    private void InitializeLazyEnemy()
    {
        StartCoroutine("DelayedCheckNeighbors");
    }
    private IEnumerator DelayedCheckNeighbors()
    {
        float currTime = 0, duration = 1f;
        while (!inRange)
        {
            yield return null;
            currTime += Time.deltaTime;
            if (currTime >= duration)
            {
                currTime = 0;
                CheckIfPlayerInRange();
            }
        }
        IntitializeActiveEnemy();
    }
    private void CheckIfPlayerInRange()
    {
        int minX = Mathf.Min(0, CurrentNode.X - Range); if (minX < 0) minX = 0;
        int maxX = Mathf.Min(CurrentNode.X + Range, GameManager.PersistentData.GameConfiguration.GridSizeX);
        int minY = Mathf.Min(0, CurrentNode.Y - Range); if (minY < 0) minY = 0;
        int maxY = Mathf.Min(CurrentNode.Y + Range, GameManager.PersistentData.GameConfiguration.GridSizeY);
        bool foundPlayer = false;
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                if (board[i, j].IsPlayer)
                {
                    foundPlayer = true;
                    break;
                }
            }
            if (foundPlayer) break;
        }
        inRange = foundPlayer;
    }
    #endregion
    public override IEnumerator DelayedAttemptMove()
    {
        isMoving = true;
        float distance = Vector3.Distance(transform.position, target.tileRef.transform.position);
        while (distance > 0.01f)
        {
            float step = speed * Time.deltaTime;
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, target.tileRef.transform.position, step);
            distance = Vector3.Distance(transform.position, target.tileRef.transform.position);
        }
        target.AttempChangeState(Node.eState.ENEMY);
        board[X, Y].AttempChangeState(board[X, Y].PrevState);
        X = target.X;
        Y = target.Y;
        isMoving = false;
        target = null;
    }

#if UNITY_EDITOR
    public bool findPathAgain = false;
    private void Update()
    {
        if (findPathAgain)
        {
            findPathAgain = false;
            IntitializeActiveEnemy();
        }
    }
#endif
}
