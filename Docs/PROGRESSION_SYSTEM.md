# Progression System (No Difficulty - Minigames + Upgrades Only)

## Overview

The game progression is **purely driven by two mechanics**:
1. **Stacking Minigames** — Each day adds a new minigame type
2. **Upgrades** — Meaningful rewards that give players an advantage

There is **no difficulty scaling**. All tasks pay out fixed rewards. Upgrades are the only way players gain mechanical advantages.

## Minigame Stacking

Minigames accumulate across days:

```
Day 1:                                    Typing
Day 2:                            Typing + MultipleChoice
Day 3:                      Typing + MultipleChoice + Math
Day 4:             Typing + MultipleChoice + Math + PhotoReveal
Day 5:                      Typing (fallback - impossible quota)
```

## Task Rewards

All tasks have **fixed base rewards** (no difficulty scaling):

### Typing Tasks
- Variety of email/documentation tasks
- Rewards: 10-300 points (varies by task, not by difficulty)

### Multiple Choice Tasks
- Office-themed questions
- Rewards: 45-210 points (varies by task)

### Math Tasks
- Addition/subtraction problems
- Random difficulty each round (1-99 operands)
- Rewards: 220-250 points

### Photo Reveal Tasks
- Mosaic image hover-to-uncover
- Rewards: 280-300 points

## Upgrade System

Upgrades are the **only source of mechanical progression**. They give meaningful edges:

### Day 1 Upgrades
- **Faster Typing**: Type fewer keys to complete typing minigame
- **Auto Stamp**: (Placeholder) Auto-complete tickets without minigame

### Day 2 Upgrades
- **Number Lock**: (Day 2 hint/advantage in future minigames)
- **Batch Process**: (Process multiple tickets simultaneously - future mechanic)

### Day 3 Upgrades
- **Pre-Sorted**: (Future mechanic for Day 3+ minigames)
- **Quick Scan**: (Future mechanic for scanning/review)

### Day 4 Upgrades
- **Overclock**: (Speed up minigame mechanics)
- **Memory Assist**: Pre-reveal 30% of photo tiles at start

## Gameplay Loop

### Standard Day (1-4)

1. **Printer spawns tickets** at regular intervals
2. **Player completes minigames** (no difficulty scaling, same mechanics across day)
3. **Each completion awards fixed points**
4. **Upgrades make completion easier/faster** (not harder tasks)
5. **Meet quota → Sleep → Choose upgrade → Advance**

### Example: Day 2 with "Faster Typing" Upgrade

- Without upgrade: Type 50 keys to complete typing task
- With upgrade: Type 25 keys to complete same task
- Same reward: 60 points
- **Upgrade benefit**: Faster completion time = more tickets per day = more quota progress

### Example: Day 4 with "Memory Assist" Upgrade

- Photo Reveal task normally: Hover to reveal 100% of 36 tiles for completion
- With upgrade: 30% pre-revealed at start → only hover 70% more to complete
- Same reward: 290 points
- **Upgrade benefit**: Faster completion = more tickets per day

## Task Pools

Tasks are organized in 3 pools (easy, medium, hard) for distribution, but:
- **No difficulty scaling by progression**
- **No difficulty gates by day**
- **All minigame types from a pool are available each day** (filtered only by day stacking rule)

### Task Pool Organization

**Easy Pool** (Difficulty 1):
- Typing tasks only (Day 1)

**Medium Pool** (Difficulty 2):
- Typing tasks
- Multiple Choice tasks (Day 2+)

**Hard Pool** (Difficulty 3):
- Typing tasks
- Multiple Choice tasks
- Math tasks (Day 3+)
- Photo Reveal tasks (Day 4+)

**Why these pools?**
- Purely for task variety management
- Nothing to do with player progression
- Printer randomly picks from appropriate pool

## Day 5: The Impossible Quota

- **Quota**: 999,999 (impossible to reach)
- **Minigames available**: Typing only
- **Upgrade**: None (Day 5 has no upgrade choice)
- **Outcome**: Player loses, triggers narrative ending (AI reveal)

## Difficulty Variation (Not Scaling)

The **only randomness** is in minigame difficulty:

### Math Minigame
- Each problem gets random difficulty (1, 2, or 3)
- Not based on player progression
- Just for variety within a single task
- Operands: 1-20, 10-50, or 20-99

### All Other Minigames
- Same mechanics regardless of round

## Progression Mechanics (What Actually Changes)

| Mechanism | Day 1 | Day 2 | Day 3 | Day 4 |
|-----------|-------|-------|-------|-------|
| Available Minigames | Typing | +MC | +Math | +Photo |
| Minigame Mechanics | Same | Same | Same | Same |
| Task Rewards | Fixed | Fixed | Fixed | Fixed |
| Upgrade Available | Yes | Yes | Yes | Yes |
| Upgrade Benefit | Faster typing | Future | Future | Faster photos |

## No Difficulty Tiers

✗ No Easy/Medium/Hard tasks
✗ No difficulty gates by progression
✗ No reward scaling by completion speed
✗ No cumulative difficulty increase
✓ **ONLY**: Stacking minigames + Upgrade rewards

## Summary

Players progress by:
1. **Learning new minigames** (Day 2: Multiple Choice, Day 3: Math, Day 4: Photos)
2. **Choosing strategic upgrades** to gain advantages
3. **Using upgrades to complete tasks faster** to meet quotas

Upgrades are **not stat increases** — they're **mechanical advantages** that change how minigames work (faster, easier, assisted).
