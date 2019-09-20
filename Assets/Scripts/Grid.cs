using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public int gridX, gridY;

    public HashSet<GridObject> occupants;
    public int occupantCount;

    private void Awake()
    {
        occupants = new HashSet<GridObject>();
        
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (occupants != null)
        {
            occupantCount = occupants.Count;
        }
    }

    public bool CanMoveTo()
    {
        foreach (GridObject occupant in occupants)
        {
            if (occupant != null && occupant.isSolid)
            {
                return false;
            }
        }
        return true;
    }

    public bool EnemyCanMoveHere()
    {
        foreach (GridObject occupant in occupants)
        {
            if (occupant != null && (occupant.isSolid || occupant.gameObject.tag == "Loot"))

            {
                return false;
            }
        }
        return true;
    }

    public void AddOccupant(GridObject newOccupant)
    {
        occupants.Add(newOccupant);
    }

    public void RemoveOccupant(GridObject occupant)
    {
        occupants.Remove(occupant);
    }

    // return the first occupant with given tag
    // return null if not in occupants
    public GridObject GetOccupantWithTag(string tag)
    {
        //print("looking for " + tag + " in " + this);
        GridObject result = null;
        foreach (GridObject o in occupants)
        {
            if (o != null && o.gameObject.tag == tag)
            {
                result = o;
            }
        }
        return result;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}]", gridX, gridY);
    }
}
