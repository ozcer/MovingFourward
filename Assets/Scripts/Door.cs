using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : Collectible {

    public Constant.QuadDir side;
    public bool enterable;
    public string type;

    public bool monsterLocked;

    private void Awake()
    {
        enterable = true;
        monsterLocked = false;
    }

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
        isSolid = !enterable;
        if (!enterable)
        {
            //GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        if (monsterLocked && GameManager.Instance.GetMonsterCount() == 0)
        {
            enterable = true;
        }
	}

    private void OnDrawGizmos()
    {
        //Handles.Label(transform.position, "type: " + type);
    }

    public override void GetPickedUp(Player player)
    {
        GameManager gm = GameManager.Instance;
        Constant.QuadDir oppositeSide = Constant.Opposite(side);
        gm.EnterDoor(this); // going to left room comes out next room from right
        base.GetPickedUp(player);
    }

    public void MonsterLock()
    {
        monsterLocked = true;
        enterable = false;
    }
}
