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

    //bools for setting tile properties 
    public bool isTrap;
    public bool isCrate;
    public bool isInaccessible;
    public bool isGoal;

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
                ResetVisuals();
                SetUpVisuals(defaultAnimator, defaultSprite, defaultTileColor);
                break;

            case Type.CRATE:
                ResetVisuals();
                SetUpVisuals(visuals.crateVisuals.animController, visuals.crateVisuals.sprite, visuals.crateVisuals.color);

                isCrate = true;
                isInaccessible = true;
                break;

            case Type.TRAP:
                ResetVisuals();
                SetUpVisuals(visuals.trapVisuals.animController, visuals.trapVisuals.sprite, visuals.trapVisuals.color);

                isTrap = true;
                break;

            case Type.BLOCK:
                ResetVisuals();
                SetUpVisuals(visuals.blockVisuals.animController, visuals.blockVisuals.sprite, visuals.blockVisuals.color);

                isInaccessible = true;
                break;

            case Type.GOAL:
                ResetVisuals();
                SetUpVisuals(visuals.goalVisuals.animController, visuals.goalVisuals.sprite, visuals.goalVisuals.color);

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
            } else if (isInaccessible)
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
        {   tileColor = _color;
            rend.color = tileColor;
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

}
