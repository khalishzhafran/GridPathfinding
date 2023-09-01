using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathFinder<T>
{
    #region Delegates for cost calculation
    public delegate float CostFunction(T a, T b);
    public CostFunction HeuristicCost { get; set; }
    public CostFunction NodeTraversalCost { get; set; }
    #endregion

    #region PathFinderNode
    public class PathFinderNode
    {
        public PathFinderNode Parent { get; set; }
        public Node<T> Location { get; private set; }
        public float FCost { get; private set; }
        public float GCost { get; private set; }
        public float HCost { get; private set; }

        public PathFinderNode(Node<T> location, PathFinderNode parent, float gCost, float hCost)
        {
            Location = location;
            Parent = parent;
            HCost = hCost;
            SetGCost(gCost);
        }

        public void SetGCost(float gCost)
        {
            GCost = gCost;
            FCost = GCost + HCost;
        }
    }
    #endregion

    #region Properties
    public PathFinderStatus Status { get; private set; } = PathFinderStatus.NOT_INITIALIZED;
    public Node<T> Start { get; private set; }
    public Node<T> Goal { get; private set; }
    public PathFinderNode CurrentNode { get; private set; }
    #endregion

    #region Open and Closed Lists and Associated Functions
    protected List<PathFinderNode> openList = new List<PathFinderNode>();
    protected List<PathFinderNode> closedList = new List<PathFinderNode>();

    protected PathFinderNode GetLeastCostNode(List<PathFinderNode> list)
    {
        int bestIndex = 0;
        float bestPriority = list[0].FCost;

        for (int i = 1; i < list.Count; i++)
        {
            if (bestPriority > list[i].FCost)
            {
                bestIndex = i;
                bestPriority = list[i].FCost;
            }
        }

        PathFinderNode node = list[bestIndex];
        return node;
    }

    protected int IsInList(List<PathFinderNode> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i].Location.Value, value))
            {
                return i;
            }
        }

        return -1;
    }
    #endregion

    #region Delegates for Action Callbacks
    public delegate void DelegatePathFinderNode(PathFinderNode node);
    public DelegatePathFinderNode onChangeCurrentNode;
    public DelegatePathFinderNode onAddToOpenList;
    public DelegatePathFinderNode onAddToClosedList;
    public DelegatePathFinderNode onDestinationFound;

    public delegate void DelegatePathFinder();
    public DelegatePathFinder onStarted;
    public DelegatePathFinder onRunning;
    public DelegatePathFinder onFailure;
    public DelegatePathFinder onSuccess;
    #endregion

    #region Actual Path Finding Search Functions
    public bool Initialize(Node<T> start, Node<T> goal)
    {
        if (Status == PathFinderStatus.RUNNING)
        {
            return false;
        }

        Reset();

        Start = start;
        Goal = goal;

        float hCost = HeuristicCost(Start.Value, Goal.Value);

        PathFinderNode root = new PathFinderNode(Start, null, 0, hCost);

        CurrentNode = root;
        openList.Add(root);

        onChangeCurrentNode?.Invoke(CurrentNode);
        onStarted?.Invoke();

        Status = PathFinderStatus.RUNNING;

        return true;
    }

    public PathFinderStatus Step()
    {
        closedList.Add(CurrentNode);

        onAddToClosedList?.Invoke(CurrentNode);

        if (openList.Count == 0)
        {
            Status = PathFinderStatus.FAILURE;
            onFailure?.Invoke();
            return Status;
        }

        CurrentNode = GetLeastCostNode(openList);

        onChangeCurrentNode?.Invoke(CurrentNode);

        openList.Remove(CurrentNode);

        if (EqualityComparer<T>.Default.Equals(CurrentNode.Location.Value, Goal.Value))
        {
            Status = PathFinderStatus.SUCCESS;
            onDestinationFound?.Invoke(CurrentNode);
            onSuccess?.Invoke();
            return Status;
        }

        List<Node<T>> neighbours = CurrentNode.Location.GetNeighbours();

        foreach (Node<T> node in neighbours)
        {
            AlgorithmSpecificImplementation(node);
        }

        Status = PathFinderStatus.RUNNING;
        onRunning?.Invoke();
        return Status;
    }

    protected abstract void AlgorithmSpecificImplementation(Node<T> node);

    private void Reset()
    {
        if (Status == PathFinderStatus.RUNNING)
        {
            return;
        }

        CurrentNode = null;
        openList.Clear();
        closedList.Clear();

        Status = PathFinderStatus.NOT_INITIALIZED;
    }
    #endregion
}
