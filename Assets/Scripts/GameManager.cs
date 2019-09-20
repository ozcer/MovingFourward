using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Animator anim;
    public GameObject thoughtText;
    public Image thoughtBubble;
    public Color thoughtColor = Color.black;

    public GameObject gridPrefab;
    public GameObject wallPrefab;
    public GameObject skeletonPrefab;
    public GameObject knightPrefab;
    public GameObject monkPrefab;
    public GameObject doorPrefab;
    public static Vector3 origin = new Vector3(0, 0, 0);

    public int maxX = 8;
    public int maxY = 8;
    public int margin = 100;
    public GameObject[,] gridTiles;

    public GameObject playerPrefab;
    public Player player;
    public int spawnPointX = 1;
    public int spawnPointY = 1;
    public float spawnHeight = 0f;

    public GameObject flagPrefab;
    Flag flag;

    public int turn;
    public delegate void OnTurnEnd();
    public event OnTurnEnd OnTurnEndSubscribers;

    //public delegate void OnDeath(DeathInfo deathInfo);
    //public event OnDeath OnDeathSubscribers;

    public Constant.QuadDir roomEntranceDir;

    public int level;
    public int room;

    string[] rTypes = { "Monster", "Monster", "Monster", "Monk" };
    public List<string> roomTypes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        turn = 1;
        roomEntranceDir = Constant.QuadDir.Down;

        level = 1;
        room = 0;

        roomTypes = new List<string>();
        roomTypes.AddRange(rTypes);

    }


    void Start () {
        Door entrance = GenerateRoom(roomEntranceDir);
        SpawnPlayerRandom();
        Grid oneGridFromEntrance = GetGrid(
            entrance.gridX,
            entrance.gridY,
            Constant.Opposite(roomEntranceDir),
            1);
        player.TeleportTo(oneGridFromEntrance);
        
        SpawnSkeletonRandom();
        Think("WASD to move");
    }
	
	// Update is called once per frame
	void Update () {
        Text text = thoughtText.GetComponent<Text>();
        Color color = text.color;
        color.a -= 0.007f;
        text.color = color;
    }


    public Door GenerateRoom(Constant.QuadDir entranceDir)
    {
        print("generating room entrance at " + entranceDir);
        maxX = Random.Range(5, 8);
        maxY = Random.Range(4, 6);
        print("maxX " + maxX + " maxY " + maxY);
        gridTiles = new GameObject[maxX + 1, maxY + 1];
        GenerateGrid();
        SpawnWalls();


        roomEntranceDir = entranceDir;
        if (entranceDir == Constant.QuadDir.Down)
        {
            SpawnDoor(Constant.QuadDir.Right);
            SpawnDoor(Constant.QuadDir.Left);
        }
        else if (entranceDir == Constant.QuadDir.Left)
        {
            SpawnDoor(Constant.QuadDir.Up);
            SpawnDoor(Constant.QuadDir.Right);
        }
        else if (entranceDir == Constant.QuadDir.Right)
        {
            SpawnDoor(Constant.QuadDir.Up);
            SpawnDoor(Constant.QuadDir.Left);
        }
        Door entranceDoor = SpawnEntranceDoor(entranceDir);
        entranceDoor.monsterLocked = false;
        entranceDoor.ChangeColor(Color.black);

        OnTurnEndSubscribers = null;

        return entranceDoor;

    }

    void GenerateGrid()
    {
        for (int i = 0; i <= maxX; i++)
        {
            for (int j = 0; j <= maxY; j++)
            {
                // x and z axis forms the horizontal plane
                Vector3 pos = new Vector3(origin.x + margin * i, origin.y, origin.z + margin * j);
                GameObject grid = Instantiate(gridPrefab, pos, Quaternion.identity);
                grid.transform.SetParent(gameObject.transform);
                grid.GetComponent<Grid>().gridX = i;
                grid.GetComponent<Grid>().gridY = j;

                gridTiles[i, j] = grid;
            }
        }

    }


    public Grid GetGrid(int gridX, int gridY)
    {
        GameManager gm = GameManager.Instance;
        if (0 <= gridX && gridX <= gm.maxX && 0 <= gridY && gridY <= gm.maxY)
        {
            return gm.gridTiles[gridX, gridY].GetComponent<Grid>();
        }
        return null;
    }

    // given src grid, directon and distance, return the Grid, dist grids away, in dir direction
    public Grid GetGrid(int srcGridX, int srcGridY, Constant.QuadDir dir, int dist)
    {
        GameManager gm = GameManager.Instance;
        GameObject[,] grids = gm.gridTiles;
        Grid targGrid = null;
        //print("src " + srcGridX +" "+ srcGridY+" "+ dir);
        // TODO bound check
        if (dir == Constant.QuadDir.Up && srcGridY < maxY)
        {
            targGrid = grids[srcGridX, srcGridY + 1].GetComponent<Grid>();
        }
        else if (dir == Constant.QuadDir.Left && srcGridX >= 1)
        {
            targGrid = grids[srcGridX - 1, srcGridY].GetComponent<Grid>();
        }
        else if (dir == Constant.QuadDir.Down && srcGridY >= 1)
        {
            targGrid = grids[srcGridX, srcGridY - 1].GetComponent<Grid>();
        }
        else if (dir == Constant.QuadDir.Right && srcGridX < maxX)
        {
            targGrid = grids[srcGridX + 1, srcGridY].GetComponent<Grid>();
        }
        else
        {
            Debug.Log("bad direction"); 
        }

        return targGrid;
    }

    Grid GetRandomGrid()
    {
        int randX = Random.Range(0, maxX);
        int randY = Random.Range(0, maxY);
        return GetGridTile(randX, randY).GetComponent<Grid>();
    }

