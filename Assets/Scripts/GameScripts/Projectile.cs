using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector]public Vector3 direction;
    private SpriteRenderer rend;
    private VisualProperties.ProjectileVisuals visuals;
    private Color color;
    private Sprite sprite;
    private RuntimeAnimatorController animator;
    private Transform gfxTransform;


    public void Init()
    {
        //Debug.Log("Direction: " + direction);
        rend = GetComponentInChildren<SpriteRenderer>();
        gfxTransform = GetComponentsInChildren<Transform>()[1];
        //Debug.Log("Gfx Transform: " + gfxTransform.gameObject.name);
        visuals = VisualProperties.inst.projectileVisuals;
        updateFacingDirection();
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
        {
            color = visuals.color;
            rend.color = color;
        }
    }
    private void updateFacingDirection()
    {

        if (direction.x > 0)
        {

            //rend.flipX = false;
            //gfxTransform.rotation = Quaternion.Euler(0, 0, -180);

        }
        else if (direction.x < 0)
        {
            // rend.flipX = true;
            // rend.flipY = true;
            gfxTransform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (direction.y < 0)
        {
            // rend.flipY = true;
            gfxTransform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (direction.y > 0)
        {
            // rend.flipY = false;
            gfxTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    void Update()
    {
        if (rend != null && rend.isVisible)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }     
    }
}
