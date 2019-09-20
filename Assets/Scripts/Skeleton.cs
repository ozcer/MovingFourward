using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy {

    bool isBoss;

    public  void Awake()
    {
        base.Awake();
        int lv = GameManager.Instance.level;
        health = 35 + lv * 2;
        attack = 9 + lv  * 1;
        maxTurnsUntilAction = 1;
        turnsUntilAction = maxTurnsUntilAction;
        isBoss = false;
        expReward = 20;
    }

    // Use this for initialization
    public void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	public void Update () {
		base.Update();
	}

    public override void Action()
    {
        if (isBoss)
        {
            int i = Random.Range(1, 22);
            print("rolled " + i);
            if (i <= GameManager.Instance.level)
            {
                SummonSkeleton();
            }
            else
            {
                HuntPlayer();
                
            }
        }
        else
        {
            HuntPlayer();
        }
        
    }

    public void SummonSkeleton()
    {
        GameManager.Instance.SpawnSkeletonRandom();
        anim.Play("Wave");
    }

    public override void Bossify()
    {
        print("SUPER SKELETON!");
        transform.localScale = new Vector3(1f, 1.5f, 1f);
        maxTurnsUntilAction = 2;
        turnsUntilAction = maxTurnsUntilAction;
        health *= 3;
        attack *= 3;
        expReward *= 3;
        isBoss = true;
    }

    public override void DropLoot()
    {
        int i = Random.Range(0, 4);
        print("rolled a " + i);
        if (i - 1 <= GameManager.Instance.level)
        {
            base.DropLoot();
        }
        
    }

}
