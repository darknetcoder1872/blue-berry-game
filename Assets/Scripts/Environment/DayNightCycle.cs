using UnityEngine;

/// <summary>
/// Manages dynamic day/night cycle with lighting and effects.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    [Header("Lighting")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.3f, 0.3f, 0.5f);
    [SerializeField] private float dayIntensity = 1f;
    [SerializeField] private float nightIntensity = 0.3f;

    [Header("Fog")]
    [SerializeField] private Color dayFogColor = new Color(0.7f, 0.8f, 1f);
    [SerializeField] private Color nightFogColor = new Color(0.1f, 0.1f, 0.2f);
    [SerializeField] private float dayFogDensity = 0.01f;
    [SerializeField] private float nightFogDensity = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioSource ambientAudio;
    [SerializeField] private AudioClip dayAmbient;
    [SerializeField] private AudioClip nightAmbient;

    private void Start()
    {
        // Subscribe to day/night changes
        GameManager.OnDayNightChanged += OnDayNightChanged;
        TimeManager.OnSunrise += OnSunrise;
        TimeManager.OnSunset += OnSunset;
    }

    private void OnDestroy()
    {
        GameManager.OnDayNightChanged -= OnDayNightChanged;
        TimeManager.OnSunrise -= OnSunrise;
        TimeManager.OnSunset -= OnSunset;
    }

    private void Update()
    {
        // Update lighting based on time of day
        if (TimeManager.Instance != null)
        {
            UpdateLighting(TimeManager.Instance.GetNormalizedDayTime());
        }
    }

    private void UpdateLighting(float normalizedTime)
    {
        if (directionalLight == null) return;

        // Rotate sun/moon
        directionalLight.transform.rotation = Quaternion.Euler(normalizedTime * 360f - 90f, 0, 0);

        // Adjust intensity and color based on time
        if (normalizedTime < 0.25f) // Night to sunrise
        {
            float t = normalizedTime / 0.25f; // 0-1
            directionalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, t);
            directionalLight.color = Color.Lerp(nightColor, dayColor, t);
        }
        else if (normalizedTime < 0.75f) // Day
        {
            directionalLight.intensity = dayIntensity;
            directionalLight.color = dayColor;
        }
        else // Sunset to night
        {
            float t = (normalizedTime - 0.75f) / 0.25f; // 0-1
            directionalLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, t);
            directionalLight.color = Color.Lerp(dayColor, nightColor, t);
        }
    }

    private void OnDayNightChanged(int day, bool isNight)
    {
        // Update fog
        RenderSettings.fogColor = isNight ? nightFogColor : dayFogColor;
        RenderSettings.fogDensity = isNight ? nightFogDensity : dayFogDensity;
    }

    private void OnSunrise()
    {
        Debug.Log("[DayNightCycle] Sunrise!");
        if (ambientAudio != null && dayAmbient != null)
        {
            ambientAudio.clip = dayAmbient;
            ambientAudio.Play();
        }
    }

    private void OnSunset()
    {
        Debug.Log("[DayNightCycle] Sunset!");
        if (ambientAudio != null && nightAmbient != null)
        {
            ambientAudio.clip = nightAmbient;
            ambientAudio.Play();
        }
    }
}