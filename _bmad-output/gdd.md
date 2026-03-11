---
stepsCompleted: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14]
inputDocuments:
  - '_bmad-output/game-brief.md'
documentCounts:
  briefs: 1
  research: 0
  brainstorming: 0
  projectDocs: 0
workflowType: 'gdd'
lastStep: 0
project_name: 'basic-game'
user_name: 'Nate'
date: '2026-02-27'
game_type: 'tower-defense'
game_name: 'Basic Tower Defense'
---

# Basic Tower Defense - Game Design Document

**Author:** Nate
**Game Type:** Tower Defense
**Target Platform(s):** PC (Desktop, Standalone Executable)

---

## Executive Summary

### Game Name

Basic Tower Defense

### Core Concept

Basic Tower Defense is a clean, no-frills implementation of the classic tower defense formula.
Players place specialized towers on any non-path tile to intercept waves of enemies marching
along a fixed path. With only 3 lives and enemies tuned to be a genuine threat from the start,
every placement decision matters.

Four tower types — Fast/Weak, Slow/Heavy, AOE, and Slow — each fill a distinct combat role.
Players earn currency per wave and spend it on placing new towers or upgrading existing ones.
The core loop is earn, place, survive, repeat — escalating until you either outlast all waves
or run out of lives.

The design philosophy is "barely surviving": waves are balanced to punish passive play and
reward strategic tower positioning. Success comes from smart composition and placement, not
just spending efficiently.

### Game Type

**Type:** Tower Defense
**Framework:** This GDD uses the tower-defense template with type-specific sections covering
tower types and upgrades, enemy wave design, path and placement strategy, economy and
resources, and difficulty systems.

### Target Audience

Solo developer (Nate) as both developer and player. Core/hardcore gamer, fully
genre-literate — no onboarding needed. Medium-to-long sessions spanning many levels.

### Unique Selling Points (USPs)

1. **Genuine Difficulty** — Waves are tuned to punish passive or careless play. If a wave
   is too easy, the root cause is investigated and corrected. Hard to beat by design.
2. **Clean Implementation** — No hacks, no shortcuts, nothing that doesn't earn its place.
   Code is structured and readable. A design principle as much as a technical one.

---

## Target Platform(s)

### Primary Platform

PC (Desktop) — standalone executable, offline only. No launcher or distribution platform
required at this stage.

### Platform Considerations

- **Resolution:** 2560x1440 default target
- **Framerate:** 60fps minimum; 144fps target where achievable
- **Networking:** None — fully offline, no online features
- **Distribution:** Standalone executable (no Steam or storefront dependency)

### Control Scheme

Mouse-only. All gameplay interactions — tower placement, upgrades, wave management — handled
via mouse clicks. No keyboard input required for core gameplay.

---

## Target Audience

### Demographics

Solo developer (Nate) as both developer and player. This is a personal learning project
with no external audience.

### Gaming Experience

**Core/Hardcore** — The player is an experienced gamer comfortable with strategic depth,
resource optimization, and challenge-tuned gameplay.

### Genre Familiarity

**Fully genre-literate.** No tutorial or genre onboarding needed. Tower defense conventions
(pathing, placement, wave escalation, currency loops) are assumed knowledge.

### Session Length

**Medium-to-long sessions.** Individual levels last several minutes each; a full run spans
many levels. Expected play session: 30–90+ minutes.

### Player Motivations

Learning game development through hands-on implementation of a well-understood genre.
The "audience" is the developer — success means a complete, functional, genuinely
challenging game, not a polished or accessible product.

---

## Goals and Context

### Project Goals

1. **Learn game development** — Gain practical, hands-on understanding of game design
   and implementation by building a complete project from start to finish.
2. **Experience the BMAD process** — Work through the full BMAD planning and development
   workflow on a real project.
3. **Ship something complete** — Deliver a finished, end-to-end playable game. Not
   abandoned mid-build. The core loop works, waves escalate, win/lose conditions fire.
