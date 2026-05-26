using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Central game manager that controls overall game state, progression, and lifecycle.
/// Handles game initialization, scene transitions, and global game events.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private bool debugMode = false;

    // Game State
    public enum GameState { MainMenu, Loading, Playing, Paused, GameOver, Victory }
    private GameState currentState = GameState.MainMenu;
    public GameState CurrentState => currentState;

    // Day/Night tracking
    private int currentDay = 1;
    private bool isNight = false;
    private float dayProgress = 0f; // 0-1

    // Events
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int, bool> OnDayNightChanged; // day, isNight
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load game settings
        if (gameSettings == null)
        {
            gameSettings = Resources.Load<GameSettings>("GameSettings");
            if (gameSettings == null)
            {
                Debug.LogError("GameSettings ScriptableObject not found in Resources folder!");
                gameSettings = ScriptableObject.CreateInstance<GameSettings>();
            }
        }

        InitializeGame();
    }

    private void Start()
    {
        if (debugMode)
            Debug.Log("[GameManager] Game Started - Day: " + currentDay + ", Time: " + dayProgress);
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            UpdateDayNightCycle();
        }

        // Debug pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
                PauseGame();
            else if (currentState == GameState.Paused)
                ResumeGame();
        }
    }

    /// <summary>
    /// Initialize game on startup
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log("[GameManager] Initializing game...");
        SetGameState(GameState.MainMenu);
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void StartNewGame()
    {
        Debug.Log("[GameManager] Starting new game...");
        currentDay = 1;
        isNight = false;
        dayProgress = 0f;
        SetGameState(GameState.Loading);
        StartCoroutine(LoadGameScene());
    }

    /// <summary>
    /// Load the gameplay scene
    /// </summary>
    private IEnumerator LoadGameScene()
    {
        // Simulate loading
        yield return new WaitForSeconds(2f);
        SetGameState(GameState.Playing);
        OnDayNightChanged?.Invoke(currentDay, isNight);
    }

    /// <summary>
    /// Update the day/night cycle
    /// </summary>
    private void UpdateDayNightCycle()
    {
        dayProgress += Time.deltaTime / gameSettings.DayLengthSeconds;

        if (dayProgress >= 1f)
        {
            dayProgress = 0f;
            isNight = !isNight;

            if (!isNight) // Day started
                currentDay++;

            OnDayNightChanged?.Invoke(currentDay, isNight);

            if (debugMode)
                Debug.Log($"[GameManager] Day {currentDay}, {'Night' if isNight else 'Day'}");

            // Check for game end conditions
            if (currentDay > 6 && isNight)
            {
                Debug.Log("[GameManager] FINAL NIGHT! Blue Berry temple revealed!");
            }
        }
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();

        if (debugMode)
            Debug.Log("[GameManager] Game Paused");
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;

        currentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();

        if (debugMode)
            Debug.Log("[GameManager] Game Resumed");
    }

    /// <summary>
    /// End game with failure
    /// </summary>
    public void GameOver(string reason = "")
    {
        Debug.LogWarning("[GameManager] GAME OVER: " + reason);
        SetGameState(GameState.GameOver);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// End game with victory
    /// </summary>
    public void Victory()
    {
        Debug.Log("[GameManager] VICTORY! Blue Berry collected!");
        SetGameState(GameState.Victory);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Set the current game state
    /// </summary>
    private void SetGameState(GameState newState)
    {
        if (currentState == newState) return;

        GameState oldState = currentState;
        currentState = newState;

        Debug.Log($"[GameManager] State changed: {oldState} -> {newState}");
        OnGameStateChanged?.Invoke(newState);
    }

    // Getters
    public int CurrentDay => currentDay;
    public bool IsNight => isNight;
    public float DayProgress => dayProgress;
    public GameSettings GameSettings => gameSettings;

    /// <summary>
    /// Get time until day change (0-1)
    /// </summary>
    public float GetTimeUntilDayChange()
    {
        return 1f - dayProgress;
    }
}

/// <summary>
/// Game configuration settings (ScriptableObject)
/// </summary>
public class GameSettings : ScriptableObject
{
    [Header("Day/Night Cycle")]
    [SerializeField] public float DayLengthSeconds = 180f; // 3 minutes for testing
    [SerializeField] public float NightLengthMultiplier = 1.2f; // Nights are 20% longer

    [Header("Difficulty")]
    [SerializeField] public float ZombieSpawnRateMultiplier = 1f;
    [SerializeField] public float DamageMultiplier = 1f;
    [SerializeField] public float ResourceRarity = 0.8f;

    [Header("Player Starting Stats")]
    [SerializeField] public float StartingHealth = 100f;
    [SerializeField] public float StartingHunger = 50f;
    [SerializeField] public float StartingThirst = 50f;
    [SerializeField] public float StartingTemperature = 37f; // Celsius

    [Header("World Settings")]
    [SerializeField] public float IslandSize = 500f;
    [SerializeField] public bool EnableDynamicWeather = true;
    [SerializeField] public bool EnableMultiplayer = false;
}