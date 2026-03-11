---
stepsCompleted: [1, 2, 3, 4, 5, 6, 7, 8]
inputDocuments: []
documentCounts:
  brainstorming: 0
  research: 0
  notes: 0
workflowType: 'game-brief'
lastStep: 0
project_name: 'basic-game'
user_name: 'Nate'
date: '2026-02-27'
game_name: 'Basic Tower Defense'
---

# Game Brief: Basic Tower Defense

**Date:** 2026-02-27
**Author:** Nate
**Status:** Draft for GDD Development

---

## Executive Summary

Basic Tower Defense is a clean, no-frills implementation of the classic tower defense formula built as a personal learning project.

**Target Audience:** Solo developer (Nate) — learning game development through hands-on implementation.

**Core Pillars:** Clean Implementation, Challenging Difficulty

**Key Differentiators:** Genuinely hard difficulty tuned to require strategic placement to survive.

**Platform:** PC (desktop), offline only

**Success Vision:** A fully finished, playable tower defense game — complete core loop working end-to-end, and real knowledge of game design gained through the process.

---

## Game Vision

### Core Concept

A tower defense game where players strategically place specialized towers along enemy paths to stop increasingly powerful waves from breaking through.

### Elevator Pitch

Basic Tower Defense is a clean, no-frills implementation of the classic tower defense formula. Players place specialized towers anywhere off a predefined enemy path to intercept progressively harder waves before they reach the end. The focus is on solid, well-executed fundamentals — no gimmicks, just the core loop done right.

### Vision Statement

A personal learning project to build a fully functional tower defense game from the ground up. Success means a playable, complete implementation of the genre's core mechanics: path-following enemies, wave progression, tower placement, and varied tower types. Clean and functional over novel or commercial.

---

## Target Market

### Primary Audience

Solo developer learning project. The sole audience is the developer (Nate) using this project to learn game development fundamentals through hands-on implementation of a well-understood genre.

**Demographics:**
Single developer, building for personal skill development.

**Gaming Preferences:**
Familiar with the tower defense genre — the rules are known, so the focus can stay on implementation rather than design discovery.

**Motivations:**
Learning by building something complete and functional. Tower defense provides a well-scoped, well-understood problem space ideal for a learning project.

### Secondary Audience

None. This is a personal learning project with no target audience beyond the developer.

### Market Context

Not a commercial product. The "market" context is the tower defense genre itself as a learning vehicle — proven mechanics, clear rules, and a definable "done" state make it ideal for learning.

**Similar Successful Games:**
Bloons TD, Kingdom Rush, Plants vs. Zombies — used as reference points for established mechanics, not competition.

**Market Opportunity:**
Personal skill development and project completion.

---

## Game Fundamentals

### Core Gameplay Pillars

1. **Clean Implementation** — Well-structured, readable code and gameplay. No extraneous systems. Every feature earns its place.
2. **Challenging Difficulty** — Enemies are a genuine threat. Waves are tuned to punish passive or careless placement. The game pushes back.

**Pillar Priority:** When pillars conflict, *challenging difficulty* wins on design decisions; *clean implementation* wins on technical decisions.

### Primary Mechanics

- **Place towers** — Spend currency to place towers on any non-path tile
- **Four tower types:**
  - Fast/Weak — high fire rate, low damage; good for finishing wounded enemies
  - Slow/Heavy — low fire rate, high damage; core damage dealer
  - AOE — area-of-effect damage; essential for dense waves
  - Slow — reduces enemy movement speed; enables other towers to deal more damage
- **Upgrade towers** — Spend currency to upgrade placed towers, improving their stats
- **Enemy waves** — Enemies follow a fixed path in escalating waves, growing stronger and more numerous over time
- **3 lives** — Each enemy that reaches the path's end costs one life; lose all three and it's game over

**Core Loop:** Earn currency → strategically place and upgrade towers → survive the wave → repeat with increasing difficulty

### Player Experience Goals

The target experience is *barely surviving* — constant tension, waves that genuinely threaten your setup, and the satisfaction of holding the line through smart placement rather than brute force.

**Emotional Journey:** Tense anticipation before each wave → pressure and quick decisions mid-wave → relief (or panic) at wave end → strategic recalibration before the next wave.

---

## Scope and Constraints

### Target Platforms

**Primary:** PC (desktop)
**Secondary:** None planned

### Development Timeline

Personal learning project — no fixed timeline.

### Budget Considerations

Personal learning project with no budget. Development relies entirely on free or already-owned tools. No art, audio, or marketing spend planned.

### Team Resources

Solo developer (Nate). All roles — design, programming, art, and audio — handled by one person.

**Skill Gaps:** Engine/framework selection is still open; part of the learning process is evaluating and choosing the right tool for the job.

### Technical Constraints

- Offline only — no server, no networking, no online features
- No multiplayer or leaderboards
- Engine/framework TBD — decision to be made during technical architecture phase