4. **Build genuine challenge** — Produce a tower defense that is hard to beat. Waves are
   tuned to require strategic placement; if something is too easy, we investigate and fix
   it — not patch over it.

### Background and Rationale

Personal learning project driven by curiosity about game development and a desire to
experience the BMAD workflow end-to-end. Tower defense was chosen deliberately: the genre
is well-understood, the rules are established, and there is a clear, definable "done" state.
This lets the focus stay on implementation and process rather than design discovery.

---

## Unique Selling Points (USPs)

1. **Genuine Difficulty** — Waves are tuned to punish passive or careless play. If a wave
   is too easy, the root cause is investigated and corrected. The game is hard to beat on
   first attempt by design, not accident.

2. **Clean Implementation** — No hacks, no shortcuts, nothing that doesn't earn its place.
   Code is structured and readable. This commitment exists both as a learning goal and as a
   design principle: a well-built foundation makes every future fix and feature honest work.

### Competitive Positioning

Not a commercial product. As design commitments: where most tower defense games optimize
for accessibility and player comfort, Basic Tower Defense optimizes for challenge and
implementation integrity. These two commitments are the guardrails for every design decision.

---

## Core Gameplay

### Game Pillars

1. **Clean Implementation** — Well-structured code and gameplay. No extraneous systems.
   Every mechanic, feature, and line of code must earn its place. Nothing exists just because
   it's typical for the genre.

2. **Challenging Difficulty** — Enemies are a genuine threat from early waves onward.
   Waves are tuned to punish passive or careless placement. Players must think strategically
   to survive.

**Pillar Prioritization:** When pillars conflict, prioritize in this order:
- Design decisions → Challenging Difficulty wins
- Technical decisions → Clean Implementation wins

### Core Gameplay Loop

Players alternate between two phases each wave:

**Inter-Wave Phase (Planning):**
Player has free time to assess the battlefield, place new towers, upgrade existing towers,
or sell towers for a partial refund. No time pressure — this is the strategic layer.

**Wave Phase (Execution):**
Enemies march along the fixed path. Towers fire automatically. Player watches, adapts if
possible, and holds the line.

**Loop Diagram:**

```
Earn Currency (wave end)
        ↓
  Inter-Wave Phase
  [Place / Upgrade / Sell Towers]
        ↓
   Launch Wave
  [Enemies March / Towers Fire]
        ↓
  Survive? → Yes → Earn Currency (repeat, harder)
            → No (life lost) → Lives remaining? → Yes → Continue wave
                                                → No  → GAME OVER
```

**Loop Timing:** Inter-wave phase: player-controlled duration.
Wave phase: several minutes per wave. Full run: many waves across a long session.

**Loop Variation:** Each wave escalates — more enemies, faster enemies, or higher health.
Tower composition and placement decisions compound over time, making each iteration
meaningfully different from the last.

### Win/Loss Conditions

#### Victory Conditions

Survive all waves. No bonus objectives, no star ratings, no lives-remaining scoring.
Finishing is the only success state.

#### Failure Conditions

Losing all 3 lives triggers an immediate game over. Each enemy that reaches the end of
the path costs one life. No mid-run checkpoints — failure sends the player back to the
very beginning.

#### Failure Recovery

None — game over is total. This is intentional: the high stakes of a full restart reinforce
the "barely surviving" design philosophy. Every wave matters because failure is always
permanent.

---

## Game Mechanics

### Primary Mechanics

1. **Place Tower**
   - Spend currency to place a tower on any non-path grid tile
   - Free-form placement — any valid tile qualifies, no designated build zones
   - One tower per tile; stacking is not permitted
   - While in placement mode, tower range/coverage is visualized on the map
   - Serves pillar: *Challenging Difficulty* (placement positioning is the core skill)

2. **Upgrade Tower**
   - Click a placed tower to open its action panel; spend currency to upgrade
   - Linear 3-tier upgrade path: Level 1 → Level 2 → Level 3
   - Each upgrade improves the tower's core stats (damage, range, or fire rate
     depending on tower type)
   - No branching upgrade paths — one upgrade line per tower type
   - Serves pillar: *Clean Implementation* (simple, predictable progression)

