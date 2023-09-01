using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node<T>
{
    public T Value { get; private set; }

    public abstract List<Node<T>> GetNeighbours();

    public Node(T value)
    {
        Value = value;
    }
}
