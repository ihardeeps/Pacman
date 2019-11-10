using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public enum eDirection
    {
        NONE = -1,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    public float speed = 10;
    public bool isMoving = false;
    protected Node target;
    protected bool requirePathUpdate = false;

    protected Node[,] board;
    protected int X, Y;
    public Node CurrentNode { get { return board[X, Y]; } }
    public virtual void Init(Node[,] boardRef, int x, int y)
    {
        board = boardRef;
        X = x;
        Y = y;
        transform.localPosition = board[X, Y].tileRef.transform.localPosition;
    }
    public bool AttemptMove(eDirection direction)
    {
        Node nextNode = board[X, Y].IsTraversible(direction);
        if (nextNode != null && !isMoving)
        {
            MoveTo(nextNode);
            return true;
        }
        return false;
    }
    private void MoveTo(Node targetRef)
    {
        target = targetRef;
        StartCoroutine("DelayedAttemptMove");
    }
    public virtual IEnumerator DelayedAttemptMove()
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
        target.AttempChangeState(Node.eState.PLAYER);
        board[X, Y].AttempChangeState(Node.eState.EMPTY);
        X = target.X;
        Y = target.Y;
        isMoving = false;
        target = null;
    }

    #region PATH FINDING ALGORITHM
    protected List<Node> GetPath(Node start, Node target)
    {
        //Typical A* algorythm from here and on

        List<Node> foundPath = new List<Node>();

        //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //We start adding to the open set
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {
                //We check the costs for the current node
                //You can have more opt. here but that's not important now
                if (openSet[i].TotalCost < currentNode.TotalCost || (openSet[i].TotalCost == currentNode.TotalCost && openSet[i].XCost < currentNode.XCost))
                {
                    //and then we assign a new current node
                    if (!currentNode.Equals(openSet[i]))
                    {
                        currentNode = openSet[i];
                    }
                }
            }

            //we remove the current node from the open set and add to the closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //if the current node is the target node
            if (currentNode.Equals(target))
            {
                //that means we reached our destination, so we are ready to retrace our path
                foundPath = RetracePath(start, currentNode);
                break;
            }

            //if we haven't reached our target, then we need to start looking the neighbours
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!closedSet.Contains(neighbour))
                {
                    //we create a new movement cost for our neighbours
                    float newCost = currentNode.XCost + GetDistance(currentNode, neighbour);

                    //and if it's lower than the neighbour's cost
                    if (newCost < neighbour.XCost || !openSet.Contains(neighbour))
                    {
                        //we calculate the new costs
                        neighbour.YCost = newCost;
                        neighbour.XCost = GetDistance(neighbour, target);
                        //Assign the parent node
                        neighbour.parentNode = currentNode;
                        //And add the neighbour node to the open set
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        //we return the path at the end
        return foundPath;
    }
    protected List<UnitController.eDirection> GetDirections(Node start, Node target)
    {
        List<Node> foundPath = GetPath(start, target);

        List<UnitController.eDirection> directions = new List<eDirection>();
        for (int i = 0; i < foundPath.Count - 1; i++)
        {
            if (i == 0)
                directions.Add(start.GetDirection(foundPath[i]));
            directions.Add(foundPath[i].GetDirection(foundPath[i + 1]));
            if (i == foundPath.Count - 1)
                directions.Add(foundPath[i + 1].GetDirection(target));

        }
        if (foundPath.Count == 1)
            directions.Add(start.GetDirection(foundPath[0]));
        //we return the directions at the end
        return directions;
    }
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        //Retrace the path, is basically going from the endNode to the startNode
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            //by taking the parentNodes we assigned
            currentNode = currentNode.parentNode;
        }

        //then we simply reverse the list
        path.Reverse();

        return path;
    }
    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighborList = new List<Node>();
        Node neighborNode = node.IsTraversible(eDirection.UP);
        if (neighborNode != null) neighborList.Add(neighborNode);
        neighborNode = node.IsTraversible(eDirection.DOWN);
        if (neighborNode != null) neighborList.Add(neighborNode);
        neighborNode = node.IsTraversible(eDirection.LEFT);
        if (neighborNode != null) neighborList.Add(neighborNode);
        neighborNode = node.IsTraversible(eDirection.RIGHT);
        if (neighborNode != null) neighborList.Add(neighborNode);
        return neighborList;

    }
    private float GetDistance(Node node1, Node node2)
    {
        //We find the distance between each node
        //not much to explain here

        int distX = Mathf.Abs(node1.X - node2.X);
        int distY = Mathf.Abs(node1.Y - node2.Y);

        return Mathf.Sqrt(distX * distX + distY * distY);
    }
    #endregion
}
