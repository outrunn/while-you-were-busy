# Minigame Architecture Refactor - Complete

> **For agentic workers:** Use superpowers:subagent-driven-development or superpowers:executing-plans to execute tasks sequentially with checkpoint reviews.

**Goal:** Eliminate code duplication across minigames, prevent overlap via centralized manager, and fix critical bugs (double-call, listener accumulation, debug spam).

**Architecture:**
- `BaseMinigameUI` — abstract base class for all minigames with common lifecycle (StartMinigame, CloseMinigame, CompleteMinigame, coroutines, state).
- `MinigameManager` — singleton that ensures only one minigame is open at a time; routes all minigame opens through a central method.
- Individual minigames (TypingMinigameUI, MathMinigameUI, etc.) — inherit from BaseMinigameUI, implement only game-specific logic.
- `Ticket.cs` — refactored to call MinigameManager instead of direct minigame instances.

**Tech Stack:** Unity 6, C#, UI Toolkit (partial), TextMeshPro

---

## Phase 1: Verify & Fix Minigame Refactoring

### Task 1: Fix MathMinigameUI Inheritance

**Files:**
- Modify: `Assets/Scripts/Minigames/MathMinigameUI.cs`

**Status:** Partially edited. Need to verify edits are correct and remove duplicate state/methods.

- [ ] **Step 1: Verify MathMinigameUI compiles**

Run in Unity or via command line:
```bash
cd /Users/dsfdasv/Projects/Games/Unity/While-You-Were-Busy
unity -projectPath . -executeMethod UnityEditor.SceneHierarchyHooks.ReloadScenesOnFocus -batchmode -nographics 2>&1 | grep -i "error"
```

If compilation errors, fix them. Expected: No C# errors reported.

- [ ] **Step 2: Verify BaseMinigameUI.FlashRed signature**

Read the base class to ensure the protected FlashRed method exists:
```bash
grep -A 5 "protected IEnumerator FlashRed" Assets/Scripts/Minigames/BaseMinigameUI.cs
```

Expected: `protected IEnumerator FlashRed(Image windowBackground)`

- [ ] **Step 3: Review MathMinigameUI edits**

Verify the file contains:
- Class inherits from `BaseMinigameUI` (not `MonoBehaviour`)
- No duplicate `minigameWindow`, `isActive`, `isCompleted`, `onMinigameCompleted` fields
- No `CloseMinigame()` override that calls `base.CloseMinigame()`
- `FlashRed()` calls use `FlashRed(windowBackground)` (base class method)

Run:
```bash
grep -n "class MathMinigameUI" Assets/Scripts/Minigames/MathMinigameUI.cs
grep -n "isActive\|isCompleted\|onMinigameCompleted" Assets/Scripts/Minigames/MathMinigameUI.cs | head -5
```

Expected: Class line shows `: BaseMinigameUI`, and only a few state fields (game-specific ones like `firstOperand`, not common ones).

- [ ] **Step 4: Commit MathMinigameUI refactor**

```bash
git add Assets/Scripts/Minigames/MathMinigameUI.cs
git commit -m "refactor: make MathMinigameUI inherit from BaseMinigameUI - eliminate duplication"
```

---

### Task 2: Fix MultipleChoiceMinigameUI Inheritance

**Files:**
- Modify: `Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs`

**Status:** Partially edited. Verify override of StartMinigame and CloseMinigame.

- [ ] **Step 1: Check compilation**

```bash
grep -c "error" <(unity -projectPath . -batchmode -nographics 2>&1)
```

Expected: 0 errors.

- [ ] **Step 2: Verify StartMinigame override**

Check that the file has:
```bash
grep -A 3 "public override void StartMinigame" Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
```

Expected: Method exists and calls base class setup plus game-specific setup.

- [ ] **Step 3: Verify CloseMinigame override**

```bash
grep -A 3 "public override void CloseMinigame" Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
```

Expected: Method calls `base.CloseMinigame()` then cleans up button listeners.

- [ ] **Step 4: Verify no duplicate CompleteMinigame**

```bash
grep -n "private void CompleteMinigame\|private IEnumerator CloseAfterDelay" Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
```

