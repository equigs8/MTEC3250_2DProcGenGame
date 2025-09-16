using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator inst;

    [HideInInspector] public int rows;
    [HideInInspector] public int columns;
    public Tile[,] tiles;
    public GameObject tilePrefab;

    private Vector3 originPos;

    private int blockCount;
    private int trapTileCount;
    private int crateCount;

    private List<Tile> trapTiles = new List<Tile>();
    private List<Tile> inaccessibleTiles = new List<Tile>();
    private List<Tile> crates = new List<Tile>();

    [HideInInspector]public Tile goalTile;


    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        originPos = GameProperties.inst.gridOriginPosition;
        rows = GameProperties.inst.gridColumns;
        columns = GameProperties.inst.gridRows;

        blockCount = GameProperties.inst.blockCount;
        trapTileCount = GameProperties.inst.trapTileCount;
        crateCount = GameProperties.inst.crateCount;

        tiles = new Tile[rows, columns];

        MakeGrid();
        
    }

    private void MakeGrid()
    {
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                float sizeX = tilePrefab.GetComponent<SpriteRenderer>().size.x;
                float sizeY = tilePrefab.GetComponent<SpriteRenderer>().size.y;
                Vector2 pos = new Vector3(originPos.x + sizeX * r, originPos.y + sizeY * c,0);

                GameObject o = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                Tile t = o.GetComponent<Tile>();

                tiles[r, c] = t;
                tiles[r, c].name = "[" + r.ToString() + "," + c.ToString() + "]";

                //I think colomn and row are reversed

                t.row = r;
                t.column = c;

                t.Init(Type.DEFAULT);
            }

        }

        for (int i = 0; i < blockCount; i++)
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


    public Vector3 GetTilePosition(int r, int c)
    {
        return tiles[r, c].transform.position;

    }
    public Vector3 GetTilePosition(Tile t)
    {
        return t.transform.position;

    }


    private void AddTraps()
    {
        Tile t = GetRandomTile();

        while (t == tiles[0,0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t))
        {
            t = GetRandomTile();
        }


        trapTiles.Add(t);
        t.Init(Type.TRAP);
;

    }

    private void AddHoles()
    {
        Tile t = GetRandomTile();

        while (t == tiles[0, 0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t))
        {
            t = GetRandomTile();
        }

        inaccessibleTiles.Add(t);
        t.Init(Type.BLOCK);

    }

    private void AddCrates()
    {

        Tile t = GetRandomTile();

        while (t == tiles[0, 0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t)|| crates.Contains(t))
        {
            t = GetRandomTile();
        }
        crates.Add(t);
        t.Init(Type.CRATE);

    }

    private void AddGoalTile()
    {
        Tile t = GetRandomTileInRange(rows - 1, rows, columns - 1, columns);
        while (t == tiles[0, 0] || inaccessibleTiles.Contains(t) || trapTiles.Contains(t) || crates.Contains(t))
        {
            t = GetRandomTileInRange(rows - 2, rows, 0, columns);
        }

        goalTile = t;
        t.Init(Type.GOAL);
    }


    private Tile GetRandomTile()
    {
        return tiles[Random.Range(0, rows), Random.Range(0, columns)];
    }

    private Tile GetRandomTileInRange(int minX,int maxX,int minY, int maxY)
    {
        if (minX < 0) minX = 0;
        if (maxX > rows) maxX = rows;
        if (minY < 0) minY = 0;
        if (maxY > columns) maxY = columns;
        return tiles[Random.Range(minX, maxX), Random.Range(minY, maxY)];
    }

}
