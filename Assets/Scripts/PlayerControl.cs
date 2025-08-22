using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControl : MonoBehaviour
{
    private void Control(Vector3 direction)
    {
        if (rotationEnabled)
        {
            RotatePlayer(direction);
        }

        if (visuals.animController != null)
        {

        }
        else if (visuals.sprite != null)
        {


        }
    }


    private float moveSpeed;
    private float projectileSpeed;
    private GridGenerator grid;
    public GameObject projectilePrefab;
    private bool isMoving;
    private VisualProperties.PlayerVisuals visuals;
    private Color color;
    private Sprite sprite;
    private RuntimeAnimatorController animator;
    private GameObject arrow;
    private bool rotationEnabled;

    private SpriteRenderer rend;

    private AudioClip playerMove;
    private AudioClip enteringTrap;
    private AudioClip firing;
    private AudioClip goalReached;

   
    private int rIndex = 0;
    private int lastRIndex = 0;
    private int lastCIndex = 0;
    private int cIndex = 0;

    private Tile currentTile;
    private Tile lastTile;
    private Tile targetTile;

    private WaitForSeconds blinkDuration = new WaitForSeconds(0.04f);

    public static event Action PlayerMoved;


    private void Start()
    {
        visuals = VisualProperties.inst.playerVisuals;

        rend = GetComponentInChildren<SpriteRenderer>();
        grid = GridGenerator.inst;
        //This sets the player to grid place 0,0 at start
        transform.position = grid.GetTilePosition(rIndex, cIndex);

        //We make sure that the current tile and target tile are set to our current 0,0 tile at start
        currentTile = grid.tiles[rIndex, cIndex];
        targetTile = currentTile;

        moveSpeed = GameProperties.inst.playerMoveSpeed;
        projectileSpeed = GameProperties.inst.projectileSpeed;

        Init();

        playerMove = Sounds.inst.playerMove;
        enteringTrap = Sounds.inst.enterTrap;
        firing = Sounds.inst.fireProjectile;
        goalReached = Sounds.inst.goalReached;


    }

    private void Init()
    {
        rotationEnabled = visuals.allowPlayerRotation;
        arrow = visuals.arrow;

        if (visuals.animController != null)
        {
            animator = visuals.animController;
            var anim = gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = animator;
        }
        else if (visuals.sprite != null)
        {
            sprite = visuals.sprite;
            rend.sprite = sprite;
        }

        else
        {   color = visuals.color;
            rend.color = color;
            arrow.SetActive(true);
        }
    }


    private void Update()
    {
        //If the player is not moving, WASD can be used to set the direction the player needs to move.
        //Player will then move a single space in that direction
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Control(Vector3.right);
                SetTargetTile(Vector3.right);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Control(Vector3.left);
                SetTargetTile(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Control(Vector3.up);
                SetTargetTile(Vector3.up);
            }
            else if (Input.GetKeyDown(KeyCode.S)) 
            {
                Control(Vector3.down);
                SetTargetTile(Vector3.down);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireProjectile();
            }
        }
        
        //here is what triggers the player to move, once the targettile and currentle are not the samw
        //We run the coroutine that moves the player from their current position to their target position
        if (targetTile != currentTile)
        {
            StartCoroutine(MovePlayer(grid.GetTilePosition(currentTile), grid.GetTilePosition(targetTile)));

            //We then make a referenc to the last tile in case we need it again later -See: Traps
            lastTile = currentTile;
            //And we make sure to set currentTile to the targettile so that we dont run the corourine endlessly
            currentTile = targetTile;
        }   
    }

    private void RotatePlayer(Vector2 direction )
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

    }

    //These two functions do the same thing but take in different arguments.
    //The first sets the target tile based on WASD direction  
    private void SetTargetTile(Vector3 dir)
    {
        if (rIndex + dir.x >= 0 && rIndex + dir.x < grid.rows && cIndex + dir.y >= 0 && cIndex + dir.y < grid.columns)
        {
            var t = grid.tiles[rIndex + (int)dir.x, cIndex + (int)dir.y];

            if (!t.isInaccessible)
            {
                targetTile = t;
                lastRIndex = rIndex;
                lastCIndex = cIndex;
                rIndex += (int)dir.x;
                cIndex += (int)dir.y;
            }
        }      
    }

    //The second let's us direct pass a tile as target tile.
    //In this case we also need to remeber to update the row and column indices since we'll need them 
    //to keep track of where we are in the grid
    private void SetTargetTile(Tile t)
    {
        if (!t.isInaccessible)
        {
            targetTile = t;
            rIndex = lastRIndex;
            cIndex = lastCIndex;
        }

    }

    private void FireProjectile()
    {
        AudioManager.inst.PlaySound(firing);
        var pos = transform.position + (transform.up * 0.5f);
        GameObject go = Instantiate(projectilePrefab, pos, Quaternion.identity);
        var pjt = go.GetComponent<Projectile>();
        pjt.speed = projectileSpeed;
        pjt.direction = transform.up;
        pjt.Init();
    }

    //We move the player in a coroutine by sending a start and an end pos and just lerping between then
    //for the duration set. The duration is currently 1/ moveSpeed 
    private IEnumerator MovePlayer(Vector3 startPos, Vector3 endPos)
    {
        isMoving = true;

        float timeElapsed = 0;
        float duration = 1 / moveSpeed;

        AudioManager.inst.PlaySound(playerMove);
        while (timeElapsed < duration)
        {
            //The lerping happens in the while loop
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        PlayerMoved?.Invoke();
        //Once the player has completely shifted to the target location, we're gonna run a function that process tile effects
        //We also make sure to set the position of the object to the exact endPos in case it's off by a very tiny amount
        //This tiny amounts can compound over time so this is necessary
        transform.position = endPos;
        ProcessTileEvents();
    }

    private void ProcessTileEvents()
    {
        //this function checks if the current tile is a trap tile
        if (currentTile.isTrap)
        {
            AudioManager.inst.PlaySound(enteringTrap);
            //If it is, move the player back to the last tile they were on
            StartCoroutine(FlashPlayer());
            //And we call the Camera shake function, passing it the shake duration we want (it's currently 1/4 a sec) 
            CameraShake.inst.Shake(0.25f);

            //This step is also necessary to make sure out place on the grid gets properly updated
            SetTargetTile(lastTile);
        }
        else if (currentTile.isGoal)
        {
            isMoving = true;

            AudioManager.inst.StopMusic();
            AudioManager.inst.PlaySound(goalReached);

            if (goalReached == null) GameManager.inst.RestartGame();
            else
            {
                var gm = GameManager.inst;
                gm.Invoke("RestartGame", goalReached.length);
            } 

        }
        else
        {
            isMoving = false;
        }

    }



    //This coroutine just flashs the player red when they are hit by a trap
    private IEnumerator FlashPlayer()
    {
        rend.color = Color.red;
        yield return blinkDuration;
        rend.color = color;
        yield return blinkDuration;
        rend.color = Color.red;
        yield return blinkDuration;
        rend.color = color;
        yield return blinkDuration;
        rend.color = Color.red;
        yield return blinkDuration;
        rend.color = color;

    }
}
