using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using NUnit.Framework;

public enum Type
{
    DEFAULT,
    CRATE,
    TRAP,
    BLOCK,
    GOAL
}
public enum TileLocation
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
    CENTER
}

public class Tile : MonoBehaviour
{
    public Type type;

    private GridGenerator grid;

    private Color tileColor;
    private Color defaultTileColor;

    private Sprite tileSprite;
    private Sprite defaultSprite;
    private Sprite squareSprite;

    private RuntimeAnimatorController tileAnimator;
    private RuntimeAnimatorController defaultAnimator;

    private SpriteRenderer rend;

    //For storing the row and column of the tile (for easy identification)
    public int row;
    public int column;

    public TileLocation tileLocation;

    //bools for setting tile properties 
    public bool isTrap;
    public bool isCrate;
    public bool isInaccessible;
    public bool isGoal;
    public bool isChecked;
    private VisualProperties visuals;

    private AudioClip projectileImpact;
    private AudioClip crateDestroyed;

    public static event Action<Tile> CrateDestroyed;
    public static event Action<Vector3> ProjectileHit;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        squareSprite = rend.sprite;
    }

    public void Init(Type _type)
    {
        isChecked = false;
        grid = GridGenerator.inst;
        visuals = VisualProperties.inst;

        projectileImpact = Sounds.inst.projectileImpact;
        crateDestroyed = Sounds.inst.crateDestroyed;

        type = _type;

        defaultAnimator = visuals.tileVisuals.animController != null ? visuals.tileVisuals.animController : null;
        defaultSprite = visuals.tileVisuals.sprite != null ? visuals.tileVisuals.sprite : null;
        defaultTileColor = visuals.tileVisuals.color;

        switch (type)
        {
            case Type.DEFAULT:
                ResetVisuals();
                SetUpTileVisuals(defaultSprite, visuals.tileVisuals.topLeftCorner, visuals.tileVisuals.topRightCorner, visuals.tileVisuals.bottomLeftCorner, visuals.tileVisuals.bottomRightCorner, visuals.tileVisuals.bottom, visuals.tileVisuals.top, visuals.tileVisuals.left, visuals.tileVisuals.right);
                //DisableBackgroundTile();
                SetUpBackground(defaultSprite, visuals.tileVisuals.topLeftCorner, visuals.tileVisuals.topRightCorner, visuals.tileVisuals.bottomLeftCorner, visuals.tileVisuals.bottomRightCorner, visuals.tileVisuals.bottom, visuals.tileVisuals.top, visuals.tileVisuals.left, visuals.tileVisuals.right);
                rend.sortingOrder = 1;
                break;

            case Type.CRATE:
                ResetVisuals();
                SetUpVisuals(visuals.crateVisuals.animController, visuals.crateVisuals.sprite, visuals.crateVisuals.color);
                SetUpBackground(defaultSprite, visuals.tileVisuals.topLeftCorner, visuals.tileVisuals.topRightCorner, visuals.tileVisuals.bottomLeftCorner, visuals.tileVisuals.bottomRightCorner, visuals.tileVisuals.bottom, visuals.tileVisuals.top, visuals.tileVisuals.left, visuals.tileVisuals.right);
                rend.sortingOrder = 2;
                isCrate = true;
                isInaccessible = true;
                break;

            case Type.TRAP:
                ResetVisuals();
                SetUpVisuals(visuals.trapVisuals.animController, visuals.trapVisuals.sprite, visuals.trapVisuals.color);
                SetUpBackground(defaultSprite, visuals.tileVisuals.topLeftCorner, visuals.tileVisuals.topRightCorner, visuals.tileVisuals.bottomLeftCorner, visuals.tileVisuals.bottomRightCorner, visuals.tileVisuals.bottom, visuals.tileVisuals.top, visuals.tileVisuals.left, visuals.tileVisuals.right);
                rend.sortingOrder = 2;
                isTrap = true;
                break;

            case Type.BLOCK:
                ResetVisuals();
                SetUpVisuals(visuals.blockVisuals.animController, visuals.blockVisuals.sprite, visuals.blockVisuals.color);
                SetUpBackground(defaultSprite, visuals.tileVisuals.topLeftCorner, visuals.tileVisuals.topRightCorner, visuals.tileVisuals.bottomLeftCorner, visuals.tileVisuals.bottomRightCorner, visuals.tileVisuals.bottom, visuals.tileVisuals.top, visuals.tileVisuals.left, visuals.tileVisuals.right);
                rend.sortingOrder = 2;
                isInaccessible = true;
                break;

            case Type.GOAL:
                ResetVisuals();
                SetUpVisuals(visuals.goalVisuals.animController, visuals.goalVisuals.sprite, visuals.goalVisuals.color);
                SetUpBackground(defaultSprite, visuals.tileVisuals.topLeftCorner, visuals.tileVisuals.topRightCorner, visuals.tileVisuals.bottomLeftCorner, visuals.tileVisuals.bottomRightCorner, visuals.tileVisuals.bottom, visuals.tileVisuals.top, visuals.tileVisuals.left, visuals.tileVisuals.right);
                rend.sortingOrder = 2;
                isGoal = true;
                break;
        }
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (isCrate)
            {
                ProjectileHit?.Invoke(collision.transform.position);
                Destroy(collision.gameObject);
                AudioManager.inst.PlaySound(projectileImpact, Sounds.inst.projectileImpactVolume);
                DestroyCrate();
            }
            else if (isInaccessible)
            {
                AudioManager.inst.PlaySound(projectileImpact, Sounds.inst.projectileImpactVolume);
                ProjectileHit?.Invoke(collision.transform.position);
                Destroy(collision.gameObject);
            }
        }
    }

    public void DestroyCrate()
    {
        if (!isCrate) return;

        AudioManager.inst.PlaySound(crateDestroyed, Sounds.inst.crateDestroyedVolume);

        Init(Type.DEFAULT);

        CrateDestroyed?.Invoke(this);


        isCrate = false;
        isInaccessible = false;
    }



    private void DisableBackgroundTile()
    {
        SpriteRenderer backgroundRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
        backgroundRenderer.enabled = false;
    }
    private void SetUpBackground(Sprite sprite, Sprite topLeft, Sprite topRight, Sprite bottomLeft, Sprite bottomRight, Sprite bottom, Sprite top, Sprite left, Sprite right)
    {
        SpriteRenderer backgroundRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
        //Debug.Log("Background Renderer: " + backgroundRenderer.gameObject.name);
        tileLocation = getTileLocation();

        switch (tileLocation)
        {
            case TileLocation.TOP_LEFT:
                backgroundRenderer.sprite = topLeft;
                break;
            case TileLocation.TOP_RIGHT:
                backgroundRenderer.sprite = topRight;
                break;
            case TileLocation.BOTTOM:
                backgroundRenderer.sprite = bottom;
                break;
            case TileLocation.TOP:
                backgroundRenderer.sprite = top;
                break;
            case TileLocation.LEFT:
                backgroundRenderer.sprite = left;
                break;
            case TileLocation.RIGHT:
                backgroundRenderer.sprite = right;
                break;
            case TileLocation.BOTTOM_LEFT:
                backgroundRenderer.sprite = bottomLeft;
                break;
            case TileLocation.BOTTOM_RIGHT:
                backgroundRenderer.sprite = bottomRight;
                break;
            case TileLocation.CENTER:
                backgroundRenderer.sprite = sprite;
                break;
            default:
                backgroundRenderer.sprite = sprite;
                break;
        }

        backgroundRenderer.sortingOrder = 0;

    }

    private void SetUpVisuals(RuntimeAnimatorController _animController, Sprite _sprite, Color _color)
    {
        if (_animController != null)
        {
            tileAnimator = _animController;
            var anim = GetAnimator();
            anim.runtimeAnimatorController = tileAnimator;
        }
        else if (_sprite != null)
        {
            tileSprite = _sprite;
            rend.sprite = tileSprite;
        }
        else
        {
            tileColor = _color;
            rend.color = tileColor;
        }
    }

    private void SetUpTileVisuals(Sprite sprite, Sprite topLeft, Sprite topRight, Sprite bottomLeft, Sprite bottomRight, Sprite bottom, Sprite top, Sprite left, Sprite right)
    {

        tileLocation = getTileLocation();

        switch (tileLocation)
        {
            case TileLocation.TOP_LEFT:
                rend.sprite = topLeft;
                break;
            case TileLocation.TOP_RIGHT:
                rend.sprite = topRight;
                break;
            case TileLocation.BOTTOM:
                rend.sprite = bottom;
                break;
            case TileLocation.TOP:
                rend.sprite = top;
                break;
            case TileLocation.LEFT:
                rend.sprite = left;
                break;
            case TileLocation.RIGHT:
                rend.sprite = right;
                break;
            case TileLocation.BOTTOM_LEFT:
                rend.sprite = bottomLeft;
                break;
            case TileLocation.BOTTOM_RIGHT:
                rend.sprite = bottomRight;
                break;
            case TileLocation.CENTER:
                rend.sprite = sprite;
                break;
            default:
                rend.sprite = sprite;
                break;
        }
    }

    private void ResetVisuals()
    {

        tileAnimator = null;
        tileSprite = null;
        rend.sprite = squareSprite;
        rend.color = Color.white;

        var anim = GetAnimator();
        anim.runtimeAnimatorController = null;
    }

    private Animator GetAnimator()
    {
        var anim = gameObject.GetComponent<Animator>();

        if (anim == null)
        {
            anim = gameObject.AddComponent<Animator>();

        }
        return anim;
    }

    public List<Tile> Neighbors()
    {
        List<Tile> neighbors = new List<Tile>();

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };


        for (int i = 0; i < 4; i++)
        {
            int neighborX = row + dx[i];
            int neighborY = column + dy[i];

            if (neighborX >= 0 && neighborX < grid.rows && neighborY >= 0 && neighborY < grid.columns)
            {
                if (!grid.tiles[neighborX, neighborY].isInaccessible && !grid.tiles[neighborX, neighborY].isTrap)
                    neighbors.Add(grid.tiles[neighborX, neighborY]);
            }
        }

        return neighbors;
    }


    public Vector2 ToVector()
    {
        return transform.position;
    }

    public TileLocation getTileLocation()
    {
        //Debug.Log(row + " " + column);
        //Debug.Log("Grid: " + grid.rows + " " + grid.columns);
        if (row == 0 && column == 0)
        {
            return TileLocation.BOTTOM_LEFT;
        }
        else if (row == 0 && column == grid.columns - 1)
        {
            return TileLocation.TOP_LEFT;
        }
        else if (row == grid.rows - 1 && column == 0)
        {
            return TileLocation.BOTTOM_RIGHT;
        }
        else if (row == grid.rows - 1 && column == grid.columns - 1)
        {
            return TileLocation.TOP_RIGHT;
        }
        else if (row == 0)
        {
            return TileLocation.LEFT;
        }
        else if (column == grid.columns - 1)
        {
            return TileLocation.TOP;
        }
        else if (column == 0)
        {
            return TileLocation.BOTTOM;
        }
        else if (row == grid.rows - 1)
        {
            return TileLocation.RIGHT;
        }
        else
        {
            return TileLocation.CENTER;
        }

    }

    public void UpdateSpriteBasedOnNeighbors()
    {
        if (isChecked) return;
        if(type != Type.CRATE) return;

        Tile topNeighbor = HasNeighborOfType(row, column + 1, type);
        Tile bottomNeighbor = HasNeighborOfType(row, column - 1, type);
        Tile leftNeighbor = HasNeighborOfType(row - 1, column , type);
        Tile rightNeighbor = HasNeighborOfType(row + 1, column, type);

        this.isChecked = true;

        if (topNeighbor != null)
        {

            //Four tiles sprite
            //Four Tile square where this sprite is in the bottom left

            if (HasNeighborOfType(row + 1, column + 1, type) != null && rightNeighbor != null)
            {
                // Debug.Log("Tile : " + name);
                // Debug.Log("Top neighbor: " + topNeighbor.name);
                // Debug.Log("Bottom Right neighbor: " + rightNeighbor.name);
                // Debug.Log("Top Right neighbor: " + HasNeighborOfType(row + 1, column + 1, type).name + " with tile type: " + topNeighbor.HasNeighborOfType(row + 1, column, type).type);

                // topNeighbor.GetComponent<SpriteRenderer>().color = Color.green;
                // HasNeighborOfType(row + 1, column+1, type).GetComponent<SpriteRenderer>().color = Color.blue;
                // rightNeighbor.GetComponent<SpriteRenderer>().color = Color.yellow;
                // rend.color = Color.red;

                //Set sprites to one sprite
                topNeighbor.GetComponent<SpriteRenderer>().sprite = null;
                HasNeighborOfType(row + 1, column + 1, type).GetComponent<SpriteRenderer>().sprite = null;
                rightNeighbor.GetComponent<SpriteRenderer>().sprite = null;
                rend.sprite = visuals.crateVisuals.enemySprite4;

                topNeighbor.isChecked = true;
                HasNeighborOfType(row + 1, column + 1, type).isChecked = true;
                rightNeighbor.isChecked = true;

                Debug.Log("Four tiles square");
                return;
            }
            else
            {
                topNeighbor.isChecked = true;
                topNeighbor.GetComponent<SpriteRenderer>().sprite = null;
                // topNeighbor.GetComponent<SpriteRenderer>().color = Color.red;
                rend.sprite = visuals.crateVisuals.enemySprite2;
                
            }
            //Four Tile square where this sprite is in the top left
            // if (bottomNeighbor.HasNeighborOfType(row - 1, column, type) != null && rightNeighbor != null)
            // {
            //     bottomNeighbor.GetComponent<SpriteRenderer>().color = Color.green;
            //     bottomNeighbor.HasNeighborOfType(row - 1, column, type).GetComponent<SpriteRenderer>().color = Color.green;
            //     rightNeighbor.GetComponent<SpriteRenderer>().color = Color.green;
            //     rend.color = Color.green;
            //     Debug.Log("Four tiles square");
            //     return;
            // }

            // Debug.Log("Four tiles sprite");
            // rend.color = Color.red;
            return;

            //Two tiles Tall sprite

            // Debug.Log("Two tiles Tall sprite");
            // topNeighbor.GetComponent<SpriteRenderer>().color = Color.blue;
            // rend.color = Color.red;
        }

    }

    private Tile HasNeighborOfType(int row, int column, Type type)
    {
        if (row >= 0 && row < grid.rows && column >= 0 && column < grid.columns)
        {
            if (grid.tiles[row, column].type == type && !grid.tiles[row, column].isChecked)
            {
                return grid.tiles[row, column];
            }
        }
        return null;
    }

}
