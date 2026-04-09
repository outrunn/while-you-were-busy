# Minigame Overhaul - Implementation Checklist

## Code Changes ✅ COMPLETE

- [x] Delete FormReviewMinigameUI.cs
- [x] Delete FileSortingMinigameUI.cs
- [x] Delete DataEntryMinigameUI.cs
- [x] Add MinigameType enum to TaskDatabase.cs
- [x] Update TaskData class with minigameType field
- [x] Update TaskDatabase.cs to create day-gated tasks (Typing, MultipleChoice, Math, PhotoReveal)
- [x] Update Ticket.cs to dispatch to correct minigame based on MinigameType
- [x] Create MultipleChoiceMinigameUI.cs (8 hardcoded questions)
- [x] Create MathMinigameUI.cs (addition/subtraction, difficulty-scaled)
- [x] Create PhotoRevealMinigameUI.cs (hover-to-reveal tiles, 80% threshold)

## UI Setup 🔧 NEEDS TO BE DONE IN UNITY EDITOR

### MultipleChoiceMinigameUI
- [ ] Create Panel "MultipleChoiceMinigameUI" on Canvas
- [ ] Add TextMeshPro for question display
- [ ] Create 4 buttons in 2×2 grid
- [ ] Add TextMeshPro text to each button
- [ ] Add feedback text (bottom)
- [ ] Add Image component for background (for red flash)
- [ ] Assign all references to script

### MathMinigameUI
- [ ] Create Panel "MathMinigameUI" on Canvas
- [ ] Add TextMeshPro for problem display (e.g. "47 + 38 = ?")
- [ ] Add TextMeshPro for input display
- [ ] Add feedback text (bottom)
- [ ] Add Image component for background (for red flash)
- [ ] Assign all references to script

### PhotoRevealMinigameUI
- [ ] Create TilePrefab (Image + EventTrigger)
- [ ] Create Panel "PhotoRevealMinigameUI" on Canvas
- [ ] Create GridLayoutGroup container (6×6)
- [ ] Add TextMeshPro for progress display
- [ ] Assign Tile Prefab
- [ ] Assign office-themed image as Reveal Sprite
- [ ] Assign all references to script

## Testing 🧪 READY TO TEST

Once UI is set up, use Debug Commands:
- Press `T` → Print a ticket for current day
- Press `M` → Start minigame on first ticket
- Press `N` → Advance day
- Press `F` → Jump to Day 5
- Press `S` → Log state

Example workflow:
1. `T` (Day 1 → Typing ticket)
2. `M` (start typing minigame)
3. `N` (advance to Day 2)
4. `T` (Day 2 → 50% chance Typing, 50% chance Multiple Choice)
5. `M` (start minigame)
6. `N` (advance to Day 3)
7. `T` (Day 3 → Math ticket only)
8. `M` (start math minigame)
9. `N` (advance to Day 4)
10. `T` (Day 4 → Photo Reveal ticket only)
11. `M` (start photo reveal minigame)

## Notes

- All scripts follow the canonical minigame pattern (Singleton, StartMinigame, CloseAfterDelay)
- Day-gating is now fully implemented in TaskDatabase.GetRandomTaskForDay()
- Wrong answers are forgiving (no lives lost, just red flash + retry)
- All minigames auto-stamp and award reward when completed
- Memory Assist upgrade pre-reveals 30% of tiles in PhotoReveal
