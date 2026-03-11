# Basic Tower Defense - Development Epics

## Epic Overview

| # | Epic Name | Dependencies | Est. Stories |
|---|---|---|---|
| 1 | Game Foundation | None | 5 |
| 2 | Enemy Path System | Epic 1 | 4 |
| 3 | Tower Placement | Epic 1 | 5 |
| 4 | Combat System | Epics 2, 3 | 6 |
| 5 | Economy & Upgrades | Epic 4 | 5 |
| 6 | Wave System | Epic 5 | 5 |
| 7 | Polish & Completion | Epic 6 | 5 |

---

## Epic 1: Game Foundation

### Goal

Establish the engine, grid map, core game loop, and end screens so all subsequent
epics have a working foundation to build on.

### Scope

**Includes:**
- Engine/framework setup and project structure
- Grid map rendering (path tiles + buildable tiles)
- Main game screen (no gameplay yet)
- Game over screen (triggered by 0 lives)
- Win screen (triggered by wave 25 completion)
- Restart functionality from both screens

**Excludes:**
- Any enemies, towers, or gameplay systems
- UI beyond basic screen layouts

### Dependencies

None — this is the foundation.

### Deliverable

A running game window showing a grid map with a path. Game over and win screens
navigate correctly. Nothing else works yet.

### Stories

- As a player, I can launch the game and see the grid map rendered
- As a player, I can see path tiles and buildable tiles visually distinct on the grid
- As a player, I see a game over screen when the game over condition is triggered
- As a player, I see a win screen when the win condition is triggered
- As a player, I can restart the game from the game over or win screen

---

## Epic 2: Enemy Path System

### Goal

Enemies spawn, follow the fixed path, and cost the player a life when they reach
the end — establishing the core threat system.

### Scope

**Includes:**
- Enemy spawning at path start
- Enemy movement along the fixed path
- Lives system (3 lives, displayed in UI)
- Game over trigger when all lives lost
- Basic enemy visual (placeholder shape)

**Excludes:**
- Wave management (enemies spawn in a test loop for now)
- Tower interaction — enemies are invincible in this epic
- Enemy stat scaling

### Dependencies

Epic 1 (grid map and game screens)

### Deliverable

Enemies march down the path and cost lives. Game over fires at 0 lives.
No towers yet — the path is fully undefended.

### Stories

- As a player, I see enemies spawn at the start of the path and move along it
- As a player, I lose a life when an enemy reaches the end of the path
- As a player, I can see my current lives count in the UI
- As a player, the game ends when all 3 lives are lost

---

## Epic 3: Tower Placement

### Goal

The player can select tower types, preview range, and place towers on buildable tiles —
establishing the core placement mechanic before combat is wired up.

### Scope

**Includes:**
- Tower type selection UI panel (all 4 types visible)
- Placement mode: range preview on hover, left click to place
- Placement validation (buildable tiles only, one per tile)
- Cancel placement with right click
- Currency display (hardcoded starting amount for testing)
- Tower visual on placed tiles (placeholder shapes)

**Excludes:**
- Towers actually doing anything (no targeting, no firing)
- Upgrade or sell mechanics
- Real currency system (hardcoded values for testing)

### Dependencies

Epic 1 (grid map)

### Deliverable

The player can place towers on the map. Range preview works. Invalid placements
are blocked. Towers appear visually but do nothing yet.

### Stories

- As a player, I can select a tower type from the UI panel
- As a player, I see a range preview when hovering over a valid tile in placement mode
- As a player, I can place a tower on any valid buildable tile with a left click
- As a player, I cannot place a tower on a path tile or an already-occupied tile
- As a player, I can cancel placement mode with a right click

---

## Epic 4: Combat System

### Goal

Towers detect enemies, fire projectiles, deal damage, and all 4 tower types work
correctly — completing the core gameplay loop.

### Scope

**Includes:**
- Tower targeting logic (detect enemies in range)
- Configurable targeting priority per tower (First, Last, Nearest, Strongest, Weakest)
- Projectile system (towers fire at targets)
- Enemy health and damage (enemies die when health reaches 0)
- All 4 tower types implemented with correct stats:
  - Fast/Weak: high fire rate, low damage
  - Slow/Heavy: low fire rate, high damage
  - AOE: ground explosion, area damage, high fire rate
  - Slow: no damage, reduces enemy movement speed
- Enemy and tower stats (placeholder values, tuned in Epic 7)

