# Build Minigame UIs - Step-by-Step Instructions

Complete these steps in order. Each minigame needs its own Canvas Panel with specific child elements.

---

## Part 1: MultipleChoiceMinigameUI Panel

### Step 1: Create the Root Panel
1. In Hierarchy, right-click on **Canvas** → **UI** → **Panel**
2. Rename it to `MultipleChoiceMinigameUI`
3. In Inspector:
   - Set **RectTransform** anchors to stretch (top-left corner icon → "Stretch")
   - Left/Right/Top/Bottom: 0
   - This makes the panel full-screen

### Step 2: Add Background Image
1. Right-click on `MultipleChoiceMinigameUI` → **UI** → **Image**
2. Rename to `Background`
3. In Inspector:
   - **Image** → Color: `(0, 0, 0, 0.8)` (black, 80% opacity)
   - This creates a dark overlay

### Step 3: Add Question Text
1. Right-click on `MultipleChoiceMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `QuestionText`
3. Configure RectTransform:
   - Anchors: Top Center (top-middle)
   - Position X: 0, Y: -100
   - Width: 800, Height: 150
4. In TextMeshPro component:
   - Text: "Sample Question"
   - Font Size: 48
   - Alignment: Center
   - Color: White

### Step 4: Create Answer Grid (2×2 Layout)
1. Right-click on `MultipleChoiceMinigameUI` → **UI** → **Panel**
2. Rename to `AnswerGrid`
3. Configure RectTransform:
   - Anchors: Center
   - Position X: 0, Y: -150
   - Width: 600, Height: 400
4. Add component: **GridLayoutGroup**
   - Child Force Expand: Width ON, Height ON
   - Spacing: (20, 20)
   - Constraint: Fixed Column Count = 2
   - Constraint Count: 2

### Step 5: Create 4 Answer Buttons
**Repeat this 4 times (for buttons 1, 2, 3, 4):**

1. Right-click on `AnswerGrid` → **UI** → **Button**
2. Rename to `AnswerButton1` (then Button2, Button3, Button4)
3. Leave default layout (GridLayoutGroup will arrange)
4. **Inside each button, add text:**
   - Right-click button → **UI** → **TextMeshPro - Text**
   - Delete the auto-created text child
   - Click on the button itself
   - In Hierarchy, expand the button to see "Text (TMP)"
   - Rename this child to just match the option letter (optional)
5. Configure the text inside:
   - Text: "Option A" (or B, C, D)
   - Font Size: 32
   - Alignment: Center
   - Color: White

### Step 6: Add Feedback Text
1. Right-click on `MultipleChoiceMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `FeedbackText`
3. Configure RectTransform:
   - Anchors: Bottom Center
   - Position X: 0, Y: 50
   - Width: 400, Height: 100
4. In TextMeshPro:
   - Text: "Correct!" or "Incorrect!"
   - Font Size: 36
   - Alignment: Center
   - Color: Green (or Red for error state)

### Step 7: Assign Script and References
1. Right-click on `MultipleChoiceMinigameUI` panel → **Add Component** → **MultipleChoiceMinigameUI**
2. In Inspector, expand the script component and assign:
   - **Minigame Window**: Drag `MultipleChoiceMinigameUI` panel to this field
   - **Question Text**: Drag `QuestionText` to this field
   - **Answer Buttons[0-3]**: Drag each button in order (Button1, Button2, Button3, Button4)
   - **Answer Texts[0-3]**: Drag the TextMeshPro text INSIDE each button (in order)
   - **Feedback Text**: Drag `FeedbackText`
   - **Window Background**: Drag `Background` image

### Step 8: Hide on Start
1. Select `MultipleChoiceMinigameUI` panel
2. Uncheck the checkbox next to the panel name in Hierarchy
3. This hides it by default (script shows it when minigame starts)

---

## Part 2: MathMinigameUI Panel

### Step 1: Create the Root Panel
1. Right-click on **Canvas** → **UI** → **Panel**
2. Rename to `MathMinigameUI`
3. In Inspector:
   - Set RectTransform anchors to stretch (full-screen)
   - Left/Right/Top/Bottom: 0

