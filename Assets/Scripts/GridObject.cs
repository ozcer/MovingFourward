using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour {
    public int gridX;
    public int gridY;
    public float moveSpeed = 3f;
    public float turnSpeed = 5f;
    public Vector3 targetDir;
    public Vector3 dest;

    public int health = 420;
    public int maxHealth = 420;
    public int attack = 1;

    public int level = 1;
    public int exp = 0;
    public int maxExp = 1;

    public bool isSolid;
    public Animator anim;

    public GameObject spotlightPrefab;

    public virtual void Awake()
    {
        isSolid = false;
    }

    // Use this for initialization
    protected void Start () {
        dest = transform.position;
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("Moving", false);
        }
    }
	
	// Update is called once per frame
	public void Update () {
        UpdateTowardDest();
        UpdateRotation();
	}


    

    public bool MoveInDirection(Constant.QuadDir dir)
    {
        GameObject[,] grids = GameManager.Instance.gridTiles;
        Grid destGrid = null;
        if (dir == Constant.QuadDir.Up)
        {
            destGrid = grids[gridX, gridY + 1].GetComponent<Grid>();
        }
        else if (dir == Constant.QuadDir.Left)
        {
            destGrid = grids[gridX - 1, gridY].GetComponent<Grid>();
        }
        else if (dir == Constant.QuadDir.Down)
        {
            destGrid = grids[gridX, gridY - 1].GetComponent<Grid>();
        }
        else if (dir == Constant.QuadDir.Right)
        {
            destGrid = grids[gridX + 1, gridY].GetComponent<Grid>();
        }

        if (destGrid && destGrid.CanMoveTo())
        {
            MoveToGrid(destGrid);

        }
        else
        {
            Debug.LogFormat("{0} tried to move {1} but cannot move to", this, dir);
            return false;
        }
        Face(dir);
        return true;
    }

    public void MoveToGrid(Grid targetGrid)
    {
        Grid currentGrid = GetMyGrid();
        if (currentGrid != null)
        {
            currentGrid.RemoveOccupant(this);
        }
        targetGrid.AddOccupant(this);

        this.dest = targetGrid.transform.position;
        this.gridX = targetGrid.gridX;
        this.gridY = targetGrid.gridY;
    }


    public void UpdateTowardDest()
    {
        // update position to move closer to dest
        // only move to the dest within the horizontal plane
        // aka projection of dest onto the horizontal plane
        Vector3 destPlanePos = new Vector3(
            dest.x,
            transform.position.y,
            dest.z);
        if (Vector3.Distance(transform.position, destPlanePos) > 0.1f)
        {
            if (anim != null)
            {
                //print("anima moving");
                anim.SetBool("Moving", true);
            }
            float moveStep = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(
                transform.position,
                destPlanePos,
                moveStep);
            

        }
        else
        {
            if (anim != null)
            {
                //print("anima idling");
                anim.SetBool("Moving", false);
            }
        }

        
    }

    public void UpdateRotation()
    {
        // update rotation to face targetDir
        float rotateStep = turnSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateStep, 0.0f);
        //Debug.DrawRay(transform.position, targetDir, Color.red);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    public void Attack(GridObject target)
    {
        anim.Play("Attack");
        target.TakeDamage(this, attack);
        Debug.LogFormat("{0} attacked {1} for {2} dmg", this, target, attack);
    }

    public void Suicide()
    {
        Grid location = GetMyGrid();
        DeathInfo di = new DeathInfo(location, this, this);
        Die(di);
    }

    public virtual void Die(DeathInfo deathInfo)
    {
        OnDeath(deathInfo);
        deathInfo.location.RemoveOccupant(deathInfo.victim);
        Destroy(gameObject);
    }

    public virtual void OnDeath(DeathInfo deathInfo)
    {
        
    }


    public Grid GetMyGrid()
    {
        Grid grid = GameManager.Instance.GetGrid(gridX, gridY);
        return grid;
    }

    public void TeleportTo(Grid location)
    {
        Vector3 teleportLocation = location.transform.position;
        teleportLocation.y = transform.position.y;
        transform.position = teleportLocation;
        MoveToGrid(location);
    }

    public void Face(Constant.QuadDir dir)
    {
        Vector3 targetPos = transform.position;
        if (dir == Constant.QuadDir.Up)
        {
            targetPos.z += 1;
        }
        else if (dir == Constant.QuadDir.Left)
        {
            targetPos.x -= 1;
        }
        else if (dir == Constant.QuadDir.Down)
        {
            targetPos.z -= 1;
        }
        else if (dir == Constant.QuadDir.Right)
        {
            targetPos.x += 1;
        }

        Vector3 targetDir = targetPos - transform.position;
        this.targetDir = targetDir;
    }

    public void GiveSpotlight()
    {
        GameObject spotlightObject = Instantiate(spotlightPrefab, transform.position, Quaternion.identity);
        spotlightObject.transform.SetParent(transform);
        Light spotlight = spotlightObject.GetComponent<Light>();
    }

    public void TakeDamage(GridObject src, int amount)
    {
        anim.Play("GetHit");
        health -= amount;
        if (health <= 0)
        {
            Grid location = GameManager.Instance.GetGrid(gridX, gridY);
            DeathInfo deathInfo = new DeathInfo(location, src, this);
            Die(deathInfo);
        }
    }

    public override string ToString()
    {
        return string.Format("[{0} at {1}, {2}]", this.GetType().Name, gridX, gridY);
    }

    public int DistanceFromGrid(Grid targetGrid)
    {
        Grid myGrid = GetMyGrid();
        int deltaX = Mathf.Abs(targetGrid.gridX - myGrid.gridX);
        int deltaY = Mathf.Abs(targetGrid.gridY - myGrid.gridY);
        return deltaX + deltaY;
    }

    public void GainExp(int amount)
    {
        if (exp + amount >= maxExp)
        {
            int remainder = maxExp - exp;
            LevelUp();
            GainExp(amount - remainder);
            
        }
        else
        {
            exp += amount;
        }
    }

    public virtual void LevelUp()
    {
        level += 1;
		exp = 0;
    }


    public void ChangeColor(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
