# Complete Minigame Setup with Upgrade Integration

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Set up all 4 minigames (Typing, Math, Photo Reveal, Multiple Choice) so they are fully playable with proper win conditions, upgrade effects, and clean architecture with zero janky conflicting logic.

**Architecture:**
- Each minigame prefab is self-contained with all UI elements pre-configured
- BaseMinigameUI provides common win/loss logic
- Upgrade integration via GameEvents pub/sub pattern (no direct coupling)
- Each minigame listens to OnUpgradePurchased and applies effects independently
- Win conditions are explicit and testable

**Tech Stack:** Unity 2022+, TextMeshPro, UI/UGUI, C# events

---

## File Structure

### Existing Files (Will Modify)
- `Assets/Scripts/Minigames/BaseMinigameUI.cs` - Add common win/loss handler
- `Assets/Scripts/Minigames/TypingMinigameUI.cs` - Add FasterTyping upgrade support
- `Assets/Scripts/Minigames/MathMinigameUI.cs` - Add NumberLock upgrade support
- `Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs` - Add MemoryAssist upgrade support
- `Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs` - Add new script, add upgrade support

### New Files (Will Create)
- `Assets/Scripts/Minigames/Tile.cs` - Tile prefab for PhotoReveal (instantiated at runtime)
- `Assets/Scripts/Minigames/AnswerButton.cs` - Button prefab for MultipleChoice (instantiated at runtime)

### Prefabs (Will Modify)
- `Assets/Resources/Prefabs/Minigames/TypingMinigame.prefab`
- `Assets/Resources/Prefabs/Minigames/MathMinigame.prefab`
- `Assets/Resources/Prefabs/Minigames/PhotoRevealMinigame.prefab`
- `Assets/Resources/Prefabs/Minigames/MultipleChoiceMinigame.prefab`

---

## Task Breakdown

### Task 1: Update BaseMinigameUI with Upgrade Event Listener

**Files:**
- Modify: `Assets/Scripts/Minigames/BaseMinigameUI.cs`

**Context:** BaseMinigameUI is the abstract base class that all minigames inherit from. We need to add a protected method that all minigames can override to respond to upgrade purchases.

- [ ] **Step 1: Read BaseMinigameUI.cs to understand current structure**

Check lines that define the base class, CompleteMinigame(), and ShowWindow() methods.

- [ ] **Step 2: Add protected method for upgrade handling**

Add this method to BaseMinigameUI after the `ShowWindow()` method:

```csharp
/// <summary>
/// Called when an upgrade is purchased. Override in subclass to apply effects.
/// </summary>
protected virtual void OnUpgradePurchased(UpgradeType upgradeType)
{
    // Base implementation: do nothing
    // Subclasses override to apply upgrade-specific effects
}
```

- [ ] **Step 3: Add upgrade event subscription in Awake**

In the Awake method, after the singleton setup, add:

```csharp
// Subscribe to upgrade events
if (GameEvents.Instance != null)
{
    GameEvents.Instance.OnUpgradePurchased += OnUpgradePurchased;
}
```

- [ ] **Step 4: Add event unsubscription in OnDestroy**

Add an OnDestroy method:

```csharp
protected virtual void OnDestroy()
{
    // Unsubscribe from upgrade events
    if (GameEvents.Instance != null)
    {
        GameEvents.Instance.OnUpgradePurchased -= OnUpgradePurchased;
    }
}
```

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Minigames/BaseMinigameUI.cs
git commit -m "feat: add upgrade event listener to BaseMinigameUI"
```

---

### Task 2: Implement TypingMinigameUI with FasterTyping Upgrade

**Files:**
- Modify: `Assets/Scripts/Minigames/TypingMinigameUI.cs`

**Context:** The Typing minigame needs to track key presses. The FasterTyping upgrade reduces the number of required key presses. Currently requiredKeyPresses is hardcoded; we'll make it dynamic based on the upgrade.

- [ ] **Step 1: Add upgrade effect method to TypingMinigameUI**

Add this method at the end of the class:

```csharp
/// <summary>
/// FasterTyping upgrade reduces key presses by 20%
/// </summary>
protected override void OnUpgradePurchased(UpgradeType upgradeType)
{
    if (upgradeType == UpgradeType.FasterTyping)
    {
        Debug.Log("FasterTyping upgrade applied to Typing minigame");
        // The effect is applied when the next minigame starts via CalculateRequiredKeyPresses()
    }
}