### Step 2: Add Background Image
1. Right-click on `MathMinigameUI` → **UI** → **Image**
2. Rename to `Background`
3. Color: `(0, 0, 0, 0.8)` (dark overlay)

### Step 3: Add Problem Text
1. Right-click on `MathMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `ProblemText`
3. Configure RectTransform:
   - Anchors: Top Center
   - Position X: 0, Y: -100
   - Width: 600, Height: 150
4. In TextMeshPro:
   - Text: "47 + 38 = ?"
   - Font Size: 56
   - Alignment: Center
   - Color: White

### Step 4: Add Input Display
1. Right-click on `MathMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `InputDisplay`
3. Configure RectTransform:
   - Anchors: Center
   - Position X: 0, Y: 0
   - Width: 400, Height: 150
4. In TextMeshPro:
   - Text: "_"
   - Font Size: 48
   - Alignment: Center
   - Color: Yellow (to stand out)

### Step 5: Add Feedback Text
1. Right-click on `MathMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `FeedbackText`
3. Configure RectTransform:
   - Anchors: Bottom Center
   - Position X: 0, Y: 50
   - Width: 400, Height: 100
4. In TextMeshPro:
   - Text: "Enter your answer"
   - Font Size: 32
   - Alignment: Center
   - Color: White

### Step 6: Assign Script and References
1. Right-click on `MathMinigameUI` panel → **Add Component** → **MathMinigameUI**
2. In Inspector, assign:
   - **Minigame Window**: `MathMinigameUI` panel
   - **Problem Text**: `ProblemText`
   - **Input Text**: `InputDisplay`
   - **Feedback Text**: `FeedbackText`
   - **Window Background**: `Background`

### Step 7: Hide on Start
1. Select `MathMinigameUI`
2. Uncheck the checkbox to hide it

---

## Part 3: PhotoRevealMinigameUI Panel + Tile Prefab

### Part 3A: Create Tile Prefab First

#### Step 1: Create Tile GameObject
1. In Hierarchy, right-click → **Create Empty**
2. Rename to `TilePrefab`
3. **Important:** Delete this from Hierarchy after creating the prefab (don't keep instance in scene)

#### Step 2: Add Image Component
1. With `TilePrefab` selected, **Add Component** → **Image**
2. In Inspector:
   - Raycast Target: **ON** (important for hover detection)
   - Source Image: Assign any sprite (will be replaced at runtime)
   - Color: `(0.5, 0.5, 0.5, 1)` (gray placeholder)

#### Step 3: Add EventTrigger Component
1. **Add Component** → **EventTrigger**
2. In Inspector, you'll see "Event Trigger" component
3. Click **Add Event Type** → **Pointer Enter**
4. You don't need to set up the callback now (script does this)

#### Step 4: Save as Prefab
1. With `TilePrefab` still selected, drag it from Hierarchy into:
   - **Assets/Prefabs/** folder (create Prefabs folder if needed)
   - This creates `TilePrefab.prefab`
2. Delete the `TilePrefab` from the Hierarchy (we only needed the prefab)

### Part 3B: Create PhotoRevealMinigameUI Panel

#### Step 1: Create the Root Panel
1. Right-click on **Canvas** → **UI** → **Panel**
2. Rename to `PhotoRevealMinigameUI`
3. RectTransform: Stretch (full-screen)

#### Step 2: Add Background
1. Right-click on `PhotoRevealMinigameUI` → **UI** → **Image**
2. Rename to `Background`
3. Color: `(0, 0, 0, 0.8)`

#### Step 3: Create Image Container (for tile grid)
1. Right-click on `PhotoRevealMinigameUI` → **UI** → **Panel**
2. Rename to `ImageContainer`
3. Configure RectTransform:
   - Anchors: Center
   - Position X: 0, Y: 0
   - Width: 350, Height: 350
4. **Important:** Remove the Image component from this panel (right-click on Image in Inspector → Remove Component)
5. **Add GridLayoutGroup component:**
   - Child Force Expand: Width OFF, Height OFF
   - Cell Size: (50, 50)
   - Spacing: (2, 2)
   - Child Control Size: Width ON, Height ON
   - Constraint: Fixed Column Count
   - Constraint Count: 6

#### Step 4: Add Progress Text
1. Right-click on `PhotoRevealMinigameUI` → **UI** → **TextMeshPro - Text**
2. Rename to `ProgressText`
3. Configure RectTransform:
   - Anchors: Bottom Center
   - Position X: 0, Y: 50
   - Width: 400, Height: 100
4. In TextMeshPro:
   - Text: "0% revealed"
   - Font Size: 32
   - Alignment: Center
   - Color: White

#### Step 5: Assign Script and References
1. Right-click on `PhotoRevealMinigameUI` panel → **Add Component** → **PhotoRevealMinigameUI**
2. In Inspector, assign:
   - **Minigame Window**: `PhotoRevealMinigameUI` panel
   - **Image Container**: `ImageContainer` panel
   - **Progress Text**: `ProgressText`
   - **Tile Prefab**: Drag the `TilePrefab.prefab` from Assets/Prefabs folder
   - **Reveal Sprite**: Assign an office-themed image (you need to create/import one)
   - **Tile Count X**: 6
   - **Tile Count Y**: 6
   - **Reveal Threshold**: 0.8
   - **Completion Delay**: 2.5

#### Step 6: Create/Assign Reveal Sprite
You need an office-themed image for the reveal sprite:

**Option A: Use a placeholder**
1. In Assets/UI_Assets folder (or create a folder)
2. Import or create a simple office image (document, file cabinet, etc.)
3. Drag it to the `Reveal Sprite` field

**Option B: Create a simple sprite**
1. Create a 512×512 image in your graphics program (or use existing)
2. Save to Assets folder
3. Select it in Inspector → set **Texture Type** to **Sprite (2D and UI)**
4. Assign to `Reveal Sprite` field

#### Step 7: Hide on Start
1. Select `PhotoRevealMinigameUI`
2. Uncheck the checkbox to hide

---

## Summary: All Three Panels

When done, your Canvas Hierarchy should look like:
```
Canvas
├── MultipleChoiceMinigameUI (Panel - hidden)
│   ├── Background (Image)
│   ├── QuestionText (TextMeshPro)
│   ├── AnswerGrid (Panel with GridLayout)
│   │   ├── AnswerButton1 (Button with Text)
│   │   ├── AnswerButton2 (Button with Text)
│   │   ├── AnswerButton3 (Button with Text)
│   │   └── AnswerButton4 (Button with Text)
│   └── FeedbackText (TextMeshPro)
│
├── MathMinigameUI (Panel - hidden)
│   ├── Background (Image)
│   ├── ProblemText (TextMeshPro)
│   ├── InputDisplay (TextMeshPro)
│   └── FeedbackText (TextMeshPro)
│
└── PhotoRevealMinigameUI (Panel - hidden)
    ├── Background (Image)
    ├── ImageContainer (Panel with GridLayout)
    └── ProgressText (TextMeshPro)
```

---

## Testing After Setup

Once all three are built:

1. Press Play in Unity
2. Use Debug Commands to test:
   - `T` → Print ticket for current day
   - `M` → Start minigame
   - `N` → Advance day
   - `F` → Jump to Day 5
   - `S` → Log state

**Expected behavior:**
- Day 1: Typing games only
- Day 2: Mix of Typing + Multiple Choice (click buttons)
- Day 3: Mix of Typing + MC + Math (type answers)
- Day 4: All 4 types including Photo Reveal (hover to uncover)

---

## Troubleshooting

**"Minigame window doesn't show"**
- Make sure panel is NOT hidden (checkbox should be unchecked)
- Verify script component is attached

**"Buttons don't work"**
- Make sure you assigned the button references in order [0-3]
- Make sure text references match the text inside each button

**"Photo tiles don't appear"**
- Make sure GridLayoutGroup is configured correctly (6 columns)
- Make sure TilePrefab has Image + EventTrigger components
- Make sure you assigned TilePrefab in the PhotoRevealMinigameUI script

**"Can't click/hover on elements"**
- Make sure **Raycast Target** is ON for interactive elements (buttons, images)
- Make sure elements aren't hidden behind other UI
