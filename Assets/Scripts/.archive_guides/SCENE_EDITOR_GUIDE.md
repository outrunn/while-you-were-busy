# Scene Editor Guide

## How to Use

### Setup
1. Select your **Canvas** in the Hierarchy
2. Add the **SceneEditor** script to it
3. Leave **Editor Mode Enabled** checked

### Controls

| Action | Control |
|--------|---------|
| **Select Asset** | Click on any UI element |
| **Move Asset** | Click and drag with Left Mouse |
| **Resize Asset** | Right click + drag (from top-left corner) |
| **Nudge Position** | Arrow keys (fine adjustment) |
| **Delete Asset** | Select + Press Delete key |
| **Log Info** | Select asset + Press I |
| **Print All Positions** | Right-click SceneEditor component > "Print All Asset Positions" |

### Visual Feedback

- **Selected assets** turn **yellow** (easily see what you're editing)
- **Unselected assets** are **white** (default)

### Tips

1. **Fine-tune positions** with arrow keys instead of dragging
2. **Get exact positions** by pressing I to log to console
3. **Export positions** by right-clicking component > "Print All Asset Positions"
4. **Works in Play Mode** - edit while game is running to see real-time changes

### Example Workflow

1. Press Play
2. Click on the bulletin board to select it (turns yellow)
3. Drag to reposition
4. Right-click + drag to resize
5. Press I to log exact position/size
6. Copy that info to update your SetupAllAssets script

### Copy Setup Code

After positioning everything perfectly:
1. Right-click SceneEditor component
2. Click "Print All Asset Positions"
3. Check Console
4. Copy the output into your SetupAllAssets.cs `CreateAllAssets()` method

Now your positions are permanently saved!

