using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : GridObject
{

    public override void Awake()
    {
        isSolid = true;
    }

    // Use this for initialization
    protected void Start () {
        base.Start();
	}

    protected void Update()
    {
        base.Update();
    }
}
