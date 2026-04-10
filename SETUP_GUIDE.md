# While You Were Busy — Setup & Development Guide

## Quick Start

### 1. Open the Game Scene
- Open `Assets/Scenes/GameScene.unity` in Unity Editor
- The scene contains a `Canvas` (UI root) and a `GameBootstrapper` (initialization entry point)

### 2. Check the GameBootstrapper References
In the Inspector, the `GameBootstrapper` component should have references to:
- **GameManager** — Core game state (auto-discovered if in scene)
- **Printer** — Spawns tickets (auto-discovered if in scene)
- **BulletinBoard** — Manages tickets (auto-discovered if in scene)
- **MinigameManager** — Routes minigame opens (auto-discovered if in scene)

If any system shows as missing, find it in the scene hierarchy and drag it to the corresponding field.

### 3. Verify All Essential GameObjects Exist
The scene should contain these GameObjects (verified ✓):
- ✓ **Canvas** (UI root)
- ✓ **GameBootstrapper** (initialization, auto-discovers systems)
- ✓ **Main Camera** (tagged "MainCamera" for rendering)
- ✓ **Directional Light** (scene lighting)

The following systems are auto-discovered by GameBootstrapper:
- **GameManager** (core game logic)
- **Printer** (ticket spawning)
- **BulletinBoard** (ticket management)
- **MinigameManager** (minigame routing)

If any are missing, they need to be created or imported from prefabs.

### 4. Verify Prefabs are Present
Check that these prefab directories exist:
- `Assets/Prefabs/Minigames/` (TypingMinigame.prefab, MathMinigame.prefab, etc.)
- `Assets/Prefabs/Tickets/Ticket.prefab`
- `Assets/Prefabs/UI/TilePrefab.prefab`

### 5. Play the Game
Press Play in the Editor. You should see:
- GameBootstrapper initialization logs in the Console
- Tickets spawning on the bulletin board
- Clickable tickets that open minigames

---

## Architecture Overview

### Single Entry Point: GameBootstrapper
The `GameBootstrapper` script (`Assets/Scripts/Bootstrap/GameBootstrapper.cs`) is the only entry point. It runs in `Awake()` and initializes all systems in the correct order:

```
1. GameManager (core state)
   ↓
2. Printer (spawning)
   ↓
3. BulletinBoard (management)
   ↓
4. MinigameManager (routing)
```

Each system auto-discovers the previous one. If any system is missing, an error is logged but the game continues (with reduced functionality).

### Core Systems

**GameManager** — Manages day/task progression, win/lose conditions
- `StartDay(int day)` — Begin a new day
- `CompleteTask()` — Increment task counter
- `GetCurrentDay()` → Current day (1-5)

**Printer** — Spawns tickets on a schedule (every 15 seconds)
- Assign `ticketPrefab` in Inspector → `Assets/Prefabs/Tickets/Ticket.prefab`
- Tickets spawn at `ticketSpawnPoint` Transform

**BulletinBoard** — Manages on-screen tickets
- Automatically registers tickets when created
- Snaps them to a grid for organization

**MinigameManager** — Routes minigames centrally
- `OpenMinigame(MinigameType type, Action callback)` — Open Math/PhotoReveal/MultipleChoice
- `OpenTypingMinigame(TypingTaskSO task, Action callback)` — Open Typing (special case)
- Prevents multiple minigames from opening simultaneously

### Minigames
All minigames inherit from `BaseMinigameUI` and are located in `Assets/Scripts/Minigames/`:
- **TypingMinigameUI** (Day 1)
- **MathMinigameUI** (Day 3)
- **MultipleChoiceMinigameUI** (Day 2)
- **PhotoRevealMinigameUI** (Day 4)

Each prefab is in `Assets/Prefabs/Minigames/`.

---

## Inspector Setup Checklist

### GameBootstrapper Component
- [ ] Assign **GameManager** (if not auto-discovered)
- [ ] Assign **Printer** (if not auto-discovered)
- [ ] Assign **BulletinBoard** (if not auto-discovered)
- [ ] Assign **MinigameManager** (if not auto-discovered)

### Printer Component
- [ ] Assign **ticketPrefab** → `Assets/Prefabs/Tickets/Ticket.prefab`
- [ ] Assign **ticketSpawnPoint** → Transform where tickets appear
- [ ] Assign **bulletinBoard** → Reference to BulletinBoard GameObject

### Minigame Prefabs (in scene)
Each minigame prefab in `Assets/Prefabs/Minigames/` should be in the scene (as children of Canvas, inactive):
- TypingMinigame.prefab
- MathMinigame.prefab
- MultipleChoiceMinigame.prefab
- PhotoRevealMinigame.prefab

MinigameManager auto-discovers these when needed.

---

## Prefab System

### Creating/Managing Minigames
Each minigame is a prefab containing:
1. A Canvas (for UI layering)
2. UI elements (buttons, text, input fields)
3. The MinigameUI script component

To instantiate a minigame:
```csharp
MinigameManager.Instance.OpenMinigame(MinigameType.Math, () => {
    Debug.Log("Math minigame completed!");
});
```

