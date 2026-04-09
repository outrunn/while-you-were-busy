# Photo Reveal Minigame - Detailed Setup Guide

This is the most complex minigame UI. Follow each step carefully.

---

## Step 1: Create Tile Prefab (Do This First!)

### 1.1: Create Assets/Prefabs Folder
1. In **Project window**, right-click on **Assets** folder
2. **Create** → **Folder**
3. Name it `Prefabs`

### 1.2: Create Empty GameObject for Prefab
1. In **Hierarchy**, right-click → **Create Empty**
2. Rename to `TilePrefab`
3. **Important:** Don't worry about positioning, it's just temporary

### 1.3: Add Image Component to TilePrefab
1. With `TilePrefab` selected, in **Inspector** → **Add Component** → **Image**
2. Configure the Image component:
   - **Source Image**: Any sprite (it will be overridden by script)
   - **Color**: (128, 128, 128, 255) - gray placeholder
   - **Raycast Target**: **ON** (IMPORTANT! Needed for hover detection)

### 1.4: Add EventTrigger Component
1. **Add Component** → **Event Trigger**
2. Leave it as-is (script will configure the hover events)

### 1.5: Set RectTransform Size
1. In **RectTransform**, set:
   - Width: 50
   - Height: 50
   - This is the tile size

### 1.6: Save as Prefab
1. **Drag** `TilePrefab` from Hierarchy into **Assets/Prefabs** folder
2. This creates `TilePrefab.prefab`
3. Now **delete** the `TilePrefab` from the Hierarchy (we only need the prefab file)

**Result:** You should have `Assets/Prefabs/TilePrefab.prefab` in your Project window

---

## Step 2: Create PhotoRevealMinigameUI Panel

### 2.1: Create Root Panel
1. Right-click on **Canvas** → **UI** → **Panel**
2. Rename to `PhotoRevealMinigameUI`
3. In **RectTransform**:
   - Click the anchor preset (top-left corner icon)
   - Select **Stretch** (full-screen)
   - Left/Right/Top/Bottom: 0

### 2.2: Add Background Image
1. Right-click on `PhotoRevealMinigameUI` → **UI** → **Image**
2. Rename to `Background`
3. In **Image** component:
   - Color: (0, 0, 0, 204) - black at ~80% opacity

### 2.3: Create ImageContainer (The Tile Grid)
This is the parent panel that holds all the tiles.

1. Right-click on `PhotoRevealMinigameUI` → **UI** → **Panel**
2. Rename to `ImageContainer`
3. In **RectTransform**:
   - Anchors: Center
   - Position X: 0
   - Position Y: 0
   - Width: 350
   - Height: 350
4. **Important:** In **Image** component, click the **X** to remove it
   - We don't want the panel itself to have a background image

### 2.4: Add GridLayoutGroup to ImageContainer
1. With `ImageContainer` selected, **Add Component** → **GridLayoutGroup**
2. Configure these settings exactly:
   - **Child Force Expand**: Width ON, Height ON
   - **Child Control Size**: Width ON, Height ON
   - **Cell Size**: X: 50, Y: 50
   - **Spacing**: X: 2, Y: 2
   - **Start Corner**: Upper Left
   - **Start Axis**: Horizontal
   - **Constraint**: Fixed Column Count
   - **Constraint Count**: 6

**This creates a 6×6 grid (36 tiles total)**

