# While You Were Busy — Game Architecture

## Overview

This is a single-player time management minigame where the player completes tasks to advance through days. The game uses a clean, modular architecture with a single entry point and prefab-based UI system.

## Core Systems

### 1. GameBootstrapper (Entry Point)
**Location:** `Assets/Scripts/Bootstrap/GameBootstrapper.cs`
**Responsibility:** Single initialization point that starts all systems in correct order.

```
GameBootstrapper.Awake()
  ├── Initialize GameManager (core state, day timer)
  ├── Initialize Printer (ticket spawning)
  ├── Initialize BulletinBoard (on-screen ticket management)
  └── Initialize MinigameManager (prevents minigame overlap)
```

**Why this order matters:**
- GameManager must exist first (other systems depend on it)
- Printer needs GameManager to check current day
- BulletinBoard needs Printer to manage spawned tickets
- MinigameManager tracks which minigame is open

### 2. GameManager (Core Loop)
**Location:** `Assets/Scripts/Core/GameManager.cs`
**Responsibility:** Main game state, day/task progression, win/lose checks.

**Key Methods:**
- `StartDay(int dayNumber)` — Initialize a new day
- `CompleteTask()` — Increment task counter
- `GetCurrentDay()` → Current day (1-5)
- `IsDayComplete()` → Check if 4 tasks done (win condition)
- `IsTimeExpired()` → Check if 90 seconds passed (lose condition)

**State:**
- `currentDay` — Current day number (1-5)
- `taskCompletionCount` — Tasks completed this day
- `dayTimer` — 90-second countdown timer

### 3. Printer (Ticket Spawning)
**Location:** `Assets/Scripts/Core/Printer.cs`
**Responsibility:** Spawns tickets on a schedule (every 15 seconds, 6 per day).

**Key Methods:**
- `SpawnTicket()` — Create a ticket at a spawn point
- `GetNextTaskForDay(int day)` → Random task for the day

**Config:**
- `spawnInterval` = 15 seconds
- `spawnPoint` = Transform where tickets appear

### 4. BulletinBoard (Ticket Management)
**Location:** `Assets/Scripts/Core/BulletinBoard.cs`
**Responsibility:** Manages on-screen tickets, auto-snaps them to grid positions.

**Key Methods:**
- `RegisterTicket(Ticket ticket)` — Add ticket to board
- `SnapToGrid(Ticket ticket)` — Position on grid
- `UnregisterTicket(Ticket ticket)` — Remove when completed

### 5. MinigameManager (Minigame Routing)
**Location:** `Assets/Scripts/Core/MinigameManager.cs`
**Responsibility:** Centralized minigame control. Prevents multiple minigames from opening simultaneously.

**Key Methods:**
- `OpenMinigame(MinigameType type, Action onComplete)` — Open Math/PhotoReveal/MultipleChoice
- `OpenTypingMinigame(TypingTaskSO task, Action onComplete)` — Open Typing (special case)
- `CloseCurrentMinigame()` — Close active minigame

**Safety:**
Automatically closes the previous minigame before opening a new one.

### 6. Minigames (Task Implementations)
**Location:** `Assets/Scripts/Minigames/`

All minigames inherit from `BaseMinigameUI` (abstract):

- **TypingMinigameUI** (Day 1) — Type to reveal text
- **MathMinigameUI** (Day 3) — Solve arithmetic problems
- **MultipleChoiceMinigameUI** (Day 2) — Answer questions
- **PhotoRevealMinigameUI** (Day 4) — Hover to reveal photo

Each implements:
- `StartMinigame(Action completionCallback)` — Begin the minigame
- `CloseMinigame()` — Clean up and close UI
- Game-specific logic (key handling, answer checking, etc.)

### 7. Ticket (Individual Task)
**Location:** `Assets/Scripts/Core/Ticket.cs`
**Responsibility:** Represents a single task. Handles click-to-start, minigame dispatch, stamping.

**Flow:**
```
Player clicks Ticket
  → Ticket.OnStartTaskButtonClicked()
  → Routes to MinigameManager.OpenTypingMinigame() or OpenMinigame()
  → MinigameManager closes previous minigame (if any) and opens new one
  → MinigameUI calls completionCallback when done
  → Ticket.OnMinigameCompleted()
  → GameManager.CompleteTask()
  → Ticket gets stamped
```

---

## Scene Structure

