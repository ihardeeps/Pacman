using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class holds all data and calculations related to player, enemies, collectibles.
public class Node
{
    //This enum is used to set different states of the node when Enemies or Players traverse through the board
    public enum eState
    {
        NONE = -1,
        EMPTY,
        START,
        END_LOCKED,
        END_OPEN,
        COLLECTIBLE,
        BLOCKER,
        PLAYER,
        ENEMY,
        MAX
    }
    public enum eResult
    {
        NONE = -1,
        SUCCESS,
        BLOCKED,
        COLLECTED,
        WIN,
        LOSE
    }
    //Pre defined neighbors 
    private Node upNode, downNode, leftNode, rightNode;
    public Node parentNode;
    //holding reference of collectible object. Used to get value of the item and update the collection accordingly, 
    private Collectible collectible;
    //used to check if node is traversable while finding the path
    public bool traversable = true;
    //saving previous and current state of the node. Used while attempting change of state.
    private eState state, prevState;
    public eState State { get { return state; } }
    public eState PrevState { get { return prevState; } }

    //holding Tile objects reference for movement and positioning of items
    public Tile tileRef;
    //holding x and y position on the board 
    public int X, Y;

    //Cost of nodes while finding path between two nodes
    public float XCost;
    public float YCost;
    public float TotalCost { get { return XCost + YCost; } }

    public void CreateConnection(UnitController.eDirection direction, Node node)
    {
        switch (direction)
        {
            case UnitController.eDirection.UP:
                upNode = node;
                break;
            case UnitController.eDirection.DOWN:
                downNode = node;
                break;
            case UnitController.eDirection.LEFT:
                leftNode = node;
                break;
            case UnitController.eDirection.RIGHT:
                rightNode = node;
                break;
        }
    }
    public void SetStart()
    {
        //Set the tile state and attempt change
        tileRef.stateLabel.text = string.Format("[{0},{1}]\n{2}", X, Y, eState.START.ToString());
        AttempChangeState(eState.START);
    }
    public void SetPlayer()
    {
        //Set the tile state and attempt change
        tileRef.stateLabel.text = string.Format("[{0},{1}]\n{2}", X, Y, eState.START.ToString());
        AttempChangeState(eState.START);
    }
    public void SetBlocker()
    {
        //Set the tile state and attempt change
        tileRef.stateLabel.text = string.Format("[{0},{1}]\n{2}", X, Y, eState.BLOCKER.ToString());
        AttempChangeState(eState.BLOCKER);
    }
    public void SetEnd(bool locked)
    {
        //Set the tile state and attempt change 
        tileRef.stateLabel.text = locked ? string.Format("[{0},{1}]\n{2}", X, Y, eState.END_LOCKED.ToString()) : string.Format("[{0},{1}]\n{2}", X, Y, eState.END_OPEN.ToString());
        tileRef.SetState(locked ? eState.END_LOCKED : eState.END_OPEN);
        state = locked ? eState.END_LOCKED : eState.END_OPEN;
    }
    public void SetCollectible(Collectible item)
    {
        collectible = item;
        collectible.transform.localPosition = tileRef.transform.localPosition;
        AttempChangeState(eState.COLLECTIBLE);
    }
    //When player is on a tile which holds a collectible
    private void AttempAcquireCollectible(eState newState)
    {
        if (state == eState.COLLECTIBLE && newState == eState.PLAYER)
        {
            GameManager.PlayerData.Collect(collectible);
            collectible.Collected();
            PanelGameHUD.Refresh();
        }
    }
    //This is called every signle time Player or Enemy moves around the boar. This sets the node state. This is sued to check if the objective if complete 
    public void AttempChangeState(eState newState)
    {
        tileRef.stateLabel.text = string.Format("[{0},{1}]\n{2}, {3}, {4}", X, Y, prevState.ToString(), state.ToString(), newState.ToString());
        //When all the collectibles are collected the node state is set to END_OPEN i.e the door gets open and player can enter. This is win check
        if (state == eState.END_OPEN && newState == eState.PLAYER)
        {
            LevelManager.Instance.OnChangeState(LevelManager.eState.WIN);
            return;
        }
        //Lose check
        if ((state == eState.ENEMY && newState == eState.PLAYER) || state == eState.PLAYER && newState == eState.ENEMY)
        {
            LevelManager.Instance.OnChangeState(LevelManager.eState.LOSE);
            return;
        }
        //Checking if Player is on either Start or End and end is closed then do nothing on that node.
        if (IsUnchangeable && newState != eState.END_OPEN) return;

        AttempAcquireCollectible(newState);
        if (state != newState)
            prevState = state;
        state = newState;
        CheckForObjectiveComplete();
        tileRef.SetState(newState);
    }
    private void CheckForObjectiveComplete()
    {
        if (GameManager.PlayerData.Coins == GameManager.PlayerData.TargetCoins && GameManager.PlayerData.TargetCoins > 0)
        {
            LevelManager.Instance.OnChangeState(LevelManager.eState.TARGET_ACHIEVED);
        }
    }

    public bool IsEnemy { get { return state == eState.ENEMY; } }
    public bool IsPlayer { get { return state == eState.PLAYER; } }
    public bool IsBlocked { get { return state == eState.BLOCKER; } }
    public bool IsStart { get { return state == eState.START; } }
    public bool IsEnd { get { return state == eState.END_OPEN || state == eState.END_LOCKED; } }
    //This check is created because I am just changing the Tile sprite instead of adding another GameObject on top of it. This is done for Start, end and block nodes.
    public bool IsUnchangeable { get { return (state == eState.END_LOCKED || state == eState.END_OPEN || state == eState.BLOCKER || state == eState.START); } }
    //Checking if Node is not null and not blocked in the direction to move, if traversable then returns the node it can move to.
    public Node IsTraversible(UnitController.eDirection direction)
    {
        switch (direction)
        {
            case UnitController.eDirection.UP:
                if (upNode != null && !upNode.IsBlocked)
                    return upNode;
                break;
            case UnitController.eDirection.DOWN:
                if (downNode != null && !downNode.IsBlocked)
                    return downNode;
                break;
            case UnitController.eDirection.LEFT:
                if (leftNode != null && !leftNode.IsBlocked)
                    return leftNode;
                break;
            case UnitController.eDirection.RIGHT:
                if (rightNode != null && !rightNode.IsBlocked)
                    return rightNode;
                break;

        }
        return null;
    }
    //Gets neighbor nodes in a list
    public Node GetNieghbors()
    {
        List<Node> nodes = new List<Node>();
        if (upNode != null) nodes.Add(upNode);
        if (downNode != null) nodes.Add(downNode);
        if (leftNode != null) nodes.Add(leftNode);
        if (rightNode != null) nodes.Add(rightNode);
        int index = Random.Range(0, nodes.Count);
        return nodes[index];
    }

    public UnitController.eDirection GetDirection(Node node)
    {
        if (node.Equals(upNode))
            return UnitController.eDirection.UP;
        if (node.Equals(downNode))
            return UnitController.eDirection.DOWN;
        if (node.Equals(leftNode))
            return UnitController.eDirection.LEFT;
        if (node.Equals(rightNode))
            return UnitController.eDirection.RIGHT;
        return UnitController.eDirection.NONE;
    }
    //Simple check for node equality
    public bool Equals(Node node)
    {
        if (node == null) return false;
        return X == node.X && Y == node.Y;
    }
}
