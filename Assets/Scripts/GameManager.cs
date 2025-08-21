using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public TextMeshProUGUI cratesRemainingText;
    private int cratesRemaining;
    public TextMeshProUGUI stepsTakenText;
    private int stepsTaken = 0;
    private Pathfinding pathfinding;
    private Stack<PathPoint> pathToGoal;
    
    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        pathfinding = GetComponent<Pathfinding>();   
    }

    private void Start()
    {
        cratesRemaining = GridGenerator.inst.crateCount;
        cratesRemainingText.text = cratesRemaining.ToString("00");

        AudioManager.inst.PlayMusic();
    }

    private void OnEnable()
    {
        Tile.CrateDestroyed += UpdateCrateTextUI;
        PlayerControl.PlayerMoved += UpdateStepsTakenUI;
    }
    private void OnDisable()
    {
        Tile.CrateDestroyed -= UpdateCrateTextUI;
        PlayerControl.PlayerMoved -= UpdateStepsTakenUI;
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateCrateTextUI()
    {
        if (cratesRemaining < 1) return;
        cratesRemaining--;
        cratesRemainingText.text = cratesRemaining.ToString("00");
    }

    private void UpdateStepsTakenUI()
    {
        stepsTaken++;
        stepsTakenText.text = stepsTaken.ToString("000");
    }

    //public void GetPath()
    //{
    //    pathToGoal = pathfinding.SearchAndGetPath(GridGenerator.inst.tiles[0,0],GridGenerator.inst.goalTile);
    //}
}
