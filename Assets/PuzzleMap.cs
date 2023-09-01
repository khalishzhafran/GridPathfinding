using System.Collections;
using System.Collections.Generic;

public class PuzzleMap
{
    public List<Node<PuzzleState>> GetNeighbours(PuzzleNode loc)
    {
        List<Node<PuzzleState>> neighbours = new List<Node<PuzzleState>>();

        int emptyIndex = loc.Value.GetEmptyTileIndex();
        List<int> intArray = GetNeighbors(emptyIndex);
        for (int i = 0; i < intArray.Count; ++i)
        {
            PuzzleNode state = new PuzzleNode(this, new PuzzleState(loc.Value));

            state.Value.SwapWithEmpty(intArray[i]);
            neighbours.Add(state);
        }
        return neighbours;
    }

    #region Constructor
    public PuzzleMap(int numRowsOrCols)
    {
        CreateGraphForNPuzzle(numRowsOrCols);
    }
    #endregion

    #region Private variables and functions
    private Dictionary<int, List<int>> edgeDictionary = new Dictionary<int, List<int>>();

    private List<int> GetNeighbors(int id)
    {
        return edgeDictionary[id];
    }

    private void CreateGraphForNPuzzle(int rowsOrCols)
    {
        for (int i = 0; i < rowsOrCols; i++)
        {
            for (int j = 0; j < rowsOrCols; j++)
            {
                int index = i * rowsOrCols + j;
                List<int> li = new List<int>();
                if (i - 1 >= 0) li.Add((i - 1) * rowsOrCols + j);
                if (i + 1 < rowsOrCols) li.Add((i + 1) * rowsOrCols + j);
                if (j - 1 >= 0) li.Add(i * rowsOrCols + j - 1);
                if (j + 1 < rowsOrCols) li.Add(i * rowsOrCols + j + 1);

                edgeDictionary[index] = li;
            }
        }
    }
    #endregion

    #region Static functions for calculating the Manhattan cost
    public static float GetManhattanCost(PuzzleState a, PuzzleState b)
    {
        // NOTE: We do not use a variable goal state. For 8 puzzle
        // the goal state is predefined to be same.
        // See PuzzleState for more information.
        return a.GetManhattanCost();
    }

    public static float GetCostBetweenTwoCells(PuzzleState a, PuzzleState b)
    {
        // The cost of movement from 1 cell to the next is always 1.
        return 1.0f;
    }
    #endregion
}
