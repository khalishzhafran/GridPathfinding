using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyPathFinder<T> : PathFinder<T>
{
    protected override void AlgorithmSpecificImplementation(Node<T> node)
    {
        if (IsInList(closedList, node.Value) == -1)
        {
            float gCost = 0f;
            float hCost = HeuristicCost(node.Value, Goal.Value);

            int idOpenList = IsInList(openList, node.Value);
            if (idOpenList == -1)
            {
                PathFinderNode newNode = new PathFinderNode(node, CurrentNode, gCost, hCost);
                openList.Add(newNode);

                onAddToOpenList?.Invoke(newNode);
            }
            else
            {
                float oldGCost = openList[idOpenList].GCost;
                if (gCost < oldGCost)
                {
                    openList[idOpenList].Parent = CurrentNode;
                    openList[idOpenList].SetGCost(gCost);

                    onAddToOpenList?.Invoke(openList[idOpenList]);
                }
            }
        }
    }
}
