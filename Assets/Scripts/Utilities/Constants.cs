using UnityEngine;

/// <summary>
/// Global constants for the Blue Berry game
/// </summary>
public static class GameConstants
{
    // Game
    public const string GAME_TITLE = "Blue Berry";
    public const string GAME_VERSION = "1.0.0";

    // Layers
    public const int LAYER_PLAYER = 8;
    public const int LAYER_ZOMBIE = 9;
    public const int LAYER_GROUND = 10;
    public const int LAYER_OBSTACLE = 11;
    public const int LAYER_INTERACTABLE = 12;

    // Tags
    public const string TAG_PLAYER = "Player";
    public const string TAG_ZOMBIE = "Zombie";
    public const string TAG_GROUND = "Ground";
    public const string TAG_INTERACTABLE = "Interactable";

    // Player Stats
    public const float PLAYER_HEIGHT = 1.8f;
    public const float PLAYER_WIDTH = 0.4f;
    public const float MAX_STAMINA = 100f;
    public const float MAX_HEALTH = 100f;

    // World
    public const float ISLAND_SIZE = 500f;
    public const int ZOMBIE_SPAWN_PER_NIGHT = 20;

    // Time
    public const float DAY_LENGTH_SECONDS = 180f; // 3 minutes for demo
    public const int TOTAL_DAYS = 6;
    public const int TOTAL_NIGHTS = 5;

    // UI
    public const float UI_FADE_SPEED = 0.3f;
    public const float HUD_UPDATE_RATE = 0.1f;

    // Audio
    public const float MASTER_VOLUME = 0.8f;
    public const float MUSIC_VOLUME = 0.6f;
    public const float SFX_VOLUME = 0.7f;
    public const float AMBIENT_VOLUME = 0.5f;
}