using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    DEFAULT,
    CRATE,
    TRAP,
    BLOCK,
    GOAL
}

//This is the Tile script that is on the prefab object, we can set hese variables insided the nested for loop in the Gridgenerator
public class Tile : MonoBehaviour
{
    public Type type;

    private GridGenerator grid;

    //for storing the tile color
    private Color tileColor;
    private Color defaultTileColor;

    private Sprite tileSprite;
    private Sprite defaultSprite;

    private RuntimeAnimatorController tileAnimator;
    private RuntimeAnimatorController defaultAnimator;

    private SpriteRenderer rend;

    //For storing the row and column of the tile (for easy identification)
    public int row;
    public int column;

    //bools for setting tile properties 
    public bool isTrap;
    public bool isCrate;
    public bool isInaccessible;
    public bool isGoal;

    private VisualProperties visuals;

    private AudioClip projectileImpact;
    private AudioClip crateDestroyed;

    public static event Action CrateDestroyed;


    public void Init(Type _type)
    {
        rend = GetComponent<SpriteRenderer>();
        grid = GridGenerator.inst;
        visuals = VisualProperties.inst;

        projectileImpact = Sounds.inst.projectileImpact;
        crateDestroyed = Sounds.inst.crateDestroyed;

        type = _type;

        defaultAnimator = visuals.tileVisuals.animController != null? visuals.tileVisuals.animController:null;
        defaultSprite = visuals.tileVisuals.sprite != null? visuals.tileVisuals.sprite:null;
        defaultTileColor = visuals.tileVisuals.color;

        switch (type)
        {
            case Type.DEFAULT:
                SetUpVisuals(defaultAnimator, defaultSprite, defaultTileColor);
                //if (defaultAnimator != null) { tileAnimator = defaultAnimator; }
                //else if (defaultSprite != null) { tileSprite = defaultSprite; rend.sprite = tileSprite; }
                //else { tileColor = defaultTileColor; rend.color = tileColor; }
                break;

            case Type.CRATE:
                SetUpVisuals(visuals.crateVisuals.animController, visuals.crateVisuals.sprite, visuals.crateVisuals.color);
                //if (visuals.crateVisuals.animator != null) { tileAnimator = visuals.crateVisuals.animator; }
                //else if (visuals.crateVisuals.sprite != null) { tileSprite = visuals.crateVisuals.sprite; rend.sprite = tileSprite; }
                //else { tileColor = visuals.crateVisuals.color; rend.color = tileColor; }

                isCrate = true;
                isInaccessible = true;
                break;

            case Type.TRAP:
                SetUpVisuals(visuals.trapVisuals.animController, visuals.trapVisuals.sprite, visuals.trapVisuals.color);
                //if (visuals.trapVisuals.animator != null) { tileAnimator = visuals.trapVisuals.animator; }
                //else if (visuals.trapVisuals.sprite != null) { tileSprite = visuals.trapVisuals.sprite; rend.sprite = tileSprite; }
                //else { tileColor = visuals.trapVisuals.color; rend.color = tileColor; }

                isTrap = true;
                break;

            case Type.BLOCK:
                SetUpVisuals(visuals.blockVisuals.animController, visuals.blockVisuals.sprite, visuals.blockVisuals.color);
                //if (visuals.blockVisuals.animator != null) { tileAnimator = visuals.blockVisuals.animator; }
                //else if (visuals.blockVisuals.sprite != null) { tileSprite = visuals.blockVisuals.sprite; rend.sprite = tileSprite; }
                //else { tileColor = visuals.blockVisuals.color; rend.color = tileColor; }

                isInaccessible = true;
                break;

            case Type.GOAL:
                SetUpVisuals(visuals.goalVisuals.animController, visuals.goalVisuals.sprite, visuals.goalVisuals.color);
                //if (visuals.goalVisuals.animator != null) { tileAnimator = visuals.goalVisuals.animator; }
                //else if (visuals.goalVisuals.sprite != null) { tileSprite = visuals.goalVisuals.sprite; rend.sprite = tileSprite; }
                //else { tileColor = visuals.goalVisuals.color; rend.color = tileColor; }

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
                Destroy(collision.gameObject);
                AudioManager.inst.PlaySound(projectileImpact);
                DestroyCrate();
            } else if (isInaccessible)
            {
                AudioManager.inst.PlaySound(projectileImpact);
                Destroy(collision.gameObject);
            }
        }
    }

    public void DestroyCrate()
    {
        if (!isCrate) return;

        AudioManager.inst.PlaySound(crateDestroyed);
        if (tileAnimator != null) { tileAnimator = defaultAnimator; }
        else if (tileSprite != null) { tileSprite = defaultSprite; }
        else { tileColor = defaultTileColor; rend.color = tileColor; }

        CrateDestroyed?.Invoke();

        isCrate = false;
        isInaccessible = false;
    }

    private void SetUpVisuals(RuntimeAnimatorController _animController, Sprite _sprite, Color _color)
    {
        if (_animController != null)
        {
            tileAnimator = _animController;
            var anim = gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = tileAnimator;        
        }
        else if (_sprite != null)
        {
            tileSprite = _sprite;
            rend.sprite = tileSprite;
        }
        else
        {   tileColor = _color;
            rend.color = tileColor;
        }
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

}
