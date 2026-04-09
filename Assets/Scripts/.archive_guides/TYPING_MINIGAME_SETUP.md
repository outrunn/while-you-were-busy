# Typing Minigame Setup Instructions

## Overview

The typing minigame system allows players to complete tasks by "typing" predetermined messages. When players spam any keys, the message appears character by character in a fake Google Docs-style window.

---

## Quick Setup Steps

### 1. Create TypingTaskDatabase GameObject

1. Create an empty GameObject in your scene
2. Name it "TypingTaskDatabase"
3. Attach the `TypingTaskDatabase.cs` script
4. In Inspector, ensure "Create Sample Tasks At Runtime" is checked (enabled by default)
   - This will create 5 sample typing tasks automatically when the game starts
   - These tasks include messages like "Draft Server Report", "Email to Stakeholders", etc.

### 2. Create Typing Minigame UI Window

Create a UI Panel to serve as the Google Docs-style window:

#### Main Window Panel
1. Right-click Canvas → UI → Panel
2. Name it "TypingMinigameWindow"
3. Set anchors to center-stretch for a large centered window
4. Suggested size: Width: 800, Height: 600
5. Background color: White or light gray (to look like a document)

#### Window Header (Google Docs style)
1. Create UI → Panel as child of TypingMinigameWindow
2. Name it "Header"
3. Anchor to top-stretch, Height: 60
4. Background: Slightly darker gray (#F5F5F5)

#### Document Title Text
1. Create UI → Text (TextMeshPro) as child of Header
2. Name it "DocumentTitleText"
3. Position in center-left of header
4. Font size: 16-18
5. Color: Dark gray or black
6. This will display the task title (e.g., "Draft Server Report")

#### Fake Toolbar (Optional - for visual authenticity)
1. Create UI → Panel as child of TypingMinigameWindow
2. Name it "Toolbar"
3. Anchor to top-stretch, below Header, Height: 40
4. Add some UI Images as children (fake buttons for Bold, Italic, etc.)
5. This is purely cosmetic!

#### Document Content Area
1. Create UI → Panel as child of TypingMinigameWindow
2. Name it "DocumentArea"
3. Anchor to fill remaining space below toolbar
4. Background: Pure white (#FFFFFF)

#### Document Content Text
1. Create UI → Text (TextMeshPro) as child of DocumentArea
2. Name it "DocumentContentText"
3. Anchor to stretch-stretch with some padding (10-20 units on all sides)
4. Alignment: Top-Left
5. Font size: 14-16
6. Color: Black
7. Enable word wrap
8. This will display the message being "typed"

#### Progress Display (Optional but recommended)
1. Create UI → Text (TextMeshPro) as child of TypingMinigameWindow
2. Name it "ProgressText"
3. Position at bottom-right corner
4. Font size: 12
5. Color: Gray
6. Shows "25 / 150" (keypresses / required)

#### Progress Bar (Optional)
1. Create UI → Slider as child of TypingMinigameWindow
2. Name it "ProgressBar"
3. Position at bottom of window
4. Remove the Handle (delete Handle Slide Area)
5. Style the Fill Area to look like a thin progress bar
6. Disable interactable (it's display only)

### 3. Setup TypingMinigameUI Component

1. Create an empty GameObject under Canvas
2. Name it "TypingMinigameUI"
3. Attach the `TypingMinigameUI.cs` script
4. In the Inspector, assign references:

**UI References:**
- Minigame Window: Drag "TypingMinigameWindow" Panel
- Document Title Text: Drag "DocumentTitleText"
- Document Content Text: Drag "DocumentContentText"
- Progress Text: Drag "ProgressText" (if created)
- Progress Bar: Drag "ProgressBar" → Fill (the Image component) (if created)

**Settings:**
- Typing Speed: 0.05 (how fast characters appear - not currently used, but good to have)
- Show Progress Bar: Check if you created the progress bar

### 4. Update Ticket Prefab

Your existing Ticket prefab needs a "Start Task" button:

1. Open your Ticket prefab in Prefab mode
2. Create UI → Button as a child of the Ticket
3. Name it "StartTaskButton"
4. Position it prominently on the ticket (e.g., bottom-center)
5. Change button text to "START TASK" or "Begin Typing"
6. Style it distinctively (e.g., green or blue background)

7. Select the Ticket root object
8. In the Ticket component Inspector, find "Start Task Button" field
9. Drag the "StartTaskButton" you just created into this field

### 5. Verify Scene Hierarchy

Your hierarchy should look like this:

```
Canvas
├── [Existing UI elements...]
├── TypingMinigameUI (Empty GameObject with script)
│   └── TypingMinigameWindow (Panel)
│       ├── Header (Panel)
│       │   └── DocumentTitleText (TextMeshPro)
│       ├── Toolbar (Panel) [Optional]
│       │   └── [Fake toolbar buttons]
│       ├── DocumentArea (Panel)
│       │   └── DocumentContentText (TextMeshPro)
│       ├── ProgressText (TextMeshPro) [Optional]
│       └── ProgressBar (Slider) [Optional]

TypingTaskDatabase (Empty GameObject with script)
GameManager (Existing)
TaskDatabase (Existing)
Printer (Existing)
```

---

## Testing the Typing Minigame

### Test Checklist

1. **Start the game** and wait for tickets to print
2. **Look for tickets with "Draft Status Email", "Security Protocol Update", etc.** - these are typing tasks
3. **Drag ticket to bulletin** (if needed based on your game flow)
4. **Click "START TASK" button** on the ticket
5. **Typing window should open** showing the fake Google Docs interface
6. **Press any keys rapidly** (spam the keyboard!)
7. **Watch the message appear** character by character
8. **Progress bar fills up** (if you added it)
9. **Window auto-closes** after ~2.5 seconds when complete
10. **Ticket gets stamped**, player receives reward, world health degrades

### Common Issues

**Button doesn't appear on tickets:**
- Check that TypingTaskDatabase is in the scene and initialized
- Verify "Create Sample Tasks At Runtime" is checked
- Make sure TaskDatabase's Awake runs AFTER TypingTaskDatabase's Awake
  - You can set Script Execution Order in Unity: Edit → Project Settings → Script Execution Order
  - Set TypingTaskDatabase to run before TaskDatabase (e.g., -100)

**Typing window doesn't open:**
- Ensure TypingMinigameUI.Instance is not null (check console for errors)
- Verify all UI references are assigned in TypingMinigameUI Inspector
- Check that Minigame Window is assigned

**Keys don't register:**
- Make sure EventSystem exists in scene (should auto-create with UI)
- Check that TypingMinigameUI component is enabled
- Verify no UI elements are blocking input

**Message doesn't display:**
- Check DocumentContentText reference is assigned
- Verify the TypingTaskSO has a message (check in TypingTaskDatabase)
- Look for errors in console

---

## Customization Options

### Creating Custom Typing Tasks

#### Option 1: Runtime Creation (Easy)
Edit `TypingTaskDatabase.cs` → `CreateSampleTasks()` method:

```csharp
allTypingTasks.Add(CreateTypingTask(
    "Your Task Title",
    "Task description",
    "The exact message that will be typed out when player spams keys.",
    2.5f // delay before auto-close
));
```

#### Option 2: ScriptableObject Assets (Advanced)
1. Right-click in Project window
2. Create → Minigames → Typing Task
3. Name it "MyTypingTask"
4. Fill in Inspector:
   - Task Title: "My Custom Task"
   - Task Description: "Description here"
   - Message To Type: "Your full message here..."
   - Completion Delay: 2.5
5. Add to TypingTaskDatabase's "All Typing Tasks" list in Inspector

### Adjusting Difficulty

In TaskDatabase.cs, you can adjust which difficulty tiers get typing tasks:

```csharp
// Add more typing tasks to easy pool
AddTypingTask(easyTasks, "Simple Email", "Quick status update", 12f, 1);
AddTypingTask(easyTasks, "Brief Report", "Short system report", 15f, 1);

// Or add them to hard pool for bigger rewards
AddTypingTask(hardTasks, "Complex Documentation", "Write technical docs", 300f, 3);
```

### Styling the Google Docs Window

**Make it look more like Google Docs:**
1. Header background: #F1F3F4 (light gray)
2. Add small UI Images in header for fake menu items ("File", "Edit", "View")
3. Add fake toolbar icons (bold, italic, underline buttons)
4. Use a monospace or document-style font for content text
5. Add a subtle shadow to the window panel

**Make it look more "hacker/terminal" themed:**
1. Background: Dark gray or black (#1E1E1E)
2. Text: Green (#00FF00) or cyan (#00FFFF)
3. Use a monospace font (Courier New, Consolas)
4. Add scanline effect or glitch shader

---

## Advanced Features

### Auto-Remove Completed Tickets

In `Ticket.cs` → `OnMinigameCompleted()`, uncomment:
```csharp
Destroy(gameObject, 1f); // Remove ticket 1 second after completion
```

Don't forget to call:
```csharp
if (Printer.Instance != null)
{
    Printer.Instance.OnTicketProcessed();
}
```

### Variable Typing Speed

Currently, each keypress reveals one character. You could modify `TypingMinigameUI.cs` to:
- Reveal multiple characters per keypress
- Require faster typing for harder tasks
- Add a time limit

### Sound Effects

Add typing sounds:
1. Add AudioSource component to TypingMinigameUI
2. Import typing sound effect
3. In `OnKeyPressed()`:
```csharp
if (typingSound != null)
{
    typingSound.Play();
}
```

---

## Integration with Existing Systems

The typing minigame integrates seamlessly:

- **TaskDatabase**: Automatically includes typing tasks in random pool
- **Printer**: Generates typing task tickets automatically
- **Ticket System**: Tickets show "START TASK" button for minigame tasks
- **GameManager**: Rewards and world health degradation work normally
- **SystemLog**: Logs task completion messages

No additional integration needed!

---

## Script Execution Order

For optimal performance:

1. Edit → Project Settings → Script Execution Order
2. Set order:
   - TypingTaskDatabase: -100 (runs first)
   - TaskDatabase: 0 (default)
   - GameManager: 0 (default)

This ensures typing tasks are created before TaskDatabase tries to use them.

---

## Troubleshooting Reference

| Issue | Solution |
|-------|----------|
| No typing tasks appear | Check TypingTaskDatabase in scene with sample tasks enabled |
| Button not visible on tickets | Verify Start Task Button reference assigned in Ticket prefab |
| Window doesn't open | Check TypingMinigameUI singleton and UI references |
| Keys don't work | Ensure EventSystem exists and window is active |
| Progress bar doesn't fill | Check Progress Bar reference points to Fill Image, not Slider |
| No reward given | Verify GameManager.Instance exists and AddOutputs is called |

---

## Next Steps

Once typing minigame is working:

1. **Create more typing tasks** with varied corporate messages
2. **Add other minigame types** (using the same pattern)
3. **Polish the UI** with better Google Docs styling
4. **Add sound effects** for typing and completion
5. **Create minigame-only task categories** in TaskDatabase
6. **Add achievements** for typing task completion

Happy typing!
