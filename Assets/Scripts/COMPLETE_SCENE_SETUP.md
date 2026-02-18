# Complete Scene Setup Guide - Step by Step

## 🎯 Goal
Get your game fully working with tickets printing, minigames playing, and quota system functioning.

---

## ⚠️ Prerequisites

Make sure you have:
- Unity project open
- All scripts compiled with no errors
- TextMeshPro package imported (Window → TextMeshPro → Import TMP Essential Resources)

---

## 📋 Setup Checklist

### Phase 1: Core GameObjects (10 min)
- [ ] TaskDatabase
- [ ] TypingTaskDatabase
- [ ] SystemLog
- [ ] GameManager (you have this)

### Phase 2: UI GameObjects (15 min)
- [ ] Canvas with all text elements
- [ ] Click Button
- [ ] Sleep Button
- [ ] Printer
- [ ] BulletinBoard
- [ ] ProcessingMachine
- [ ] TypingMinigameUI

### Phase 3: Prefabs (10 min)
- [ ] Ticket Prefab

### Phase 4: Connections (10 min)
- [ ] Wire all references
- [ ] Test everything

**Total Time: ~45 minutes**

---

## 🛠️ PHASE 1: Core GameObjects

### 1.1 Create TaskDatabase

1. **Create GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Name it: `TaskDatabase`
   - Position: (0, 0, 0)

2. **Add Script:**
   - Select TaskDatabase
   - Click Add Component
   - Search "TaskDatabase"
   - Add TaskDatabase.cs

3. **Verify:**
   - Check Inspector shows TaskDatabase component
   - Should have Easy/Medium/Hard task lists (auto-populated)

---

### 1.2 Create TypingTaskDatabase

1. **Create GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Name it: `TypingTaskDatabase`

2. **Add Script:**
   - Add Component → TypingTaskDatabase.cs

3. **Configure:**
   - Check "Create Sample Tasks At Runtime" is CHECKED (for now)
   - All Typing Tasks list size = 0 (empty for now)

---

### 1.3 Create/Verify SystemLog

1. **If you don't have it, create:**
   - Right-click Hierarchy → Create Empty
   - Name it: `SystemLog`
   - Add Component → SystemLog.cs

2. **We'll connect UI later**

---

### 1.4 Verify GameManager

You already have this, but verify:
- GameManager GameObject exists
- Has GameManager.cs script
- Shows all the new fields (Day/Quota System, World Time, UI References)

If fields don't show, the script didn't compile. Check Console for errors.

---

## 🖼️ PHASE 2: UI Setup

### 2.1 Create Main UI Text Elements

**Under your Canvas, create these TextMeshPro text objects:**

1. **QuotaText**
   - Right-click Canvas → UI → Text - TextMeshPro
   - Name: `QuotaText`
   - Position: Top-left (X: -600, Y: 300)
   - Text: "Quota: 0/100"
   - Font Size: 24
   - Color: White

2. **DayText**
   - Create another TMP text
   - Name: `DayText`
   - Position: Top-left (X: -600, Y: 250)
   - Text: "Day 1"
   - Font Size: 24
   - Color: White

3. **TimeText**
   - Create another TMP text
   - Name: `TimeText`
   - Position: Top-left (X: -600, Y: 200)
   - Text: "Time: 6:00 AM"
   - Font Size: 20
   - Color: White

4. **SystemLogText** (if not created)
   - Create TMP text
   - Name: `SystemLogText`
   - Position: Bottom-center (X: 0, Y: -400)
   - Text: "System Status: Optimal"
   - Font Size: 16
   - Color: Yellow
   - Alignment: Center

---

### 2.2 Create Sleep Button

1. **Create Button:**
   - Right-click Canvas → UI → Button - TextMeshPro
   - Name: `SleepButton`
   - Position: Top-right (X: 600, Y: 300)
   - Size: Width: 150, Height: 50

2. **Configure:**
   - Select child Text element
   - Change text to: "SLEEP"
   - Font Size: 18
   - Bold

