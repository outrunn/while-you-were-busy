# While You Were Busy - Quick Start Setup

## Prefab-Based Workflow (No Manual Wiring!)

### Option 1: Auto-Setup (Easiest)
1. **Create a new empty GameObject** in the scene called `GameSetup`
2. **Drag `GameSetup.cs` component** onto it
3. **Assign all the prefabs** in the inspector (drop them in the prefab fields)
4. **Hit Play** — everything initializes automatically

### Option 2: Manual Minimal Setup (Flexible)
If you prefer to manually place systems:

1. **Create these GameObjects in your scene:**
   - `GameManager` (drag GameManager prefab or component)
   - `DayManager` (drag DayManager prefab or component)
   - `UpgradeManager` (drag UpgradeManager prefab or component)
   - `SystemLog` (drag SystemLog prefab or component)
   - `TaskDatabase` (drag TaskDatabase prefab or component)
   - All minigame UIs (TypingMinigameUI, DataEntryMinigameUI, etc.)

2. **Wire these in GameManager inspector:**
   - Click Button: assign the main action button
   - Sleep Button: assign the sleep button
   - Output/Quota/Day/Time TextMeshPro fields

3. **Wire these in Printer inspector:**
   - Ticket Prefab: assign the Ticket prefab
   - Ticket Spawn Point: assign where new tickets appear

4. **Hit Play**

---

## Customization Points

### Easy Changes (No Code)

**Printer Settings (in Printer component):**
- `autoPrintInterval`: Time between tickets (default: 30s)
- `maxActiveTickets`: Max tickets on screen (default: 5)

**Day Settings (in GameManager component):**
- `baseQuota`: Starting day 1 quota (default: 100)
- `quotaMultiplier`: How much quota grows each day (default: 1.5x)
- `timeProgressionSpeed`: How fast in-game time passes (default: 60 real seconds = 1 game hour)

**Upgrade Effects (checked by minigames from UpgradeManager):**
- `FasterTyping`: Reduces required key presses in typing minigame
- `AutoStamp`: Auto-stamps tickets (implement in TypingMinigameUI)
- `NumberLock`: Shows answer after first wrong attempt in DataEntry
- `PreSorted`: Pre-places 2 files in File Sorting
- `QuickScan`: Highlights error row in Form Review
- Etc.

### Harder Changes (Requires Code)

**Add a new minigame type:**
1. Create a new script `NewMinigameUI.cs` following the pattern of `TypingMinigameUI.cs`
2. Make it a singleton with `StartMinigame(Action callback)` method
3. Add tasks to `TaskDatabase.cs` for your new minigame
4. Hook it into the Printer's day-based task selection

**Change quota progression:**
- Edit `GameManager.StartNewDay()` to adjust `dailyQuota` formula

**Change the ending text:**
- Edit the narrative string in `DayManager.PlayTypewriterEnding()` coroutine

---

## Architecture at a Glance

```
GameSetup.cs (entry point)
  ↓ Instantiates ↓

GameManager (core loop, quota, time, outputs)
  ↓ calls ↓
DayManager (handles sleep, upgrades, ending)
  ↓ stores state in ↓
UpgradeManager (tracks purchases)
  ↓ uses ↓
Printer (spawns tickets with tasks from TaskDatabase)
  ↓ tickets start ↓
Minigames (TypingMinigameUI, DataEntryMinigameUI, etc.)
  ↓ reward goes to ↓
ProcessingMachine (grants outputs, degrades world health)
```

## Testing Checklist

- [ ] Press Play → Day 1 starts, printer generates typing tickets
- [ ] Click ticket → typing minigame opens
- [ ] Finish minigame → ticket stamps, shows ready to process
- [ ] Drag to ProcessingMachine → outputs awarded, quota progresses
- [ ] Meet quota → Sleep button enabled
- [ ] Click Sleep → upgrade modal shows two choices
- [ ] Pick upgrade → modal fades, Day 2 starts
- [ ] Day 2 → new minigame types appear
- [ ] Reach Day 5 → let clock hit 6:00 PM → ending text types out → Continue button → quit

---

## File Locations

- **Systems**: `Assets/Scripts/GameManager.cs`, `DayManager.cs`, `UpgradeManager.cs`
- **Minigames**: `TypingMinigameUI.cs`, `DataEntryMinigameUI.cs`, `FileSortingMinigameUI.cs`, `FormReviewMinigameUI.cs`
- **Utilities**: `Printer.cs`, `TaskDatabase.cs`, `ProcessingMachine.cs`, `BulletinBoard.cs`
- **UI**: `SystemLog.cs`, `BackgroundController.cs`

---

## Prefab Factory Setup (Optional)

If you want to create reusable prefabs:

1. Create each system as a complete prefab with all child UI
2. Store in `Assets/Prefabs/Systems/`
3. Reference them in `GameSetup.cs`
4. Drag GameSetup into the scene, assign prefabs, done

This makes it trivial to swap out entire systems or create alternate configurations.
