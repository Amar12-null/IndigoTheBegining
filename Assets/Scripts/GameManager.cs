using System;
using UnityEngine;
using Indigo.UI; // Dodali smo ovo da GameManager može pričati sa koleginim UI-jem

public enum GameState 
{
    MainMenu,
    Playing,
    Paused,
    UpgradeShop
}

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance; 
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    // Referenca na kolegin PauseMenu
    private PauseMenu pauseMenu;

    void Awake() 
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); 
    }

    void Start() 
    {
        UpdateGameState(GameState.MainMenu);
    }

    void Update()
    {
        // Provjeravamo da li igrač pritišće ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Ako smo u igri, pauziraj
        if (State == GameState.Playing)
        {
            UpdateGameState(GameState.Paused);
        }
        // Ako je već pauza, vrati nas u igru
        else if (State == GameState.Paused)
        {
            UpdateGameState(GameState.Playing);
        }
        // Ako smo u MainMenu ili UpgradeShop, ignorisati pritisak ESC
    }

    public void UpdateGameState(GameState newState) 
    {
        State = newState;
        
        // Logika za pauziranje vremena u igri
        if (newState == GameState.Paused)
        {
            Time.timeScale = 0f; // Zaustavlja fiziku i animacije
            UIEvents.SetPaused(true); // Govori koleginim skriptama da smo pauzirali
            Cursor.visible = true; // Pokazuje miša da možemo klikati po meniju
            Cursor.lockState = CursorLockMode.None;
        }
        else if (newState == GameState.Playing)
        {
            Time.timeScale = 1f; // Vraća vrijeme u normalu
            UIEvents.SetPaused(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}