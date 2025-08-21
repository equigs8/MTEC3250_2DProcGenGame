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


    public void Init()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
        visuals = VisualProperties.inst.projectileVisuals;
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
