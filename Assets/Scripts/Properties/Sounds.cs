using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds inst;

    [Header("Music")]
    public AudioClip music;
    [Range(0, 1)] public float musicVolume=1;
    [Header("Ambience")]
    public AudioClip ambience;
    [Range(0, 1)] public float ambienceVolume =1;
    [Header("SFX")]
    public AudioClip playerMove;
    [Range(0, 1)] public float playerMoveVolume=1;
    public AudioClip fireProjectile;
    [Range(0, 1)] public float fireProjectileVolume =1;
    public AudioClip projectileImpact;
    [Range(0, 1)] public float projectileImpactVolume =1;
    public AudioClip crateDestroyed;
    [Range(0, 1)] public float crateDestroyedVolume =1;
    public AudioClip enterTrap;
    [Range(0, 1)] public float enterTrapVolume =1;
    public AudioClip goalReached;
    [Range(0, 1)] public float goalReachedVolume =1;


    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

}