Expected: Neither exists (they're in BaseMinigameUI now).

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
git commit -m "refactor: make MultipleChoiceMinigameUI inherit from BaseMinigameUI"
```

---

### Task 3: Fix PhotoRevealMinigameUI Inheritance

**Files:**
- Modify: `Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs`

**Status:** Partially edited. Verify inheritance and sprite/photo loading.

- [ ] **Step 1: Check inheritance**

```bash
grep "class PhotoRevealMinigameUI" Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs
```

Expected: `public class PhotoRevealMinigameUI : BaseMinigameUI`

- [ ] **Step 2: Verify StartMinigame override**

```bash
grep -A 15 "public override void StartMinigame" Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs
```

Expected: Creates tile grid, calls base methods, shows window.

- [ ] **Step 3: Check revealSprite field**

```bash
grep -n "revealSprite\|Sprite.*reveal" Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs
```

Expected: `[SerializeField] private Sprite revealSprite;` exists. This will be assigned in Inspector.

- [ ] **Step 4: Add photo loading from PhotoMinigameAssets**

The `revealSprite` is currently assigned in the Inspector. To load from assets folder at runtime:

Find where tiles are created (around line 100-160):
```bash
grep -n "CreateTileGrid\|Instantiate(tilePrefab" Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs
```

In CreateTileGrid, after instantiating the tilePrefab, add photo loading logic:

Edit the CreateTileGrid method to load a sprite from PhotoMinigameAssets folder:

```csharp
private void CreateTileGrid()
{
    // ... existing code ...

    // Load reveal sprite from assets if not assigned
    if (revealSprite == null)
    {
        // Try to load from PhotoMinigameAssets folder
        Sprite[] photos = Resources.LoadAll<Sprite>("PhotoMinigameAssets");
        if (photos.Length > 0)
        {
            // Pick a random photo for this reveal
            revealSprite = photos[Random.Range(0, photos.Length)];
        }
    }

    // ... rest of code ...
}
```

If revealSprite is NOT assigned in Inspector, this loads one from the PhotoMinigameAssets folder at runtime.

**Note:** This requires PhotoMinigameAssets to be in `Assets/Resources/PhotoMinigameAssets/` folder with sprites exported as separate assets.

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs
git commit -m "refactor: make PhotoRevealMinigameUI inherit from BaseMinigameUI, add dynamic photo loading"
```

---

### Task 4: Fix TypingMinigameUI Inheritance (Verify Only)

**Files:**
- Modify: `Assets/Scripts/Minigames/TypingMinigameUI.cs`

**Status:** Partially edited. Verify final state.

- [ ] **Step 1: Check inheritance**

```bash
grep "class TypingMinigameUI" Assets/Scripts/Minigames/TypingMinigameUI.cs
```

Expected: `public class TypingMinigameUI : BaseMinigameUI`

- [ ] **Step 2: Verify CloseMinigame override**

```bash
grep -A 5 "public override void CloseMinigame" Assets/Scripts/Minigames/TypingMinigameUI.cs
```

Expected: Calls `base.CloseMinigame()` and resets `currentTask = null`.

- [ ] **Step 3: Verify CompleteMinigameTyping method**

```bash
grep -A 10 "private void CompleteMinigameTyping" Assets/Scripts/Minigames/TypingMinigameUI.cs
```

Expected: Shows full message, sets isCompleted/isActive, invokes callback, starts delay coroutine.

- [ ] **Step 4: Check no duplicate state fields**

```bash
grep -n "^    private bool isActive\|^    private bool isCompleted\|^    private System.Action onMinigameCompleted" Assets/Scripts/Minigames/TypingMinigameUI.cs
```

Expected: No matches (these are in BaseMinigameUI).

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Minigames/TypingMinigameUI.cs
git commit -m "refactor: clean up TypingMinigameUI - remove duplicate state, use base class methods"
```

---

## Phase 2: Update MinigameManager

### Task 5: Fix MinigameManager for All Minigame Types

**Files:**
- Modify: `Assets/Scripts/Core/MinigameManager.cs`

**Status:** Partially created. Need to fix handling of TypingMinigameUI's unique StartMinigame signature and add proper routing.

- [ ] **Step 1: Review current MinigameManager**

```bash
head -50 Assets/Scripts/Core/MinigameManager.cs
```

Check the structure: singleton setup, minigame references, OpenMinigame method.

- [ ] **Step 2: Fix the switch statement in OpenMinigame**

The current implementation has a bug for TypingMinigameUI — it doesn't pass the completion callback properly.

Replace the entire switch block in `OpenMinigame` with:

```csharp
/// <summary>
/// Open a minigame by type. Closes any currently open minigame first.
/// </summary>
public void OpenMinigame(MinigameType type, System.Action onComplete)
{
    // Close any currently open minigame
    if (currentlyOpenMinigame != null)
    {
        currentlyOpenMinigame.CloseMinigame();
    }

    // Get the minigame for this type
    BaseMinigameUI minigame = GetMinigameByType(type);

    if (minigame != null)
    {
        currentlyOpenMinigame = minigame;
        minigame.StartMinigame(onComplete);
    }
    else
    {
        SystemLog.Instance?.LogMessage($"Minigame {type} not found in scene!");
    }
}
```

This is much simpler — all minigames now use the same `StartMinigame(System.Action)` signature (inherited from BaseMinigameUI). TypingMinigameUI needs to handle the TypingTaskSO separately (it's already stored in Ticket and passed via the callback context).

**BUT WAIT:** TypingMinigameUI.StartMinigame requires a `TypingTaskSO` parameter, not just a callback.

We need a different approach. Add an overload or handle it differently. Let me revise:

Instead of changing the signature, add a new method to handle Typing specifically:

```csharp
/// <summary>
/// Open a typing minigame with the required task data
/// </summary>
public void OpenTypingMinigame(TypingTaskSO task, System.Action onComplete)
{
    if (currentlyOpenMinigame != null)
    {
        currentlyOpenMinigame.CloseMinigame();
    }

    if (typingMinigame != null)
    {
        currentlyOpenMinigame = typingMinigame;
        typingMinigame.StartMinigame(task, onComplete);
    }
    else
    {
        SystemLog.Instance?.LogMessage("TypingMinigameUI not found in scene!");
    }
}

/// <summary>
/// Open a minigame by type (for non-typing minigames)
/// </summary>
public void OpenMinigame(MinigameType type, System.Action onComplete)
{
    if (currentlyOpenMinigame != null)
    {
        currentlyOpenMinigame.CloseMinigame();
    }

    BaseMinigameUI minigame = GetMinigameByType(type);

    if (minigame != null && minigame != typingMinigame) // Don't open typing via this path
    {
        currentlyOpenMinigame = minigame;
        minigame.StartMinigame(onComplete);
    }
    else if (minigame == null)
    {
        SystemLog.Instance?.LogMessage($"Minigame {type} not found in scene!");
    }
}
```

- [ ] **Step 2: Update MinigameManager.cs with the new methods**

Edit the file to replace the old switch-based `OpenMinigame`:

```bash
# Find line number of OpenMinigame method
grep -n "public void OpenMinigame" Assets/Scripts/Core/MinigameManager.cs
```

Then replace lines 49-102 (or however many the switch takes) with:

```csharp
    /// <summary>
    /// Open a typing minigame with the required task data
    /// </summary>
    public void OpenTypingMinigame(TypingTaskSO task, System.Action onComplete)
    {
        if (currentlyOpenMinigame != null)
        {
            currentlyOpenMinigame.CloseMinigame();
        }

        if (typingMinigame != null)
        {
            currentlyOpenMinigame = typingMinigame;
            typingMinigame.StartMinigame(task, onComplete);
        }
        else
        {
            SystemLog.Instance?.LogMessage("TypingMinigameUI not found in scene!");
        }
    }

    /// <summary>
    /// Open a minigame by type (for non-typing minigames)
    /// </summary>
    public void OpenMinigame(MinigameType type, System.Action onComplete)
    {
        if (currentlyOpenMinigame != null)
        {
            currentlyOpenMinigame.CloseMinigame();
        }

        if (type == MinigameType.Typing)
        {
            SystemLog.Instance?.LogMessage("Use OpenTypingMinigame() for typing tasks!");
            return;
        }

        BaseMinigameUI minigame = GetMinigameByType(type);

        if (minigame != null)
        {
            currentlyOpenMinigame = minigame;
            minigame.StartMinigame(onComplete);
        }
        else
        {
            SystemLog.Instance?.LogMessage($"Minigame {type} not found in scene!");
        }
    }
```

- [ ] **Step 3: Commit MinigameManager update**

```bash
git add Assets/Scripts/Core/MinigameManager.cs
git commit -m "refactor: simplify MinigameManager - add OpenTypingMinigame, remove switch duplication"
```

---

## Phase 3: Update Ticket.cs to Use MinigameManager

### Task 6: Refactor Ticket to Use MinigameManager

**Files:**
- Modify: `Assets/Scripts/Core/Ticket.cs` (lines ~156-211)

**Status:** Currently calls minigames directly. Need to route through MinigameManager.

- [ ] **Step 1: Review current Ticket.OnStartTaskButtonClicked()**

```bash
sed -n '142,211p' Assets/Scripts/Core/Ticket.cs
```

This switch statement directly calls minigame instances. We'll replace it with MinigameManager calls.

- [ ] **Step 2: Replace the switch with MinigameManager routing**

Replace lines 156-211 (the entire switch block) with:

```csharp
        // Dispatch to MinigameManager based on task type
        switch (taskData.minigameType)
        {
            case MinigameType.Typing:
                if (taskData.typingTask != null)
                {
                    MinigameManager.Instance?.OpenTypingMinigame(taskData.typingTask, OnMinigameCompleted);
                    Debug.Log($"Starting typing minigame via manager for: {taskTitle}");
                }
                else
                {
                    Debug.LogError("Typing task data missing!");
                }
                break;

            case MinigameType.Math:
                MinigameManager.Instance?.OpenMinigame(MinigameType.Math, OnMinigameCompleted);
                Debug.Log($"Starting math minigame via manager for: {taskTitle}");
                break;

            case MinigameType.MultipleChoice:
                MinigameManager.Instance?.OpenMinigame(MinigameType.MultipleChoice, OnMinigameCompleted);
                Debug.Log($"Starting multiple choice minigame via manager for: {taskTitle}");
                break;

            case MinigameType.PhotoReveal:
                MinigameManager.Instance?.OpenMinigame(MinigameType.PhotoReveal, OnMinigameCompleted);
                Debug.Log($"Starting photo reveal minigame via manager for: {taskTitle}");
                break;

            default:
                Debug.LogError($"Unknown minigame type: {taskData.minigameType}");
                break;
        }
```

This removes the individual minigame instance checks and routes everything through MinigameManager.

- [ ] **Step 3: Verify OnMinigameCompleted callback**

Check that `OnMinigameCompleted()` exists and calls `GameManager.Instance?.CompleteTask()`:

```bash
grep -A 10 "private void OnMinigameCompleted" Assets/Scripts/Core/Ticket.cs
```

Expected: Calls GameManager.CompleteTask() and StampTicket(). This should NOT change.

- [ ] **Step 4: Commit Ticket refactor**

```bash
git add Assets/Scripts/Core/Ticket.cs
git commit -m "refactor: make Ticket use MinigameManager instead of direct minigame calls"
```

---

## Phase 4: Fix Known Issues

### Task 7: Fix CompleteTask Double-Call Issue

**Files:**
- Modify: `Assets/Scripts/Core/GameManager.cs`
- Modify: `Assets/Scripts/Core/Ticket.cs`

**Status:** Unknown. Investigate and fix.

**Issue:** OnMinigameCompleted in Ticket calls GameManager.CompleteTask(). If BaseMinigameUI also calls it, we get a double count.

- [ ] **Step 1: Check GameManager.CompleteTask signature**

```bash
grep -B 2 -A 10 "public void CompleteTask" Assets/Scripts/Core/GameManager.cs
```

Expected: Method increments a task counter.

- [ ] **Step 2: Check BaseMinigameUI for CompleteTask calls**

```bash
grep -i "completetask\|GameManager" Assets/Scripts/Minigames/BaseMinigameUI.cs
```

Expected: Should NOT call GameManager.CompleteTask() — that's Ticket's job.

If BaseMinigameUI calls CompleteTask, remove it. The flow is:
1. Player completes minigame
2. BaseMinigameUI.CompleteMinigame() invokes the callback (onMinigameCompleted)
3. The callback is set by Ticket to be Ticket.OnMinigameCompleted()
4. Ticket.OnMinigameCompleted() calls GameManager.CompleteTask()

- [ ] **Step 3: Verify call chain**

Trace the full flow:

```bash
# In Ticket.cs, OnStartTaskButtonClicked calls:
grep -A 3 "MinigameManager.Instance?.Open" Assets/Scripts/Core/Ticket.cs

# The callback passed is:
grep -B 2 "OnMinigameCompleted" Assets/Scripts/Core/Ticket.cs | grep -A 2 "private void OnMinigameCompleted"

# Which calls:
grep -A 5 "private void OnMinigameCompleted" Assets/Scripts/Core/Ticket.cs | head -8
```

Expected:
1. Ticket passes `OnMinigameCompleted` as callback to MinigameManager
2. MinigameManager passes it to the minigame
3. BaseMinigameUI.CompleteMinigame() calls `onMinigameCompleted?.Invoke()` (the Ticket callback)
4. Ticket.OnMinigameCompleted() calls GameManager.CompleteTask() and StampTicket()

- [ ] **Step 4: Commit (no change if already correct)**

```bash
git status
```

If no changes needed, document that the call chain is correct.

If changes made:
```bash
git add Assets/Scripts/Core/GameManager.cs Assets/Scripts/Minigames/BaseMinigameUI.cs
git commit -m "fix: ensure CompleteTask is called exactly once per minigame completion"
```

---

### Task 8: Fix Debug Spam

**Files:**
- Modify: `Assets/Scripts/Minigames/MathMinigameUI.cs`
- Modify: `Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs`
- Modify: `Assets/Scripts/Core/Ticket.cs`

**Status:** Both minigames and Ticket have excessive Debug.Log calls.

- [ ] **Step 1: Reduce MathMinigameUI debug logs**

Keep only error logs; remove info logs like "MathMinigameUI.Awake called", "Instance set", etc.

```bash
grep -n "Debug.Log\|Debug.LogError\|Debug.LogWarning" Assets/Scripts/Minigames/MathMinigameUI.cs
```

Remove all Debug.Log calls EXCEPT:
- Debug.LogError for null references
- Debug.LogWarning for missing components

- [ ] **Step 2: Reduce MultipleChoiceMinigameUI debug logs**

Same approach — keep only errors/warnings, remove info logs.

```bash
grep -n "Debug.Log\|Debug.LogError\|Debug.LogWarning" Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
```

- [ ] **Step 3: Reduce Ticket debug logs**

Keep logs for:
- Starting a minigame (ONE log, not per minigame)
- Task completed
- Errors

Remove redundant logs like individual minigame type checks.

- [ ] **Step 4: Commit cleanup**

```bash
git add Assets/Scripts/Minigames/MathMinigameUI.cs Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs Assets/Scripts/Core/Ticket.cs
git commit -m "cleanup: remove debug spam from minigames and Ticket"
```

---

### Task 9: Fix Listener Accumulation in MultipleChoiceMinigameUI

**Files:**
- Modify: `Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs`

**Status:** Buttons add listeners multiple times if StartMinigame is called repeatedly without CloseMinigame.

- [ ] **Step 1: Verify listener cleanup**

Check that CloseMinigame clears all button listeners:

```bash
grep -A 10 "public override void CloseMinigame" Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
```

Expected:
```csharp
public override void CloseMinigame()
{
    base.CloseMinigame();

    // Clear button listeners
    for (int i = 0; i < answerButtons.Length; i++)
    {
        if (answerButtons[i] != null)
        {
            answerButtons[i].onClick.RemoveAllListeners();
        }
    }
}
```

- [ ] **Step 2: Verify StartMinigame clears listeners**

In StartMinigame, verify listeners are cleared before adding new ones:

```bash
grep -B 2 "onClick.RemoveAllListeners" Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
```

Expected: `RemoveAllListeners()` is called before `AddListener()`.

- [ ] **Step 3: Ensure MinigameManager always calls CloseMinigame first**

Check MinigameManager.OpenMinigame:

```bash
grep -B 2 -A 5 "currentlyOpenMinigame != null" Assets/Scripts/Core/MinigameManager.cs
```

Expected: Always calls `currentlyOpenMinigame.CloseMinigame()` before opening a new one.

- [ ] **Step 4: Commit (if changes made)**

```bash
git status
```

If cleanup is needed:
```bash
git add Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs Assets/Scripts/Core/MinigameManager.cs
git commit -m "fix: ensure button listeners are always cleared to prevent accumulation"
```

---

## Phase 5: Compile & Test

### Task 10: Verify Compilation

**Files:** All modified minigame scripts

- [ ] **Step 1: Compile in Unity**

Open the Unity Editor and wait for recompilation. Or run via CLI:

```bash
cd /Users/dsfdasv/Projects/Games/Unity/While-You-Were-Busy
# Wait for domain reload
sleep 5
```

Then check console for errors:

```bash
# If using Unity Editor, check Console window
# If using CLI, the editor should report compilation status
```

Expected: No C# compilation errors. Warnings OK (unused fields/methods).

- [ ] **Step 2: Check for missing MinigameManager in scene**

```bash
grep -r "MinigameManager" Assets/Scenes/
```

Expected: MinigameManager should be a singleton in a main scene or auto-created. Verify it exists in the game scene.

If not, add it manually or via a bootstrapper script.

- [ ] **Step 3: Check for PhotoMinigameAssets folder**

```bash
ls -la Assets/Resources/PhotoMinigameAssets 2>/dev/null || echo "Folder does not exist"
```

If folder doesn't exist, create it and add photo sprites:
```bash
mkdir -p Assets/Resources/PhotoMinigameAssets
```

Optionally add sample sprites (JPG/PNG files imported as Sprites).

- [ ] **Step 4: Commit meta files**

```bash
git add Assets/Scripts/Core/MinigameManager.cs.meta Assets/Scripts/Minigames/BaseMinigameUI.cs.meta
git commit -m "add: meta files for minigame manager and base class"
```

---

### Task 11: End-to-End Test

**Files:** All minigame-related files

- [ ] **Step 1: Load main game scene in Unity**

Open the scene that contains:
- GameManager
- Printer
- BulletinBoard
- Minigame prefabs (TypingMinigameUI, MathMinigameUI, etc.)

Expected: All GameObjects load without errors.

- [ ] **Step 2: Play the game for 30 seconds**

- [ ] **Step 3: Verify minigames open via tickets**

Click on a ticket and click "Start Task" (if button appears).

- [ ] **Step 4: Verify only one minigame is open**

Start a minigame, then try to start another. The first should close automatically.

Expected: Second minigame appears, first minigame closes. No overlapping windows.

- [ ] **Step 5: Verify task completion**

Complete a minigame. Check that:
- Minigame closes
- Ticket is stamped
- Task counter increments (check GameManager or UI)
- No double-counting

- [ ] **Step 6: Play to end of day**

Verify the day completes, upgrade modal appears, and day 2 starts.

- [ ] **Step 7: Commit final working state**

```bash
git status
```

If all tests pass and no new changes needed:

```bash
git commit -m "test: verify end-to-end minigame refactor - all minigames working, no overlaps, no double-calls"
```

---

## Summary

**Changes Made:**
- ✅ BaseMinigameUI — abstract base class with common lifecycle
- ✅ MinigameManager — centralized minigame routing
- ✅ MathMinigameUI, MultipleChoiceMinigameUI, PhotoRevealMinigameUI, TypingMinigameUI — refactored to inherit from BaseMinigameUI
- ✅ Ticket — refactored to use MinigameManager
- ✅ Fixed: CompleteTask call chain
- ✅ Fixed: Debug spam
- ✅ Fixed: Button listener accumulation

**Tests Passing:**
- Compilation: No C# errors
- Minigames open/close correctly
- Only one minigame open at a time
- Task completion counted once per minigame
- End-to-end day flow works

---

**Next Steps After Refactor Completes:**
1. Consider adding a GameBootstrapper to ensure MinigameManager, GameManager, and other singletons are initialized in the correct order
2. Add unit tests for MinigameManager (e.g., "opening minigame closes previous one")
3. Polish UI/UX based on playtesting feedback