3. **Style (Optional):**
   - Select SleepButton
   - Image component → Color: Dark Blue or Gray
   - This button will be disabled until quota is met

---

### 2.3 Create Printer

1. **Create Empty GameObject:**
   - Right-click Canvas → Create Empty
   - Name: `Printer`
   - Position: Left side (X: -400, Y: 0)

2. **Add Script:**
   - Select Printer
   - Add Component → Printer.cs

3. **Create Printer UI Children:**

   **a) PrinterBody (Image):**
   - Right-click Printer → UI → Image
   - Name: `PrinterBody`
   - Size: 200x150
   - Color: Gray (#CCCCCC)
   - This is the visual of the printer

   **b) PrintButton:**
   - Right-click Printer → UI → Button - TextMeshPro
   - Name: `PrintButton`
   - Position: Bottom of printer (Y: -50)
   - Size: 150x40
   - Text: "PRINT TICKET"

   **c) StatusText:**
   - Right-click Printer → UI → Text - TextMeshPro
   - Name: `StatusText`
   - Position: Top of printer (Y: 50)
   - Font Size: 12
   - Text: "Next Ticket: 30s"
   - Alignment: Center

   **d) TicketSpawnPoint:**
   - Right-click Printer → Create Empty
   - Name: `TicketSpawnPoint`
   - Position: Below printer (Y: -100)
   - This is where tickets will appear

4. **Wire Printer References:**
   - Select Printer GameObject
   - In Inspector, find Printer component fields:
     - Ticket Prefab: (Leave empty for now, we'll create it)
     - Ticket Spawn Point: Drag `TicketSpawnPoint`
     - Print Button: Drag `PrintButton`
     - Status Text: Drag `StatusText`
     - Printer Animation: Drag `PrinterBody` (optional)

---

### 2.4 Create BulletinBoard

1. **Create Panel:**
   - Right-click Canvas → UI → Panel
   - Name: `BulletinBoard`
   - Position: Center-left (X: -200, Y: 0)
   - Size: Width: 600, Height: 500
   - Color: Brown or Cork-like (#8B4513 with low alpha)

2. **Add Script:**
   - Add Component → BulletinBoard.cs

3. **Create TicketContainer:**
   - Right-click BulletinBoard → Create Empty
   - Name: `TicketContainer`
   - Anchor: Stretch-Stretch (fill parent)
   - This will hold pinned tickets

4. **Wire References:**
   - Select BulletinBoard
   - Ticket Container: Drag `TicketContainer`
   - Configure settings:
     - Max Pinned Tickets: 10
     - Ticket Spacing: 10
     - Tickets Per Row: 3
     - Ticket Size: X: 180, Y: 120

---

### 2.5 Create ProcessingMachine

1. **Create Panel:**
   - Right-click Canvas → UI → Panel
   - Name: `ProcessingMachine`
   - Position: Right side (X: 400, Y: 0)
   - Size: Width: 250, Height: 200
   - Color: Dark Gray (#333333)

2. **Add Script:**
   - Add Component → ProcessingMachine.cs

3. **Create Children:**

   **a) MachineBody:**
   - Right-click ProcessingMachine → UI → Image
   - Name: `MachineBody`
   - Size: Fill parent
   - Color: Medium Gray

   **b) MachineStatus:**
   - Right-click ProcessingMachine → UI → Text - TextMeshPro
   - Name: `MachineStatus`
   - Position: Top (Y: 50)
   - Text: "Ready"
   - Font Size: 16
   - Alignment: Center

   **c) ProgressBar:**
   - Right-click ProcessingMachine → UI → Slider
   - Name: `ProgressBar`
   - Position: Bottom (Y: -60)
   - Size: Width: 200
   - Delete Handle Slide Area (we don't need dragging)
   - Set Fill Area → Fill → Color to Green
   - Set Interactable to OFF (it's display only)

4. **Wire References:**
   - Select ProcessingMachine
   - Status Text: Drag `MachineStatus`
   - Machine Image: Drag `MachineBody`
   - Progress Bar: Drag `ProgressBar`
   - Printer: Drag `Printer` GameObject (from Hierarchy)
   - Require Stamped Tickets: Check ON
   - Processing Time: 2 seconds

---

### 2.6 Create TypingMinigameUI

1. **Create Empty GameObject:**
   - Right-click Canvas → Create Empty
   - Name: `TypingMinigameUI`

2. **Add Script:**
   - Add Component → TypingMinigameUI.cs

3. **Create Minigame Window:**

   **a) Main Window Panel:**
   - Right-click TypingMinigameUI → UI → Panel
   - Name: `TypingMinigameWindow`
   - Anchor: Center
   - Size: Width: 800, Height: 600
   - Color: White (#FFFFFF)
   - **IMPORTANT: Set Active to OFF** (hidden by default)

   **b) Header (inside window):**
   - Right-click TypingMinigameWindow → UI → Panel
   - Name: `Header`
   - Anchor: Top-Stretch
   - Height: 60
   - Color: Light Gray (#F5F5F5)

   **c) DocumentTitleText (inside Header):**
   - Right-click Header → UI → Text - TextMeshPro
   - Name: `DocumentTitleText`
   - Position: Center-left
   - Text: "Document Title"
   - Font Size: 18
   - Color: Black

   **d) DocumentArea (inside window):**
   - Right-click TypingMinigameWindow → UI → Panel
   - Name: `DocumentArea`
   - Anchor: Stretch-Stretch (below header)
   - Offset Top: -60 (to account for header)
   - Color: Pure White (#FFFFFF)

   **e) DocumentContentText (inside DocumentArea):**
   - Right-click DocumentArea → UI → Text - TextMeshPro
   - Name: `DocumentContentText`
   - Anchor: Stretch-Stretch
   - Padding: 20px all sides
   - Alignment: Top-Left
   - Font Size: 16
   - Color: Black
   - Enable Word Wrap

   **f) ProgressText (inside window):**
   - Right-click TypingMinigameWindow → UI → Text - TextMeshPro
   - Name: `ProgressText`
   - Position: Bottom-right (X: 350, Y: -250)
   - Text: "0 / 100"
   - Font Size: 14
   - Color: Gray

   **g) ProgressBar (inside window):**
   - Right-click TypingMinigameWindow → UI → Slider
   - Name: `ProgressBar`
   - Position: Bottom (Y: -270)
   - Width: 700
   - Delete Handle
   - Set Fill to Yellow/Green
   - Interactable: OFF

