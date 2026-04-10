# UI Positioning Guide

## How to Adjust UI Element Positions

The game uses a single entry point architecture where `AutoSetupScene` creates and positions all UI elements at runtime. To adjust positions:

### 1. Edit AutoSetupScene.cs
Location: `Assets/Scripts/UI/AutoSetupScene.cs`

### 2. Find the CreateAsset() calls
Look for lines like:
```csharp
CreateAsset("Wallpaper", canvas, wallpaper, new Vector2(960f, 540f), new Vector2(1920f, 1080f), 0, false);
```

### 3. Adjust the position and size values

**Parameters:**
- `name`: Element name (don't change)
- `canvas`: Canvas reference (don't change)
- `sprite`: Sprite asset (don't change)
- **`new Vector2(960f, 540f)`** ← Position (X, Y)
- **`new Vector2(1920f, 1080f)`** ← Size (Width, Height)
- `0`: Sorting order (layer depth, 0 = behind, 5 = in front)
- `false`: Raycast target (don't change)

### 4. Position Coordinate System

The coordinate system uses **center-anchored positioning**:
- **X**: 0 = left edge, 960 = center, 1920 = right edge
- **Y**: 0 = bottom edge, 540 = center, 1080 = top edge

### 5. Test Your Changes

Play the game to see the new positions. Adjust values and replay until satisfied.

### 6. Example Adjustments

**Move Desk higher (closer to center):**
```csharp
// Old:
CreateAsset("Desk", canvas, desk, new Vector2(960f, 70f), new Vector2(1920f, 200f), 1, false);

// New (moved from 70 to 150):
CreateAsset("Desk", canvas, desk, new Vector2(960f, 150f), new Vector2(1920f, 200f), 1, false);
```

**Make Printer wider:**
```csharp
// Old:
CreateAsset("Printer", canvas, printer, new Vector2(200f, 150f), new Vector2(296f, 367f), 5, false);

// New (width 296 → 400):
CreateAsset("Printer", canvas, printer, new Vector2(200f, 150f), new Vector2(400f, 367f), 5, false);
```

## Current UI Elements and Their Positions

| Element | Position | Size | Layer |
|---------|----------|------|-------|
| Wallpaper | (960, 540) | 1920×1080 | 0 |
| Desk | (960, 70) | 1920×200 | 1 |
| Laptop | (960, 450) | 900×600 | 4 |
| Frame | (960, 450) | 800×600 | 3 |
| BulletinBoard | (540, 400) | 785×398 | 5 |
| Printer | (200, 150) | 296×367 | 5 |
| Shredder | (1720, 150) | 250×280 | 5 |
| Flower | (350, 140) | 94×210 | 5 |
| Window | (1520, 380) | 541×455 | 5 |

## To View Layout in Editor (Not in Play Mode)

Currently, the UI only renders when the game is played. To see the layout while editing:

1. Click **Play** to enter play mode
2. Open the **Scene View** (window → Scene)
3. You'll see colored boxes representing each UI element
4. Make notes of what you want to adjust
5. Exit play mode and edit AutoSetupScene.cs
6. Repeat until satisfied
