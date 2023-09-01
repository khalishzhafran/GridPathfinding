using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNode : Node<PuzzleState>
{
    private PuzzleMap puzzleMap;

    public PuzzleNode(PuzzleState value) : base(value) { }

    public PuzzleNode(PuzzleMap puzzleMap, PuzzleState value) : base(value)
    {
        this.puzzleMap = puzzleMap;
    }

    public override List<Node<PuzzleState>> GetNeighbours()
    {
        return puzzleMap.GetNeighbours(this);
    }
}