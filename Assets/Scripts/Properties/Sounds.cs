using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds inst;

    public AudioClip music;
    public AudioClip playerMove;
    public AudioClip fireProjectile;
    public AudioClip projectileImpact;
    public AudioClip crateDestroyed;
    public AudioClip enterTrap;
    public AudioClip goalReached;
    

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

}