/// <summary>
/// Calculate required key presses for current task, accounting for upgrades
/// </summary>
private int CalculateRequiredKeyPresses(TypingTaskSO task)
{
    int baseKeyPresses = task.targetMessage.Length;

    // Apply FasterTyping upgrade: reduce by 20%
    if (GameEvents.Instance?.GetUpgradeManager()?.IsUpgradePurchased(UpgradeType.FasterTyping) ?? false)
    {
        baseKeyPresses = Mathf.RoundToInt(baseKeyPresses * 0.8f);
        Debug.Log($"FasterTyping applied: {task.targetMessage.Length} -> {baseKeyPresses} key presses");
    }

    return Mathf.Max(1, baseKeyPresses); // Minimum 1 key press
}
```

- [ ] **Step 2: Modify StartMinigame to use calculated key presses**

Find the StartMinigame method and locate where requiredKeyPresses is set. Replace that line with:

```csharp
requiredKeyPresses = CalculateRequiredKeyPresses(currentTask);
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Minigames/TypingMinigameUI.cs
git commit -m "feat: add FasterTyping upgrade support to TypingMinigameUI"
```

---

### Task 3: Implement MathMinigameUI Complete Script with NumberLock Upgrade

**Files:**
- Modify: `Assets/Scripts/Minigames/MathMinigameUI.cs`

**Context:** Math minigame needs UI references wired up and upgrade support. The NumberLock upgrade shows answer hints.

- [ ] **Step 1: Add hint display field**

Add this field at the top with the other SerializeField declarations:

```csharp
[SerializeField] private TextMeshProUGUI hintText;
```

- [ ] **Step 2: Add upgrade effect method**

Add at the end of the class:

```csharp
/// <summary>
/// NumberLock upgrade shows the correct answer as a hint
/// </summary>
protected override void OnUpgradePurchased(UpgradeType upgradeType)
{
    if (upgradeType == UpgradeType.NumberLock)
    {
        Debug.Log("NumberLock upgrade applied to Math minigame");
        ShowHintIfUpgradeActive();
    }
}

/// <summary>
/// Show hint if NumberLock upgrade is purchased
/// </summary>
private void ShowHintIfUpgradeActive()
{
    if (!isActive) return;

    if (GameEvents.Instance?.GetUpgradeManager()?.IsUpgradePurchased(UpgradeType.NumberLock) ?? false)
    {
        if (hintText != null)
        {
            hintText.text = $"Answer: {correctAnswer}";
            hintText.color = Color.yellow;
            Debug.Log($"Hint shown: {correctAnswer}");
        }
    }
}
```

- [ ] **Step 3: Call hint display in StartMinigame**

In the StartMinigame method, after clearing feedback text, add:

```csharp
// Show hint if upgrade is active
ShowHintIfUpgradeActive();
```

- [ ] **Step 4: Clear hint in CloseMinigame**

In the CloseMinigame method, add:

```csharp
// Clear hint
if (hintText != null)
{
    hintText.text = "";
}
```

- [ ] **Step 5: Commit**

```bash
git add Assets/Scripts/Minigames/MathMinigameUI.cs
git commit -m "feat: add NumberLock upgrade and hint display to MathMinigameUI"
```

---

### Task 4: Create Tile Prefab Script for PhotoReveal

**Files:**
- Create: `Assets/Scripts/Minigames/Tile.cs`

**Context:** PhotoRevealMinigame instantiates tiles at runtime. Each tile needs to handle click/hover events and track reveal state.

- [ ] **Step 1: Create Tile.cs**

```csharp
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual mosaic tile in the Photo Reveal minigame.
/// Handles hover detection and reveal state.
/// </summary>
public class Tile : MonoBehaviour
{
    private Image image;
    private bool isRevealed = false;
    private Color mosaicColor;

