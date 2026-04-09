# Minigame System - Complete Summary

## What's Done ✅

### Code Implementation
- Removed FormReview, FileSorting, DataEntry minigame scripts
- Added `MinigameType` enum (Typing, MultipleChoice, Math, PhotoReveal)
- Updated `TaskData` with minigame type system
- Rewrote `TaskDatabase.InitializeDefaultTasks()` with day-gated tasks
- Implemented `GetRandomTaskForDay()` with proper day/minigame filtering
- Updated `Ticket.cs` to dispatch to all 4 minigame types
- Created 3 new minigame scripts: MultipleChoiceMinigameUI, MathMinigameUI, PhotoRevealMinigameUI

### Day-to-Minigame Mapping
- **Day 1**: Typing only
- **Day 2**: Typing OR Multiple Choice (50/50 random per ticket)
- **Day 3**: Math only (addition/subtraction, difficulty-scaled)
- **Day 4**: Photo Reveal only (hover to uncover mosaic)
- **Day 5**: Typing (fallback, doesn't use real minigames due to impossible quota)

---

## What's Left 🔧

You need to build 3 UI Panels in Unity (see UI_HIERARCHY_GUIDE.md for exact layout):

1. **MultipleChoiceMinigameUI Panel**
   - 1 question text
   - 4 answer buttons (2×2 grid)
   - 1 feedback text
   - 1 background image (for red flash)

2. **MathMinigameUI Panel**
   - 1 problem text (e.g. "47 + 38 = ?")
   - 1 input display (shows typed answer)
   - 1 feedback text
   - 1 background image (for red flash)

3. **PhotoRevealMinigameUI Panel**
   - 1 grid container (6×6, GridLayoutGroup)
   - 1 TilePrefab (Image with EventTrigger)
   - 1 progress text
   - 1 reveal sprite (office-themed image)

---

## Gameplay Flow

### Day 1: Email Typing
1. Ticket spawns with Email Typing task
2. Player clicks "Start Task"
3. Fake Google Docs window opens
4. Player spams keys to type message
5. When key presses match message length, minigame completes
6. Ticket auto-stamps
7. Player drags to Processing Machine for reward

### Day 2: Typing OR Multiple Choice
**Example 1 - Typing:**
- Same as Day 1

**Example 2 - Multiple Choice:**
1. Ticket spawns with Multiple Choice task
2. Player clicks "Start Task"
3. Window shows: "Which file format is read-only?"
4. Player clicks one of 4 buttons (PDF, DOCX, XLSX, etc.)
5. If wrong: red flash, can retry
6. If correct: "Correct!" message, auto-completes in 2.5 seconds
7. Ticket stamps, player drags to Machine

### Day 3: Math Problems
1. Ticket spawns with Math task
2. Player clicks "Start Task"
3. Window shows: "47 + 38 = ?"
4. Player types on keyboard: "85"
5. Player presses Enter
6. If wrong: red flash, input clears, can retry
7. If correct: "Correct!" message, auto-completes
8. Ticket stamps, player drags to Machine

### Day 4: Photo Reveal
1. Ticket spawns with Photo Reveal task
2. Player clicks "Start Task"
3. Window shows 6×6 grid of mosaic tiles (colored blocks)
4. Player hovers mouse over tiles to reveal the real image
5. At 80% revealed, minigame auto-completes
6. Ticket stamps, player drags to Machine

---

## Minigame Characteristics

### MultipleChoiceMinigameUI
- **Questions**: 8 hardcoded office-themed Q&As
- **Attempts**: 1 wrong answer per question (can retry)
- **Completion**: Instant on correct answer
- **Upgrades**: None specific to this minigame
- **UI**: Click-based (buttons)

### MathMinigameUI
- **Operations**: Addition or Subtraction
- **Difficulty scaling**:
  - Easy (Diff 1): 1-20
  - Medium (Diff 2): 10-50
  - Hard (Diff 3): 20-99
- **Subtraction constraint**: No negative results
- **Input**: Keyboard only (number keys, Backspace, Enter)
- **Attempts**: Unlimited wrong answers (just retry)
- **Completion**: Instant on correct answer
- **Upgrades**: None specific
- **UI**: Text-based (type answer, press Enter)

### PhotoRevealMinigameUI
- **Tiles**: 6×6 grid (36 total)
- **Starting state**: Each tile is a random muted color (mosaic effect)
- **Reveal method**: Mouse hover (no click needed)
- **Completion**: 80% of tiles revealed
- **Mosaic colors**: Randomly selected from 5 muted colors
- **Reveal image**: Office-themed sprite (you assign)
- **Upgrades**: Memory Assist pre-reveals 30% of tiles
- **UI**: Hover-based (no clicking)

---

## Integration Points

### TaskDatabase.cs
- Reads current `GameManager.GetCurrentDay()`
- Filters tasks by allowed minigame types per day
- Returns random task from filtered pool

### Ticket.cs
- Reads `TaskData.minigameType`
- Dispatches to correct minigame singleton:
  - `MinigameType.Typing` → TypingMinigameUI
  - `MinigameType.MultipleChoice` → MultipleChoiceMinigameUI
  - `MinigameType.Math` → MathMinigameUI
  - `MinigameType.PhotoReveal` → PhotoRevealMinigameUI

### UpgradeManager.cs
- PhotoRevealMinigameUI reads `IsMemoryAssistActive()`
- If true, pre-reveals 30% of tiles

---

## Testing Checklist

1. **Day 1 Workflow**
   - Press `T` → Typing ticket spawns
   - Press `M` → Typing minigame starts
   - Complete typing (spam keys matching message)
   - Verify ticket stamps and can be dragged

2. **Day 2 Workflow**
   - Press `N` → Advance to Day 2
   - Press `T` multiple times → Mix of Typing and Multiple Choice
   - Press `M` on Multiple Choice → Window shows question + 4 buttons
   - Try wrong answer → Red flash, can retry
   - Try correct answer → "Correct!" message, auto-completes

3. **Day 3 Workflow**
   - Press `N` → Advance to Day 3
   - Press `T` → Math ticket
   - Press `M` → Window shows "XX + YY = ?"
   - Type answer on keyboard, press Enter
   - Try wrong → Red flash, clears, retry
   - Try correct → "Correct!" message, auto-completes

4. **Day 4 Workflow**
   - Press `N` → Advance to Day 4
   - Press `T` → Photo Reveal ticket
   - Press `M` → Window shows 6×6 mosaic grid
   - Hover mouse over tiles to reveal real image
   - At 80%, auto-completes

5. **Day 5 Ending**
   - Press `F` → Jump to Day 5
   - Verify quota is impossible (999,999)
   - Verify time eventually reaches 6 PM (18:00)
   - Verify ending panel triggers with AI reveal narrative

---

## Files Modified

- `Assets/Scripts/TaskDatabase.cs` — added enum, updated task system
- `Assets/Scripts/Ticket.cs` — updated dispatch logic

## Files Created

- `Assets/Scripts/MultipleChoiceMinigameUI.cs`
- `Assets/Scripts/MathMinigameUI.cs`
- `Assets/Scripts/PhotoRevealMinigameUI.cs`
- `Assets/Scripts/DebugCommands.cs` (already created)

## Files Deleted

- `Assets/Scripts/FormReviewMinigameUI.cs`
- `Assets/Scripts/FileSortingMinigameUI.cs`
- `Assets/Scripts/DataEntryMinigameUI.cs`

---

## Next Steps

1. Read `UI_HIERARCHY_GUIDE.md` for exact GameObject hierarchy
2. Build 3 UI panels in Unity Inspector
3. Attach scripts to panels
4. Assign all UI references in Inspector
5. Create TilePrefab for Photo Reveal
6. Test with Debug Commands (press T, M, N, F, S)
7. Verify day-based minigame spawning works correctly
8. Verify all minigame mechanics (click, type, hover) work
9. Verify upgrades don't break anything
10. Celebrate! 🎉
