using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public GameObject background;
    public TextMeshProUGUI cratesRemainingText;
    private int cratesRemaining;
    public TextMeshProUGUI stepsTakenText;
    private int stepsTaken = 0;
    
    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        cratesRemaining = GameProperties.inst.crateCount;
        cratesRemainingText.text = cratesRemaining.ToString("00");

        if (VisualProperties.inst.backgroundImage != null)
        {
            background.GetComponent<RawImage>().texture = VisualProperties.inst.backgroundImage.texture;
            background.SetActive(true);
        }
        
        AudioManager.inst.PlayMusic(Sounds.inst.musicVolume);
        AudioManager.inst.PlayAmbience(Sounds.inst.ambienceVolume);
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

    private void UpdateCrateTextUI(Tile tile)
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

}
