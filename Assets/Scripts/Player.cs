using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class Player : Solid {
    public int coins;

    public bool inControl;

    public override void Awake()
    {
        base.Awake();
        inControl = true;
        health = 150;
        maxHealth = health;
        attack = 13;
        coins = 0;

        level = 1;
        exp = 0;
        maxExp = 40;
    }

    // Use this for initialization
    protected void Start () {
        base.Start();
        GiveSpotlight();
	}

    // Update is called once per frame
    protected void Update () {
        base.Update();
    }

    private void OnDrawGizmos()
    {
        
    }


    public void InteractInDirection(Constant.QuadDir dir)
    {
        GameManager gm = GameManager.Instance;
        Grid targetGrid = gm.GetGrid(gridX, gridY, dir, 1);
        if (targetGrid.CanMoveTo())
        {
            MoveInDirection(dir);
            if (targetGrid.GetOccupantWithTag("Loot") != null) anim.Play("Interact");
        }
        else if (targetGrid.GetOccupantWithTag("Enemy") != null)
        {
            Enemy enemy = (Enemy)targetGrid.GetOccupantWithTag("Enemy");
            Attack(enemy);
        }
        else if (targetGrid.GetOccupantWithTag("Npc") != null)
        {
            Npc npc = (Npc)targetGrid.GetOccupantWithTag("Npc");
            Interact(npc);
        }
        else if (targetGrid.GetOccupantWithTag("Loot") != null)
        {
            Door door = (Door)targetGrid.GetOccupantWithTag("Loot");
            anim.Play("Interact");
            if (!door.enterable)
            {
                gm.Think("It's locked...");
            }
        }
        else
        {
            Debug.Log("Nothing to interact at " + targetGrid);
        }
        Face(dir);

        StartCoroutine(EndTurn());

    }

    IEnumerator EndTurn()
    {
        inControl = false;
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.EndTurn();
        inControl = true;
    }

    public void Interact(Npc npc)
    {
        npc.OnInteraction(this);
    }


    public void PickUp(Collectible loot)
    {
        loot.GetPickedUp(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Loot")
        {

            PickUp(collision.gameObject.GetComponent<Collectible>());
        }
    }

    public void Heal(int amount)
    {
        health = Math.Min(maxHealth, health + amount);
    }

    public override void LevelUp()
    {
        base.LevelUp();
        attack += 4;
        maxHealth += 10;
        health += 10;
        maxExp = (int) (maxExp * 1.3);
        GameManager.Instance.Think("Level up!");
    }

    public override void Die(DeathInfo di)
    {
        anim.Play("Die");
    }

}
