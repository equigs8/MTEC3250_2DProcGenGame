using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator inst;

    // How big the gird is  (how many rows and columns)
    public int rows;
    public int columns;
    public Tile[,] tiles;

    // Tile prefab were going to use to make the grid
    public GameObject tilePrefab;

    //Origin tile position , All subsequent tiles will be positioned based on this one
    //Origin tile is [0,0];
    public Vector3 originPos = new Vector3(-3, -3, 0);

    [Range(0, 10)] public int holeCount;
    [Range(0, 10)] public int trapTileCount;
    [Range(0, 30)] public int crateCount;

    public List<Tile> trapTiles = new List<Tile>();
    private List<Tile> inaccessibleTiles = new List<Tile>();
    public List<Tile> crates = new List<Tile>();

    [HideInInspector]public Tile goalTile;


    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        //Initilize 2D array
        tiles = new Tile[rows, columns];

        //Call code that makes the grid
        MakeGrid();
        
    }

    private void MakeGrid()
    {
        //Nested for loops to create the rows and columns
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                //Here we want to get the size of the Tile sprite so that he can place them side by side
                float sizeX = tilePrefab.GetComponent<SpriteRenderer>().size.x;
                float sizeY = tilePrefab.GetComponent<SpriteRenderer>().size.y;
                Vector2 pos = new Vector3(originPos.x + sizeX * r, originPos.y + sizeY * c,0);

                //Here we Instantiate the GameObject and then immediately get a reference to it's Tile script.
                GameObject o = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                Tile t = o.GetComponent<Tile>();

                //We make sure to set the newly created tile in the appropriate slot in the 2D array and then name it accordingly
                tiles[r, c] = t;
                tiles[r, c].name = "[" + r.ToString() + "," + c.ToString() + "]";

                t.Init(Type.DEFAULT);
            }

        }

        // We run some for loops after making the Grid to set any specific tiles.
        for (int i = 0; i < holeCount; i++)
        {
            AddHoles();
        }
        for (int i = 0; i < trapTileCount; i++)
        {
            AddTraps();
        }
        for (int i = 0; i < crateCount; i++)
        {
            AddCrates();
        }

        AddGoalTile();
    }

    //If we ever need the position for a tile, we can get it from one of these two functions.
    //The first one is for getting a position using the row anf column index
    public Vector3 GetTilePosition(int r, int c)
    {
        return tiles[r, c].transform.position;

    }
    //The second one is for getting a position using the tile itself
    public Vector3 GetTilePosition(Tile t)
    {
        return t.transform.position;

    }


    private void AddTraps()
    {
        //We get a random tile 
        Tile t = GetRandomTile();

        //We check that it isnt already been inluded as either a trap or Hole and that it doesnt set the player's start position 
        //as a trap. We do this by checking that, while the tile is either the origin tile, a hole or a trap, we keep getting a new tile

        while (t == tiles[0,0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t))
        {
            t = GetRandomTile();
        }

        //...when we break out of the while loop, it means what the random tile selected fulfills the above criteria
        //So we add it to the appropriate list, color it and set the appropriate bool to true
        trapTiles.Add(t);
        t.Init(Type.TRAP);
        //t.AdjustColor(Color.red);
        //t.isTrap = true;

    }

    private void AddHoles()
    {
        //We get a random tile 
        Tile t = GetRandomTile();

        //We check that it isnt already been inluded as either a trap or Hole and that it doesnt set the player's start position 
        //as a hole. We do this by checking that, while the tile is either the origin tile, a hole or a trap, we keep getting a new tile

        while (t == tiles[0, 0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t))
        {
            t = GetRandomTile();
        }

        //...when we break out of the while loop, it means what the random tile selected fulfills the above criteria
        //So we add it to the appropriate list, color it and set the appropriate bool to true
        inaccessibleTiles.Add(t);
        t.Init(Type.BLOCK);
        //t.AdjustColor(Color.black);
        //t.isInaccessible = true;
    }

    private void AddCrates()
    {
        //We get a random tile 
        Tile t = GetRandomTile();

        //We check that it isnt already been inluded in the other tile lists and that it doesnt set the player's start position 
        //as a crate. We do this by checking that, while the tile is either the origin tile, a hole, a trap, or a crate we keep getting a new tile

        while (t == tiles[0, 0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t)|| crates.Contains(t))
        {
            t = GetRandomTile();
        }
        crates.Add(t);
        t.Init(Type.CRATE);
        //t.AdjustColor(Color.yellow);
        //t.isCrate = true;
        //t.isInaccessible = true;

    }

    private void AddGoalTile()
    {
        //this time we call a different method, get random in tile within a rangle of rows and columns, to get a random tile from the far side of the map
        Tile t = GetRandomTileInRange(rows - 1, rows, columns - 1, columns);
        while (t == tiles[0, 0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t) || crates.Contains(t))
        {
            t = GetRandomTileInRange(rows - 2, rows, 0, columns);
        }

        //...when we break out of the while loop, it means what the random tile selected fulfills the above criteria
        //So we add it to the appropriate list, color it and set the appropriate bool to true
        goalTile = t;
        t.Init(Type.GOAL);
        //t.AdjustColor(Color.green);
        //t.isGoal = true;

    }


    private Tile GetRandomTile()
    {
        //This returns a random tile from the 2D array but using a random row and random column index
        return tiles[Random.Range(0, rows), Random.Range(0, columns)];



    }

    private Tile GetRandomTileInRange(int minX,int maxX,int minY, int maxY)
    {
        //This returns a random tile from the 2D array but using random rows and columns within a range
        if (minX < 0) minX = 0;
        if (maxX > rows) maxX = rows;
        if (minY < 0) minY = 0;
        if (maxY > columns) maxY = columns;
        return tiles[Random.Range(minX, maxX), Random.Range(minY, maxY)];
    }




}
