using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : MonoBehaviour
{

    public enum QuadDir
    {
        Up,
        Down,
        Right,
        Left
    }

    public static QuadDir Opposite(QuadDir side)
    {
        if (side == QuadDir.Up)
        {
            return QuadDir.Down;
        }
        else if (side == QuadDir.Down)
        {
            return QuadDir.Up;
        }
        else if (side == QuadDir.Left)
        {
            return QuadDir.Right;
        }
        else if (side == QuadDir.Right)
        {
            return QuadDir.Left;
        }
        else
        {
            throw new System.Exception("bad side");
        }
        
    }
}
