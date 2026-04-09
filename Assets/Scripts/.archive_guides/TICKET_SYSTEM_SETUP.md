# Ticket System Setup Guide

## Overview

The Ticket System adds a task-based gameplay loop where:
1. **Printer** outputs tickets with tasks
2. **Bulletin Board** lets you organize active tickets
3. **Stamp** marks tickets as completed
4. **Processing Machine** submits completed tickets for rewards

---

## Component Setup

### 1. Task Database Setup

1. Create an empty GameObject in your scene, name it "TaskDatabase"
2. Attach `TaskDatabase.cs` script to it
3. The script automatically populates with default tasks
4. Optional: Add custom tasks in the Inspector under Easy/Medium/Hard task lists

---

### 2. Printer Setup

#### Create Printer GameObject
1. Create empty GameObject under Canvas, name it "Printer"
2. Attach `Printer.cs` script to it

#### Create Printer UI
1. Right-click Printer → UI → Image (name it "PrinterBody")
   - Set size to 200x150
   - Give it a printer-like appearance (gray/white color)

2. Create UI → Button as child of Printer (name it "PrintButton")
   - Position at bottom of printer
   - Text: "PRINT TICKET"

3. Create UI → Text as child of Printer (name it "StatusText")
   - Position at top
   - Font size: 14

4. Create empty GameObject as child of Printer (name it "TicketSpawnPoint")
   - Position where tickets should appear (below the printer)

#### Ticket Prefab Creation
1. Right-click in Hierarchy → UI → Panel
2. Name it "TicketPrefab"
3. Set size to 180x120
4. Add these UI Text children:
   - "TitleText" - Bold, size 16
   - "DescriptionText" - Size 12, word wrap enabled
5. Add UI → Image as child, name it "StampImage"
   - Position in top-right corner
   - Set sprite to a stamp/checkmark graphic
   - Initially disabled
6. Add `Ticket.cs` script to the Panel
7. Add `CanvasGroup` component (required for dragging)
8. Drag prefab to Project folder to save
9. Delete from Hierarchy

#### Configure Printer Inspector
- **Auto Print Interval**: 30 (prints every 30 seconds)
- **Auto Print Enabled**: True
- **Max Active Tickets**: 5
- **Ticket Prefab**: Drag your TicketPrefab here
- **Ticket Spawn Point**: Drag TicketSpawnPoint GameObject
- **Print Button**: Drag PrintButton
- **Status Text**: Drag StatusText
- **Printer Animation**: Drag PrinterBody Image (optional)

---

### 3. Bulletin Board Setup

1. Create UI → Panel under Canvas, name it "BulletinBoard"
2. Size: 600x400 (or larger based on your needs)
3. Position on left/right side of screen
4. Attach `BulletinBoard.cs` script

5. Create empty GameObject as child, name it "TicketContainer"
   - This holds pinned tickets
   - Add `Vertical Layout Group` or `Grid Layout Group` component (optional)

#### Configure BulletinBoard Inspector
- **Max Pinned Tickets**: 10
- **Ticket Container**: Drag TicketContainer GameObject
- **Ticket Spacing**: 10
- **Tickets Per Row**: 3
- **Ticket Size**: 150, 100

---

### 4. Stamp Setup

1. Create UI → Image under Canvas, name it "Stamp"
2. Size: 100x100
3. Set sprite to a stamp graphic
4. Position prominently (center-bottom or side)
5. Attach `Stamp.cs` script

6. Add UI → Button component to Stamp GameObject
7. Create UI → Text as child (name it "StampStatus")
   - Shows "Ready" or "ACTIVE"

#### Configure Stamp Inspector
- **Click To Stamp Mode**: True (recommended)
- **Stamp Button**: Auto-detected or drag Button component
- **Stamp Image**: Drag the Image component
- **Status Text**: Drag StampStatus Text
- **Normal Color**: White
- **Active Color**: Yellow
- **Stamped Color**: Green

---

### 5. Processing Machine Setup

1. Create UI → Panel under Canvas, name it "ProcessingMachine"
2. Size: 250x200
3. Position opposite to Printer
4. Give it a distinct appearance (different color)
5. Attach `ProcessingMachine.cs` script

6. Create these UI children:
   - UI → Text (name "MachineStatus") - Shows processing state
   - UI → Slider (name "ProgressBar") - Shows processing progress
   - UI → Image (name "MachineBody") - Visual representation

#### Configure ProcessingMachine Inspector
- **Processing Time**: 2 (seconds to process each ticket)
- **Require Stamped Tickets**: True
- **Status Text**: Drag MachineStatus
- **Machine Image**: Drag MachineBody
- **Progress Bar**: Drag ProgressBar
- **Idle Color**: Gray
- **Processing Color**: Green
- **Error Color**: Red
- **Printer**: Drag Printer GameObject (for ticket count tracking)

---

