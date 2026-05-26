using UnityEngine;
using System;

/// <summary>
/// Manages game time, day/night cycle, and time-based events.
/// Synchronized with GameManager for consistency.
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private float gameTimeMultiplier = 2f; // 1 real second = 2 game seconds
    [SerializeField] private bool debugMode = false;

    private float gameTime = 0f; // Total game time in seconds
    private float currentDayTime = 0f; // Time within current day (0-86400 seconds)
    private const float SECONDS_PER_DAY = 86400f; // 24 hours in seconds

    // Events
    public static event Action<float> OnTimeChanged; // Normalized time 0-1
    public static event Action OnMidnight;
    public static event Action OnNoon;
    public static event Action OnSunrise;
    public static event Action OnSunset;

    private bool sunriseTriggered = false;
    private bool sunsetTriggered = false;
    private bool midnightTriggered = false;
    private bool noonTriggered = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        UpdateTime();
    }

    private void UpdateTime()
    {
        gameTime += Time.deltaTime * gameTimeMultiplier;
        currentDayTime += Time.deltaTime * gameTimeMultiplier;

        // Wrap to 24 hour cycle
        if (currentDayTime >= SECONDS_PER_DAY)
            currentDayTime -= SECONDS_PER_DAY;

        // Trigger time events
        CheckTimeEvents();

        // Broadcast normalized time
        float normalizedTime = currentDayTime / SECONDS_PER_DAY;
        OnTimeChanged?.Invoke(normalizedTime);
    }

    private void CheckTimeEvents()
    {
        float normalizedTime = currentDayTime / SECONDS_PER_DAY;

        // Sunrise: 6 AM (0.25)
        if (normalizedTime >= 0.25f && !sunriseTriggered)
        {
            OnSunrise?.Invoke();
            sunriseTriggered = true;
            sunsetTriggered = false;
            if (debugMode) Debug.Log("[TimeManager] Sunrise!");
        }

        // Noon: 12 PM (0.5)
        if (normalizedTime >= 0.5f && !noonTriggered)
        {
            OnNoon?.Invoke();
            noonTriggered = true;
            if (debugMode) Debug.Log("[TimeManager] Noon!");
        }

        // Sunset: 6 PM (0.75)
        if (normalizedTime >= 0.75f && !sunsetTriggered)
        {
            OnSunset?.Invoke();
            sunsetTriggered = true;
            sunriseTriggered = false;
            if (debugMode) Debug.Log("[TimeManager] Sunset!");
        }

        // Midnight: 12 AM (0 or 1)
        if ((normalizedTime >= 0.99f || normalizedTime <= 0.01f) && !midnightTriggered)
        {
            OnMidnight?.Invoke();
            midnightTriggered = true;
            noonTriggered = false;
            if (debugMode) Debug.Log("[TimeManager] Midnight!");
        }
        else if (normalizedTime > 0.01f && normalizedTime < 0.99f)
        {
            midnightTriggered = false;
        }
    }

    // Getters
    public float GetGameTime() => gameTime;
    public float GetCurrentDayTime() => currentDayTime;
    public float GetNormalizedDayTime() => currentDayTime / SECONDS_PER_DAY;
    public float GetHourOfDay() => (currentDayTime / SECONDS_PER_DAY) * 24f;
    public string GetTimeString() => Mathf.Floor(GetHourOfDay()).ToString("00") + ":00";
}