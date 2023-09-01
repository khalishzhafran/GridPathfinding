using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSolver : MonoBehaviour
{
    public PuzzleState_Viz puzzleStateViz;

    private PuzzleNode currentState;
    private PuzzleNode goalState;

    private GreedyPathFinder<PuzzleState> astarSolver = new GreedyPathFinder<PuzzleState>();
    private PuzzleMap puzzle = new PuzzleMap(3);

    private void Start()
    {
        currentState = new PuzzleNode(puzzle, new PuzzleState(3));
        goalState = new PuzzleNode(puzzle, new PuzzleState(3));

        astarSolver.NodeTraversalCost = PuzzleMap.GetCostBetweenTwoCells;
        astarSolver.HeuristicCost = PuzzleMap.GetManhattanCost;
    }

    private void Update()
    {
        // Solve the puzzle and immediately show
        // the solution.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            puzzleStateViz.SetPuzzleState(currentState.Value);
            astarSolver.Initialize(currentState, goalState);

            SolvePuzzle();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (astarSolver.Status == PathFinderStatus.RUNNING)
                astarSolver.Step();
            if (astarSolver.Status == PathFinderStatus.SUCCESS)
            {
                Debug.Log("Found solution. Displaying solution now");
                StartCoroutine(ShowSolution());
            }
            if (astarSolver.Status == PathFinderStatus.FAILURE)
            {
                Debug.Log("Failure");
            }
        }

        // Randomize the puzzle.
        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomPuzzle();
        }
    }

    private IEnumerator Solve()
    {
        // Keep calling step as long as the pathfinder's status
        // is RUNNING.
        while (astarSolver.Status == PathFinderStatus.RUNNING)
        {
            astarSolver.Step();
            yield return null;
        }

        // SUCCESS.
        // Show the solution in a smooth way.
        if (astarSolver.Status == PathFinderStatus.SUCCESS)
        {
            Debug.Log("Found solution. Displaying solution now");
            StartCoroutine(ShowSolution());
        }

        // FAILURE
        // Failed finding path.
        if (astarSolver.Status == PathFinderStatus.FAILURE)
        {
            Debug.Log("Failure");
        }
    }

    public void SolvePuzzle()
    {
        StartCoroutine(Solve());
    }

    private IEnumerator Randomize(int depth)
    {
        int i = 0;

        while (i < depth)
        {
            List<Node<PuzzleState>> neighbours = puzzle.GetNeighbours(currentState);

            // get a random neighbour.
            int randomNeighbour = Random.Range(0, neighbours.Count);

            currentState.Value.SwapWithEmpty(neighbours[randomNeighbour].Value.GetEmptyTileIndex());

            i++;

            puzzleStateViz.SetPuzzleState(currentState.Value);
            yield return null;
        }
    }

    public void RandomPuzzle(int depth = 50)
    {
        StartCoroutine(Randomize(depth));
    }

    private IEnumerator ShowSolution()
    {
        List<PuzzleState> reverseSolution = new List<PuzzleState>();
        PathFinder<PuzzleState>.PathFinderNode node = astarSolver.CurrentNode;

        while (node != null)
        {
            reverseSolution.Add(node.Location.Value);
            node = node.Parent;
        }

        if (reverseSolution.Count > 0)
        {
            puzzleStateViz.SetPuzzleState(reverseSolution[reverseSolution.Count - 1]);

            if (reverseSolution.Count > 2)
            {
                for (int i = reverseSolution.Count - 2; i >= 0; i -= 1)
                {
                    puzzleStateViz.SetPuzzleState(reverseSolution[i], 0.5f);
                    yield return new WaitForSeconds(1.0f);
                }
            }
        }
    }
}