4. **Wire TypingMinigameUI References:**
   - Select TypingMinigameUI GameObject
   - Minigame Window: Drag `TypingMinigameWindow`
   - Document Title Text: Drag `DocumentTitleText`
   - Document Content Text: Drag `DocumentContentText`
   - Progress Text: Drag `ProgressText`
   - Progress Bar: Drag `ProgressBar` → Fill (the Image inside)
   - Show Progress Bar: Check ON

---

## 🎫 PHASE 3: Create Ticket Prefab

### 3.1 Create Ticket GameObject

1. **Create Panel:**
   - Right-click Canvas → UI → Panel
   - Name: `TicketPrefab`
   - Size: Width: 180, Height: 120
   - Color: Light Yellow (#FFFFCC)

2. **Add Components:**
   - Add Component → CanvasGroup (required for dragging)
   - Add Component → Ticket.cs

3. **Create Children:**

   **a) TitleText:**
   - Right-click TicketPrefab → UI → Text - TextMeshPro
   - Name: `TitleText`
   - Position: Top (Y: 40)
   - Text: "Task Title"
   - Font Size: 14
   - Bold
   - Alignment: Center
   - Color: Black

   **b) DescriptionText:**
   - Right-click TicketPrefab → UI → Text - TextMeshPro
   - Name: `DescriptionText`
   - Position: Center (Y: 0)
   - Size: Width: 160, Height: 50
   - Text: "Task description here"
   - Font Size: 10
   - Alignment: Top-Center
   - Enable Word Wrap
   - Color: Dark Gray

   **c) StampImage:**
   - Right-click TicketPrefab → UI → Image
   - Name: `StampImage`
   - Position: Top-right (X: 60, Y: 40)
   - Size: 40x40
   - Color: Red or Green (#FF0000)
   - **Set Active to OFF** (hidden until stamped)
   - Optional: Add a checkmark sprite

   **d) StartTaskButton:**
   - Right-click TicketPrefab → UI → Button - TextMeshPro
   - Name: `StartTaskButton`
   - Position: Bottom (Y: -35)
   - Size: Width: 120, Height: 30
   - Text: "START TASK"
   - Font Size: 12
   - Bold
   - Button Color: Bright Green (#00FF00)
   - **Set Active to OFF** (will show when task has minigame)

4. **Wire Ticket References:**
   - Select TicketPrefab root
   - In Ticket component:
     - Title Text: Drag `TitleText`
     - Description Text: Drag `DescriptionText`
     - Stamp Image: Drag `StampImage`
     - Start Task Button: Drag `StartTaskButton`

---

### 3.2 Save as Prefab

1. **Create Prefabs Folder:**
   - In Project window, create: Assets/Prefabs/

2. **Save Prefab:**
   - Drag `TicketPrefab` from Hierarchy into Assets/Prefabs/ folder
   - This creates the prefab

3. **Delete from Hierarchy:**
   - Delete `TicketPrefab` from Hierarchy (we only need the prefab)

---

## 🔌 PHASE 4: Connect All References

### 4.1 Connect GameManager

1. Select **GameManager** GameObject
2. Fill in Inspector fields:

**UI References:**
- Outputs Text: Drag your existing OutputsText
- Outputs Per Second Text: Drag existing OutputsPerSecondText
- Quota Text: Drag `QuotaText`
- Day Text: Drag `DayText`
- Time Text: Drag `TimeText`
- Click Button: Drag your existing ClickButton
- Sleep Button: Drag `SleepButton`

**Settings (verify defaults):**
- Base Quota: 100
- Quota Multiplier: 1.5
- Time Progression Speed: 60
- Start Time: 6

---

### 4.2 Connect Printer

1. Select **Printer** GameObject
2. Fill in Inspector:

**Prefab:**
- Ticket Prefab: Drag the prefab from Assets/Prefabs/TicketPrefab

**UI References (should already be done):**
- Verify all are connected

**Settings:**
- Auto Print Interval: 30
- Auto Print Enabled: Check ON
- Max Active Tickets: 5

---

### 4.3 Connect ProcessingMachine

1. Select **ProcessingMachine**
2. Verify all references are connected (you did this earlier)
3. Double-check Printer reference is set

---

### 4.4 Connect TypingMinigameUI

1. Select **TypingMinigameUI**
2. Verify all window references are connected

---

### 4.5 Connect SystemLog

1. Select **SystemLog**
2. Log Text: Drag `SystemLogText`

---

### 4.6 Connect TaskDatabase

1. Select **TaskDatabase**
2. **Nothing to connect** - it auto-populates tasks

---

### 4.7 Connect TypingTaskDatabase

1. Select **TypingTaskDatabase**
2. Verify "Create Sample Tasks At Runtime" is CHECKED
3. All Typing Tasks list = 0 (empty is OK for now)

---

## ✅ PHASE 5: Test Everything!

### 5.1 Quick Verification

Before pressing Play, check:
- [ ] All GameObjects exist in Hierarchy
- [ ] All scripts are attached (no "Script Missing" warnings)
- [ ] All references are filled (no "None (Object)" in Inspector)
- [ ] TypingMinigameWindow is set to inactive
- [ ] Ticket Prefab exists in Assets/Prefabs/

---

### 5.2 Press Play and Test!

**Test 1: Printing**
- Wait 30 seconds or click PRINT TICKET button
- ✅ Ticket should appear at TicketSpawnPoint

**Test 2: Minigame**
- Click START TASK button on ticket
- ✅ Typing window should open
- Spam keyboard
- ✅ Message types out
- ✅ Window auto-closes
- ✅ Ticket shows stamp

**Test 3: Processing**
- Drag stamped ticket to ProcessingMachine
- ✅ Progress bar fills
- ✅ Ticket disappears
- ✅ Outputs increase
- ✅ Quota progress updates

**Test 4: Quota/Sleep**
- Complete enough tickets to reach quota (100 points)
- ✅ Sleep button enables
- Click SLEEP
- ✅ Day advances
- ✅ New quota set
- ✅ Time resets to 6:00 AM

---

## 🐛 Troubleshooting

### Issue: Tickets Don't Print

**Check:**
- [ ] Printer has Ticket Prefab assigned
- [ ] TaskDatabase exists in scene
- [ ] TypingTaskDatabase exists in scene
- [ ] Console for errors

### Issue: Can't Click START TASK Button

**Check:**
- [ ] EventSystem exists (should auto-create with Canvas)
- [ ] CanvasGroup on Ticket has Interactable checked
- [ ] Button has Button component
- [ ] TypingMinigameUI exists in scene

### Issue: Minigame Doesn't Open

**Check:**
- [ ] TypingMinigameUI references all filled
- [ ] TypingMinigameWindow child exists
- [ ] Console for errors

### Issue: Processing Machine Rejects Ticket

**Check:**
- [ ] Ticket is stamped (stamp visible)
- [ ] "Require Stamped Tickets" is checked on ProcessingMachine
- [ ] Printer reference is set on ProcessingMachine

### Issue: Sleep Button Never Enables

**Check:**
- [ ] GameManager has Sleep Button reference
- [ ] Quota is actually being reached (check QuotaText)
- [ ] Console for errors in UpdateSleepButton()

---

## 📊 Scene Hierarchy Reference

Your final hierarchy should look like this:

```
Canvas
├── Background (Image - your existing)
├── BackgroundController
├── OutputsText (TMP - your existing)
├── OutputsPerSecondText (TMP - your existing)
├── QuotaText (TMP - new)
├── DayText (TMP - new)
├── TimeText (TMP - new)
├── ClickButton (Button - your existing)
├── SleepButton (Button - new)
├── SystemLogText (TMP)
├── Printer
│   ├── PrinterBody (Image)
│   ├── PrintButton (Button)
│   ├── StatusText (TMP)
│   └── TicketSpawnPoint (Empty)
├── BulletinBoard (Panel)
│   └── TicketContainer (Empty)
├── ProcessingMachine (Panel)
│   ├── MachineBody (Image)
│   ├── MachineStatus (TMP)
│   └── ProgressBar (Slider)
└── TypingMinigameUI
    └── TypingMinigameWindow (Panel - INACTIVE)
        ├── Header (Panel)
        │   └── DocumentTitleText (TMP)
        ├── DocumentArea (Panel)
        │   └── DocumentContentText (TMP)
        ├── ProgressText (TMP)
        └── ProgressBar (Slider)

Scene Root
├── GameManager (your existing)
├── TaskDatabase (new)
├── TypingTaskDatabase (new)
└── SystemLog (new or verify)
```

---

## 🎉 Success Checklist

Once everything works, you should see:
- ✅ Tickets print every 30 seconds
- ✅ Clicking START TASK opens minigame
- ✅ Typing reveals message character by character
- ✅ Completing minigame stamps ticket
- ✅ Dragging stamped ticket to machine gives rewards
- ✅ Quota tracks progress
- ✅ Sleep button enables when quota met
- ✅ Time progresses
- ✅ Day advances when sleeping

---

## 🚀 Next Steps

After basic setup works:
1. Create custom typing tasks (Tools → Typing Task Creator)
2. Style your UI to match your game's aesthetic
3. Add sound effects
4. Polish animations and transitions
5. Create more varied typing messages

---

**Estimated Time:** 45 minutes
**Difficulty:** Medium (lots of UI setup, but straightforward)

Good luck! Follow each step carefully and you'll have a fully working game! 🎮
