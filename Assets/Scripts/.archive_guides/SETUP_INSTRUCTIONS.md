# Unity Setup Instructions - "While You Were Busy"

## Quick Setup Guide

### 1. Scene Setup

1. Create a new scene or open your existing scene
2. Create an empty GameObject and name it "GameManager"
3. Attach the `GameManager.cs` script to this GameObject

### 2. Background Setup

1. Create a UI Image:
   - Right-click in Hierarchy → UI → Image
   - Name it "Background"
   - Anchor it to stretch full screen (Anchor Presets: stretch/stretch)
   - Set it as the first child under Canvas (bottom of rendering order)

2. Create an empty GameObject under Canvas, name it "BackgroundController"
3. Attach `BackgroundController.cs` to it
4. Drag the Background Image into the "Background Image" field in the Inspector

### 3. Main UI Setup

Create these UI elements under your Canvas:

#### Click Button
- Right-click Canvas → UI → Button
- Name it "ClickButton"
- Position it prominently (center or bottom-center)
- Change button text to "PROCESS OUTPUTS" or similar

#### Output Display
- Create UI → Text, name it "OutputsText"
- Position at top-left
- Set font size to 24+

- Create UI → Text, name it "OutputsPerSecondText"
- Position below OutputsText
- Set font size to 18+

#### System Log
- Create UI → Text, name it "SystemLogText"
- Position at bottom or top-right
- Set font size to 14-16
- Optional: Set text color to a subtle gray or yellow

- Create empty GameObject, name it "SystemLog"
- Attach `SystemLog.cs` to it
- Drag SystemLogText into the "Log Text" field

### 4. Connect GameManager References

Select the GameManager GameObject and in the Inspector:

**UI References:**
- Drag "OutputsText" to the "Outputs Text" field
- Drag "OutputsPerSecondText" to the "Outputs Per Second Text" field
- Drag "ClickButton" to the "Click Button" field

**Adjust Settings (optional):**
- World Health Decay Per Click: 0.1 (default)
- World Health Decay Per Output: 0.005 (default)
- Modify upgrade costs and bonuses as desired

### 5. Upgrade Buttons Setup

For each upgrade, create:

1. Right-click Canvas → UI → Button
2. Name appropriately ("AutoProcessorButton", "ScalingEngineButton", "ExpansionProtocolButton")
3. Attach `UpgradeButton.cs` to each button
4. Configure each in Inspector:

#### Auto Processor Button
- Upgrade Type: AutoProcessor
- Upgrade Name: "Auto Processor"
- Upgrade Description: "+1 output/sec"
- (Button and text fields auto-detect, or manually assign)

#### Scaling Engine Button
- Upgrade Type: ScalingEngine
- Upgrade Name: "Scaling Engine"
- Upgrade Description: "x2 per click"

#### Expansion Protocol Button
- Upgrade Type: ExpansionProtocol
- Upgrade Name: "Expansion Protocol"
- Upgrade Description: "+10 outputs/sec"

**For each button, add child text elements:**
- Create 3 Text children under each button:
  - "NameText" - displays upgrade name
  - "CostText" - displays current cost
  - "DescriptionText" - displays what it does
- Drag these into the corresponding fields in UpgradeButton Inspector

### 6. Hierarchy Structure (Example)

```
Canvas
├── Background (Image)
├── BackgroundController (Empty GameObject with script)
├── OutputsText (Text)
├── OutputsPerSecondText (Text)
├── ClickButton (Button)
├── SystemLog (Empty GameObject with script)
│   └── SystemLogText (Text) - child or separate
├── AutoProcessorButton (Button + UpgradeButton.cs)
│   ├── NameText
│   ├── CostText
│   └── DescriptionText
├── ScalingEngineButton (Button + UpgradeButton.cs)
│   ├── NameText
│   ├── CostText
│   └── DescriptionText
└── ExpansionProtocolButton (Button + UpgradeButton.cs)
    ├── NameText
    ├── CostText
    └── DescriptionText

GameManager (Empty GameObject with GameManager.cs)
```

---

## Quick Testing Checklist

- [ ] Click the main button - outputs should increase
- [ ] Background color should start light and gradually darken as you play
- [ ] System log should show different messages as world health decreases
- [ ] Purchase an Auto Processor - passive generation should start
- [ ] Purchase a Scaling Engine - click value should double
- [ ] Purchase Expansion Protocol - passive generation should increase significantly
- [ ] Upgrade buttons should disable when you can't afford them
- [ ] Background should transition through: light blue → desaturated → dark → very dark

---

## Customization Tips

### Adjust World Health Decay Rate
In GameManager Inspector:
- `World Health Decay Per Click`: How much health is lost per click (default 0.1)
- `World Health Decay Per Output`: Health lost per output generated (default 0.005)

### Modify Background Colors
In BackgroundController Inspector:
- `Healthy Color`: Starting color (default light blue)
- `Degraded Color`: Below 70 health (default desaturated)
- `Critical Color`: Below 40 health (default dark)
- `Collapsed Color`: Below 15 health (default very dark)
- `Transition Speed`: How fast colors change (default 0.5)

### Change Upgrade Costs & Effects
In GameManager Inspector under "Upgrade Costs" and "Upgrade Values"

### Update System Log Messages
Edit `SystemLog.cs` in the `GetMessageForHealth()` method to customize narrative

---

## Notes

- The game intentionally hides world health from the player
- Visual and narrative feedback are the only clues
- You can expose worldHealth in GameManager for debugging (remove `[SerializeField]` to hide it)
- All costs scale automatically after purchase
- No external packages required
- Single scene, minimal dependencies

---

## Troubleshooting

**Buttons not responding:**
- Check that GameManager.Instance is not null
- Verify button references are assigned in Inspector

**Background not changing:**
- Ensure Background Image is assigned in BackgroundController
- Check that GameManager exists in scene

**System log not updating:**
- Verify Log Text is assigned in SystemLog Inspector
- Check that GameManager.Instance exists

**Upgrade buttons always disabled:**
- Ensure UpgradeButton has button reference
- Verify GameManager has upgrade cost getters implemented

---

Good luck with your game jam!
