# Quick Setup Checklist ✓

## Core GameObjects (Scene Root)
- [ ] **GameManager** (existing) with GameManager.cs
- [ ] **TaskDatabase** with TaskDatabase.cs
- [ ] **TypingTaskDatabase** with TypingTaskDatabase.cs
- [ ] **SystemLog** with SystemLog.cs

## UI GameObjects (Under Canvas)
- [ ] **QuotaText** (TMP) - Shows "Quota: 0/100"
- [ ] **DayText** (TMP) - Shows "Day 1"
- [ ] **TimeText** (TMP) - Shows "Time: 6:00 AM"
- [ ] **SleepButton** (Button) - "SLEEP"
- [ ] **SystemLogText** (TMP) - Shows system messages

## Printer Setup
- [ ] **Printer** GameObject under Canvas
- [ ] Printer.cs script attached
- [ ] **PrinterBody** child (Image)
- [ ] **PrintButton** child (Button)
- [ ] **StatusText** child (TMP)
- [ ] **TicketSpawnPoint** child (Empty)

## BulletinBoard Setup
- [ ] **BulletinBoard** (Panel) under Canvas
- [ ] BulletinBoard.cs script attached
- [ ] **TicketContainer** child (Empty)

## ProcessingMachine Setup
- [ ] **ProcessingMachine** (Panel) under Canvas
- [ ] ProcessingMachine.cs script attached
- [ ] **MachineBody** child (Image)
- [ ] **MachineStatus** child (TMP)
- [ ] **ProgressBar** child (Slider)

## TypingMinigameUI Setup
- [ ] **TypingMinigameUI** GameObject under Canvas
- [ ] TypingMinigameUI.cs script attached
- [ ] **TypingMinigameWindow** child (Panel - INACTIVE!)
  - [ ] **Header** → **DocumentTitleText** (TMP)
  - [ ] **DocumentArea** → **DocumentContentText** (TMP)
  - [ ] **ProgressText** (TMP)
  - [ ] **ProgressBar** (Slider)

## Ticket Prefab
- [ ] Create **TicketPrefab** (Panel)
- [ ] Add CanvasGroup component
- [ ] Add Ticket.cs script
- [ ] **TitleText** child (TMP)
- [ ] **DescriptionText** child (TMP)
- [ ] **StampImage** child (Image - INACTIVE)
- [ ] **StartTaskButton** child (Button - INACTIVE)
- [ ] Save as prefab in Assets/Prefabs/
- [ ] Delete from Hierarchy

## GameManager Connections
- [ ] Outputs Text → existing
- [ ] Outputs Per Second Text → existing
- [ ] Quota Text → QuotaText
- [ ] Day Text → DayText
- [ ] Time Text → TimeText
- [ ] Click Button → existing
- [ ] Sleep Button → SleepButton

## Printer Connections
- [ ] Ticket Prefab → Assets/Prefabs/TicketPrefab
- [ ] Ticket Spawn Point → TicketSpawnPoint
- [ ] Print Button → PrintButton
- [ ] Status Text → StatusText
- [ ] Printer Animation → PrinterBody (optional)

## BulletinBoard Connections
- [ ] Ticket Container → TicketContainer

## ProcessingMachine Connections
- [ ] Status Text → MachineStatus
- [ ] Machine Image → MachineBody
- [ ] Progress Bar → ProgressBar
- [ ] Printer → Printer GameObject
- [ ] Require Stamped Tickets → CHECKED

## TypingMinigameUI Connections
- [ ] Minigame Window → TypingMinigameWindow
- [ ] Document Title Text → DocumentTitleText
- [ ] Document Content Text → DocumentContentText
- [ ] Progress Text → ProgressText
- [ ] Progress Bar → ProgressBar Fill
- [ ] Show Progress Bar → CHECKED

## SystemLog Connections
- [ ] Log Text → SystemLogText

## Ticket Prefab Connections (in prefab)
- [ ] Title Text → TitleText
- [ ] Description Text → DescriptionText
- [ ] Stamp Image → StampImage
- [ ] Start Task Button → StartTaskButton

## Final Checks
- [ ] Console has no errors
- [ ] All "None (Object)" fields filled
- [ ] TypingMinigameWindow is inactive
- [ ] TextMeshPro imported

## Test Run
- [ ] Press Play
- [ ] Wait 30s or click Print
- [ ] Ticket appears ✓
- [ ] Click START TASK ✓
- [ ] Minigame opens ✓
- [ ] Spam keyboard ✓
- [ ] Ticket stamps ✓
- [ ] Drag to machine ✓
- [ ] Get rewards ✓
- [ ] Quota increases ✓
- [ ] Sleep button enables at 100 ✓

---

**Time Estimate:** 45 minutes
**All checked?** → Game is ready! 🎉
