# Blue Berry - Unity Setup Guide

## Initial Project Setup

### Step 1: Create Folders Structure
1. Open the project in Unity
2. Create the following folder structure in Assets:
   - Scripts/ (with subfolders: Core, Player, Inventory, Crafting, AI, Combat, Environment, Vehicles, UI, Quests, Audio, Utilities)
   - Prefabs/ (with subfolders: Zombies, Weapons, Vehicles, Items, UI, Environment)
   - ScriptableObjects/ (with subfolders: Items, Recipes, Quests, ZombieConfigs, GameSettings)
   - Scenes/
   - Materials/
   - Textures/
   - Audio/ (with subfolders: Music, SFX, Ambient)
   - Models/

### Step 2: Set Up Scenes
1. Create new scenes:
   - MainMenu.unity
   - Gameplay.unity
   - FinalNight.unity

### Step 3: Install URP (Universal Render Pipeline)
1. Window > TextureImporter > Package Manager
2. Search for "Universal RP"
3. Click Install

### Step 4: Configure Project Settings
1. Edit > Project Settings
2. Quality:
   - Create presets for Mobile, Console, PC
   - Set shadow quality and draw distance
3. Physics:
   - Set gravity to -9.81
   - Configure physics materials

### Step 5: Create GameSettings ScriptableObject
1. Right-click in Assets/ScriptableObjects/GameSettings/
2. Create > GameSettings
3. Configure:
   - Day length: 180 seconds (for testing)
   - Starting health: 100
   - Starting hunger: 50
   - Starting thirst: 50
   - Zombie spawn rate: 20 per night

## Core Systems Integration

### GameManager Setup
1. Create an empty GameObject in Gameplay scene: "GameManager"
2. Add GameManager.cs script
3. Assign GameSettings asset in inspector
4. Set Debug Mode to true for testing

### TimeManager Setup
1. Create empty GameObject: "TimeManager"
2. Add TimeManager.cs script
3. Set game time multiplier to 2
4. Enable debug mode for testing

### Player Setup
1. Create player Capsule in Gameplay scene
2. Add CharacterController component
3. Add PlayerController.cs, PlayerStats.cs, InputManager.cs
4. Configure:
   - Walk speed: 5
   - Sprint speed: 10
   - Max stamina: 100
   - Max health: 100

### Zombie Setup
1. Create Capsule for zombie
2. Add NavMeshAgent component
3. Add Animator component
4. Add ZombieController.cs script
5. Configure:
   - Max health: 30
   - Vision range: 20
   - Attack range: 2
   - Damage: 10

## Input System Configuration

1. Window > TextureImporter > Input System
2. Create input actions for:
   - Move (WASD)
   - Look (Mouse/Analog stick)
   - Sprint (Shift/RT)
   - Crouch (Ctrl/LB)
   - Jump (Space/A button)
   - Interact (E/Y button)
   - Fire (LMB/RT)
   - Aim (RMB/LT)
   - Pause (ESC/Menu)

## NavMesh Setup

1. Select your terrain/level geometry
2. Mark as "Walkable" in the Inspector
3. Window > AI > Navigation
4. Click Bake
5. Verify zombie navigation works

## Audio Setup

1. Create AudioListener on camera
2. Create AudioSource objects for:
   - Music
   - Ambient
   - SFX
   - UI
3. Import audio clips into Assets/Audio/
4. Assign clips to AudioSources

## UI Setup

1. Create Canvas in Gameplay scene
2. Add UI elements:
   - Health bar
   - Hunger/Thirst bars
   - Stamina bar
   - Mini-map
   - Objective text
   - Quick slots
3. Link to HUDManager.cs

## Testing

### First Test
1. Load Gameplay scene
2. Press Play
3. You should see:
   - Player can move with WASD
   - Sprint works with Shift
   - Day/night cycle progresses
   - Stats display on HUD

### Zombie Test
1. Place zombie in scene
2. Press Play
3. Verify:
   - Zombie patrols
   - Zombie detects player
   - Zombie chases when close
   - Zombie attacks

### Save/Load Test
1. Implement SaveSystem.cs
2. Test saving after collecting items
3. Test loading previous save

## Performance Optimization

1. Setup LOD groups for models
2. Enable occlusion culling
3. Use object pooling for zombies/bullets
4. Enable GPU instancing on materials
5. Setup addressable assets for async loading

## Mobile Optimization

1. Project Settings > Quality > Mobile preset
2. Lower shadow quality
3. Reduce draw distance
4. Enable dynamic resolution
5. Test on mobile device

## Next Steps

1. Build out island terrain
2. Add crafting system
3. Implement weapons and combat
4. Create quest system
5. Add UI polish and animations
6. Implement save/load system
7. Add sound and music
8. Polish and optimize for release
