using UnityEngine;
using System;

/// <summary>
/// Tracks player vital statistics: health, hunger, thirst, temperature.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Hunger")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float currentHunger;
    [SerializeField] private float hungerDecayRate = 5f; // Per minute

    [Header("Thirst")]
    [SerializeField] private float maxThirst = 100f;
    [SerializeField] private float currentThirst;
    [SerializeField] private float thirstDecayRate = 8f; // Per minute (faster than hunger)

    [Header("Temperature")]
    [SerializeField] private float currentTemperature = 37f; // Normal body temp
    [SerializeField] private float normalTemperature = 37f;
    [SerializeField] private float temperatureAdjustmentRate = 0.1f;

    // Events
    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnHungerChanged;
    public static event Action<float> OnThirstChanged;
    public static event Action<float> OnTemperatureChanged;
    public static event Action<string> OnPlayerDamaged;
    public static event Action OnPlayerDeath;

    private bool isDead = false;

    private void Start()
    {
        // Initialize from game settings
        GameSettings settings = GameManager.Instance.GameSettings;
        currentHealth = settings.StartingHealth;
        currentHunger = settings.StartingHunger;
        currentThirst = settings.StartingThirst;
        currentTemperature = settings.StartingTemperature;

        maxHealth = settings.StartingHealth;
        maxHunger = 100f;
        maxThirst = 100f;
    }

    private void Update()
    {
        if (isDead) return;

        // Decay hunger and thirst over time
        currentHunger = Mathf.Max(0, currentHunger - (hungerDecayRate / 60f) * Time.deltaTime);
        currentThirst = Mathf.Max(0, currentThirst - (thirstDecayRate / 60f) * Time.deltaTime);

        // Apply damage from extreme hunger/thirst
        if (currentHunger <= 0)
            TakeDamage(1f, "Starvation");
        if (currentThirst <= 0)
            TakeDamage(2f, "Dehydration");

        // Broadcast changes
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        OnHungerChanged?.Invoke(currentHunger / maxHunger);
        OnThirstChanged?.Invoke(currentThirst / maxThirst);
        OnTemperatureChanged?.Invoke(currentTemperature);
    }

    /// <summary>
    /// Apply damage to player
    /// </summary>
    public void TakeDamage(float damage, string reason = "")
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnPlayerDamaged?.Invoke(reason);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnPlayerDeath?.Invoke();
            GameManager.Instance.GameOver(reason);
        }
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    /// <summary>
    /// Restore hunger
    /// </summary>
    public void RestoreHunger(float amount)
    {
        currentHunger = Mathf.Min(maxHunger, currentHunger + amount);
    }

    /// <summary>
    /// Restore thirst
    /// </summary>
    public void RestoreThirst(float amount)
    {
        currentThirst = Mathf.Min(maxThirst, currentThirst + amount);
    }

    // Getters
    public float GetHealthPercent() => currentHealth / maxHealth;
    public float GetHungerPercent() => currentHunger / maxHunger;
    public float GetThirstPercent() => currentThirst / maxThirst;
    public float GetTemperature() => currentTemperature;
    public bool IsDead => isDead;
    public float CurrentHealth => currentHealth;
}