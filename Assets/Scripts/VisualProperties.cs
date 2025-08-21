using UnityEngine;
using System;

public class VisualProperties : MonoBehaviour
{
    public static VisualProperties inst;

    [Serializable]
    public class PlayerVisuals
    {
        public Color color;
        public GameObject arrow;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
        public bool allowPlayerRotation;
    }

    [Serializable]
    public class ProjectileVisuals
    {
        public Color color;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    [Serializable]
    public class TileVisuals
    {
        public Color color;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    [Serializable]
    public class CrateTileVisuals
    {
        public Color color;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    [Serializable]
    public class TrapTileVisuals
    {
        public Color color;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    [Serializable]
    public class BlockTileVisuals
    {
        public Color color;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    [Serializable]
    public class GoalTileVisuals
    {
        public Color color;
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    public PlayerVisuals playerVisuals;
    public ProjectileVisuals projectileVisuals;
    public TileVisuals tileVisuals;
    public CrateTileVisuals crateVisuals;
    public TrapTileVisuals trapVisuals;
    public BlockTileVisuals blockVisuals;
    public GoalTileVisuals goalVisuals;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }


}
