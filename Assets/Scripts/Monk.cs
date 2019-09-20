using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : Npc {

    public bool used;

	// Use this for initialization
	void Start () {
        base.Start();
        Face(Constant.QuadDir.Down);
        used = false;
        GiveSpotlight();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    public override void OnInteraction(Player interactee)
    {

        if (interactee.coins < 2)
        {
            GameManager.Instance.Think("I need 2 coin");
        }
        else if (interactee.health == interactee.maxHealth)
        {
            GameManager.Instance.Think("I'm already healthy");
        }
        else if(!used)
        { 
            anim.Play("Heal");
            interactee.Heal(40);
            interactee.coins -= 2;
            used = true;
            interactee.anim.Play("Interact");
            char c = '\u2764';
            GameManager.Instance.Think(c.ToString());
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.5f);
        used = false;
    }


}