**Excludes:**
- Upgrade stats (towers work at Level 1 only)
- Wave management (enemies still spawn in test loop)
- Economy (currency still hardcoded)

### Dependencies

Epics 2 (enemies) and 3 (tower placement)

### Deliverable

**Vertical slice complete.** Enemies walk the path, towers shoot them, enemies die
or reach the end. All 4 tower types functional. Core gameplay loop proven.

### Stories

- As a player, placed towers automatically target enemies within their range
- As a player, I can configure the targeting priority for each placed tower
- As a player, I see towers fire projectiles at their targeted enemy
- As a player, enemies take damage from projectiles and are destroyed at 0 health
- As a player, all 4 tower types work with their correct attack roles
- As a player, the Slow tower reduces the movement speed of enemies in range

---

## Epic 5: Economy & Upgrades

### Goal

The full currency loop works — earning, spending on placements, upgrading towers
to L3, and selling for partial refund.

### Scope

**Includes:**
- Currency earned at end of each wave (fixed amounts, tuned in Epic 7)
- Real placement costs deducted from currency
- Tower upgrade system (Level 1 → 2 → 3) with stat improvements per type
- Sell mechanic (75% refund of placement cost)
- Full UI: currency display, tower costs, upgrade costs, sell value
- Insufficient currency prevents placement/upgrade

**Excludes:**
- Wave difficulty scaling (waves still simple for testing)
- Balance tuning (costs and rewards are placeholder)

### Dependencies

Epic 4 (fully working combat system)

### Deliverable

The player earns currency, buys and upgrades towers, and can sell for partial refund.
The economic decision loop (place vs. upgrade vs. sell) is fully functional.

### Stories

- As a player, I receive currency at the end of each wave
- As a player, placing a tower deducts its cost from my currency
- As a player, I can upgrade a placed tower through 3 levels, each improving its stats
- As a player, I can sell a placed tower for 75% of its placement cost
- As a player, I can see current currency, tower costs, upgrade costs, and sell values in the UI

---

## Epic 6: Wave System

### Goal

All 25 waves are defined, difficulty scales correctly, boss waves fire at every 5th
wave, and the manual wave launch flow works end-to-end.

### Scope

**Includes:**
- Manual wave launch button (inter-wave planning phase)
- 25 distinct wave definitions (enemy count, speed, health per wave)
- Difficulty scaling across all 25 waves (quantity + speed + health mix)
- Boss wave logic (waves 5, 10, 15, 20, 25) — single high-stat enemy
- Starting currency set to enough for 1 tower
- Currency earned per wave scales with wave number
- Win condition triggers after wave 25

**Excludes:**
- Final balance tuning (covered in Epic 7)
- Any sprites or visual polish

### Dependencies

Epic 5 (full economy and upgrade loop)

### Deliverable

A complete 25-wave run is playable start to finish. Every system is connected.
The game is finishable (and loseable). Balance is rough but functional.

### Stories

- As a player, I can launch the next wave manually when I'm ready
- As a player, each wave is harder than the last (more enemies, faster, tankier)
- As a player, every 5th wave features a single boss enemy with high health and speed
- As a player, I start with enough currency to place 1 tower
- As a player, completing wave 25 triggers the win screen

---

## Epic 7: Polish & Completion

### Goal

All sprites are in, UI is clean, and the game is balanced to the "barely surviving"
target across all 25 waves.

### Scope

**Includes:**
- All tower sprites (4 types × 3 upgrade levels = 12 sprites)
- Enemy sprite and boss enemy sprite
- Projectile sprites (4 types)
- Path and buildable tile sprites
- UI polish (clean layout, readable fonts, clear visual hierarchy)
- Full balance pass: wave scaling, tower costs, upgrade costs, currency earn rates
- Boss wave tuning (each boss wave feels like a genuine spike)
- End-to-end playtest: waves 20–25 punish bad placement

**Excludes:**
- Audio (deferred entirely from v1)
- Any new gameplay features

### Dependencies

Epic 6 (complete 25-wave system)

### Deliverable

**Shipped v1.** A visually complete, balanced, fully playable tower defense game.
Hard to beat. Clean to look at. Done.

### Stories

- As a player, I see proper sprite assets for all towers, enemies, and projectiles
- As a player, the UI is clean, readable, and shows all critical information
- As a player, wave difficulty is balanced so "barely surviving" is the target experience
- As a player, boss waves at levels 5, 10, 15, 20, 25 are noticeably harder spikes
- As a player, late waves (20–25) punish poor tower placement decisions made earlier