3. **Sell Tower**
   - Click a placed tower and choose to sell it for a partial currency refund
   - Enables strategic pivoting — recoup some investment to reallocate elsewhere
   - Refund amount is less than the original placement cost (exact ratio TBD in
     balance pass)
   - Serves pillar: *Challenging Difficulty* (selling at a loss creates real cost
     to poor early placement)

4. **Launch Wave**
   - Player manually triggers each wave when ready, ending the inter-wave planning phase
   - No time limit on the planning phase; no bonus for launching early
   - Once launched, the wave runs until all enemies are defeated or a life is lost
   - Serves pillar: *Challenging Difficulty* (player owns the pacing; no excuse for
     being unprepared)

### Mechanic Interactions

- **Currency** is the connective tissue: earned per wave completion, spent on Place
  and Upgrade, partially recovered by Sell. Every decision has a currency cost.
- **Place + Range Preview** creates the strategic skill layer: visualizing coverage
  before committing encourages thoughtful positioning.
- **Sell + Place** enables mid-run adaptation: if a tower isn't pulling its weight,
  you can sell at a loss and reposition — but the currency penalty keeps it costly.
- **Upgrade vs. Place** creates a recurring tension: is it better to strengthen an
  existing tower or add a new one? This is the primary strategic decision each wave.

### Mechanic Progression

No unlocks — all 4 tower types are available from the start. Progression comes through:
- Tower upgrades (Level 1 → 3 per tower)
- Player knowledge compounding across waves (no mechanical unlock, just skill)

---

## Controls and Input

### Control Scheme (PC — Mouse Only)

| Action | Input |
|---|---|
| Enter placement mode (select tower type) | Left click tower in UI panel |
| Place tower | Left click on valid tile |
| Cancel placement | Right click |
| Select placed tower (open upgrade/sell panel) | Left click on placed tower |
| Deselect / close panel | Right click |
| Preview tower range | Hover while in placement mode |
| Launch wave | Left click wave launch button |

### Input Feel

Precise and responsive. Mouse-only means every action is a deliberate click — no
accidental inputs from held keys. Placement confirmation should feel snappy; no
animation delay between click and tower appearing.

### Accessibility Controls

None planned. Personal project with a single known player.

---

## Tower Defense Specific Design

### Tower Types and Upgrades

All 4 tower types are available from the start. No unlocks — full roster day one.
Each tower has a linear 3-tier upgrade path. Costs TBD during balance testing.

| Tower | Role | Damage | Fire Rate | Range | Slow % |
|---|---|---|---|---|---|
| Fast/Weak | DPS finisher | Low | High | Base | — |
| Slow/Heavy | Core damage dealer | High | Low | Base | — |
| AOE | Wave clearing | High (area) | High | Base | — |
| Slow | Utility | None | — | Base | Yes |

**Upgrade Focus per Tower:**

- **Fast/Weak** — Range + Fire Rate increase per level
- **Slow/Heavy** — Damage increase per level
- **AOE** — Damage + Fire Rate increase per level (ground explosion, radius TBD)
- **Slow** — Slow % increase per level; no damage at any tier

**Targeting Priority:**
Each tower's targeting logic is player-configurable. Options:
- First (furthest along path)
- Last (closest to spawn)
- Nearest (closest to tower)
- Strongest (highest current health)
- Weakest (lowest current health)

Default targeting priority: TBD in balance pass.

### Enemy Wave Design

**Enemy Type:**
Single base enemy type that scales with each wave. No visual variants — enemies are
abstract geometric shapes. Scaling is a mix of three dimensions simultaneously:

- **Quantity** — more enemies per wave
- **Speed** — enemies move faster
- **Health** — enemies are tankier

Exact scaling formula TBD in balance testing, tuned toward genuine difficulty.

