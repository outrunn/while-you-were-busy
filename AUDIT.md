# While You Were Busy — Architecture Audit Report

**Date:** 2026-04-15
**Status:** ⚠️ SIGNIFICANT ISSUES FOUND — Recommend rebuild with clean architecture

---

## Executive Summary

The project has **working functionality** but suffers from **critical overlapping responsibilities, weak separation of concerns, and fragile initialization chains**. While individual systems are reasonably well-documented, they interlock in ways that make the codebase brittle and hard to extend.

**Key Issues:**
1. **Overlapping state management** — Multiple systems modify the same state (day, task completion)
2. **Fragile initialization order** — GameBootstrapper and AutoSetupScene hide critical setup, breaking if order changes
3. **Circular dependencies** — Ticket → MinigameManager → Ticket, Printer → TaskDatabase → TypingTaskDatabase
4. **Mixed concerns** — Ticket handles minigame launching, drag-drop, stamping, AND rendering state
5. **Brittle resource loading** — AutoSetupScene does runtime GameObject creation + setup in Start(), feels hacky
6. **Singletons everywhere** — 6+ singletons make testing impossible and hide dependencies

---

## Detailed Audit

### 1. **Singleton Proliferation**

| Singleton | Responsibility |
|-----------|-----------------|
| `GameManager` | Day timer, task completion, outputs, health |
| `MinigameManager` | Minigame state, prevent overlaps |
| `TaskDatabase` | Task pools by difficulty, random selection |
| `TypingTaskDatabase` | Typing task SO references, random selection |
| `DayManager` | Day transitions, upgrade modals, ending |
| `SystemLog` | Debug logging |
| `TicketPool` | Object pooling (optional) |
| `BulletinBoard` | Pinned ticket tracking |

**Problem:** 8 singletons for a game this size. Each hides dependencies, makes unit testing impossible, creates hidden initialization order.

**Example Issue:** TaskDatabase.Start() checks if TypingTaskDatabase exists, creates it if not (line 88-92). This is a bandaid for missing initialization order.

---

### 2. **Overlapping Responsibilities**

#### GameManager
- **Manages:** day timer, tasksCompleted counter, day progression
- **Also calls:** CompleteTask() when minigame finishes (line 269)
- **Also calls:** TriggerEnding() through DayManager (line 120)
- **Also called by:** Ticket.OnMinigameCompleted(), ShredderUI.ShredTicket()

**Issue:** Three different systems (Ticket, ShredderUI, Minigame) all call GameManager.CompleteTask(). No clear ownership of "task completion" semantics.

#### Ticket
- Handles minigame launching (50+ lines on minigame routing)
- Handles drag-drop with raycast detection
- Manages expand/collapse animation
- Stores task data and calls GameManager.CompleteTask()
- Calls MinigameManager methods
- **Issue:** A "ticket" class shouldn't know about minigame types, raycasting, or game state. It's a view-model hybrid.

#### Printer
- Spawns tickets every 15 seconds
- Tracks active ticket count
- Calls TaskDatabase.GetRandomTaskForDay()
- Calls TicketPool.GetTicket()
- Calls BulletinBoard.GetNextSlotAnchoredPosition()
- **Issue:** Printer is responsible for 5 different concerns (timing, pooling, selection, animation, integration).

#### AutoSetupScene
- Loads 10 sprites from Resources
- Creates GameObjects dynamically
- Configures Canvas in Start() (line 27-39)
- Called from GameBootstrapper.Start() (line 98)
- **Issue:** UI setup happens at **runtime** in Start(), not at edit time. Fragile and slow.

---

### 3. **Fragile Initialization Order**

**Current order (GameBootstrapper.Awake):**
1. GameManager.Instance (Awake)
2. Printer.Instance (FindFirstObjectByType)
3. BulletinBoard.Instance (FindFirstObjectByType)
4. MinigameManager.Instance (FindFirstObjectByType)

**Then GameBootstrapper.Start() calls:**
5. AutoSetupScene.SetupAllAssets() (creates UI at runtime)

**Problem:** If any system's Start() runs before GameBootstrapper.Awake finishes, initialization breaks.

**Evidence:**
- Printer.Start() calls TicketPool.Instance, TaskDatabase.Instance, BulletinBoard methods
- TaskDatabase.Start() creates TypingTaskDatabase if missing
- GameBootstrapper uses FindFirstObjectByType() instead of assigned references

**Cascade risk:** Move one component to a different Awake() timing, and the whole chain breaks.

---

### 4. **Circular Dependencies**

```
Ticket
  ↓ (calls MinigameManager.Open*())
MinigameManager
  ↓ (minigame.StartMinigame(onComplete))
BaseMinigameUI (e.g., TypingMinigameUI)
  ↓ (calls onMinigameCompleted callback)
Ticket.OnMinigameCompleted()
  ↓ (calls GameManager.CompleteTask())
GameManager
```

This isn't broken, but it's a chain that's hard to test.

**Second cycle:**
```
Printer.PrintTicket()
  ↓
TaskDatabase.GetRandomTaskForDay()
  ↓
TypingTaskDatabase.GetRandomTypingTask()
  ↓ (TypingTaskDatabase auto-created if missing)
TaskDatabase.EnsureInitialized()
```

---

### 5. **State Management Issues**

#### tasksCompleted is incremented in two places:
1. **Ticket.OnMinigameCompleted()** → calls GameManager.CompleteTask()
2. **ShredderUI.ShredTicket()** → calls GameManager.CompleteTask()

Both paths increment the same counter. But Ticket stamps the task, then ShredderUI shreds it. If a ticket is stamped but not shredded, the task is counted as complete but the player can still drag it around.