    public void Initialize(Color mosaic)
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
        }

        mosaicColor = mosaic;
        image.color = mosaicColor;
        isRevealed = false;
    }

    public void Reveal(Sprite revealSprite = null)
    {
        if (isRevealed) return;

        isRevealed = true;

        if (revealSprite != null)
        {
            image.sprite = revealSprite;
        }
        image.color = Color.white;
    }

    public bool IsRevealed => isRevealed;
}
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Minigames/Tile.cs
git commit -m "feat: create Tile script for PhotoReveal minigame"
```

---

### Task 5: Implement PhotoRevealMinigameUI with MemoryAssist Upgrade

**Files:**
- Modify: `Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs`

**Context:** Photo Reveal needs to support the MemoryAssist upgrade which pre-reveals 30% of tiles at game start.

- [ ] **Step 1: Add upgrade effect method**

Add at the end of the class:

```csharp
/// <summary>
/// MemoryAssist upgrade pre-reveals 30% of tiles
/// </summary>
protected override void OnUpgradePurchased(UpgradeType upgradeType)
{
    if (upgradeType == UpgradeType.MemoryAssist)
    {
        Debug.Log("MemoryAssist upgrade applied to Photo Reveal minigame");
        // Effect is applied during StartMinigame via PrerevealTiles()
    }
}

/// <summary>
/// Pre-reveal 30% of tiles if MemoryAssist upgrade is active
/// </summary>
private void PrerevealTilesIfUpgradeActive()
{
    if (tiles == null || tiles.Length == 0) return;

    if (GameEvents.Instance?.GetUpgradeManager()?.IsUpgradePurchased(UpgradeType.MemoryAssist) ?? false)
    {
        int tilesToReveal = Mathf.RoundToInt(totalTiles * 0.3f); // 30%

        List<int> indices = new List<int>();
        for (int i = 0; i < totalTiles; i++)
        {
            indices.Add(i);
        }

        // Random shuffle to pick random tiles
        for (int i = 0; i < tilesToReveal && indices.Count > 0; i++)
        {
            int randomIdx = Random.Range(0, indices.Count);
            int tileIdx = indices[randomIdx];
            indices.RemoveAt(randomIdx);

            RevealTile(tileIdx);
        }

        Debug.Log($"MemoryAssist: pre-revealed {tilesToReveal} tiles");
    }
}
```

- [ ] **Step 2: Update StartMinigame to call pre-reveal**

Find the StartMinigame method. After CreateTileGrid() call, add:

```csharp
// Apply memory assist upgrade if active
PrerevealTilesIfUpgradeActive();
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Minigames/PhotoRevealMinigameUI.cs
git commit -m "feat: add MemoryAssist upgrade support to PhotoRevealMinigameUI"
```

---

### Task 6: Create AnswerButton Script for MultipleChoice

**Files:**
- Create: `Assets/Scripts/Minigames/AnswerButton.cs`

**Context:** Multiple Choice needs dynamic button creation at runtime with proper event handling.

- [ ] **Step 1: Create AnswerButton.cs**

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Answer button in the Multiple Choice minigame.
/// Handles click detection and answer index tracking.
/// </summary>
public class AnswerButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI answerText;
    private int answerIndex = -1;
    private System.Action<int> onAnswerClickedCallback;

    public void Initialize(string answerContent, int index, System.Action<int> callback)
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        answerText = GetComponentInChildren<TextMeshProUGUI>();
        if (answerText == null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(transform);
            answerText = textObj.AddComponent<TextMeshProUGUI>();
        }

        answerIndex = index;
        answerText.text = answerContent;
        onAnswerClickedCallback = callback;

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        onAnswerClickedCallback?.Invoke(answerIndex);
    }

    public int AnswerIndex => answerIndex;
}
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Minigames/AnswerButton.cs
git commit -m "feat: create AnswerButton script for MultipleChoice minigame"
```

---

### Task 7: Implement MultipleChoiceMinigameUI with Upgrade Support

**Files:**
- Modify: `Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs`

**Context:** Multiple Choice doesn't have a specific upgrade in the current system (PreSorted is for file sorting, QuickScan is for error highlighting). However, we should prepare the structure for future upgrades and ensure the game is fully playable.

- [ ] **Step 1: Verify upgrade event subscription is inherited**

The class already inherits from BaseMinigameUI, so it will automatically subscribe to upgrade events. Add this comment at the class level:

```csharp
/// <summary>
/// Note: MultipleChoice minigame is prepared for future upgrades.
/// Currently, no upgrades directly modify difficulty, but the event system
/// is in place for future enhancement (e.g., ShowAnswersUpgrade, TimerBoostUpgrade).
/// </summary>
```