**Boss Waves:**
Boss waves occur at set intervals (interval TBD). A boss is a single enemy with
dramatically higher health and potentially higher speed than the wave's standard enemies.
No special boss abilities — just a significant health/speed spike that forces focused fire.

**Wave Pacing:**
Fully deterministic — all wave compositions and scaling values are fixed, not randomized.
Players can learn and plan across runs.

### Path and Placement Strategy

**Path Structure:**
Single fixed path per map. One map in the initial build. Path layout is predetermined —
players cannot modify or influence the path.

**Placement:**
- Free-form on any buildable (non-path) tile
- One tower per tile, no stacking
- Grid size: TBD
- Tile types: path (unbuildable) and buildable — no special terrain

**Range Visualization:**
Tower range/coverage is displayed while in placement mode. No range display on idle
placed towers unless selected.

**Strategic Space:**
Fixed path creates natural choke points. Tower placement relative to path curves and
straights is the core strategic decision space.

### Economy and Resources

**Single Currency System:**
One resource — currency. No secondary resources, no special currencies.

| Event | Currency Change |
|---|---|
| Start of game | Enough to place 1 tower (exact value TBD) |
| Wave completion | +X currency (amount scales TBD) |
| Tower placement | −cost (TBD per tower type) |
| Tower upgrade | −cost (TBD per tier) |
| Tower sell | +75% of original placement cost |

**Economic Strategy:**
Primary tension: place new towers vs. upgrade existing towers. Selling at 75% refund
creates a meaningful but not crippling penalty for repositioning.

### Abilities and Powers

None. No player-activated abilities in this iteration. Tower placement, upgrades, and
sells are the full extent of player actions. Abilities may be considered for a future
iteration once the core loop is solid.

### Difficulty and Replayability

**Difficulty:**
Single difficulty mode. No easy/normal/hard selection. The game is tuned to be
genuinely hard — that is the difficulty.

**Replayability:**
None by design. Finishing the game is the end state. No New Game+, no prestige,
no meta-progression, no challenge modifiers.

**Determinism:**
Fully deterministic. Wave compositions, enemy stats, and scaling are fixed values.
No randomized elements. Players who replay encounter identical conditions.

---

## Progression and Balance

### Player Progression

**Progression Type: Skill + In-Session Power**

Basic Tower Defense uses two complementary progression types:

1. **Skill Progression** — The player learns the path, enemy behavior, and optimal
   tower placement through repetition. No mechanical reward for this — just mastery.

2. **In-Session Power Progression** — Towers placed and upgraded during a run persist
   across all 25 levels. Currency accumulates wave by wave, enabling the player to expand
   and upgrade their tower network over time. By later levels, the player commands a full
   defensive setup built over the entire run.

**No Meta-Progression:**
Each run starts completely fresh — starting currency, no towers, no bonuses. Nothing
carries between runs.

#### Progression Pacing

- Level 1: Minimal resources, 1 tower possible, gentle enemy pressure
- Levels 1–5: Tower network establishment phase — limited resources, prioritize coverage
- Levels 6–15: Expansion phase — upgrading key towers, filling gaps in coverage
- Levels 16–24: Optimization phase — resources are meaningful but enemies are severe
- Level 25: Final boss wave — the ultimate test of the network built over the full run

### Difficulty Curve

**Pattern: Sawtooth with Exponential Trend**

Each level escalates enemy difficulty (quantity + speed + health mix). Boss waves at
every 5th level create deliberate spikes, followed by a slight relative relief before
the next escalation cycle begins again.

#### Challenge Scaling (per level)

| Level Range | Enemy Quantity | Enemy Speed | Enemy Health | Boss? |
|---|---|---|---|---|
| 1–4 | Low | Slow | Low | No |
| 5 | Low–Med | Med | High | Yes |
| 6–9 | Med | Med | Med | No |
| 10 | Med | Med–High | High | Yes |
| 11–14 | Med–High | High | Med–High | No |
| 15 | High | High | High | Yes |
| 16–19 | High | High | High | No |
| 20 | High | Very High | Very High | Yes |
| 21–24 | Very High | Very High | Very High | No |
| 25 | Very High | Extreme | Extreme | Yes (Final) |