### 2.5: Add Progress Text
1. Right-click on `PhotoRevealMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `ProgressText`
3. In **RectTransform**:
   - Anchors: Bottom Center
   - Position X: 0
   - Position Y: 50
   - Width: 400
   - Height: 100
4. In **TextMeshProUGUI** component:
   - Text: "0% revealed"
   - Font Size: 36
   - Alignment: Center (bottom middle)
   - Color: White

### 2.6: Attach Script to Panel
1. Select `PhotoRevealMinigameUI` panel
2. **Add Component** → **PhotoRevealMinigameUI**

### 2.7: Assign All References in Inspector

Click on `PhotoRevealMinigameUI` panel and in the Inspector, find the **PhotoRevealMinigameUI** script component.

Fill in these fields:

#### UI References
- **Minigame Window**: Drag `PhotoRevealMinigameUI` panel here
- **Image Container**: Drag `ImageContainer` panel here
- **Progress Text**: Drag `ProgressText` here
- **Tile Prefab**: Drag `TilePrefab.prefab` from Assets/Prefabs folder

#### Settings
- **Tile Count X**: 6
- **Tile Count Y**: 6
- **Reveal Threshold**: 0.8 (80% = completion)
- **Completion Delay**: 2.5

#### The Reveal Sprite
- **Reveal Sprite**: (See next section)

### 2.8: Hide the Panel
1. With `PhotoRevealMinigameUI` selected
2. Uncheck the checkbox next to its name in Hierarchy
3. This hides it until the minigame starts

---

## Step 3: Create/Assign the Reveal Sprite

You need an office-themed image that will be revealed by hovering over tiles.

### Option A: Use an Existing Image
1. Find an office-themed image file (document, desk, etc.)
2. Drag it into **Assets/UI_Assets** (or any folder in Assets)
3. Click on the image in Project window
4. In Inspector, set **Texture Type** to **Sprite (2D and UI)**
5. Click **Apply**
6. Drag this sprite into the **Reveal Sprite** field on PhotoRevealMinigameUI

### Option B: Create a Simple Placeholder
1. Open any image editor (Photoshop, GIMP, Paint, etc.)
2. Create a 512×512 image
3. Draw something office-themed (document, filing cabinet, computer, etc.)
4. Save as PNG in **Assets/UI_Assets/**
5. Follow Option A steps 3-6

### Option C: Use Built-in UI Sprite (Quick Test)
1. In the Inspector for **Reveal Sprite** field
2. Click the circle icon (object picker)
3. Search "Square" and select the built-in **Square** sprite
4. This will work for testing (white square revealed)

---

## Final Hierarchy Check

Your Canvas should look like this:

```
Canvas
├── MultipleChoiceMinigameUI (Panel - hidden)
│   └── [previously configured]
│
├── MathMinigameUI (Panel - hidden)
│   └── [previously configured]
│
└── PhotoRevealMinigameUI (Panel - hidden) ← NEW
    ├── Background (Image)
    ├── ImageContainer (Panel with GridLayoutGroup)
    │   └── (Tiles will be generated here at runtime)
    └── ProgressText (TextMeshProUGUI)
```

---

## Testing Photo Reveal

1. **Press Play** in Unity
2. Use Debug Commands:
   - `F` → Jump to Day 4
   - `T` → Print a Photo Reveal ticket
   - `M` → Start the minigame
3. You should see:
   - A 6×6 grid of gray tiles
   - "0% revealed" at bottom
   - **Move your mouse over tiles** to reveal the hidden image
   - When 80%+ revealed, minigame completes

---

## Troubleshooting

### "Tiles don't appear"
- Make sure **GridLayoutGroup** is on `ImageContainer`
- Make sure Constraint is set to **Fixed Column Count: 6**
- Make sure `TilePrefab.prefab` is assigned in the script

### "Can't hover on tiles"
- Make sure **Raycast Target** is ON in the Tile Image component
- Make sure **EventTrigger** is attached to TilePrefab
- Check that tiles are visible (not behind something)

### "Reveal image doesn't show"
- Make sure you assigned a **Reveal Sprite**
- Make sure the sprite has **Texture Type: Sprite (2D and UI)**
- Try using a simple color (like the built-in Square sprite) to test

### "Completion doesn't trigger"
- Check that **Reveal Threshold** is set to 0.8 (80%)
- Make sure you're actually revealing tiles (cursor over them)
- Check the Console for any error messages

---

## How It Works (Under the Hood)

When the minigame starts:
1. `StartMinigame()` is called
2. Script creates 36 tile GameObjects in `ImageContainer`
3. Each tile gets:
   - An Image component (shows mosaic color initially)
   - An EventTrigger (listens for mouse hover)
4. Player hovers over tiles
5. `OnTileHover()` is triggered
6. Tile reveals: Image shows the Reveal Sprite, color becomes white
7. Script tracks revealed count
8. At 80% revealed, `CompleteMinigame()` triggers
9. After 2.5 seconds, minigame closes and callback fires
10. Ticket gets stamped

---

## Inspector Checklist

Before testing, verify all these are assigned:

✓ PhotoRevealMinigameUI script attached to PhotoRevealMinigameUI panel
✓ Minigame Window = PhotoRevealMinigameUI panel
✓ Image Container = ImageContainer panel
✓ Progress Text = ProgressText
✓ Tile Prefab = TilePrefab.prefab (from Assets/Prefabs)
✓ Reveal Sprite = Your office image
✓ Tile Count X = 6
✓ Tile Count Y = 6
✓ Reveal Threshold = 0.8
✓ Completion Delay = 2.5
✓ Panel is HIDDEN (checkbox unchecked)

Once all are checked, you're ready to test! 🎉
