# Minigame UI Setup Guide

All 4 minigame scripts have been created and implemented. Here's what you need to build in the Unity Inspector for each one.

## Summary

| Day | Minigame | Type | Script | Status |
|-----|----------|------|--------|--------|
| 1 | Email Typing | Keyboard | TypingMinigameUI.cs | ✅ Already implemented |
| 2 | Multiple Choice | Click 4 buttons | MultipleChoiceMinigameUI.cs | 🔧 Needs UI |
| 3 | Math Problems | Keyboard input | MathMinigameUI.cs | 🔧 Needs UI |
| 4 | Photo Reveal | Mouse hover | PhotoRevealMinigameUI.cs | 🔧 Needs UI |
| 5 | Day 5 Ending | Narrative | (Uses DayManager) | ✅ Already implemented |

---

## 1. MultipleChoiceMinigameUI Panel

**Prefab/GameObject to create:**
- Create a Panel named "MultipleChoiceMinigameUI"
- Attach script: `MultipleChoiceMinigameUI.cs`

**Inspector assignments:**
- `Minigame Window` → the Panel itself (or a child container)
- `Question Text` → TextMeshPro text, large font (center-aligned)
- `Answer Buttons[0]` → Button (top-left, "A")
- `Answer Buttons[1]` → Button (top-right, "B")
- `Answer Buttons[2]` → Button (bottom-left, "C")
- `Answer Buttons[3]` → Button (bottom-right, "D")
- `Answer Texts[0..3]` → TextMeshPro text on each button
- `Feedback Text` → TextMeshPro text (bottom, shows "Correct!" or "Incorrect!")
- `Window Background` → Image component (used for red flash effect)

**Hardcoded questions in script:** 8 office-themed Q&A pairs (defined in `questions` array)

---

## 2. MathMinigameUI Panel

**Prefab/GameObject to create:**
- Create a Panel named "MathMinigameUI"
- Attach script: `MathMinigameUI.cs`

**Inspector assignments:**
- `Minigame Window` → the Panel itself
- `Problem Text` → TextMeshPro text, large font (shows "47 + 38 = ?")
- `Input Text` → TextMeshPro text (shows typed digits: "125")
- `Feedback Text` → TextMeshPro text (shows "Correct!" or "Incorrect!")
- `Window Background` → Image component (used for red flash)

**Input method:** Keyboard only (0-9 keys or numpad, Enter to submit, Backspace to delete)

**Problem generation:**
- Easy (difficulty 1): random 1-20
- Medium (difficulty 2): random 10-50
- Hard (difficulty 3): random 20-99
- Operations: Addition or Subtraction (subtraction ensures no negatives)

---

## 3. PhotoRevealMinigameUI Panel

**Prefab/GameObject to create:**
- Create a Panel named "PhotoRevealMinigameUI"
- Attach script: `PhotoRevealMinigameUI.cs`

**First, create a Tile Prefab:**
1. Create a new empty GameObject named "TilePrefab"
2. Add an Image component
3. Set Image → raycastTarget to TRUE (so hover detection works)
4. Add EventTrigger component (part of EventSystem)
5. Save as a prefab in Assets/Prefabs/TilePrefab.prefab

**Inspector assignments on PhotoRevealMinigameUI:**
- `Minigame Window` → the Panel
- `Image Container` → Child panel with GridLayoutGroup component
  - GridLayoutGroup settings:
    - Cell Size: ~50×50 (adjust based on screen size)
    - Spacing: 2, 2
    - Constraint: Fixed Column Count = 6
    - Child Force Expand: Width/Height unchecked
- `Progress Text` → TextMeshPro text (bottom, shows "52% revealed")
- `Tile Prefab` → the TilePrefab you created
- `Tile Count X` → 6 (width in tiles)
- `Tile Count Y` → 6 (height in tiles)
- `Reveal Threshold` → 0.8 (80% to complete)
- `Completion Delay` → 2.5
- `Reveal Sprite` → Sprite to display when tiles are revealed (office-themed image)

**How it works:**
- Tiles start as colored mosaic blocks (random muted colors)
- On hover, a tile transitions to the `revealSprite` (the real image)
- At 80% revealed, minigame completes automatically
- Upgrade hook: Memory Assist pre-reveals 30% of tiles on start

---

## Layout Recommendations

All three minigames should be placed as sibling Panels on the main Canvas:

```
Canvas
├── TypingMinigameUI (already exists)
├── MultipleChoiceMinigameUI (create)
├── MathMinigameUI (create)
└── PhotoRevealMinigameUI (create)
```

Each should be **hidden by default** (`SetActive(false)`) and turned on when the minigame starts.

---

## Testing with Debug Commands

Use the debug keyboard shortcuts to test:
- `T` → Print a ticket (will spawn minigame tasks based on current day)
- `N` → Advance to next day
- `F` → Jump to Day 5
- `M` → Start the minigame on the first unstamped ticket
- `S` → Log current game state

Example workflow:
1. Press `T` to print a Day 2 ticket (will be Typing or Multiple Choice)
2. Press `M` to start the minigame
3. Complete the minigame
4. Press `N` to go to Day 3 (Math only)
5. Repeat

---

## Notes

- All minigames follow the canonical pattern: Singleton, StartMinigame(), CloseAfterDelay, callback before close
- Wrong answers flash red and reset (no harsh penalties, player can retry)
- Correct answers complete immediately with a "Correct!" message
- All minigames auto-complete and stamp the ticket when done