Exact stat values TBD in balance testing. Tuning target: player should feel
"barely surviving" at each boss wave.

#### Difficulty Options

Single difficulty. No adjustments. The game is hard — that is the design.

### Economy and Resources

Defined in Tower Defense Specific Design (see above). Summary:

- **Single currency** — no secondary resources
- **Starting amount** — enough for 1 tower at level 1 (exact value TBD)
- **Earning** — fixed amount per level completion (TBD, scales with level)
- **Spending** — tower placement and upgrades (costs TBD)
- **Sell refund** — 75% of placement cost
- **Key tension** — upgrade existing towers vs. place new ones each level

---

## Level Design Framework

### Structure Type

**Linear Wave Sequence** — 25 waves played in order on a single fixed-path map.
No hub, no level select, no branching. The player progresses wave by wave from 1 to 25.
Completing wave 25 is the win condition.

### Level Types

**Standard Waves (1–4, 6–9, 11–14, 16–19, 21–24):**
Regular enemy waves with escalating quantity, speed, and health. These are the
building blocks of the run — earn currency, expand the tower network, survive.

**Boss Waves (5, 10, 15, 20, 25):**
Every 5th wave features a boss — a single enemy with dramatically higher health
and speed than the wave's standard enemies. No special boss abilities; the threat
is raw stats. Wave 25 is the final boss and win condition.

#### Tutorial Integration

None. The game starts immediately with Wave 1 and starting currency. No prompts,
no guided placement, no onboarding. The player is expected to know the genre.

#### Special Levels

No secret levels, bonus levels, or optional content. 25 waves — that's the game.

### Level Progression

**Linear Sequence — Survival Unlock:**
Surviving a wave unlocks the next wave. There is no other unlock mechanism.
Players who fail are sent back to Wave 1 with no towers and starting currency.

#### Unlock System

Implicit — surviving each wave advances the player to the next. No explicit unlock
screen, no stars, no ratings.

#### Replayability

Players can restart a run at any time (voluntarily or after game over). Each restart
begins at Wave 1 with starting currency and no towers. Content is identical every run
(fully deterministic).

### Level Design Principles

1. **Later waves punish bad placement** — Early waves are forgiving enough that most
   reasonable tower positions will work. As waves progress, suboptimal placement
   (poor choke point coverage, wrong tower type for the threat) becomes increasingly
   costly. By waves 20–25, placement errors from earlier in the run should become
   visible liabilities.

2. **The path creates natural choke points** — The map path is designed with curves
   and tight segments that reward towers placed at key coverage positions. Players
   who identify and control choke points will outperform those who spread towers
   evenly.

---

## Art and Audio Direction

### Art Style

**Style: Clean Geometric / Abstract**

All game elements — towers, enemies, path, and UI — are represented as simple geometric
shapes. No character art, no decorative elements, no thematic world-building. Visual
clarity and functional differentiation are the only goals.

#### Visual References

No specific reference titles. Target aesthetic: clean abstract shapes with strong
color differentiation. Clarity over decoration in every visual decision.

#### Color Palette

Three distinct color families, never overlapping:

- **Enemies** — one color family (e.g., red/orange tones)
- **Towers** — another color family (e.g., blue/teal tones)
- **Path** — clearly distinct from both (e.g., neutral grey/brown)
- **Buildable tiles** — background contrast that doesn't compete with towers or enemies

Exact palette TBD during implementation. Principle: any element should be immediately
identifiable by color alone, even at a glance.

#### Camera and Perspective

**Top-down 2D** — camera looks straight down at the grid. No isometric angle, no
perspective distortion. The full map is visible at all times; no camera movement or
zoom required (though zoom may be considered if grid size demands it).

#### Asset Complexity

- Towers: static shapes, no idle animation
- Enemies: moving shapes, no complex animation — movement along path only
- Projectiles: simple shapes (dots, lines) traveling from tower to enemy
- UI: minimal — currency display, lives counter, wave indicator, tower panel