- [ ] **Step 2: Add optional upgrade effect method**

```csharp
protected override void OnUpgradePurchased(UpgradeType upgradeType)
{
    // Reserved for future upgrades specific to MultipleChoice
    // Example: PreSorted could provide answer order hints
    // Example: QuickScan could highlight commonly missed answers
    Debug.Log($"MultipleChoice minigame received upgrade event: {upgradeType}");
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Minigames/MultipleChoiceMinigameUI.cs
git commit -m "feat: prepare MultipleChoiceMinigameUI for upgrade integration"
```

---

### Task 8: Update GameEvents to Expose UpgradeManager

**Files:**
- Modify: `Assets/Scripts/Events/GameEvents.cs`

**Context:** Minigames need access to UpgradeManager to check if upgrades are purchased. We'll add a getter method to GameEvents for clean access.

- [ ] **Step 1: Read GameEvents.cs**

Understand the current singleton structure.

- [ ] **Step 2: Add UpgradeManager reference**

Add this field near the top of the class:

```csharp
private UpgradeManager upgradeManager;
```

- [ ] **Step 3: Add public getter method**

```csharp
public UpgradeManager GetUpgradeManager()
{
    if (upgradeManager == null)
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
    }
    return upgradeManager;
}
```

- [ ] **Step 4: Commit**

```bash
git add Assets/Scripts/Events/GameEvents.cs
git commit -m "feat: expose UpgradeManager via GameEvents getter"
```

---

### Task 9: Configure TypingMinigame Prefab UI Elements

**Files:**
- Modify: `Assets/Resources/Prefabs/Minigames/TypingMinigame.prefab`

**Context:** The prefab needs proper UI hierarchy with document title, content, progress text, and progress bar.

- [ ] **Step 1: Open the prefab in the scene**

In Unity, go to Assets/Resources/Prefabs/Minigames/ and double-click TypingMinigame.prefab to enter prefab edit mode.

- [ ] **Step 2: Add UI hierarchy**

Under MinigameWindow, create:
- DocumentTitle (TextMeshProUGUI) - positioned at top
- DocumentContent (TextMeshProUGUI) - positioned in center
- ProgressText (TextMeshProUGUI) - positioned at bottom
- ProgressBar (Image) - positioned at bottom

Ensure all are children of MinigameWindow with proper anchoring.

- [ ] **Step 3: Wire up references in TypingMinigameUI inspector**

Select the TypingMinigame GameObject (root). In the TypingMinigameUI script component, drag:
- DocumentTitle to documentTitleText field
- DocumentContent to documentContentText field
- ProgressText to progressText field
- ProgressBar to progressBar field

- [ ] **Step 4: Save and exit prefab edit mode**

Press Escape or click the X on the prefab breadcrumb.

- [ ] **Step 5: Commit**

```bash
git add Assets/Resources/Prefabs/Minigames/TypingMinigame.prefab
git commit -m "ui: configure TypingMinigame prefab with UI hierarchy"
```

---

### Task 10: Configure MathMinigame Prefab UI Elements

**Files:**
- Modify: `Assets/Resources/Prefabs/Minigames/MathMinigame.prefab`

**Context:** Math minigame needs problem display, input field, feedback, hint text, and background image.

- [ ] **Step 1: Open the prefab in the scene**

Double-click MathMinigame.prefab to enter prefab edit mode.

- [ ] **Step 2: Add UI hierarchy**

Under MinigameWindow, create:
- ProblemText (TextMeshProUGUI) - "5 + 3 = ?"
- InputField (TMP_InputField) - centered, placeholder "Enter answer"
- FeedbackText (TextMeshProUGUI) - for "Correct!" / "Incorrect!"
- HintText (TextMeshProUGUI) - for "Answer: 8" when upgrade active

Position logically within the 600x400 window.

- [ ] **Step 3: Wire up references in MathMinigameUI inspector**

Select MathMinigame GameObject. In MathMinigameUI script component, drag:
- ProblemText to problemText field
- InputField to inputField field
- FeedbackText to feedbackText field
- HintText to hintText field
- MinigameWindow Image to windowBackground field

- [ ] **Step 4: Save and exit prefab edit mode**

- [ ] **Step 5: Commit**

