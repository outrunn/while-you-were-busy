# Typing Tasks - 60 Second Quick Start

## 1. Create Sample Tasks (10 seconds)

1. Unity menu → **Tools → Typing Task Creator**
2. Click **"Create Sample Easy Tasks (5)"**
3. Click **"Create Sample Medium Tasks (5)"**
4. Click **"Create Sample Hard Tasks (5)"**

**Result:** 15 typing task assets created in `Assets/ScriptableObjects/TypingTasks/`

---

## 2. Add Tasks to Database (20 seconds)

1. Select **TypingTaskDatabase** GameObject in scene
2. In Inspector, click **"Select All Typing Task Assets in Project"**
3. Click **"Add Selected Assets to List"**
4. **UNCHECK** "Create Sample Tasks At Runtime"

**Result:** Database now has 15 custom tasks, runtime samples disabled

---

## 3. Test It! (30 seconds)

1. Press **Play**
2. Wait for ticket to print (or click Print button)
3. **Drag ticket to bulletin board**
4. Click **"START TASK"** button on ticket
5. **Spam keyboard** to type the message
6. Window auto-closes when done
7. Ticket is auto-stamped
8. **Drag stamped ticket to Processing Machine**
9. Get rewards!

**Result:** Full typing workflow working with custom messages!

---

## Creating Your Own Tasks

**Quick Method:**
1. Tools → Typing Task Creator
2. Fill in title, description, message
3. Click "Create Typing Task"
4. It auto-adds to the database

**Manual Method:**
1. Right-click Project → Create → Minigames → Typing Task
2. Fill in Inspector
3. Add to TypingTaskDatabase list

---

## Message Length Guide

- **Easy (10-15 pts):** 50-150 characters
- **Medium (55-70 pts):** 150-400 characters
- **Hard (250-300 pts):** 400+ characters

---

## That's It!

The system is now modular and easy to expand. Just create new typing task assets and they automatically become available in the game.

**Full documentation:** See TYPING_PROMPTS_GUIDE.md