### Scope Realities

This is a focused, single-developer learning project. Scope is intentionally narrow: one map, four tower types, progressive wave difficulty, currency/upgrade system, 3 lives. Features beyond the core loop are out of scope unless the fundamentals are solid first.

---

## Reference Framework

### Inspiration Games

**Bloons TD**
- Taking: Tower variety and role differentiation, wave structure and pacing
- Not Taking: Cartoon aesthetic, extensive meta-progression, large tower roster size

### Competitive Analysis

**Direct Competitors:**
Bloons TD series, Kingdom Rush, Plants vs. Zombies — established tower defense titles on PC.

**Competitor Strengths:**
Polished progression systems, high content volume, strong visual identity.

**Competitor Weaknesses:**
Tend toward accessibility over challenge — most can be completed without strategic depth. Difficulty often comes from content gates rather than genuine mechanical pressure.

### Key Differentiators

Since this is a personal learning project, commercial differentiation isn't the goal. The one meaningful design differentiator worth committing to:

1. **Genuine difficulty** — tuned to require strategic placement to survive, not just optimized spending. Enemies are a real threat from early waves onward.

**Unique Value Proposition:**
A clean, no-frills tower defense that actually makes you work for the win.

---

## Content Framework

### World and Setting

Abstract and minimal. No theme, lore, or world-building. Enemies and towers are represented as clean geometric shapes. The map is a stylized path on a grid — functional, not decorative.

### Narrative Approach

None. Pure gameplay — no story, cutscenes, dialogue, or narrative systems needed.

**Story Delivery:** N/A

### Content Volume

Single map, four tower types, one enemy type scaled by wave (speed/health increases), progressive wave count TBD during design. Minimal UI — currency display, lives counter, wave indicator.

---

## Art and Audio Direction

### Visual Style

Clean geometric/abstract. Towers and enemies are simple shapes with clear visual differentiation by color or form. UI is minimal and functional. No animation complexity beyond movement and projectiles.

**References:** Abstract tower defense aesthetics — clarity over decoration.

### Audio Style

None in initial implementation. Audio deferred entirely to keep scope focused on core mechanics.

### Production Approach

All assets created by the solo developer using basic drawing tools or programmatically. No outsourcing, no asset store dependencies. Geometric style specifically chosen to keep art production near-zero.

---

## Risk Assessment

### Key Risks

1. **Engine/framework paralysis** — TBD engine selection could stall the project before a line of code is written
2. **Difficulty balancing** — Getting "barely surviving" right is harder than it sounds; too easy or too hard kills the project's core goal
3. **Scope creep** — Temptation to add more towers, maps, or enemy types before core loop is solid

### Technical Challenges

- Pathfinding and enemy movement along a fixed path
- Tower targeting logic (nearest, first, strongest)
- Wave scaling math that produces genuine challenge without becoming unfair

### Market Risks

N/A — personal learning project, not commercial.

### Mitigation Strategies

- **Engine selection:** Make a decision early and commit; don't evaluate endlessly
- **Difficulty balancing:** Playtest each wave set before adding more; tune aggressively toward harder
- **Scope creep:** Core loop must be fully working before any new features are considered

---

## Success Criteria

### MVP Definition

A fully functional tower defense game with:
- Enemies that follow a fixed path wave by wave
- Four working tower types (fast/weak, slow/heavy, AOE, slow) placeable on non-path tiles
- Currency earned per wave, spendable on tower placement and upgrades
- 3 lives — lose one each time an enemy reaches the end
- Win condition (survive all waves) and lose condition (all lives lost)
- Waves that escalate in difficulty to the point of genuine challenge

### Success Metrics

- **Completion:** The project is finished end-to-end — not abandoned mid-build
- **Learning:** Gained practical understanding of game design fundamentals through the process
- **Difficulty:** The game is genuinely hard to beat on first attempt

### Launch Goals

Personal satisfaction of finishing a complete, playable game. No release or distribution planned.

---

## Next Steps

### Immediate Actions

1. Select engine/framework — make a decision and commit
2. Proceed to Game Design Document (GDD) to flesh out systems in detail
3. Begin implementation of core loop: enemy path movement first

### Research Needs

- Engine/framework evaluation (what fits a solo PC dev learning project)
- Wave balancing research — how other tower defense games scale difficulty

### Open Questions

- Which engine/framework to use?
- How many waves in the game?
- What are the specific upgrade tiers per tower type?

---

## Appendices

### A. Research Summary

No prior research conducted. Bloons TD used as primary reference for tower variety and wave structure.

### B. Stakeholder Input

Solo project — no external stakeholders.

### C. References

- Bloons TD (tower variety, wave structure)
- Kingdom Rush, Plants vs. Zombies (genre reference points)

---

_This Game Brief serves as the foundational input for Game Design Document (GDD) creation._

_Next Steps: Use the `workflow gdd` command to create detailed game design documentation._