# region Spawning

    void SpawnWalls()
    {

        for (int i = 0; i <= maxX; i++)
        {
            for (int j = 0; j <= maxY; j++)
            {
                // walls on the edges of the map               
                if (i == 0 || i == maxX || j == 0 || j == maxY)
                {
                    SpawnWall(i, j);
                }
            }
        }

    }


    public void SpawnWall(int row, int col)
    {
        GameObject targetGrid = gridTiles[row, col];
        Grid targetGridInfo = targetGrid.GetComponent<Grid>();

        // x and z axis forms the horizontal plane
        Vector3 pos = gridTiles[row, col].transform.position;
        pos.y += 1;
        GameObject wallObject = Instantiate(wallPrefab, pos, Quaternion.identity);
        Wall wall = wallObject.GetComponent<Wall>();
        wallObject.transform.SetParent(targetGrid.transform);
        targetGridInfo.AddOccupant(wall);
    }


    // Spawn door on an existing wall
    public Door SpawnDoor(Constant.QuadDir spawnSide) {
        int x, y;
        // spawn on up side(north)
        if (spawnSide == Constant.QuadDir.Up)
        {
            x = Random.Range(1, maxX - 1); //cannot spawn on corners
            y = maxY;

        }
        else if (spawnSide == Constant.QuadDir.Down)
        {
            x = Random.Range(1, maxX - 1);
            y = 0;
        }
        else if (spawnSide == Constant.QuadDir.Left)
        {
            x = 0;
            y = Random.Range(1, maxY - 1);
        }
        else if (spawnSide == Constant.QuadDir.Right)
        {
            x = maxX;
            y = Random.Range(1, maxY - 1);
        }
        else
        {
            throw new System.Exception("bad dir");
        }
        Grid grid = GetGrid(x, y);
        Wall wall = (Wall) grid.GetOccupantWithTag("Wall"); // destory the wall above the chosen grid
        wall.Suicide();

        Vector3 pos = gridTiles[x, y].transform.position;
        Quaternion rotation = Quaternion.identity;
        if (spawnSide == Constant.QuadDir.Up || spawnSide == Constant.QuadDir.Down)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }
        GameObject doorObject = Instantiate(doorPrefab, pos, rotation);
        Door door = doorObject.GetComponent<Door>();
        door.gridX = x;
        door.gridY = y;
        door.side = spawnSide;
        door.type = RandomElement(roomTypes);
        door.MonsterLock();

        if (door.type == "Monk")
        {
            door.ChangeColor(Color.magenta);
        }
        doorObject.transform.SetParent(grid.transform);
        grid.AddOccupant(door);
        
        return door;

    }

    public Door SpawnEntranceDoor(Constant.QuadDir spawnSide)
    {
        print("spawning entrance at " + spawnSide);
        Door entranceDoor = SpawnDoor(spawnSide);
        return entranceDoor;
    }
    public void SpawnPlayerRandom()
    {
        Grid emptyGrid = GetRandomEmptyGrid();
        if (emptyGrid == null)
        {
            print("No grid to spawn player");
            return;
        }
        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject playerObject = Instantiate(playerPrefab, dest, Quaternion.identity);
        player = playerObject.GetComponent<Player>();
        player.gridX = emptyGrid.gridX;
        player.gridY = emptyGrid.gridY;
        playerObject.transform.SetParent(gameObject.transform);
        emptyGrid.AddOccupant(player);
    }

    public void SpawnFlagRandom()
    {
        print("getting empty block for flag");
        Grid emptyGrid = GetRandomEmptyGrid();
        if (emptyGrid == null)
        {
            print("No grid to spawn flag");
            return;
        }

        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject flagObject = Instantiate(flagPrefab, dest, Quaternion.identity);
        flag = flagObject.GetComponent<Flag>();

        flag.gridX = emptyGrid.gridX;
        flag.gridY = emptyGrid.gridY;

        emptyGrid.AddOccupant(flag);
    }

    public void SpawnSkeletonRandom()
    {
        print("getting empty block for ene");
        Grid emptyGrid = GetRandomEmptyGrid();
        if (emptyGrid == null)
        {
            print("No grid to spawn enemy");
            return;
        }
        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject enemyObject = Instantiate(skeletonPrefab, dest, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.gridX = emptyGrid.gridX;
        enemy.gridY = emptyGrid.gridY;
        print("random spawned enemy " + enemy);
        enemyObject.transform.SetParent(gameObject.transform);
        emptyGrid.AddOccupant(enemy);
    }

    public void SpawnKnightRandom()
    {
        print("getting empty block for ene");
        Grid emptyGrid = GetRandomEmptyGrid();
        if (emptyGrid == null)
        {
            print("No grid to spawn enemy");
            return;
        }
        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject enemyObject = Instantiate(knightPrefab, dest, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.gridX = emptyGrid.gridX;
        enemy.gridY = emptyGrid.gridY;
        print("random spawned enemy " + enemy);
        enemyObject.transform.SetParent(gameObject.transform);
        emptyGrid.AddOccupant(enemy);
    }

    public void SpawnSkeletonBoss()
    {
        Grid emptyGrid = GetRandomEmptyGrid();
        if (emptyGrid == null)
        {
            print("No grid to spawn enemy");
            return;
        }
        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject enemyObject = Instantiate(skeletonPrefab, dest, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.gridX = emptyGrid.gridX;
        enemy.gridY = emptyGrid.gridY;
        print("random spawned enemy " + enemy);
        enemyObject.transform.SetParent(gameObject.transform);
        emptyGrid.AddOccupant(enemy);

        enemy.Bossify();
        Think("BIG SKELETON!!");

    }

    public void SpawnKnightBoss()
    {
        Grid emptyGrid = GetRandomEmptyGrid();
        if (emptyGrid == null)
        {
            print("No grid to spawn enemy");
            return;
        }
        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject enemyObject = Instantiate(knightPrefab, dest, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.gridX = emptyGrid.gridX;
        enemy.gridY = emptyGrid.gridY;
        print("random spawned enemy " + enemy);
        enemyObject.transform.SetParent(gameObject.transform);
        emptyGrid.AddOccupant(enemy);

        enemy.Bossify();
        Think("Knight legion!!");
    }

    public void SpawnMonkRandom()
    {
        Grid emptyGrid = GetRandomEmptyGrid(2);
        if (emptyGrid == null)
        {
            print("No grid to spawn monk");
            return;
        }
        Vector3 dest = emptyGrid.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason
        dest.y += spawnHeight;

        GameObject monkObject = Instantiate(monkPrefab, dest, Quaternion.identity);
        Monk monk = monkObject.GetComponent<Monk>();
        monk.gridX = emptyGrid.gridX;
        monk.gridY = emptyGrid.gridY;
        print("random spawned monk " + monk);
        monkObject.transform.SetParent(gameObject.transform);
        emptyGrid.AddOccupant(monk);
    }
    #endregion

    public void RestartScene()
    {
        SceneManager.LoadScene("Level1");
    }


    public void EnterDoor(Door door)
    {
        anim.SetBool("Fade", true);
        print("fade set to true");
        
        StartCoroutine(FadeIntoNewRoom(door));
    }

    IEnumerator FadeIntoNewRoom(Door door)
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag != "Player")
            {
                Destroy(child.gameObject);
            }
        }
        UpdateToNextLevel();
        Constant.QuadDir entranceSide = Constant.Opposite(door.side);
        Door entranceDoor = GenerateRoom(entranceSide);
        //print("welcome mat should be 1 block " + entranceSide + " of " + GetGrid(entranceDoor.gridX, entranceDoor.gridY));
        Grid oneGridFromEntrance = GetGrid(
            entranceDoor.gridX,
            entranceDoor.gridY,
            Constant.Opposite(entranceSide),
            1);
        player.TeleportTo(oneGridFromEntrance);

        // always fight monster(boss) on room 4
        if (door.type == "Monster" || room == 4)
        {
            
            int k = Random.Range(0, 2);
            if (k == 0) // fight skeleys
            {
                if (room == 4) SpawnSkeletonBoss();
                else
                {
                    for (int i = 0; i < level; i++)
                    {
                        SpawnSkeletonRandom();
                    }
                }
            }
            else if (k == 1) // fight knights
            {
                if (room == 4) SpawnKnightBoss();
                else
                {
                    for (int i = 0; i < level; i++)
                    {
                        SpawnKnightRandom();
                    }
                }
            }
        }
        else if (door.type == "Monk")
        {
            SpawnMonkRandom();
        }


        anim.SetBool("Fade", false);
    }

    public void UpdateToNextLevel()
    {
        room += 1;
        if (room > 4)
        {
            level += 1;
            room = 0;
        }
    }

    public void Think(string thought)
    {
        thoughtText.GetComponent<Text>().text = thought;
        thoughtText.GetComponent<Text>().color = thoughtColor;
        //FadeOut(thoughtText.GetComponent<Text>());
        thoughtBubble.GetComponent<Animator>().Play("100alpha");
        //StartCoroutine(FadeOut(thoughtText.GetComponent<Text>()));
    }


    //public IEnumerator FadeOut(Text text)
    //{
    //    yield return new WaitForSeconds(1f);
    //   StartCoroutine(FadeOutRoutine(text));
    //}
    //private IEnumerator FadeOutRoutine(Text text)
    //{
    //    Color originalColor = text.color;
    //    for (float t = 0.01f; t < 1; t += Time.deltaTime)
    //    {
    //        text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / 1));
    //        yield return null;
    //    }
    //}
    public List<Grid> GetEmptyGrids()
    {
        List<Grid> result = new List<Grid>();
        GameObject grid;
        Grid gridInfo;
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                grid = gridTiles[i, j];
                gridInfo = grid.GetComponent<Grid>();
                if (gridInfo.CanMoveTo())
                {
                    result.Add(gridInfo);
                }
            }
        }
        return result;
    }

    public List<Grid> GetEmptyGrids(int margin)
    {
        List<Grid> result = new List<Grid>();
        GameObject grid;
        Grid gridInfo;
        for (int i = margin; i <= maxX - margin; i++)
        {
            for (int j = margin; j <= maxY - margin; j++)
            {
                grid = gridTiles[i, j];
                gridInfo = grid.GetComponent<Grid>();
                if (gridInfo.CanMoveTo())
                {
                    result.Add(gridInfo);
                }
            }
        }
        return result;
    }

    public int GetMonsterCount()
    {

        Grid grid;
        int count = 0;
        for (int i = margin; i <= maxX - margin; i++)
        {
            for (int j = margin; j <= maxY - margin; j++)
            {
                grid = GetGrid(i, j);
                if (grid.GetOccupantWithTag("Enemy") != null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public GameObject GetGridTile(int x, int y)
    {
        return gridTiles[x, y];
    }

    public Grid GetRandomEmptyGrid()
    {
        List<Grid> emptyGrids = GetEmptyGrids();
        if (emptyGrids.Count == 0)
        {
            return null;
        }
        int randIndex = Random.Range(0, emptyGrids.Count - 1);
        return emptyGrids[randIndex];
    }

    public Grid GetRandomEmptyGrid(int margin)
    {
        List<Grid> emptyGrids = GetEmptyGrids(margin);
        if (emptyGrids.Count == 0)
        {
            return null;
        }
        int randIndex = Random.Range(0, emptyGrids.Count - 1);
        return emptyGrids[randIndex];
    }

    public void EndTurn()
    {
        turn += 1;
        if (OnTurnEndSubscribers != null)
        {
            OnTurnEndSubscribers();
        }
    }

    public void SpawnLoot(GameObject lootPrefab, Grid location)
    {
        Vector3 dest = location.transform.position;
        // spawn a bit higher or it pushes nearby blocks for some reason

        GameObject lootObject = Instantiate(lootPrefab, dest, Quaternion.identity);
        lootObject.transform.SetParent(gameObject.transform);
        Collectible loot = lootObject.GetComponent<Collectible>();
        //loot.gridX = location.gridX;
        //loot.gridY = location.gridY;

        //location.AddOccupant(loot);
    }

    public static T RandomElement<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }

        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

}
