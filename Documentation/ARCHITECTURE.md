# Blue Berry - Architecture Overview

## Core Design Principles

1. **Modular Systems**: Each system is independent and communicates via events
2. **Singleton Pattern**: For managers (GameManager, TimeManager, AudioManager)
3. **ScriptableObjects**: For data configuration and balance tuning
4. **Event-Driven**: Systems communicate through C# events, not direct references
5. **Performance-First**: Object pooling, LOD, and optimization throughout

## System Architecture

### 1. Core Systems

#### GameManager
- Singleton responsible for overall game state
- Manages game lifecycle (menu, loading, playing, paused, game over, victory)
- Coordinates day/night cycle progression
- Handles game pause/resume
- **Events**: OnGameStateChanged, OnDayNightChanged

#### TimeManager
- Tracks game time with customizable speed multiplier
- Broadcasts time-based events (sunrise, noon, sunset, midnight)
- Normalizes time for smooth transitions
- **Events**: OnTimeChanged, OnSunrise, OnNoon, OnSunset, OnMidnight

#### SaveSystem
- Auto-save and manual save functionality
- Saves: player position, inventory, stats, time, mission progress, enemy states
- JSON serialization to disk
- Version compatibility checking

### 2. Player Systems

#### PlayerController
- Third-person character controller
- Handles movement, jumping, crouching, climbing
- Input processing (keyboard, gamepad, mobile)
- Animation synchronization
- Stamina management for sprinting

#### PlayerStats
- Tracks vital statistics: health, hunger, thirst, temperature
- Applies damage and status effects
- Broadcasts stat changes to UI
- Handles death/game over conditions
- **Events**: OnHealthChanged, OnHungerChanged, OnThirstChanged, OnPlayerDeath

#### InputManager
- Centralized input processing
- Supports multiple input devices
- Action-based input system (not polling)
- Mobile touch support

### 3. Inventory & Crafting

#### InventorySystem
- Slot-based inventory with weight limits
- Item stacking system
- Quick slots for equipment
- Drag-and-drop support
- **Events**: OnInventoryChanged, OnInventoryFull

#### CraftingSystem
- Recipe-based crafting
- Material requirement checking
- Crafting time/progress
- Unlockable recipes
- **Events**: OnCraftStarted, OnCraftCompleted

### 4. Combat Systems

#### WeaponSystem
- Multiple weapon types with unique behaviors
- Ammo management
- Recoil and weapon sway
- Reload mechanics
- Aim-down-sights (ADS)

#### HealthSystem
- Damage application with armor mitigation
- Status effects (bleeding, infection, broken bones, poison, radiation)
- Medical item consumption for healing
- Damage type handling (bullet, melee, environmental)

#### DamageSystem
- Centralized damage calculation
- Damage type system
- Knockback physics
- Friendly fire prevention

### 5. AI Systems

#### ZombieController
- Base zombie behavior
- NavMesh pathfinding
- Vision and hearing detection
- State machine (idle, patrol, chase, attack, dead)
- Night-time stat boosts
- **Events**: OnZombieSpawned, OnZombieDead

#### ZombieAI
- Extended zombie types with unique behaviors
- Special abilities per zombie type
- Aggression scaling
- Group coordination

#### BlueGuardian (Boss)
- Final boss with multiple attack patterns
- Health phases with behavior changes
- Special abilities
- Defeat triggers game victory

### 6. Environment Systems

#### DayNightCycle
- Dynamic lighting changes
- Fog and atmosphere updates
- Audio ambience switching
- Visual effects for transitions
- Synced with TimeManager

#### WeatherSystem
- Dynamic weather generation
- Rain, storms, fog, wind effects
- Gameplay impact (reduced visibility, slipping surfaces)
- Audio effects

#### QuestSystem
- Quest tracking and progression
- Main story quests
- Side quests
- Objective markers and UI
- **Events**: OnQuestStarted, OnQuestComplete, OnObjectiveComplete

### 7. Vehicle Systems

#### VehicleController
- Player-drivable vehicles (Jeep, Boat, Bike)
- Physics-based movement
- Fuel consumption
- Damage and durability
- Storage functionality

### 8. UI Systems

#### HUDManager
- Real-time player stat display
- Compass and mini-map
- Objective tracking
- Crosshair and aim assist
- Damage feedback

#### MenuSystem
- Main menu with navigation
- Pause menu
- Settings menu
- Credits
- Scene transitions

#### UIAnimations
- Smooth transitions
- HUD element animations
- Menu effects
- Feedback animations

### 9. Audio Systems

#### AudioManager
- Master volume control
- Mixer group management (Music, SFX, Ambient, UI)
- Spatial audio for 3D effects
- Sound effect pooling

#### MusicManager
- Dynamic music system
- Day/night music switching
- Intensity scaling
- Combat music triggers

### 10. Utility Systems

#### ObjectPool
- Instance pooling for bullets, zombies, effects
- Reduces garbage collection
- Improves spawn performance

#### PerformanceOptimizer
- Dynamic quality scaling
- FPS monitoring
- LOD management
- Memory profiling

## Data Flow

```
GameManager (Master State)
    ↓
TimeManager (Time Updates)
    ↓
├─ DayNightCycle (Visual/Audio Updates)
├─ WeatherSystem (Environmental Effects)
├─ ZombieSpawner (Enemy Spawning)
└─ QuestSystem (Progress Tracking)
    ↓
PlayerController (Input Processing)
    ↓
├─ PlayerStats (Health/Hunger/Thirst)
├─ InventorySystem (Item Management)
├─ WeaponSystem (Combat)
└─ VehicleController (Transportation)
    ↓
HUDManager (UI Display)
    ↓
AudioManager (Sound/Music)
```

## Event Communication

Systems use C# events for loose coupling:

```csharp
// Publishing
GameManager.OnGameStateChanged?.Invoke(GameState.Playing);

// Subscribing
PlayerStats.OnPlayerDeath += HandlePlayerDeath;
```

## Configuration

All balance values are exposed via:
- ScriptableObjects (GameSettings.asset)
- Inspector exposure (public serialized fields)
- JSON config files
- Console commands (debug)

## Optimization Strategies

1. **Object Pooling**: Zombies, bullets, effects
2. **LOD Groups**: Distance-based detail levels
3. **Occlusion Culling**: Hide off-screen objects
4. **Spatial Partitioning**: NavMesh and grid systems
5. **Async Loading**: Addressable assets
6. **GPU Instancing**: Duplicate models
7. **Memory Management**: Efficient data structures

## Extensibility

The architecture supports easy addition of:
- New zombie types (inherit ZombieController)
- New weapons (inherit Weapon)
- New vehicles (inherit VehicleController)
- New UI screens (inherit UIScreen)
- New quests (create Quest asset)
- New audio tracks (add to AudioManager)
