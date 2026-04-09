# While You Were Busy — CLAUDE.md

## Tools & MCP
- **ALWAYS use Unity MCP** for this project: https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
- **Use the unity-mcp-skill** for Unity Editor automation and GameObject/scene management

## Minigame Order

| Day | Minigame       | Status      | Script                    |
|-----|----------------|-------------|---------------------------|
| 1   | Email Typing   | IMPLEMENTED | TypingMinigameUI.cs       |
| 2   | Riddle         | PLACEHOLDER | PlaceholderMinigameUI.cs  |
| 3   | Math Input     | PLACEHOLDER | PlaceholderMinigameUI.cs  |
| 4   | Photo Reveal   | PLACEHOLDER | PlaceholderMinigameUI.cs  |
| 5   | Connect Dots   | PLACEHOLDER | PlaceholderMinigameUI.cs  |

## Day Rules
- Each day = 1 minute 30 seconds (90 seconds)
- Display time: 8:00 AM to 8:00 PM (mapped to 90-second countdown)
- Ticket spawn: Every 15 seconds (6 tickets per day)
- Quota: Must complete 4 typing minigames per day
- Win: Complete 4 minigames before 90 seconds expire → advance to next day
- Lose: 90 seconds expire with <4 minigames completed
- Day 5 loss = narrative ending (player revealed as AI)

## Core Scripts
- **GameManager.cs** — core loop, day timer (2 min), win/lose checks
- **Printer.cs** — spawns tickets at day-based intervals
- **BulletinBoard.cs** — auto-snaps tickets, tracks completion
- **Ticket.cs** — click to start minigame, auto-completes on finish
- **TaskDatabase.cs** — tasks gated by day/minigame type (enum-based)
- **DayManager.cs** — upgrade modal, Day 5 ending
- **UpgradeManager.cs** — tracks purchased upgrades
- **PlaceholderMinigameUI.cs** — instant-complete stub for Days 2-5

## TODO: Real Minigame Scripts (when ready)
- RiddleMinigameUI.cs
- MathMinigameUI.cs
- PhotoRevealMinigameUI.cs
- ConnectDotsMinigameUI.cs
