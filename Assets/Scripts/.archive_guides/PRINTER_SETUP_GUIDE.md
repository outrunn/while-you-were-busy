# Printer Setup Guide

## What Has Been Done

The Printer GameObject has been created in **GameScene.unity** with the following structure:

```
Canvas
└── Printer (GameObject)
    ├── RectTransform (positioned at -562, -232)
    ├── Printer (script component)
    ├── GraphicRaycaster (for UI interaction)
    ├── AudioSource (for print sound effects)
    └── TicketSpawnPoint (child RectTransform)
        └── Positioned at (0, 150) relative to Printer
```

## Current Configuration

### Printer Script Fields

The Printer script is configured with the following default values:

| Field | Value | Status |
|-------|-------|--------|
| autoPrintInterval | 15s | ✓ Set |
| autoPrintEnabled | true | ✓ Set |
| maxActiveTickets | 10 | ✓ Set |
| ticketPrefab | *NEEDS ASSIGNMENT* | ⚠️ **CRITICAL** |
| ticketSpawnPoint | TicketSpawnPoint (auto-linked) | ✓ Set |
| printerPanel | *Not assigned* | ℹ️ Optional |
| printButton | *Not assigned* | ℹ️ Optional |
| statusText | *Not assigned* | ℹ️ Optional |
| printerAnimation | *Not assigned* | ℹ️ Optional |
| printSound | AudioSource (auto-linked) | ✓ Set |
| bulletinBoard | *Auto-discovers on Start* | ✓ Auto |
| travelDuration | 0.8s | ✓ Set |

## What Needs to be Done in the Unity Editor

### 1. Assign the Ticket Prefab (REQUIRED)

**Path:** Canvas → Printer → Printer (Script)

1. Select the **Printer** GameObject in the hierarchy
2. In the Inspector, find the **Printer** script component
3. Locate the **Ticket Prefab** field
4. Drag the **TicketPrefab** from `Assets/Prefabs/TicketPrefab.prefab` into this field

**Why this is critical:** Without this assignment, `PrintTicket()` will fail with a warning and no tickets will spawn.

### 2. (Optional) Assign Print Sound

**Path:** Canvas → Printer → Printer (Script)

1. In the Inspector, find the **Print Sound** field
2. Drag an audio clip (e.g., a printer sound effect) into this field
3. The audio will play whenever a ticket is printed

If no sound is assigned, tickets will print silently.

### 3. (Optional) Assign Printer UI Panel

**Path:** Canvas → Printer → Printer (Script)

1. Create a visual Printer UI panel (or find the PrinterUI created by SceneAssetSetup)
2. Assign it to the **Printer Panel** field
3. This will be hidden on Awake and shown/hidden via `Show()` and `Hide()` methods

If not assigned, the printer works fine but has no visual panel to show/hide.

### 4. (Optional) Assign Status Text and Print Button

**Path:** Canvas → Printer → Printer (Script)

1. Locate or create a TextMeshProUGUI element for **Status Text**
2. Locate or create a Button element for **Print Button**
3. Assign them to the respective fields

The Status Text will display:
- Time until next auto-print
- Current active ticket count

The Print Button will:
- Manually trigger ticket printing when clicked
- Auto-disable when ticket limit is reached

## Spawn Point Details

The **TicketSpawnPoint** is a child RectTransform positioned at:

- **Local Position:** (0, 150) — This is 150 units above the Printer center
- **Size:** (0, 0) — Just a point reference, not a visual element
- **Function:** Defines where new tickets appear before animating to the bulletin board

**Why position it above the printer?**

This mimics tickets "printing out" from the top of the printer device visually. When `PrintTicket()` is called:

1. A new ticket is instantiated at the SpawnPoint position
2. It's scaled and positioned at the spawn point initially
3. It then animates to a bulletin board slot (via `AnimateTicketToBoard()`)

You can adjust the spawn point position by selecting **Printer → TicketSpawnPoint** and changing its **Anchored Position** in the Inspector.

## How the System Works

### Ticket Lifecycle

1. **Spawn:** `PrintTicket()` creates a ticket at the spawn point
2. **Check Limits:** If `activeTicketCount >= maxActiveTickets`, abort
3. **Get Task:** Fetch a random task from TaskDatabase matching current day/difficulty
4. **Position:** Place ticket at spawn point using `ticketSpawnPoint` transform
5. **Initialize:** Call `Ticket.Initialize(task)` to set up the ticket
6. **Animate:** Coroutine animates ticket from printer to bulletin board
7. **Pin:** BulletinBoard snaps the ticket and manages completion
8. **Process:** When done, call `Printer.OnTicketProcessed()` to decrement counter

### Auto-Print Intervals

The spawn interval changes per day:

| Day | Interval | Script |
|-----|----------|--------|
| 1 | 30s | `GameManager` sets via `SetAutoPrintInterval()` |
| 2 | 25s | Adjusts in `DayManager` between days |
| 3 | 20s | Game gets harder! |
| 4 | 15s | Even harder! |
| 5 | 10s | Final day rush! |

See `GameManager.cs` → `SetCurrentDay()` for where intervals are applied.

## Troubleshooting

### "Printer: Ticket prefab or spawn point not assigned!"

**Problem:** The ticketPrefab is null
**Solution:** Assign the TicketPrefab in the Inspector (see step 1 above)

### "Printer: Too many active tickets!"

**Problem:** Tickets aren't being removed from `activeTicketCount`
**Solution:** Ensure ProcessingMachine or other systems call `Printer.OnTicketProcessed()` when tickets are completed

### Tickets don't animate to the bulletin board

**Problem:** BulletinBoard component not found
**Solution:** Create a BulletinBoard GameObject with the BulletinBoard script, or ensure it exists in the scene. The Printer auto-discovers it in `Start()`.

### Tickets spawn at wrong position

**Problem:** TicketSpawnPoint position is incorrect
**Solution:** Select **Printer → TicketSpawnPoint** and adjust **Anchored Position** in the Inspector

## Related Files

- **Printer.cs** — Main spawning logic
- **Ticket.cs** — Individual ticket behavior
- **BulletinBoard.cs** — Manages ticket pinning and completion tracking
- **TaskDatabase.cs** — Provides tasks to print
- **GameManager.cs** — Controls game flow and day timer
- **TypingMinigameUI.cs** — Day 1 minigame (first ticket type)
- **PlaceholderMinigameUI.cs** — Stub for Days 2-5 (instant completion)

## Quick Test

To test the Printer immediately after setup:

1. Assign the TicketPrefab in the Inspector
2. Play the scene (Ctrl+P or Play button)
3. A ticket should appear at the Printer's spawn point
4. It should animate to the bulletin board
5. Clicking the ticket should start a minigame (typing game on Day 1)
6. Completing the minigame should mark the ticket done

If this works, the Printer setup is complete!