### Audio and Music

**None — deferred entirely.**

No music, no sound effects, no UI audio, no feedback sounds in this iteration.
Audio is out of scope for v1. The game runs in silence. This keeps scope focused
on core mechanics and eliminates an entire production discipline from the initial build.

#### Voice/Dialogue

None. No narrative content requiring audio.

### Aesthetic Goals

- **Supports Clean Implementation** — geometric art requires minimal asset tooling,
  no artist workflow, and can be produced entirely in code or with basic drawing tools
- **Supports Challenging Difficulty** — clear color differentiation ensures the player
  can always read the battlefield; no visual noise interfering with strategic decisions
- **Scope-appropriate** — solo developer, no art budget, no audio budget; this style
  is honest about those constraints and makes them a feature

---

## Technical Specifications

### Performance Requirements

#### Frame Rate Target

- **Minimum:** 60fps
- **Target:** 144fps where achievable
- Priority: frame rate over visual fidelity — the game should never drop below 60fps
  on target hardware.

#### Resolution Support

- **Default:** 2560x1440
- Windowed and fullscreen modes expected; exact resolution options TBD during
  implementation.

#### Load Times

No specific target. Game is simple enough that load times should be negligible.
No streaming, no large assets, no network calls.

### Platform-Specific Details

#### PC Requirements

- **Distribution:** Standalone executable — no launcher, no storefront dependency
- **Networking:** None — fully offline
- **Minimum Hardware:** Any decent modern PC (no specific spec floor)
- **OS:** TBD during implementation (likely Windows primary)
- **Input:** Mouse only — no controller, no keyboard input required for core gameplay
- **Features:** No cloud saves, no achievements, no mod support, no DRM

### Asset Requirements

#### Art Assets

Sprite-based (PNG files), AI-generated. Geometric abstract style keeps asset count
low and individual assets simple.

**Estimated asset list:**

| Asset | Count |
|---|---|
| Tower sprites (4 types × 3 upgrade levels) | 12 |
| Enemy sprite | 1 |
| Boss enemy sprite | 1 |
| Projectile sprites (1 per tower type) | 4 |
| Path tile | 1 |
| Buildable tile | 1 |
| UI elements (buttons, panels, icons) | TBD |

Total sprite count: small. All assets produced via AI image generation by the
solo developer. No external asset store dependencies.

#### Audio Assets

None. Audio is deferred entirely from v1.

#### External Assets

None planned. All assets self-produced.

### Technical Constraints

- Offline only — no network calls, no backend, no telemetry
- Solo developer — no build pipeline, CI/CD, or team tooling required
- No localization — English only, single known player
- Engine/framework: TBD in Architecture workflow

---

## Development Epics

### Epic Overview

| # | Epic Name | Scope | Dependencies | Est. Stories |
|---|---|---|---|---|
| 1 | Game Foundation | Engine, grid, game loop, screens | None | 5 |
| 2 | Enemy Path System | Path, spawning, movement, lives | Epic 1 | 4 |
| 3 | Tower Placement | Grid interaction, placement, range preview | Epic 1 | 5 |
| 4 | Combat System | Targeting, projectiles, damage, all 4 tower types | Epics 2, 3 | 6 |
| 5 | Economy & Upgrades | Currency, upgrade tiers, sell mechanic, UI | Epic 4 | 5 |
| 6 | Wave System | Wave launch, 25-wave progression, bosses, scaling | Epic 5 | 5 |
| 7 | Polish & Completion | Sprites, UI polish, balance pass, full run | Epic 6 | 5 |

### Recommended Sequence

1 → 2 → 3 → 4 → 5 → 6 → 7

Foundation before content, systems before balance, mechanics before polish.

### Vertical Slice

**First playable milestone (end of Epic 4):** One enemy type walks the path,
one tower type shoots it, the player can place towers and lose lives. Core gameplay
loop proven end-to-end before economy and wave systems are added.

---

## Success Metrics