```bash
git add Assets/Resources/Prefabs/Minigames/MathMinigame.prefab
git commit -m "ui: configure MathMinigame prefab with UI hierarchy"
```

---

### Task 11: Configure PhotoRevealMinigame Prefab UI Elements

**Files:**
- Modify: `Assets/Resources/Prefabs/Minigames/PhotoRevealMinigame.prefab`

**Context:** Photo Reveal needs image container for tiles and progress text display.

- [ ] **Step 1: Open the prefab in the scene**

Double-click PhotoRevealMinigame.prefab to enter prefab edit mode.

- [ ] **Step 2: Add UI elements**

Under MinigameWindow, ensure:
- ImageContainer (Transform/RectTransform) exists for tiles
- ProgressText (TextMeshProUGUI) - positioned at bottom

- [ ] **Step 3: Wire up references in PhotoRevealMinigameUI inspector**

Select PhotoRevealMinigame GameObject. In PhotoRevealMinigameUI script component, drag:
- ImageContainer RectTransform to imageContainer field
- ProgressText to progressText field
- (Keep tilePrefab as empty - tiles are created at runtime via code)
- MinigameWindow Image to windowBackground field (if needed for flashing)

- [ ] **Step 4: Save and exit prefab edit mode**

- [ ] **Step 5: Commit**

```bash
git add Assets/Resources/Prefabs/Minigames/PhotoRevealMinigame.prefab
git commit -m "ui: configure PhotoRevealMinigame prefab with UI hierarchy"
```

---

### Task 12: Configure MultipleChoiceMinigame Prefab UI Elements

**Files:**
- Modify: `Assets/Resources/Prefabs/Minigames/MultipleChoiceMinigame.prefab`

**Context:** Multiple Choice needs question text and answer buttons laid out properly.

- [ ] **Step 1: Open the prefab in the scene**

Double-click MultipleChoiceMinigame.prefab to enter prefab edit mode.

- [ ] **Step 2: Add UI hierarchy**

Under MinigameWindow, create:
- QuestionText (TextMeshProUGUI) - positioned at top
- AnswersContainer (Transform/RectTransform) - positioned below question
  - AnswerButton1 (Button with TextMeshProUGUI child)
  - AnswerButton2 (Button with TextMeshProUGUI child)
  - AnswerButton3 (Button with TextMeshProUGUI child)
  - AnswerButton4 (Button with TextMeshProUGUI child)
- FeedbackText (TextMeshProUGUI) - positioned at bottom

Arrange buttons in a 2x2 grid or vertical stack within AnswersContainer.

- [ ] **Step 3: Wire up references in MultipleChoiceMinigameUI inspector**

Select MultipleChoiceMinigame GameObject. In MultipleChoiceMinigameUI script component, drag:
- QuestionText to questionText field
- AnswerButton1 to answerButtons[0]
- AnswerButton2 to answerButtons[1]
- AnswerButton3 to answerButtons[2]
- AnswerButton4 to answerButtons[3]
- (Get Text children automatically or wire them individually)
- FeedbackText to feedbackText field
- MinigameWindow Image to windowBackground field

- [ ] **Step 4: Save and exit prefab edit mode**

- [ ] **Step 5: Commit**

```bash
git add Assets/Resources/Prefabs/Minigames/MultipleChoiceMinigame.prefab
git commit -m "ui: configure MultipleChoiceMinigame prefab with UI hierarchy"
```

---

### Task 13: Test Typing Minigame End-to-End

**Files:**
- Test: Play the Typing minigame in the scene

**Context:** Verify that the Typing minigame starts, displays text, accepts key presses, shows progress, and completes on win condition.

- [ ] **Step 1: Load GameScene in Unity Editor**

Open Assets/Scenes/GameScene.unity.

- [ ] **Step 2: Play the scene**

Press Play in the editor.

- [ ] **Step 3: Manually start a Typing minigame**

Use the console or trigger a method to call TypingMinigameUI.Instance.StartMinigame() with a TypingTaskSO.

- [ ] **Step 4: Verify display**

Check:
- Document title is visible
- Document content displays the target message
- Progress bar updates as keys are pressed

- [ ] **Step 5: Complete minigame**

Press keys to reach the required key press count. Verify:
- Progress reaches 100%
- "Minigame completed!" message or similar appears
- Window closes after completionDelay