## Hierarchy Structure Example

```
Canvas
├── GameManager (existing)
├── Background (existing)
├── OutputsText (existing)
├── OutputsPerSecondText (existing)
│
├── Printer
│   ├── PrinterBody (Image)
│   ├── PrintButton (Button)
│   ├── StatusText (Text)
│   └── TicketSpawnPoint (Empty)
│
├── BulletinBoard (Panel)
│   └── TicketContainer (Empty)
│
├── Stamp (Image + Button)
│   └── StampStatus (Text)
│
└── ProcessingMachine (Panel)
    ├── MachineBody (Image)
    ├── MachineStatus (Text)
    └── ProgressBar (Slider)

TaskDatabase (Empty GameObject, outside Canvas)
```

---

## Workflow Instructions

### How the System Works:

1. **Printing**:
   - Printer automatically prints tickets every 30 seconds
   - Or click "PRINT TICKET" button manually
   - Tickets appear at spawn point with random tasks

2. **Organizing**:
   - Drag tickets to Bulletin Board to pin them
   - Organize multiple active tasks
   - Tickets automatically arrange in grid

3. **Completing**:
   - Click the Stamp to activate it (turns yellow)
   - Click a ticket to stamp it (checkmark appears)
   - Stamp deactivates after use

4. **Processing**:
   - Drag stamped tickets to Processing Machine
   - Machine processes for 2 seconds (progress bar fills)
   - Receive output rewards based on task difficulty
   - Ticket is destroyed after processing

---

## Testing Checklist

- [ ] Printer automatically prints tickets every 30 seconds
- [ ] Manual print button works
- [ ] Can drag tickets to bulletin board
- [ ] Tickets organize in grid on bulletin board
- [ ] Stamp activates when clicked
- [ ] Clicking ticket while stamp active stamps it
- [ ] Stamp visual appears on stamped tickets
- [ ] Can drag stamped tickets to processing machine
- [ ] Unstamped tickets are rejected by machine
- [ ] Processing machine shows progress bar
- [ ] Outputs increase after ticket processing
- [ ] Ticket is destroyed after processing
- [ ] Printer ticket count decreases after processing
- [ ] System log shows relevant messages

---

## Customization Tips

### Task Difficulty Scaling
In `TaskDatabase.cs`, edit `GetDifficultyForCurrentProgress()`:
```csharp
if (outputs < 100f) return 1;   // Easy tasks
if (outputs < 500f) return 2;   // Medium tasks
return 3;                       // Hard tasks
```

### Add Custom Tasks
Select TaskDatabase in Inspector and add to:
- Easy Tasks: Low reward, simple descriptions
- Medium Tasks: Medium reward, moderate complexity
- Hard Tasks: High reward, complex descriptions

### Adjust Print Rate
On Printer Inspector:
- **Auto Print Interval**: Change time between automatic prints
- **Max Active Tickets**: Limit simultaneous tickets

### Processing Speed
On ProcessingMachine Inspector:
- **Processing Time**: How long to process each ticket

### Reward Multipliers
In `ProcessingMachine.cs`, edit `GrantReward()`:
```csharp
float difficultyMultiplier = 1f + (ticket.GetDifficulty() - 1) * 0.5f;
```
Increase `0.5f` for bigger difficulty bonuses

---

## Integration with Existing Game

The ticket system integrates with your existing game:
- Uses `GameManager.Instance.AddOutputs()` to grant rewards
- Uses `SystemLog.Instance.LogMessage()` for player feedback
- Respects world health degradation mechanics
- Works alongside click and auto-processor systems

---

## Troubleshooting

**Tickets won't drag:**
- Ensure Ticket has CanvasGroup component
- Check Canvas has GraphicRaycaster component
- Verify EventSystem exists in scene

**Bulletin Board won't accept tickets:**
- Check BulletinBoard has required interface (IDropHandler)
- Ensure Ticket Container is assigned

**Stamp doesn't work:**
- Verify Click To Stamp Mode is enabled
- Check that EventSystem exists
- Ensure tickets have Ticket.cs script

**Processing Machine rejects tickets:**
- Confirm Require Stamped Tickets setting
- Make sure ticket is actually stamped (checkmark visible)
- Check that tickets are being dragged properly

**No tasks appearing:**
- Verify TaskDatabase GameObject exists in scene
- Check TaskDatabase.Instance is not null
- Ensure task lists populated in Inspector

---

## Advanced Features (Optional)

### Different Ticket Types
Create multiple ticket prefabs with different:
- Colors for difficulty levels
- Visual styles for task categories
- Special effects for rare tasks

### Achievement System
Track:
- Total tickets processed
- Tickets processed per session
- Highest difficulty completed

### Ticket Queue System
Modify BulletinBoard to:
- Auto-organize by priority
- Highlight urgent tasks
- Auto-submit to processing machine

---

Good luck with your ticket system implementation!
