using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Enemy {

    public Constant.QuadDir chargeDir;
    public bool blocking;

    public void Awake()
    {
        base.Awake();
        int lv = GameManager.Instance.level;
        health = 69 + lv * 3;
        attack = 20 + lv * 2;
        maxTurnsUntilAction = 2;
        if (lv >= 4)
        {
            maxTurnsUntilAction -= 1;
        }
        turnsUntilAction = maxTurnsUntilAction;

        blocking = false;
        expReward = 30;
    }

    // Use this for initialization
    public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
    }

    public override void Action()
    {
        Player player = GameManager.Instance.player;
        if (blocking)
        {
            Charge();
        }
        else
        {
            if (player.gridY == gridY && DistanceToPlayer() > 1)
            {
                print("charge horizontally");
                if (player.gridX > gridX)
                {
                    PrepCharge(Constant.QuadDir.Right);
                }
                else
                {
                    PrepCharge(Constant.QuadDir.Left);
                }
            }
            else if (player.gridX == gridX && DistanceToPlayer() > 1)
            {
                print("charge vertically");
                if (player.gridY > gridY)
                {
                    PrepCharge(Constant.QuadDir.Up);
                }
                else
                {
                    PrepCharge(Constant.QuadDir.Down);
                }
            }
            else
            {
                HuntPlayer();
            }
        }

    }

    public void PrepCharge(Constant.QuadDir dir)
    {
        chargeDir = dir;
        Face(dir);
        anim.Play("Shield");
        blocking = true;
    }

    public void Charge()
    {
        print("CHARGE");
        moveSpeed *= 2;
        Grid dest = GetMyGrid();
        Grid targetGrid;
        GameManager gm = GameManager.Instance;
        Player target = null;
        if (chargeDir == Constant.QuadDir.Up)
        {
            for (int y = gridY + 1; y <= gm.maxY; y++)
            {
                targetGrid = gm.GetGrid(gridX, y);
                if (!targetGrid.EnemyCanMoveHere())
                {
                    target = (Player)targetGrid.GetOccupantWithTag("Player");
                    dest = gm.GetGrid(gridX, y - 1);
                    break;
                }
            }

        }
        else if (chargeDir == Constant.QuadDir.Down)
        {
            for (int y = gridY - 1; y >= 0; y--)
            {
                targetGrid = gm.GetGrid(gridX, y);
                if (!targetGrid.EnemyCanMoveHere())
                {
                    target = (Player)targetGrid.GetOccupantWithTag("Player");
                    dest = gm.GetGrid(gridX, y + 1);
                    break;
                }
            }

        }
        else if (chargeDir == Constant.QuadDir.Left)
        {
            for (int x = gridX - 1; x >= 0; x--)
            {
                targetGrid = gm.GetGrid(x, gridY);
                if (!targetGrid.EnemyCanMoveHere())
                {
                    target = (Player)targetGrid.GetOccupantWithTag("Player");
                    dest = gm.GetGrid(x + 1, gridY);
                    break;
                }
            }

        }
        else if (chargeDir == Constant.QuadDir.Right)
        {
            for (int x = gridX + 1; x <= gm.maxX; x++)
            {
                targetGrid = gm.GetGrid(x, gridY);
                if (!targetGrid.EnemyCanMoveHere())
                {
                    target = (Player)targetGrid.GetOccupantWithTag("Player");
                    dest = gm.GetGrid(x - 1, gridY);
                    break;
                }
            }

        }
        else
        {
            throw new System.Exception("Bad diretion");
        }

        MoveToGrid(dest);
        Face(chargeDir);
        blocking = false;
        anim.Play("idle");
        StartCoroutine(ChargeAt(target));
    }

    IEnumerator ChargeAt(Player target)
    {
        yield return new WaitForSeconds(0.3f);
        moveSpeed /= 2;
        if (target != null)
        {
            attack *= 2;
            Attack(target);
            attack /= 2;
        }
    }

    public override void OnEndTurn()
    {
        turnsUntilAction -= 1;
        if (turnsUntilAction == 0)
        {
            Action();
            if (blocking)
            {
                turnsUntilAction = 1;
            }
            else
            {
                turnsUntilAction = maxTurnsUntilAction;
            }
            
        }
    }

    public override void Bossify()
    {
        print("SUPER KNIGHT!");
        transform.localScale = new Vector3(0.6f, 0.6f, 0.7f);
        maxTurnsUntilAction = 2;
        turnsUntilAction = maxTurnsUntilAction;
        health *= 2;
        attack *= 2;
        expReward *= 3;
    }
}
