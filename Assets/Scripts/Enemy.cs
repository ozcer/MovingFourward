using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : Solid {
    public int expReward;
    public int turnsUntilAction;
    public int maxTurnsUntilAction;

    public GameObject lootPrefab;

    public override void Awake()
    {
        base.Awake();
        health = 45;
        attack = 5;
        maxTurnsUntilAction = 2;
        turnsUntilAction = maxTurnsUntilAction;
        expReward = 15;
    }

    // Use this for initialization
    public void Start () {
        // register to listern, once turn ends, will invoke this.OnEndTurn
        GameManager.Instance.OnTurnEndSubscribers += OnEndTurn;
        base.Start();
        Face(Constant.QuadDir.Down);
    }
	
	// Update is called once per frame
	protected void Update () {
        base.Update();
	}

    //private void OnDrawGizmos()
    //{
    //    Handles.Label(transform.position, "hp: "+health);
    //}

    public void MoveRandom()
    {
        
        GameManager gm = GameManager.Instance;
        List<Constant.QuadDir> options = new List<Constant.QuadDir>();
        //print("enemy move random");
        //print("at " + this);

        Grid upGrid = gm.GetGrid(gridX, gridY, Constant.QuadDir.Up, 1);
        if (upGrid != null && upGrid.EnemyCanMoveHere())
        {
            options.Add(Constant.QuadDir.Up);
        }

        Grid downGrid = gm.GetGrid(gridX, gridY, Constant.QuadDir.Down, 1);
        if (downGrid != null && downGrid.EnemyCanMoveHere())
        {
            options.Add(Constant.QuadDir.Down);
        }

        Grid leftGrid = gm.GetGrid(gridX, gridY, Constant.QuadDir.Left, 1);
        if (leftGrid != null && leftGrid.EnemyCanMoveHere())
        {
            options.Add(Constant.QuadDir.Left);
        }

        Grid rightGrid = gm.GetGrid(gridX, gridY, Constant.QuadDir.Right, 1);
        if (rightGrid != null && rightGrid.EnemyCanMoveHere())
        {
            options.Add(Constant.QuadDir.Right);
        }

        if (options.Count == 0)
        {
            print(this + " no where to move randomly");
        }
        else
        {
            Constant.QuadDir dir = GameManager.RandomElement(options);
            MoveInDirection(dir);
        }
    }

    public void MoveTowardPlayer()
    {
        Player player = GameManager.Instance.player;
        Grid playerGrid = player.GetMyGrid();
        int distanceFromPlayer = DistanceFromGrid(playerGrid);
        if (distanceFromPlayer == 0)
        {
            return;
        }

        int deltaX = playerGrid.gridX - GetMyGrid().gridX;
        int deltaY = playerGrid.gridY - GetMyGrid().gridY;
        bool moveX = Random.Range(0, 2) == 0;
        if (moveX)
        {
            if (deltaX > 0)
            {
                bool moved = MoveInDirection(Constant.QuadDir.Right);
                if (!moved) { moveX = false; }
            }
            else if (deltaX < 0)
            {
                MoveInDirection(Constant.QuadDir.Left);
            }
            else
            {
                moveX = false;
            }
        }
        if (!moveX)
        {
            if (deltaY > 0)
            {
                MoveInDirection(Constant.QuadDir.Up);
            }
            else if (deltaY < 0)
            {
                MoveInDirection(Constant.QuadDir.Down);
            }
        }


    }

    public void AttackPlayer()
    {
        if (DistanceToPlayer() > 1)
        {
            return;
        }
        
        GameManager gm = GameManager.Instance;
        Attack(gm.player);
        Grid playerGrid = gm.player.GetMyGrid();
        if (gm.GetGrid(gridX, gridY, Constant.QuadDir.Up, 1) == playerGrid)
        {
            Face(Constant.QuadDir.Up);
        }
        else if (gm.GetGrid(gridX, gridY, Constant.QuadDir.Down, 1) == playerGrid)
        {
            Face(Constant.QuadDir.Down);
        }
        else if (gm.GetGrid(gridX, gridY, Constant.QuadDir.Left, 1) == playerGrid)
        {
            Face(Constant.QuadDir.Left);
        }
        else if (gm.GetGrid(gridX, gridY, Constant.QuadDir.Right, 1) == playerGrid)
        {
            Face(Constant.QuadDir.Right);
        }
    }

    public void HuntPlayer()
    {
        if (DistanceToPlayer() > 1)
        {
            MoveTowardPlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    public int DistanceToPlayer()
    {
        Player player = GameManager.Instance.player;
        Grid playerGrid = player.GetMyGrid();
        return DistanceFromGrid(playerGrid);

    }

    public virtual void Action()
    {
        MoveRandom();
    }

    public virtual void Bossify()
    {

    }

    public virtual void OnEndTurn()
    {
        turnsUntilAction -= 1;
        if (turnsUntilAction == 0)
        {
            turnsUntilAction = maxTurnsUntilAction;
            Action();
        }
    }

    

    public override void OnDeath(DeathInfo deathInfo)
    {
        DropLoot();
        print(deathInfo.killer + " get " + expReward);
        deathInfo.killer.GainExp(expReward);
        GameManager.Instance.OnTurnEndSubscribers -= OnEndTurn;
    }

    public virtual void DropLoot()
    {
        Grid location = GameManager.Instance.GetGrid(gridX, gridY);
        print("dropping " + lootPrefab + " at " + location);
        GameManager.Instance.SpawnLoot(lootPrefab, location);
    }




}
