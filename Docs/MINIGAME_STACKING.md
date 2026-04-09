# Minigame Stacking System

## Overview

Minigames are **cumulative** across days. Each day adds a new minigame type while keeping all previous types available.

## Day-by-Day Progression

```
Day 1:                          Typing
                                  ↓
Day 2:                    Typing + MultipleChoice
                                  ↓
Day 3:                Typing + MultipleChoice + Math
                                  ↓
Day 4:           Typing + MultipleChoice + Math + PhotoReveal
                                  ↓
Day 5:                          Typing (fallback - impossible quota)
```

## Task Pool Distribution

When a ticket is printed on each day, it randomly selects from the allowed minigame types:

### Day 1
- **Allowed**: Typing
- **Example tickets**:
  - "Draft Status Email" (Typing)
  - "Quick System Update" (Typing)
  - "Meeting Confirmation" (Typing)

### Day 2
- **Allowed**: Typing, MultipleChoice
- **Example tickets**:
  - "Security Protocol Update" (Typing) ✓
  - "File Format Question" (MultipleChoice) ✓
  - "Performance Report" (Typing) ✓
  - "Storage Question" (MultipleChoice) ✓

### Day 3
- **Allowed**: Typing, MultipleChoice, Math
- **Example tickets**:
  - "Incident Report" (Typing) ✓
  - "Protocol Question" (MultipleChoice) ✓
  - "Addition Problem" (Math) ✓
  - "System Architecture Document" (Typing) ✓
  - "Storage Limit Question" (MultipleChoice) ✓
  - "Subtraction Problem" (Math) ✓

### Day 4
- **Allowed**: Typing, MultipleChoice, Math, PhotoReveal
- **Example tickets**:
  - "Executive Summary" (Typing) ✓
  - "Authentication Question" (MultipleChoice) ✓
  - "Complex Arithmetic" (Math) ✓
  - "Identify Object" (PhotoReveal) ✓
  - "Mosaic Puzzle" (PhotoReveal) ✓
  - "Blur Challenge" (PhotoReveal) ✓

### Day 5
- **Allowed**: Typing only (fallback)
- **Note**: Quota is impossible (999,999), so player loses and triggers ending

## Implementation Details

### TaskDatabase.InitializeDefaultTasks()
Creates tasks in 3 difficulty pools (easy, medium, hard), each with the appropriate mix of minigame types:

```csharp
// easyTasks (Difficulty 1) - only Typing tasks (Day 1)
// mediumTasks (Difficulty 2) - Typing + MultipleChoice tasks (Day 2)
// hardTasks (Difficulty 3) - Typing + MultipleChoice + Math + PhotoReveal tasks (Days 3-4)
```

### TaskDatabase.GetRandomTaskForDay()
Filters available tasks by day:

```csharp
switch (currentDay)
{
    case 1: return only Typing
    case 2: return Typing OR MultipleChoice
    case 3: return Typing OR MultipleChoice OR Math
    case 4: return Typing OR MultipleChoice OR Math OR PhotoReveal
    case 5: return Typing (fallback)
}
```

## Gameplay Experience

### Example Day 2 Run
1. Print ticket #1 → "Meeting Minutes" (Typing) — player types
2. Print ticket #2 → "File Format Question" (MultipleChoice) — player clicks
3. Print ticket #3 → "Performance Report" (Typing) — player types
4. Complete 2+ tasks → Sleep button activates → Upgrade modal
5. Choose upgrade → Sleep
6. Advance to Day 3

### Example Day 3 Run
1. Print ticket #1 → "Protocol Question" (MultipleChoice) — player clicks
2. Print ticket #2 → "Addition Problem" (Math) — player types answer
3. Print ticket #3 → "Incident Report" (Typing) — player types
4. Print ticket #4 → "Subtraction Problem" (Math) — player types answer
5. Complete 2+ tasks → Sleep → Upgrade → Advance to Day 4

### Example Day 4 Run
1. Print ticket #1 → "Identify Object" (PhotoReveal) — player hovers
2. Print ticket #2 → "Executive Summary" (Typing) — player types
3. Print ticket #3 → "Authentication Question" (MultipleChoice) — player clicks
4. Print ticket #4 → "Complex Arithmetic" (Math) — player types answer
5. Complete 2+ tasks → Sleep → Upgrade → Advance to Day 5

## Why Stacking?

This design:
- **Increases complexity gradually** — players learn new mechanics one day at a time
- **Creates variety** — even on the same day, tickets can be different minigame types
- **Builds skill** — by Day 4, players are expert at all 4 mechanics
- **Maintains pacing** — no sudden difficulty spike, just one new thing per day
- **Rewards learning** — Day 1 typing skill translates to Day 4 where typing is still available

## Testing Stacking

Use Debug Commands:
1. Press `T` on Day 1 → should only get Typing tickets
2. Press `N` to advance to Day 2
3. Press `T` multiple times → should get mix of Typing and MultipleChoice
4. Press `N` to advance to Day 3
5. Press `T` multiple times → should get Typing, MultipleChoice, or Math
6. Press `N` to advance to Day 4
7. Press `T` multiple times → should get any of the 4 minigame types
8. Press `F` to jump to Day 5 → should only get Typing (or see ending trigger)
