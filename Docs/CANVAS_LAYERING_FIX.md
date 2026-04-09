# Canvas Layering Fix Summary

## Issue
Minigame UI (typing window) and expanded ticket UIs were not rendering on top of other scene elements due to Canvas sort order issues.

## Solution Implemented

### 1. TypingMinigameUI Prefab (`Assets/Prefabs/TypingMinigameUI.prefab`)

**Changed:** Added Canvas component to the root TypingMinigameUI GameObject with overlay rendering configuration.

**Canvas Settings:**
- **Render Mode:** Screen Space - Overlay (m_RenderMode: 0)
- **Override Sorting:** Enabled (m_OverrideSorting: 1)
- **Sort Order:** 100 (m_SortingOrder: 100)
- **Component ID:** 9999999999999999 (added to components list)

**Why:** The typing minigame window was previously just a UI element without its own Canvas, so it rendered with the default root Canvas sort order (0). Adding a Canvas with sortOrder=100 ensures it always appears on top.

---

### 2. SetupAllAssets Script (`Assets/Scripts/SetupAllAssets.cs`)

**Changed:** Added minigame UI instantiation during scene asset setup.

**New Features:**
- Added `typingMinigamePrefab` serialized field (assign the TypingMinigameUI prefab in Inspector)
- New `SetupMinigameUI()` method that:
  - Checks if prefab is assigned
  - Verifies minigame UI doesn't already exist
  - Instantiates prefab as child of main Canvas
  - Enforces Canvas sortOrder=100
  - Provides debug logs for verification

**Called From:** `CreateAllAssets()` method (runs on scene setup)

---

### 3. TypingMinigameUI Script (`Assets/Scripts/TypingMinigameUI.cs`)

**Changed:** Added runtime Canvas configuration in Awake method.

**New Implementation:**
- Added `minigameCanvas` field to store Canvas reference
- In Awake():
  - Gets Canvas component from gameObject
  - Sets sortOrder to 100 and overrideSorting to true
  - Provides fallback warning if Canvas not found

**Why:** Ensures Canvas settings are enforced at runtime, even if prefab settings change or get overridden.

---

### 4. Ticket Script (`Assets/Scripts/Ticket.cs`)

**Changed:** Enhanced expand/collapse behavior to ensure expanded tickets appear on top.

**New Feature:** In `ToggleExpand()` method:
- When expanding (isExpanded = false → true), calls `transform.SetAsLastSibling()`
- This moves the ticket to the last child in hierarchy, ensuring it renders last/on top

**Why:** Uses hierarchy order as secondary rendering depth. Since all tickets share the same Canvas, being the last sibling ensures they render on top of other tickets.

---

## Canvas Hierarchy

### Before Fix:
```
Canvas (sortOrder: 0)
├── BulletinBoard
├── Printer
├── Shredder
├── TypingMinigameUI (no Canvas component)
│   └── TypingMinigameWindow
└── Tickets (sorted by hierarchy)
```

### After Fix:
```
Canvas (sortOrder: 0)
├── BulletinBoard
├── Printer
├── Shredder
├── TypingMinigameUI (Canvas with sortOrder: 100)
│   └── TypingMinigameWindow
└── Tickets (last sibling when expanded)

Rendering Order:
1. Canvas with sortOrder 0 (background elements)
2. TypingMinigameUI with Canvas sortOrder 100 (on top)
3. Expanded tickets (last sibling = last rendered = on top)
```

---

## Setup Instructions

### For Scene to Work Properly:

1. **Assign the Prefab:**
   - In the scene, find the GameObject with `SetupAllAssets` script
   - In the Inspector, assign `Assets/Prefabs/TypingMinigameUI.prefab` to the "Typing Minigame Prefab" field

2. **Verify in Scene:**
   - When scene starts, `SetupAllAssets.CreateAllAssets()` will:
     - Create all UI elements
     - Instantiate TypingMinigameUI with proper Canvas layering
     - Log "✓ TypingMinigameUI Canvas sortOrder set to 100"

3. **Test Rendering:**
   - Start typing minigame: Window should appear on top of all UI
   - Expand a ticket: Should appear above other tickets and game elements
   - Minigame should always be on top of expanded tickets (sortOrder 100 > sibling depth)

---

## Technical Details

### Canvas Sorting in Unity

1. **Multiple Canvas Components:** Each Canvas maintains its own sortingOrder
2. **Override Sorting:** When enabled, a Canvas's sortOrder is absolute (not relative to parent)
3. **Sibling Order:** Within same Canvas, last sibling = rendered last = appears on top
4. **Screen Space - Overlay Mode:** Renders directly to screen at specified sort order (no world-space)

### Sort Order Values Used:
- **Root Canvas:** 0 (default, background UI)
- **Typing Minigame Canvas:** 100 (overlay, always on top)
- **Expanded Tickets:** Achieved via sibling order, not sort order value

---

## Files Modified

1. `/Assets/Prefabs/TypingMinigameUI.prefab`
   - Added Canvas component

2. `/Assets/Scripts/SetupAllAssets.cs`
   - Added minigame prefab field
   - Added SetupMinigameUI() method

3. `/Assets/Scripts/TypingMinigameUI.cs`
   - Added Canvas configuration in Awake()

4. `/Assets/Scripts/Ticket.cs`
   - Enhanced ToggleExpand() with SetAsLastSibling()

---

## Verification Checklist

- [x] TypingMinigameUI prefab has Canvas component with sortOrder=100
- [x] Canvas has m_OverrideSorting enabled
- [x] SetupAllAssets instantiates minigame UI during scene setup
- [x] TypingMinigameUI script enforces Canvas settings at runtime
- [x] Expanded tickets use hierarchy order for depth
- [x] All scripts have valid syntax (brace balance checked)

---

## Expected Behavior

1. **Minigame Window:**
   - Always renders on top of all other UI
   - Canvas sortOrder=100 ensures overlay behavior
   - Visible even when tickets are expanded

2. **Expanded Tickets:**
   - Appear above other tickets due to sibling order
   - Still appear below minigame window (sortOrder 100 > sibling depth)
   - Return to normal position when collapsed

3. **Background UI:**
   - Bulletin board, printer, shredder remain at default depth
   - Can be covered by expanded tickets
   - Covered by minigame window when active

