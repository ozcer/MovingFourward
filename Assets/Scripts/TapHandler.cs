using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapHandler : MonoBehaviour {

    public enum QuadDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    // Update is called once per frame
    void Update () {
        // Left-half of the screen.
        //Rect leftBound = new Rect(
        //    0, Screen.height / 3, 
        //    Screen.width / 3, Screen.height / 3);
        //Rect rightBound = new Rect(
        //     Screen.width * 2 / 3, Screen.height / 3,
        //    Screen.width / 3, Screen.height / 3);
        //Rect topBound = new Rect(
        //    Screen.width / 3 , Screen.height * 2 / 3,
        //    Screen.width / 3, Screen.height / 3);
        //Rect botBound = new Rect(
        //    Screen.width / 3, 0,
        //    Screen.width / 3, Screen.height / 3);


        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (leftBound.Contains(Input.mousePosition))
        //    {
        //        OnLeftTap();
        //    }
        //    else if (rightBound.Contains(Input.mousePosition))
        //    {
        //        OnRightTap();
        //    }
        //    else if (topBound.Contains(Input.mousePosition))
        //    {
        //        OnTopTap();
        //    }
        //    else if (botBound.Contains(Input.mousePosition))
        //    {
        //        OnBotTap();
        //    }
        //}
        
    }

    //private void OnBotTap()
    //{
    //    FindObjectOfType<GridManager>().MovePlayerInDirection(QuadDirection.Down);
    //}

    //private void OnTopTap()
    //{
    //    FindObjectOfType<GridManager>().MovePlayerInDirection(QuadDirection.Up);
    //}

    //private void OnRightTap()
    //{
    //    FindObjectOfType<GridManager>().MovePlayerInDirection(QuadDirection.Right);
    //}

    //private void OnLeftTap()
    //{
    //    FindObjectOfType<GridManager>().MovePlayerInDirection(QuadDirection.Left);
    //}
}
