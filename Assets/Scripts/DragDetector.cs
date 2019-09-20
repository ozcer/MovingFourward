using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public enum QuadDirection
    {
        Up,
        Down,
        Right,
        Left
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("Press position + " + eventData.pressPosition);
        //Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        //QuadDirection dir = GetDragDirection(dragVectorDirection);
        //GameObject.FindObjectOfType<GridManager>().MovePlayerInDirection(dir);
        ////print(dir);
    }

    private QuadDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        QuadDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? QuadDirection.Right : QuadDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? QuadDirection.Up : QuadDirection.Down;
        }
        Debug.Log(draggedDir);
        return draggedDir;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Press position + " + eventData.pressPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Press position + " + eventData.pressPosition);
    }
}