```
Canvas (UI Root)
  ├── TypingMinigame (prefab instance, inactive)
  ├── MathMinigame (prefab instance, inactive)
  ├── MultipleChoiceMinigame (prefab instance, inactive)
  ├── PhotoRevealMinigame (prefab instance, inactive)
  └── GameUI
      ├── BulletinBoard (container for tickets)
      ├── TicketSpawner (empty, just holds spawn point)
      └── [Other UI elements]

GameBootstrapper (empty GameObject with script)
  └── GameBootstrapper component (initializes all systems)

Main Camera

Directional Light
```

---

## Prefab System

### Minigame Prefabs
Each minigame is a prefab located in `Assets/Prefabs/Minigames/`:
- `TypingMinigame.prefab`
- `MathMinigame.prefab`
- `MultipleChoiceMinigame.prefab`
- `PhotoRevealMinigame.prefab`

Each contains:
- A Canvas (for layering)
- A RectTransform panel (the UI window)
- TextMeshPro text elements
- Input fields / buttons
- The corresponding MinigameUI script component

When a minigame is triggered:
1. MinigameManager calls `OpenMinigame(type, callback)`
2. MinigameManager closes any open minigame
3. MinigameManager instantiates the prefab (or calls StartMinigame on existing instance)
4. MinigameUI calls `callback` when complete
5. MinigameManager closes the minigame
6. Callback triggers `Ticket.OnMinigameCompleted()`

### Ticket Prefab
Located in `Assets/Prefabs/Tickets/Ticket.prefab`

Contains:
- UI Panel with Title and Description text
- "Start Task" button
- Stamp overlay (Image)
- The Ticket script component
- CanvasGroup (for fade effects)

Can be instantiated multiple times. Printer creates new instances as tickets spawn.

---

## Task Database

Tasks are defined in `Assets/Scripts/Core/TaskDatabase.cs`:
- Organized by Day (1-5) and MinigameType
- Each task includes:
  - Title
  - Description
  - Base reward
  - Minigame type
  - Task-specific data (e.g., TypingTaskSO for Day 1)

---

## Day Flow (90 seconds per day)

```
Day 1: Email Typing
  - Every 15 seconds: New Ticket spawns
  - 6 tickets total
  - Player clicks tickets to type replies
  - 4 tasks needed to advance
  - If time expires with <4: Game Over (Day 5 loses → AI ending)

Days 2-5: Similar structure with different minigames
  - Day 2: Riddle (MultipleChoice)
  - Day 3: Math (MathMinigame)
  - Day 4: Photo Reveal (PhotoReveal)
  - Day 5: Connect Dots (Placeholder for now)

After Day 5 (win or lose):
  - Show ending screen
  - Day 5 loss → "Player was AI all along" narrative
  - Day 5 win → Success ending
```

---

## Dependencies & Initialization

```
GameBootstrapper (entry point)
  ↓
GameManager (core state)
  ↓
Printer (needs GameManager for day info)
  ↓
BulletinBoard (manages Printer's spawned tickets)
  ↓
MinigameManager (optional, auto-created if needed)

Ticket
  ↓
MinigameManager (routes to minigames)
  ↓
Minigames (all inherit BaseMinigameUI)
```

**Important:** Always initialize in this order. GameManager must exist before anything else depends on it.

---

## How to Add a New Minigame

1. **Create script** — `Assets/Scripts/Minigames/MyMinigameUI.cs` inheriting from `BaseMinigameUI`
2. **Implement abstract methods** — `StartMinigame(Action)`, `CloseMinigame()`
3. **Create UI prefab** — `Assets/Prefabs/Minigames/MyMinigame.prefab`
4. **Add task data** — Define in TaskDatabase.cs
5. **Register in MinigameManager** — Add serialized field and finder logic
6. **Test** — Verify minigame opens, closes, calls callback

---

## Testing Checklist

- [ ] Scene loads without errors
- [ ] GameBootstrapper initializes all systems
- [ ] Printer spawns tickets every 15 seconds
- [ ] Tickets are clickable and show minigame on click
- [ ] Only one minigame is open at a time
- [ ] Minigame closes after completion
- [ ] Task counter increments
- [ ] Day advances after 4 tasks
- [ ] Game over occurs after 90 seconds (if <4 tasks)
- [ ] Day 5 loss shows AI ending
- [ ] All images load correctly
- [ ] No console errors (except "Referenced script missing" for old prefabs)