- [ ] **Step 6: Test FasterTyping upgrade**

In the console, purchase UpgradeType.FasterTyping. Start a new minigame and verify:
- Required key presses is 20% less (e.g., 80 instead of 100)
- Game still completes correctly

- [ ] **Step 7: Console log verification**

Check the console for messages like:
- "FasterTyping applied: 100 -> 80 key presses"
- No errors or exceptions

- [ ] **Step 8: Commit**

```bash
git commit -m "test: verify Typing minigame works end-to-end with upgrades"
```

---

### Task 14: Test Math Minigame End-to-End

**Files:**
- Test: Play the Math minigame in the scene

**Context:** Verify problem generation, input handling, answer validation, and upgrade effects.

- [ ] **Step 1: Load GameScene**

Press Play if not already playing.

- [ ] **Step 2: Start a Math minigame**

Call MathMinigameUI.Instance.StartMinigame() with a completion callback.

- [ ] **Step 3: Verify display**

Check:
- Problem text displays (e.g., "15 + 7 = ?")
- Input field is active and focused
- Hint text is empty (no upgrade yet)

- [ ] **Step 4: Submit wrong answer**

Type an incorrect number and press Enter. Verify:
- Feedback text shows "Incorrect! Try again." in red
- Window background flashes red
- Input field clears and refocuses

- [ ] **Step 5: Submit correct answer**

Type the correct answer and press Enter. Verify:
- Feedback text shows "Correct!" in green
- Minigame completes

- [ ] **Step 6: Test NumberLock upgrade**

In console, purchase UpgradeType.NumberLock. Start a new Math minigame. Verify:
- Hint text displays the correct answer in yellow
- Answer submission still works correctly

- [ ] **Step 7: Console verification**

Check for:
- "NumberLock upgrade applied to Math minigame"
- "Hint shown: [answer]"
- No errors

- [ ] **Step 8: Commit**

```bash
git commit -m "test: verify Math minigame works end-to-end with NumberLock upgrade"
```

---

### Task 15: Test Photo Reveal Minigame End-to-End

**Files:**
- Test: Play the Photo Reveal minigame in the scene

**Context:** Verify tile generation, hover reveal, progress tracking, and upgrade pre-reveal.

- [ ] **Step 1: Load GameScene**

Press Play if not already playing.

- [ ] **Step 2: Start a Photo Reveal minigame**

Call PhotoRevealMinigameUI.Instance.StartMinigame().

- [ ] **Step 3: Verify tile grid**

Check:
- 6x6 grid of tiles is created (36 tiles total)
- Each tile shows a different mosaic color
- Tiles are clickable/hoverable

- [ ] **Step 4: Hover over tiles**

Hover the mouse over several tiles. Verify:
- Tiles turn white when hovered (revealed)
- Progress text updates (e.g., "5% revealed")
- Revealed tiles don't revert to mosaic color

- [ ] **Step 5: Complete minigame**

Hover over ~28+ tiles (80% of 36) to reach the reveal threshold. Verify:
- When threshold is crossed, minigame completes
- Progress reaches "80% revealed" or higher

- [ ] **Step 6: Test MemoryAssist upgrade**

In console, purchase UpgradeType.MemoryAssist. Start a new Photo Reveal minigame. Verify:
- At start, ~11 tiles are already revealed (30% of 36)
- Remaining tiles need to be hovered to reach 80%
- Progress updates correctly include pre-revealed tiles

- [ ] **Step 7: Console verification**

Check for:
- "MemoryAssist: pre-revealed X tiles"
- Progress calculations are correct
- No errors

- [ ] **Step 8: Commit**

```bash
git commit -m "test: verify Photo Reveal minigame works end-to-end with MemoryAssist upgrade"
```

---

### Task 16: Test Multiple Choice Minigame End-to-End

**Files:**
- Test: Play the Multiple Choice minigame in the scene

**Context:** Verify question display, button functionality, answer validation, and proper win/loss handling.

- [ ] **Step 1: Load GameScene**

Press Play if not already playing.

- [ ] **Step 2: Start a Multiple Choice minigame**

Call MultipleChoiceMinigameUI.Instance.StartMinigame().

- [ ] **Step 3: Verify display**

Check:
- Question text is visible (e.g., "How many months have exactly 28 days?")
- 4 answer buttons are visible with text
- Feedback text is empty (no feedback yet)

