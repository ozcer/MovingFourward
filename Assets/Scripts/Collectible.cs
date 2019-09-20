using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : GridObject {

    // Use this for initialization
    protected virtual void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        base.Update();
    }

    public virtual void GetPickedUp(Player player)
    {
        Grid location = GetMyGrid();
        DeathInfo di = new DeathInfo(location, this, this);
        Die(di);
    }

}
