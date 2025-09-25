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
        Debug.Log("Tile Count: " + tiles.Count);
        health = tiles.Count;
    }

    public void TakeDamage()
    {

        health -= 1;
        //animator.SetTrigger("Hit");
        if (CheckIfDead())
        {
            StartCoroutine(EnemyDeath(.5f));
        }

    }

    public bool CheckIfDead()
    {
        return health <= 0;
    }

    IEnumerator EnemyDeath(float time)
    {
        //play death animation
        //animator.SetTrigger("Die");
        yield return new WaitForSeconds(time);
        //trigger particle effect   
        for (int i = 0; i < tiles.Count; i++) {
            tiles[i].resetEnemyTile();
        }
    }
    
}
