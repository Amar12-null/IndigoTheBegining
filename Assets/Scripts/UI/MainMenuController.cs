using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Ovdje ćemo u Unity-ju prevući cijeli Main Menu prozor
    [SerializeField] private GameObject mainMenuPanel;

    void Start()
    {
        // Povezujemo se na onaj naš 'razglas' iz GameManagera
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        // Uvijek se odjavljujemo sa razglasa kada skripta više ne postoji
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        // Ako je stanje MainMenu, upali panel. U suprotnom, ugasi ga.
        mainMenuPanel.SetActive(state == GameState.MainMenu);
    }

    // Ovu funkciju ćemo zakačiti na "Play" dugme
    public void OnPlayButtonClicked()
    {
        // Kažemo GameManageru da započne igru
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }
}