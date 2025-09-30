using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class EnemyController : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();
    public int health;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private WaitForSeconds blinkDuration = new WaitForSeconds(0.04f);

    public int GetHealth()
    {
        return health;
    }

    public void Init(List<Tile> tiles) // The Tile in index 0 should be the "main" tile, meaning it holds the sprite
    {
        this.tiles = tiles;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null)
            {
                Debug.Log("Tile " + i + " is null");
                return;
            }
            tiles[i].isEnemy = true;
        }
        spriteRenderer = tiles[0].GetComponent<SpriteRenderer>();
        animator = tiles[0].GetComponent<Animator>();
        Debug.Log("Tile Count: " + tiles.Count);
        health = tiles.Count;
    }

    public void TakeDamage()
    {

        health -= 1;
        //animator.SetTrigger("Hit");
        StartCoroutine(FlashEnemy());
        if (CheckIfDead())
        {
            StartCoroutine(EnemyDeath(2.35f));
        }
    }

    public bool CheckIfDead()
    {
        return health <= 0;
    }
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    IEnumerator EnemyDeath(float time)
    {
        //play death animation
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(time);
        //trigger particle effect   
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].resetEnemyTile();
        }
    }
    private IEnumerator FlashEnemy()
    {
        Color color = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return blinkDuration;
        spriteRenderer.color = color;
        yield return blinkDuration;
        spriteRenderer.color = Color.red;
        yield return blinkDuration;
        spriteRenderer.color = color;
        yield return blinkDuration;
        spriteRenderer.color = Color.red;
        yield return blinkDuration;
        spriteRenderer.color = color;

    }
    
}