**Is this intentional?** It seems like it — stamping = task complete, shredding = claim reward. But the code is ambiguous.

---

### 6. **Weak Event System**

No pub-sub or event system. Everything is:
- Direct method calls (Ticket → GameManager.CompleteTask())
- FindFirstObjectByType() lookups (ShredderUI finds Printer)
- Singletons.Instance access

This makes it hard to add observers (e.g., UI updates when a task completes).

---

### 7. **Resource Loading at Runtime**

AutoSetupScene loads 10 sprites at runtime in GameBootstrapper.Start():
```csharp
Sprite wallpaper = Resources.Load<Sprite>("UI_Assets/image 2");
// ... 9 more loads ...
```

**Problems:**
- Slow (synchronous file I/O in Start)
- Fragile (names are strings, typos silently fail)
- Can't be tested in edit mode
- Proper pattern: Set up in scene at edit time

---

### 8. **Minigame Type Routing is Error-Prone**

Ticket.OnStartTaskButtonClicked() has a switch statement (lines 163-191):
```csharp
switch (taskData.minigameType)
{
    case MinigameType.Typing:
        // ... call OpenTypingMinigame()
    case MinigameType.Math:
        // ... call OpenMinigame()
    // ... etc
}
```

If you add a new minigame type, you must:
1. Update MinigameType enum
2. Add reference to MinigameManager
3. Add case in this switch
4. Add case in TaskDatabase.GetRandomTaskForDay() (lines 241-273)
5. Add task pool logic

No clear extension point.

---

### 9. **Ticket Lifecycle is Confusing**

A Ticket goes through these states:
1. **Created** (by Printer.PrintTicket)
2. **Spawned** (in mini form, 0.3x scale)
3. **Animated to board** (travels to BulletinBoard)
4. **Pinned** (on BulletinBoard, can be clicked)
5. **Expanded** (clicked, shows details)
6. **Stamped** (minigame completed, shows stamp image)
7. **Dragged** (expanded, can drag to shredder)
8. **Shredded** (destroyed, returns to pool)

But the code handling these states is spread across:
- Ticket.cs (expand/collapse, stamping, dragging)
- Printer.cs (spawning, animation to board)
- BulletinBoard.cs (pinning, organization)
- ShredderUI.cs (destruction)

No state machine. Hard to visualize or debug.

---

### 10. **Error Handling is Ad-Hoc**

Examples:
- Printer checks if ticketPrefab is null (line 56-60) but logs warning, not error
- TaskDatabase creates TypingTaskDatabase if missing (line 88-92), hiding a setup problem
- Ticket checks if minigameType is None (line 144) but returns with warning
- ShredderUI has hardcoded debug logs everywhere (lines 23, 38, 79, 80)

No consistent error handling. Some errors are silently ignored.

---

## Recommendation: Rebuild or Refactor?

### **Refactor (if you have 2-3 days):**
- Extract state from GameManager into a separate GameState struct
- Create an event system (UnityEvent or simple pub-sub)
- Build a proper initialization layer (not GameBootstrapper, but a real dependency injector)
- Replace Ticket with a TicketPresenter + TicketModel separation
- Move minigame routing to a strategy pattern (MinigameFactory)

### **Rebuild (if you have 5-7 days):**
- **Clean slate approach:**
  - GameManager → manage only day timer and quota
  - EventBus → pub-sub for task completion, day changes, minigame events
  - TicketService → manage ticket creation, stamping, shredding (no Ticket.cs drag logic)
  - MinigameService → manage minigame lifecycle (no direct Ticket→MinigameManager calls)
  - TaskService → task selection, pooling (replaces TaskDatabase + TypingTaskDatabase)
  - No singletons — inject dependencies via constructor
  - Set up all UI in scene at edit time (no AutoSetupScene runtime creation)

**Why rebuild might be better:**
- Current code has too many implicit assumptions
- Dependencies are hidden (singletons make them invisible)
- State ownership is ambiguous
- Adding new minigames requires 5+ changes in different files

---

## Summary Table

| Issue | Severity | Impact |
|-------|----------|--------|
| 8 singletons | High | Untestable, hidden dependencies |
| Overlapping responsibility (GameManager, Ticket, Printer) | High | Ambiguous state ownership |
| Fragile initialization order | High | Breaks if components move |
| Circular dependencies | Medium | Hard to test, hard to refactor |
| Resource loading at runtime | Medium | Slow, fragile, non-editor-testable |
| No event system | Medium | Hard to add observers, tight coupling |
| Minigame type routing duplicated | Medium | Violates DRY, error-prone to extend |
| Ticket lifecycle spread across 4 files | High | Hard to debug, hard to extend |
| Ad-hoc error handling | Low | Silent failures, hidden bugs |

---

## Verdict

**Current state:** ✅ **Functionally works**, but ❌ **architecturally unsound**

**New developer experience:** "Where does task completion happen?" → Answer is spread across GameManager, Ticket, ShredderUI, MinigameManager. Confusing.

**Adding features:** Hard. Need to touch 5-10 files for any new minigame type.

**Testing:** Impossible. Can't unit test anything with 8 singletons and no DI.

---

## Next Steps

**Choose one:**

1. **Rebuild** — 5-7 days, proper architecture, scalable to 10+ minigames
2. **Refactor incrementally** — 2-3 days, still single-threaded, but cleaner
3. **Keep as-is** — Works now, but won't scale

If scaling to 10+ minigames or a team > 1 dev is planned: **Rebuild**.
If this is a one-person game with 5 minigames max: **Refactor**.