To add a new minigame:
1. Create script inheriting from `BaseMinigameUI`
2. Create UI prefab in `Assets/Prefabs/Minigames/`
3. Register in `MinigameManager` (add serialized field + auto-discovery logic)

### Ticket System
Tickets are reusable prefabs in `Assets/Prefabs/Tickets/Ticket.prefab`. Printer instantiates copies as needed.

Each ticket contains:
- Title & description text
- "Start Task" button
- Stamp overlay (visual feedback)
- Ticket script component

---

## Code Style & Conventions

### Naming
- Classes: `PascalCase` (GameManager, TypingMinigameUI)
- Methods: `PascalCase` (StartMinigame, CompleteTask)
- Fields: `camelCase` (currentDay, isActive)
- Constants: `UPPER_CASE` (SPAWN_INTERVAL)

### Logging
```csharp
Debug.Log("[System] ✓ Message");     // Major events
Debug.LogWarning("[System] Warning");  // Recoverable issues
Debug.LogError("[System] Error!");     // Failures
```

### Comments
- XMLDoc for public methods: `/// <summary>`
- Inline comments only for complex logic
- No commented-out code (delete instead)

For complete style guide, see `Assets/Docs/CODE_STYLE.md`.

---

## Common Tasks

### Adding a New Task/Minigame
1. Define the task in `TaskDatabase.cs` (organized by day/type)
2. Create minigame UI (or use placeholder)
3. Register in `MinigameManager`
4. Test opening/closing via `Ticket.OnStartTaskButtonClicked()`

### Debugging Minigame Issues
1. Enable verbose logging in `GameBootstrapper.cs`
2. Check Console for initialization order
3. Verify minigame prefab is in scene (Inspector)
4. Check that `MinigameManager` finds the minigame

### Adjusting Game Balance
- Day duration: `GameManager.dayDuration` (default: 90 seconds)
- Spawn interval: `Printer.autoPrintInterval` (default: 15 seconds)
- Tasks per day: `GameManager.requiredTasksPerDay` (default: 4)

---

## Troubleshooting

### "GameManager not found in scene!"
**Solution:** Create a GameObject named "GameManager" with the GameManager script, or drag it from a prefab.

### "TicketPrefab not assigned!"
**Solution:** In Printer Inspector, assign `ticketPrefab` → `Assets/Prefabs/Tickets/Ticket.prefab`

### Minigames not opening
**Solution:**
1. Verify MinigameManager exists in scene
2. Check Console for errors
3. Ensure minigame prefabs are in `Assets/Prefabs/Minigames/`
4. Verify MinigameManager has references to all minigames

### Missing images in PhotoRevealMinigame
**Solution:** Photos should be in `Assets/Resources/PhotoMinigameAssets/`. Ensure they're imported as Sprites (TextureType=Sprite).

---

## File Structure

```
Assets/
├── Scripts/
│   ├── Bootstrap/
│   │   └── GameBootstrapper.cs          ← Entry point
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── Printer.cs
│   │   ├── BulletinBoard.cs
│   │   ├── Ticket.cs
│   │   ├── TicketPool.cs
│   │   ├── MinigameManager.cs
│   │   └── TaskDatabase.cs
│   ├── Minigames/
│   │   ├── BaseMinigameUI.cs            ← Abstract base
│   │   ├── TypingMinigameUI.cs
│   │   ├── MathMinigameUI.cs
│   │   ├── MultipleChoiceMinigameUI.cs
│   │   └── PhotoRevealMinigameUI.cs
│   └── UI/
│       └── [Other UI scripts]
├── Prefabs/
│   ├── Minigames/
│   │   ├── TypingMinigame.prefab
│   │   ├── MathMinigame.prefab
│   │   ├── MultipleChoiceMinigame.prefab
│   │   └── PhotoRevealMinigame.prefab
│   ├── Tickets/
│   │   └── Ticket.prefab
│   ├── UI/
│   │   └── TilePrefab.prefab
│   └── Systems/
│       └── (Future system prefabs)
├── Scenes/
│   └── GameScene.unity                  ← Main scene
├── Resources/
│   └── PhotoMinigameAssets/             ← Photo sprites
├── Docs/
│   ├── ARCHITECTURE.md                  ← System design
│   └── CODE_STYLE.md                    ← Coding standards
└── SETUP_GUIDE.md                       ← This file
```

---

## Next Steps

1. **Verify the scene loads** — Press Play and check Console for initialization logs
2. **Test a minigame** — Click a ticket and verify the minigame opens
3. **Check documentation** — Read `ARCHITECTURE.md` to understand the flow
4. **Review code style** — Read `CODE_STYLE.md` before making changes

---

## Questions?

Refer to:
- **Architecture questions** → `Assets/Docs/ARCHITECTURE.md`
- **Code style questions** → `Assets/Docs/CODE_STYLE.md`
- **Implementation plan** → `docs/superpowers/plans/2026-04-10-production-architecture.md`
