# UI Hierarchy Guide for Minigames

Here's the exact GameObject hierarchy you need to build in Unity for each minigame panel.

---

## MultipleChoiceMinigameUI Hierarchy

```
Canvas
└── MultipleChoiceMinigameUI (Panel)
    ├── Background (Image) [assign to "Window Background" in script]
    ├── QuestionText (TextMeshProUGUI) [assign to "Question Text"]
    ├── AnswerGrid (Panel - for 2x2 layout)
    │   ├── AnswerButton1 (Button)
    │   │   └── Text (TextMeshProUGUI) [assign to "Answer Texts[0]"]
    │   ├── AnswerButton2 (Button)
    │   │   └── Text (TextMeshProUGUI) [assign to "Answer Texts[1]"]
    │   ├── AnswerButton3 (Button)
    │   │   └── Text (TextMeshProUGUI) [assign to "Answer Texts[2]"]
    │   └── AnswerButton4 (Button)
    │       └── Text (TextMeshProUGUI) [assign to "Answer Texts[3]"]
    └── FeedbackText (TextMeshProUGUI) [assign to "Feedback Text"]

**Panel settings:**
- Minigame Window → assign to self (MultipleChoiceMinigameUI Panel)
- Answer Buttons[0..3] → assign each button in order
```

---

## MathMinigameUI Hierarchy

```
Canvas
└── MathMinigameUI (Panel)
    ├── Background (Image) [assign to "Window Background"]
    ├── ProblemText (TextMeshProUGUI) [assign to "Problem Text"]
    │   └── Shows: "47 + 38 = ?"
    ├── InputDisplay (TextMeshProUGUI) [assign to "Input Text"]
    │   └── Shows: "125" or "_" (empty)
    └── FeedbackText (TextMeshProUGUI) [assign to "Feedback Text"]
        └── Shows: "Correct!" or "Incorrect!"

**Panel settings:**
- Minigame Window → assign to self
- Problem Text → drag TMP text component
- Input Text → drag TMP text component
- Feedback Text → drag TMP text component
```

---

## PhotoRevealMinigameUI Hierarchy

### Step 1: Create TilePrefab first

```
TilePrefab (GameObject) [PREFAB - save to Assets/Prefabs/]
├── Image component
│   - raycastTarget: TRUE
│   - color: solid color (will be overridden at runtime)
└── EventTrigger component
    └── (Auto-configured by PhotoRevealMinigameUI script)
```

### Step 2: Create PhotoRevealMinigameUI Panel

```
Canvas
└── PhotoRevealMinigameUI (Panel)
    ├── Background (Image) - optional, for styling
    ├── ImageContainer (Panel) [assign to "Image Container"]
    │   └── [GridLayoutGroup attached to this Panel]
    │       ├── GridLayout Settings:
    │       │   - Cell Size: 50, 50
    │       │   - Spacing: 2, 2
    │       │   - Constraint: Fixed Column Count = 6
    │       │   - Child Force Expand: unchecked
    │       └── (Tiles generated at runtime - no manual children needed)
    └── ProgressText (TextMeshProUGUI) [assign to "Progress Text"]
        └── Shows: "52% revealed"

**Panel settings:**
- Minigame Window → assign to self
- Image Container → drag the Panel with GridLayoutGroup
- Progress Text → drag TMP text component
- Tile Prefab → drag the TilePrefab you created
- Reveal Sprite → assign an office-themed image (e.g., document, file, server)
```

---

## Quick Setup Steps

### 1. MultipleChoiceMinigameUI
1. Right-click in Hierarchy → UI → Panel
2. Rename to "MultipleChoiceMinigameUI"
3. Add TextMeshPro text for question
4. Create AnswerGrid panel (GridLayoutGroup 2 cols, 2 rows)
5. Add 4 Buttons with TextMeshPro children
6. Add feedback text below
7. Add Image component to root panel
8. Attach MultipleChoiceMinigameUI script
9. Drag all UI elements to Inspector fields

### 2. MathMinigameUI
1. Right-click in Hierarchy → UI → Panel
2. Rename to "MathMinigameUI"
3. Add TextMeshPro text for problem display
4. Add TextMeshPro text for input display
5. Add TextMeshPro text for feedback
6. Add Image component to root panel
7. Attach MathMinigameUI script
8. Drag all UI elements to Inspector fields

### 3. PhotoRevealMinigameUI
1. **Create Tile Prefab first:**
   - Create empty GameObject "TilePrefab"
   - Add Image component (raycastTarget ON)
   - Add EventTrigger component
   - Save as prefab: Assets/Prefabs/TilePrefab.prefab

2. **Create PhotoRevealMinigameUI panel:**
   - Right-click in Hierarchy → UI → Panel
   - Rename to "PhotoRevealMinigameUI"
   - Add child Panel → GridLayoutGroup (6 columns)
   - Add TextMeshPro text for progress
   - Attach PhotoRevealMinigameUI script
   - Drag ImageContainer, ProgressText, TilePrefab, and Reveal Sprite to Inspector

---

## Inspector Field Mapping

### MultipleChoiceMinigameUI Script
```
Minigame Window:        MultipleChoiceMinigameUI (Panel)
Question Text:          QuestionText (TextMeshPro)
Answer Buttons[0]:      AnswerButton1
Answer Buttons[1]:      AnswerButton2
Answer Buttons[2]:      AnswerButton3
Answer Buttons[3]:      AnswerButton4
Answer Texts[0]:        AnswerButton1 > Text
Answer Texts[1]:        AnswerButton2 > Text
Answer Texts[2]:        AnswerButton3 > Text
Answer Texts[3]:        AnswerButton4 > Text
Feedback Text:          FeedbackText (TextMeshPro)
Window Background:      Background (Image)
```

### MathMinigameUI Script
```
Minigame Window:        MathMinigameUI (Panel)
Problem Text:           ProblemText (TextMeshPro)
Input Text:             InputDisplay (TextMeshPro)
Feedback Text:          FeedbackText (TextMeshPro)
Window Background:      Background (Image)
```

### PhotoRevealMinigameUI Script
```
Minigame Window:        PhotoRevealMinigameUI (Panel)
Image Container:        ImageContainer (Panel with GridLayoutGroup)
Progress Text:          ProgressText (TextMeshPro)
Tile Prefab:            TilePrefab (from Assets/Prefabs/)
Reveal Sprite:          <Your office-themed image>
Tile Count X:           6
Tile Count Y:           6
Reveal Threshold:       0.8
```

---

## Styling Notes

- All panels should have a dark semi-transparent background
- Button text should be white/light for contrast
- Feedback text should be green for "Correct!" and red for "Incorrect!"
- Photo reveal tiles should be small squares (adjust cell size as needed)
- All minigame panels should be **hidden by default** (unchecked)
