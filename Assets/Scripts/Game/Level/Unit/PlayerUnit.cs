using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitController
{
    //Used game to know the player has moved once so enemies can begin thier thing.
    public bool hasMoved = false;
    public override void Init(Node[,] boardRef, int x, int y)
    {
        base.Init(boardRef, x, y);
        speed = GameManager.PersistentData.GameBalance.GetPlayerSpeed(GameManager.PlayerData.Level);
        CurrentNode.AttempChangeState(Node.eState.PLAYER);
    }
    public override IEnumerator DelayedAttemptMove()
    {
        //Attempt move here.
        hasMoved = true;
        //Set its state to moving. This also can be converted to enum. 
        isMoving = true;
        float distance = Vector3.Distance(transform.position, target.tileRef.transform.position);
        while (distance > 0.01f)
        {
            //Keep moving until distance reached is minimal.
            float step = speed * Time.deltaTime;
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, target.tileRef.transform.position, step);
            distance = Vector3.Distance(transform.position, target.tileRef.transform.position);
        }
        //Reached the destination
        //Change the nodes nodes state
        target.AttempChangeState(Node.eState.PLAYER);
        //Change the current nodes state
        board[X, Y].AttempChangeState(Node.eState.EMPTY);
        //update current position to target.
        X = target.X;
        Y = target.Y;
        isMoving = false;
        target = null;
    }
}