### Technical Metrics

| Metric | Target | Measurement Method |
|---|---|---|
| Frame rate | 60fps minimum, 144fps target | Manual observation during play |
| Crashes | Zero crashes during a full run | Playtesting |
| Game completion | Wave 25 is reachable and win screen triggers | End-to-end playtest |
| Core systems functional | All 4 tower types, economy, waves, lives work correctly | Per-epic acceptance testing |

### Gameplay Metrics

| Metric | Target | Measurement Method |
|---|---|---|
| Overall difficulty | Winning should be very hard — not expected on first attempt | Personal playtesting |
| Boss wave difficulty | Each boss wave (5, 10, 15, 20, 25) is barely winnable with good placement | Targeted playtesting per boss wave |
| Late-wave placement punishment | Waves 20–25 expose poor placement decisions made earlier | Deliberate bad-placement playtests |
| Non-boss wave feel | Progressive pressure without requiring perfect play | General playtesting across full run |

**Difficulty Assessment Protocol:**
- A boss wave that can be survived without losing a life on the first attempt → too easy, needs tuning
- A boss wave that is impossible to survive with optimal placement → too hard, needs tuning
- Target: boss waves require deliberate, correct placement to survive — no guarantee even with effort

### Qualitative Success Criteria

1. **Systems understanding** — At the end of the project, I can explain how every
   system I built works: the rendering loop, enemy pathfinding, tower targeting logic,
   wave management, and economy. Not just "it works" but "here's why it works."

2. **AI development skill** — I have meaningfully improved at using AI tools in a
   game development context. I understand the BMAD workflow, can write effective prompts
   for implementation tasks, and can review AI-generated code critically.

3. **Completion** — The game is finished. Not abandoned. The core loop runs end-to-end,
   wave 25 is beatable, and the project has a clear done state.

### Metric Review Cadence

Personal project — no formal review cadence. Metrics are evaluated:
- **Per epic:** core system acceptance (does it work as defined?)
- **Post Epic 6:** first full run playtest (is the game completable?)
- **Post Epic 7:** final balance playtests (is it the right difficulty?)

---

## Out of Scope

The following are explicitly NOT in scope for Basic Tower Defense v1.0:

**Features:**
- Audio of any kind (music, SFX, UI sounds) — deferred entirely
- Player-activated abilities or powers
- Meta-progression between runs
- Difficulty settings — single fixed difficulty only
- Mod support or level editor

**Content:**
- Additional maps beyond the single fixed-path map
- Additional tower types beyond the 4 defined
- Additional enemy types beyond the one base type (plus boss variant)
- Additional game modes

**Platform & Distribution:**
- Console, mobile, or web ports
- Steam or storefront integration
- Multiplayer of any kind

### Deferred to Post-Launch

If the core game is solid and learning goals are met, candidates for a future
iteration include: audio, additional tower types, active abilities, additional maps,
and difficulty modifiers. None of these are commitments.

---

## Assumptions and Dependencies

### Key Assumptions

- **Engine/framework** is TBD — decision made during the Architecture workflow
- **Balance values** (tower costs, wave stats, currency amounts) are TBD — set during playtesting
- **Grid size** is TBD — determined during implementation
- **AI image generation** tooling is available for sprite production
- **Solo developer** throughout — no team, no external contributors

### External Dependencies

- AI image generation tool for producing tower, enemy, and UI sprites
- Engine/framework (TBD in Architecture workflow) and its standard tooling

### Risk Factors

- **Engine selection** — wrong choice could require significant rework; the Architecture
  workflow should evaluate this carefully before committing
- **Balance** — "barely surviving" difficulty is hard to tune; budget time for multiple
  full-run playtests during Epic 7

---

## Document Information

**Document:** Basic Tower Defense - Game Design Document
**Version:** 1.0
**Created:** 2026-03-05
**Author:** Nate
**Status:** Complete

### Change Log

| Version | Date | Changes |
|---|---|---|
| 1.0 | 2026-03-05 | Initial GDD complete |