- [ ] **Step 4: Click wrong answer**

Click an incorrect answer button. Verify:
- Feedback text shows "Incorrect! Try again." in red
- Window background flashes red
- Buttons remain interactive

- [ ] **Step 5: Click correct answer**

Click the correct answer button. Verify:
- Feedback text shows "Correct!" in green
- Minigame completes

- [ ] **Step 6: Verify question variety**

Start multiple minigames and verify:
- Different questions appear (not always the same question)
- Question difficulty varies
- All 4 answer options are sensible

- [ ] **Step 7: Console verification**

Check for:
- No "Button is NULL" errors
- Correct answer index matches button clicked
- No conflicting listeners or events

- [ ] **Step 8: Commit**

```bash
git commit -m "test: verify Multiple Choice minigame works end-to-end"
```

---

### Task 17: Integration Test - Play All 4 Minigames Sequentially

**Files:**
- Test: Play the full game flow

**Context:** Ensure minigames work in sequence, upgrades persist, and no state corruption occurs.

- [ ] **Step 1: Load GameScene and start a game day**

Play the game until the first minigame is triggered.

- [ ] **Step 2: Play Typing minigame**

Complete a typing task. Verify:
- Task completes successfully
- Ticket is marked completed
- No stuck state

- [ ] **Step 3: Play Math minigame**

Complete a math problem. Verify:
- Same success criteria as Step 2

- [ ] **Step 4: Play Photo Reveal minigame**

Complete photo reveal. Verify:
- Same success criteria

- [ ] **Step 5: Play Multiple Choice minigame**

Complete a question. Verify:
- Same success criteria

- [ ] **Step 6: Purchase an upgrade**

Go to upgrade modal and purchase FasterTyping (or any upgrade). Verify:
- Upgrade is marked as purchased
- Coins/resources are deducted

- [ ] **Step 7: Play minigames with upgrades active**

Play each type again. Verify:
- Upgrades apply correctly (fewer key presses for Typing, hint for Math, pre-reveal for Photo, etc.)
- No conflicts or state corruption
- Minigames still complete normally

- [ ] **Step 8: Check logs**

Review console for:
- No unhandled exceptions
- Expected upgrade messages appear
- No "null reference" errors

- [ ] **Step 9: Commit**

```bash
git commit -m "test: integration test - all 4 minigames work sequentially with upgrades"
```

---

### Task 18: Clean Up Debug Logs (Optional)

**Files:**
- Modify: All minigame scripts

**Context:** Replace Debug.Log statements with a proper logging level system (only log in development builds).

- [ ] **Step 1: Wrap critical logs with conditional compilation**

In each minigame, change:

```csharp
Debug.Log("FasterTyping applied: 100 -> 80 key presses");
```

To:

```csharp
#if DEVELOPMENT_BUILD || UNITY_EDITOR
Debug.Log("FasterTyping applied: 100 -> 80 key presses");
#endif
```

- [ ] **Step 2: Keep error logs always visible**

Leave error/warning logs unchanged:

```csharp
Debug.LogError("Critical error!");
Debug.LogWarning("Warning!");
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Minigames/
git commit -m "cleanup: wrap development logs with conditional compilation"
```

---

## Spec Coverage Check

✅ **All minigames are fully playable** - Tasks 9-16 configure prefabs and verify gameplay
✅ **Each minigame has a win condition** - Tasks 2-7 define explicit completion criteria
✅ **Upgrades modify minigames** - Tasks 2, 3, 5 implement upgrade effects
✅ **Clean, non-janky architecture** - Task 1 adds event-based upgrade system (no tight coupling)
✅ **Prefabs work out of the box** - Tasks 9-12 configure full UI hierarchies
✅ **GameEvents integration** - Task 8 exposes UpgradeManager for minigame access

---

## Execution Handoff

Plan complete and saved to `docs/superpowers/plans/2026-04-15-minigame-setup-complete.md`.

**Two execution options:**

**1. Subagent-Driven (recommended)** - I dispatch a fresh subagent per task (or per 2-3 tasks), review between tasks, iterate fast.

**2. Inline Execution** - Execute tasks in this session using superpowers:executing-plans with periodic review checkpoints.

**Which approach would you prefer?**
