# Typing Prompts System - Complete Guide

## Overview

The typing prompt system is **modular and easy to expand**. You create typing tasks as ScriptableObject assets, and the system automatically picks them randomly for tickets.

---

## Quick Start (3 Steps)

### Step 1: Open the Typing Task Creator Tool

1. In Unity, go to **Tools → Typing Task Creator** (top menu)
2. A window will open with options to create typing tasks

### Step 2: Create Sample Tasks (One-Click)

Click these buttons to instantly create 15 pre-made typing tasks:
- **Create Sample Easy Tasks (5)** - Short messages, 10-15 points
- **Create Sample Medium Tasks (5)** - Medium messages, 55-70 points
- **Create Sample Hard Tasks (5)** - Long messages, 250-300 points

This creates 15 ScriptableObject assets in `Assets/ScriptableObjects/TypingTasks/`

### Step 3: Add Tasks to TypingTaskDatabase

1. Find **TypingTaskDatabase** GameObject in your scene
2. Select it
3. In the Inspector, find **All Typing Tasks** list
4. Set size to 15 (or however many you created)
5. Drag all your typing task assets into the slots
6. **UNCHECK** "Create Sample Tasks At Runtime" (we're using assets now!)

**Done!** The system will now randomly pick from your typing tasks.

---

## System Architecture

```
┌─────────────────────────────────────────────────┐
│  TypingTaskSO (ScriptableObject Asset)         │
│  - Contains: Title, Description, Message       │
│  - Created using Tools → Typing Task Creator   │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│  TypingTaskDatabase (Scene GameObject)         │
│  - Stores list of all typing task assets       │
│  - GetRandomTypingTask() picks one randomly    │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│  TaskDatabase (Scene GameObject)                │
│  - Calls TypingTaskDatabase for random task    │
│  - Creates TaskData with typing minigame       │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│  Printer (Scene GameObject)                     │
│  - Gets random TaskData from TaskDatabase      │
│  - Creates Ticket with typing task attached    │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│  Ticket (UI GameObject)                         │
│  - Shows "START TASK" button                   │
│  - Opens TypingMinigameUI when clicked         │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│  TypingMinigameUI (UI GameObject)              │
│  - Displays message from TypingTaskSO          │
│  - Player spams keys to reveal message         │
│  - Calls completion callback when done         │
└─────────────────────────────────────────────────┘
```

---

## Creating Custom Typing Tasks

### Method 1: Using the Creator Tool (Recommended)

1. **Tools → Typing Task Creator**
2. Fill in the fields:
   - **Task Title:** "Your Task Name"
   - **Description:** "Brief description"
   - **Message Content:** (your full message here)
   - **Min Key Presses:** 0 (auto-calculates)
   - **Completion Delay:** 2.5 seconds
3. Click **Create Typing Task**
4. The asset is created and auto-selected

### Method 2: Manual Creation

1. Right-click in Project → **Create → Minigames → Typing Task**
2. Name the asset (e.g., `TypingTask_Email1`)
3. Select it and fill in the Inspector:
   - Task Title
   - Task Description
   - Message To Type
   - Minimum Key Presses
   - Completion Delay

---

## File Organization

**Recommended Folder Structure:**

```
Assets/
├── ScriptableObjects/
│   └── TypingTasks/
│       ├── Easy/
│       │   ├── Quick_Email.asset
│       │   ├── Meeting_Confirmation.asset
│       │   └── ...
│       ├── Medium/
│       │   ├── Security_Update.asset
│       │   ├── Meeting_Minutes.asset
│       │   └── ...
│       └── Hard/
│           ├── Incident_Report.asset
│           ├── System_Architecture.asset
│           └── ...
└── Scripts/
    ├── Editor/
    │   └── TypingTaskCreator.cs (The creator tool)
    ├── TypingTaskSO.cs (ScriptableObject definition)
    ├── TypingTaskDatabase.cs (Manager)
    └── ...
```

**Why organize by difficulty?**
- Easy to find tasks
- Easy to balance difficulty
- Easy to expand each category

---

## Adding Your Typing Tasks to the Game

### Option 1: Replace Runtime Tasks (Clean Slate)

1. Create your typing task assets
2. Select **TypingTaskDatabase** in scene
3. **UNCHECK** "Create Sample Tasks At Runtime"
4. Add your assets to **All Typing Tasks** list
5. Play!

### Option 2: Mix Runtime + Assets (Hybrid)

1. Create your typing task assets
2. Select **TypingTaskDatabase** in scene
3. **KEEP CHECKED** "Create Sample Tasks At Runtime"
4. Add your assets to **All Typing Tasks** list
5. The system will use both runtime samples AND your assets

### Option 3: Only Runtime Tasks (Testing)

1. Don't create any assets
2. Select **TypingTaskDatabase** in scene
3. **CHECK** "Create Sample Tasks At Runtime"
4. 5 sample tasks are auto-created when game starts

---

## Balancing Difficulty

### Easy Tasks (10-15 points)
- **Message Length:** 50-150 characters
- **Complexity:** Simple, one sentence
- **Examples:**
  - "Meeting confirmed for 2 PM."
  - "Backup completed successfully."
  - "All systems operational."

### Medium Tasks (55-70 points)
- **Message Length:** 150-400 characters
- **Complexity:** Multiple sentences, some structure
- **Examples:**
  - Email with greeting, body, closing
  - Meeting minutes with bullet points
  - Short technical documentation

### Hard Tasks (250-300 points)
- **Message Length:** 400+ characters
- **Complexity:** Multi-paragraph, structured documents
- **Examples:**
  - Incident reports with sections
  - Executive summaries
  - Compliance responses
  - Crisis communications

---

## Message Writing Tips

### 1. Corporate/Tech Theme
Match the game's aesthetic:
- Use formal business language
- Include technical jargon appropriately
- Reference "systems", "metrics", "protocols"
- Maintain ambiguous corporate dystopia vibe

### 2. Structure Matters
Good structure makes typing feel satisfying:
```
SUBJECT LINE (ALL CAPS)

Brief introduction paragraph.

SECTION HEADER
- Bullet points for clarity
- Multiple items
- Easy to scan

Closing statement or call to action.

Signature
```

### 3. Varied Length
Create variety within difficulty tiers:
- Easy: 50, 80, 120, 100, 90 characters
- Medium: 200, 350, 250, 300, 280 characters
- Hard: 500, 700, 600, 800, 650 characters

### 4. Emotional Tone
Escalate tension through message content:
- Easy: Calm, routine, optimistic
- Medium: Concerned, professional, cautious
- Hard: Urgent, critical, alarming

---

## Pre-Made Message Templates

### Template: Status Email
```
Subject: [PROJECT] Status Update

[GREETING],

I am writing to inform you that [STATUS]. All metrics are [CONDITION]. [ACTION REQUIRED/NOT REQUIRED].

Please [INSTRUCTION] if you have any questions.

Best regards,
[SENDER]
```

### Template: Incident Report
```
INCIDENT REPORT #[NUMBER]

SUMMARY: [What happened and when]

IMPACT: [What was affected and how severely]

ROOT CAUSE: [Why it happened]

RESOLUTION: [How it was fixed]

RECOMMENDATIONS: [How to prevent it]

STATUS: [Current state]
```

### Template: Meeting Minutes
```
MEETING MINUTES - [TOPIC]

Attendees: [Who attended]
Date: [When]

Key Points:
- [Point 1]
- [Point 2]
- [Point 3]

Action Items:
1. [Action 1]
2. [Action 2]
3. [Action 3]

[CLOSING STATEMENT]
```

### Template: System Documentation
```
[DOCUMENT TITLE] v[VERSION]

OVERVIEW: [High-level description]

KEY COMPONENTS:
1. [Component 1] - [Description]
2. [Component 2] - [Description]
3. [Component 3] - [Description]

PERFORMANCE CHARACTERISTICS: [Metrics and benchmarks]

RECOMMENDATIONS: [Suggested actions or considerations]
```

---

## Common Issues & Solutions

### Issue: Tasks Not Appearing
**Solution:**
- Check TypingTaskDatabase has tasks in "All Typing Tasks" list
- Verify "Create Sample Tasks At Runtime" matches your setup
- Check Console for errors

### Issue: Same Task Keeps Appearing
**Solution:**
- Add more tasks to the pool (need at least 5-10 for variety)
- System picks randomly, but with small pools you'll see repeats

### Issue: Wrong Difficulty Tasks Appearing
**Solution:**
- TaskDatabase assigns tasks randomly from TypingTaskDatabase
- It doesn't filter by difficulty - all tasks are mixed
- To separate by difficulty, you'd need to create separate databases

### Issue: Can't Find "Create → Minigames → Typing Task"
**Solution:**
- Check that TypingTaskSO.cs has `[CreateAssetMenu]` attribute (it does)
- Try refreshing: Assets → Refresh
- Restart Unity if needed

### Issue: Typing Task Creator Tool Missing
**Solution:**
- Check TypingTaskCreator.cs is in `Assets/Scripts/Editor/` folder
- Editor scripts MUST be in an "Editor" folder to work
- Restart Unity after adding

---

## Expanding the System

### Add New Minigame Types

Want to add minigames beyond typing? Here's how:

1. **Create new ScriptableObject type** (copy TypingTaskSO.cs)
   ```csharp
   [CreateAssetMenu(fileName = "New Puzzle Task", menuName = "Minigames/Puzzle Task")]
   public class PuzzleTaskSO : ScriptableObject
   {
       public string taskTitle;
       public int difficultyLevel;
       // Add puzzle-specific data
   }
   ```

2. **Create new minigame UI** (copy TypingMinigameUI.cs)

3. **Update TaskData** to support new type
   ```csharp
   public PuzzleTaskSO puzzleTask = null;
   ```

4. **Update Ticket** to check task type and open correct minigame

### Add Dynamic Content

Want messages that change based on game state?

1. **Add placeholder system:**
   ```csharp
   string message = task.messageToType;
   message = message.Replace("[DAY]", GameManager.Instance.GetCurrentDay().ToString());
   message = message.Replace("[QUOTA]", GameManager.Instance.GetDailyQuota().ToString("F0"));
   ```

2. **Use in messages:**
   ```
   Subject: Day [DAY] Quota Update

   Current quota: [QUOTA] points
   ```

### Create Category-Based Selection

Want to group tasks by theme?

1. **Add enum to TypingTaskSO:**
   ```csharp
   public enum TaskCategory { Email, Report, Documentation, Communication }
   public TaskCategory category;
   ```

2. **Filter in TypingTaskDatabase:**
   ```csharp
   public TypingTaskSO GetRandomTypingTask(TaskCategory category)
   {
       List<TypingTaskSO> filtered = allTypingTasks.Where(t => t.category == category).ToList();
       return filtered[Random.Range(0, filtered.Count)];
   }
   ```

---

## Quick Reference

### Key Files
- **TypingTaskSO.cs** - ScriptableObject definition
- **TypingTaskDatabase.cs** - Manager that stores all tasks
- **TypingTaskCreator.cs** - Editor tool for easy creation

### Key Workflow
1. Create typing task assets (Tools → Typing Task Creator)
2. Add to TypingTaskDatabase.All Typing Tasks list
3. Disable runtime creation if using only assets
4. Play and test!

### Key Customization Points
- **Message length** - Adjust for difficulty
- **Completion delay** - How long window stays open after done
- **Min key presses** - Force minimum interaction (or 0 for auto)

---

## Next Steps

1. **Create 15 sample tasks** using the Creator Tool buttons
2. **Add them to TypingTaskDatabase**
3. **Test in Play Mode**
4. **Create your own custom tasks** for your game's theme
5. **Organize into Easy/Medium/Hard folders**
6. **Expand with more tasks over time!**

The system is designed to be **super easy to add new content** - just create a ScriptableObject asset and add it to the list. No code changes needed!

---

**Happy typing!** 🎮⌨️
