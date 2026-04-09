# Approval Stamp System

## How It Works

The approved stamp **no longer exists in the scene**. Instead, it appears **dynamically** as an animated overlay when a ticket is successfully completed.

## Flow

1. **Player completes minigame** on a ticket
2. **Ticket.StampTicket()** is called
3. **TicketCompletionStamp** creates a temporary stamp GameObject
4. Stamp **animates over the ticket** with:
   - Scale-down animation (1.5x → 0.8x)
   - Rotation (720° per second)
   - Fade-out effect
5. Stamp **automatically destroys** after animation

## Components

### TicketCompletionStamp.cs
- Manages stamp creation and animation
- Spawns stamp dynamically (not in scene)
- Handles all visual effects
- Auto-cleanup after animation

### Updated Ticket.cs
- Calls `TicketCompletionStamp.ShowStampAtTicket()` when stamped
- Falls back to old system if stamp animator not found

## Setup

1. **Select your Canvas**
2. **Add TicketCompletionStamp script** to it
3. **In Inspector**, assign the **Stamp Sprite** (Rectangle 45.png)
4. **Done!** - Stamps will now appear on ticket completion

## Customization

In **TicketCompletionStamp** inspector, adjust:
- `Stamp Duration` - How long animation lasts
- `Scale Start` - Initial stamp size multiplier
- `Scale Final` - Final stamp size multiplier
- `Rotation Speed` - How fast it spins
- `Stamp Size` - Dimensions of stamp

## Result

✅ Clean scene - no unnecessary objects
✅ Professional animation effect
✅ Stamps appear only on completion
✅ Automatic cleanup (no memory leaks)

