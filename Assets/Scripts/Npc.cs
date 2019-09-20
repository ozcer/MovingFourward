using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Solid {

    // Use this for initialization
    public void Start() {
        base.Start();
    }

    // Update is called once per frame
    public void Update() {
        base.Update();
    }

    public virtual void OnInteraction(Player interactee)
    {

    }
}
