# While You Were Busy - Complete Game Guide

## Table of Contents
1. [Game Overview](#game-overview)
2. [Core Systems](#core-systems)
3. [Script Reference](#script-reference)
4. [Quick Setup](#quick-setup)
5. [Game Workflow](#game-workflow)
6. [Troubleshooting](#troubleshooting)

---

## Game Overview

**While You Were Busy** is an incremental/idle game where players complete tasks by:
- Processing tickets through a typing minigame
- Managing a daily quota system
- Watching world health degrade as they work
- Progressing through increasing difficulty days

### Core Gameplay Loop
1. Tickets print automatically from the Printer
2. Player drags tickets to Bulletin Board to organize them
3. Player opens ticket and plays typing minigame
4. Completed ticket gets stamped automatically
5. Player drags stamped ticket to Processing Machine
6. Rewards granted, world health degrades
7. Player reaches daily quota to unlock sleep
8. Sleep advances to next day with higher quota

---

## Core Systems

### 1. Ticket System
**Purpose:** Generate tasks for the player to complete

**Key Components:**
- **Printer** - Generates tickets automatically or on demand
- **Ticket** - Individual task with title, description, reward, difficulty
- **Bulletin Board** - Organizes active tickets in a grid
- **Stamp** - Marks tickets as completed (manual or automatic via minigame)
- **Processing Machine** - Accepts stamped tickets, grants rewards

### 2. Minigame System
**Purpose:** Interactive task completion

**Current Minigames:**
- **Typing Minigame** - Player spams keyboard to "type" a predetermined message

### 3. Progression System
**Purpose:** Track player progress and difficulty scaling

**Key Features:**
- **Daily Quota** - Points required to unlock sleep/next day
- **World Time** - In-game clock starting at 6:00 AM
- **Day Counter** - Tracks current day number
- **Difficulty Scaling** - Harder tasks appear as days progress

### 4. World Health System
**Purpose:** Visual/narrative feedback for player actions

**Mechanics:**
- Degrades when tickets are processed
- Background color darkens as health decreases
- System log provides narrative commentary

---

## Script Reference

### Core Game Scripts
| Script | Purpose |
|--------|---------|
| `GameManager.cs` | Main game state, progression, UI updates |
| `TaskDatabase.cs` | Stores all available tasks |
| `TypingTaskDatabase.cs` | Stores typing minigame data |

### Ticket System Scripts
| Script | Purpose |
|--------|---------|
| `Printer.cs` | Generates tickets automatically/manually |
| `Ticket.cs` | Individual ticket behavior, dragging, minigame triggering |
| `BulletinBoard.cs` | Ticket organization zone |
| `Stamp.cs` | Manual ticket completion tool |
| `ProcessingMachine.cs` | Final ticket submission for rewards |

### UI & Support Scripts
| Script | Purpose |
|--------|---------|
| `TypingMinigameUI.cs` | Controls typing minigame window |
| `TypingTaskSO.cs` | ScriptableObject for typing task data |
| `BackgroundController.cs` | Visual feedback based on world health |
| `SystemLog.cs` | Displays narrative messages to player |
| `UpgradeButton.cs` | UI for purchasing upgrades |

---

## Quick Setup

### Scene Hierarchy Structure

```
Canvas
├── Background (Image)
├── BackgroundController (Empty + Script)
├── OutputsText (Text/TMP)
├── QuotaText (Text/TMP) - NEW
├── TimeText (Text/TMP) - NEW
├── DayText (Text/TMP) - NEW
├── ClickButton (Button)
├── SleepButton (Button) - NEW
├── SystemLog (Empty + Script)
│   └── SystemLogText (Text/TMP)
├── UpgradeButtons (Optional)
│   ├── AutoProcessorButton
│   ├── ScalingEngineButton
│   └── ExpansionProtocolButton
│
├── Printer
│   ├── PrinterBody (Image)
│   ├── PrintButton (Button)
│   ├── StatusText (Text)
│   └── TicketSpawnPoint (Empty)
│
├── BulletinBoard (Panel)
│   └── TicketContainer (Empty)
│
├── Stamp (Image + Button)
│   └── StampStatus (Text)
│
├── ProcessingMachine (Panel)
│   ├── MachineBody (Image)
│   ├── MachineStatus (Text)
│   └── ProgressBar (Slider)
│
└── TypingMinigameUI (Empty + Script)
    └── TypingMinigameWindow (Panel) - Hidden by default
        ├── Header (Panel)
        │   └── DocumentTitleText (TMP)
        ├── DocumentArea (Panel)
        │   └── DocumentContentText (TMP)
        ├── ProgressText (TMP)
        └── ProgressBar (Slider)

Scene Root
├── GameManager (Empty + Script)
├── TaskDatabase (Empty + Script)
└── TypingTaskDatabase (Empty + Script)
```

### Setup Steps

#### 1. Core Game Setup
1. Create `GameManager` GameObject
2. Attach `GameManager.cs`
3. Create `TaskDatabase` GameObject
4. Attach `TaskDatabase.cs`
5. Create `TypingTaskDatabase` GameObject
6. Attach `TypingTaskDatabase.cs`
7. Enable "Create Sample Tasks At Runtime" in TypingTaskDatabase

#### 2. Background & Visual Feedback
1. Create UI Image named "Background", stretch to full screen
2. Create `BackgroundController` GameObject under Canvas
3. Attach `BackgroundController.cs`
4. Assign Background Image reference

#### 3. Basic UI
Create these Text/TextMeshPro elements:
- `OutputsText` - Shows current outputs/score
- `QuotaText` - Shows quota progress (e.g., "Quota: 150/500")
- `TimeText` - Shows in-game time (e.g., "Time: 6:45 AM")
- `DayText` - Shows current day (e.g., "Day 3")
- `SystemLogText` - Shows narrative messages

Create these Buttons:
- `ClickButton` - Manual output generation
- `SleepButton` - End day (only enabled when quota met)

#### 4. Printer Setup
1. Create empty GameObject `Printer` under Canvas
2. Attach `Printer.cs`
3. Create child Image `PrinterBody`
4. Create child Button `PrintButton`
5. Create child Text `StatusText`
6. Create empty child `TicketSpawnPoint`
7. Create Ticket Prefab:
   - UI Panel with `Ticket.cs`, `CanvasGroup`
   - Child Text elements: TitleText, DescriptionText
   - Child Image: StampImage (initially disabled)
   - Child Button: StartTaskButton
8. Assign all references in Printer Inspector

#### 5. Bulletin Board Setup
1. Create UI Panel `BulletinBoard`
2. Attach `BulletinBoard.cs`
3. Create empty child `TicketContainer`
4. Configure grid layout settings in Inspector

#### 6. Stamp Setup
1. Create UI Image `Stamp`
2. Attach `Stamp.cs`
3. Add Button component
4. Create child Text `StampStatus`
5. Assign references

#### 7. Processing Machine Setup
1. Create UI Panel `ProcessingMachine`
2. Attach `ProcessingMachine.cs`
3. Create child Image `MachineBody`
4. Create child Text `MachineStatus`
5. Create child Slider `ProgressBar`
6. Assign references and link Printer

#### 8. Typing Minigame UI
1. Create empty GameObject `TypingMinigameUI` under Canvas
2. Attach `TypingMinigameUI.cs`
3. Create child Panel `TypingMinigameWindow`
4. Inside window, create:
   - Header Panel with TitleText
   - DocumentArea Panel with ContentText
   - ProgressText, ProgressBar
5. Assign all references
6. Set window to initially hidden

#### 9. Connect All References
Select GameManager and assign:
- All UI Text fields
- Click Button
- Sleep Button (NEW)

---

## Game Workflow

### Player Actions Flow

```
START DAY
    ↓
PRINTER GENERATES TICKETS
    ↓
DRAG TICKET TO BULLETIN BOARD
    ↓
CLICK "START TASK" ON TICKET
    ↓
TYPING MINIGAME OPENS
    ↓
PLAYER SPAMS KEYBOARD
    ↓
MINIGAME COMPLETES → TICKET AUTO-STAMPED
    ↓
DRAG STAMPED TICKET TO PROCESSING MACHINE
    ↓
MACHINE PROCESSES:
  - Grants Reward (Outputs)
  - Degrades World Health
  - Destroys Ticket
    ↓
QUOTA REACHED?
  - No → Continue processing tickets
  - Yes → SLEEP BUTTON ENABLED
    ↓
CLICK SLEEP BUTTON
    ↓
ADVANCE TO NEXT DAY
  - Increase quota
  - Reset time to 6:00 AM
  - Increment day counter
    ↓
LOOP
```

### Ticket Workflow Detail

**Ticket States:**
1. **Printed** - Fresh from printer
2. **Pinned** - Dragged to bulletin board
3. **In Progress** - Minigame active
4. **Stamped** - Minigame completed
5. **Processed** - Submitted to machine, destroyed

**Important:** Tickets must be stamped (via minigame) before processing machine accepts them.

---

## Troubleshooting

### Common Issues

#### Tickets Won't Drag
- **Solution:** Ensure Ticket has `CanvasGroup` component
- Check Canvas has `GraphicRaycaster`
- Verify `EventSystem` exists in scene

#### Minigame Won't Open
- **Solution:** Check `TypingMinigameUI.Instance` is not null
- Verify all UI references assigned in Inspector
- Ensure TypingTaskDatabase is in scene

#### Processing Machine Rejects Tickets
- **Solution:** Confirm ticket is stamped (checkmark visible)
- Verify "Require Stamped Tickets" is true in Inspector
- Check ticket has completed minigame

#### Keyboard Input Doesn't Work in Minigame
- **Solution:** Verify EventSystem exists
- Check TypingMinigameUI component is enabled
- Ensure no UI elements blocking input

#### No Typing Tasks Appear
- **Solution:** Verify TypingTaskDatabase in scene
- Check "Create Sample Tasks At Runtime" is enabled
- Set Script Execution Order:
  - TypingTaskDatabase: -100
  - TaskDatabase: 0

#### Sleep Button Doesn't Enable
- **Solution:** Check quota has been reached
- Verify Sleep Button reference assigned in GameManager
- Check button interactable state in Inspector

#### Time Doesn't Progress
- **Solution:** Verify GameManager Update loop is running
- Check time progression speed setting
- Ensure Time.deltaTime is not 0

---

## Customization

### Adjust Difficulty Scaling
In `TaskDatabase.cs` → `GetDifficultyForCurrentProgress()`:
```csharp
if (outputs < 100f) return 1;  // Easy
if (outputs < 500f) return 2;  // Medium
return 3;                       // Hard
```

### Modify Quota Progression
In `GameManager.cs` (after implementation):
```csharp
dailyQuota = baseQuota * Mathf.Pow(1.5f, currentDay - 1);
```
Adjust `1.5f` multiplier to change difficulty curve.

### Change Time Progression Speed
In `GameManager.cs`:
```csharp
timeProgressionSpeed = 60f; // 60 seconds real-time = 1 hour in-game
```

### Add Custom Typing Tasks
In `TypingTaskDatabase.cs` → `CreateSampleTasks()`:
```csharp
allTypingTasks.Add(CreateTypingTask(
    "Your Task Title",
    "Task description",
    "The message players will type out...",
    2.5f // completion delay
));
```

### Modify World Health Decay
In `GameManager.cs`:
```csharp
worldHealthDecayPerClick = 0.1f;    // Per manual click
worldHealthDecayPerOutput = 0.005f; // Per passive output
```

In `ProcessingMachine.cs` (after implementation):
```csharp
GameManager.Instance.DegradeWorldHealth(ticketDifficulty * 0.5f);
```

---

## Script Execution Order

**Recommended Order:**
1. `TypingTaskDatabase` (-100) - Creates typing tasks first
2. `TaskDatabase` (0) - References typing tasks
3. `GameManager` (0) - Default
4. All others (0) - Default

**How to Set:**
Edit → Project Settings → Script Execution Order

---

## Development Notes

### Current System Status
- ✅ Ticket generation and printing
- ✅ Drag and drop to bulletin board
- ✅ Typing minigame integration
- ✅ Auto-stamping after minigame
- ✅ Processing machine with rewards
- ⚠️ Daily quota system (TO BE IMPLEMENTED)
- ⚠️ World time system (TO BE IMPLEMENTED)
- ⚠️ Sleep mechanic (TO BE IMPLEMENTED)

### Planned Improvements
1. Fix duplicate reward logic (minigame + machine)
2. Centralize all rewards to ProcessingMachine only
3. Add quota tracking
4. Add time progression
5. Add day/night cycle
6. Add sleep button and day advancement

---

## Quick Reference Tables

### Task Difficulty Levels
| Difficulty | Reward Range | Unlock Condition |
|------------|--------------|------------------|
| 1 - Easy | 8-15 outputs | 0-100 outputs |
| 2 - Medium | 50-70 outputs | 100-500 outputs |
| 3 - Hard | 200-300 outputs | 500+ outputs |

### World Health Thresholds
| Health Range | Background Color | System Log Tone |
|--------------|------------------|-----------------|
| 100-70 | Light Blue | Optimistic |
| 70-40 | Desaturated | Concerned |
| 40-15 | Dark Gray | Warning |
| 15-0 | Very Dark | Critical |

### Upgrade Costs (Initial)
| Upgrade | Base Cost | Effect |
|---------|-----------|--------|
| Auto Processor | 10 | +1 output/sec |
| Scaling Engine | 50 | x2 per click |
| Expansion Protocol | 200 | +10 outputs/sec |

---

## Additional Resources

### Unity Packages Required
- TextMeshPro (included in Unity 2019+)
- No external packages needed

### Minimum Unity Version
- Unity 2019.4 LTS or higher

### Dependencies
- Uses Unity UI (UnityEngine.UI)
- Uses TextMeshPro (TMPro)
- Uses Unity EventSystems

---

**Last Updated:** 2026-02-11
**Version:** 1.0 - Pre-Implementation
