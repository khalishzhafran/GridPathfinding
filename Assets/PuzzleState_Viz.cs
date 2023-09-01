using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleState_Viz : MonoBehaviour
{
    [Tooltip("Associate the tiles into this array")]
    public GameObject[] Tiles;

    [Tooltip("Associate the location transforms into this array")]
    public Transform[] TileLocations;

    private void Start()
    {
        PuzzleState state = new PuzzleState(3);
        SetPuzzleState(state);
    }

    public void SetPuzzleState(PuzzleState state)
    {
        for (int i = 0; i < state.Arr.Length; ++i)
        {
            Tiles[state.Arr[i]].transform.position = TileLocations[i].position;
        }
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(
                startingPos, end,
                (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = end;
    }

    public void SetPuzzleState(PuzzleState state, float duration)
    {
        for (int i = 0; i < state.Arr.Length; ++i)
        {
            StartCoroutine(MoveOverSeconds(Tiles[state.Arr[i]], TileLocations[i].position, duration));
        }
    }
}
