# Shape Match Game - Unity Project

## Overview
This is a Unity-based mobile game prototype where players match falling shapes by their attributes (shape, color, and animal). The game features physics-based interactions, special shape behaviors, and a simple matching mechanic.

## Game Features
- **Shape Matching**: Combine three identical shapes (matching shape, color, and animal) to clear them
- **Physics System**: Realistic falling and stacking of shapes with gravity and collisions
- **Special Shapes**:
  - Heavy: Falls faster than normal shapes
  - Sticky: Pulls nearby shapes when moving
  - Frozen: Initially inactive until enough shapes are cleared
- **Action Bar**: Holds up to 7 shapes before game over
- **Level Reset**: Button to reshuffle all current shapes
- **Win/Lose Conditions**: Clear all shapes to win, fill action bar to lose

## Technical Implementation

### Core Components
1. **Shape System**:
   - Each shape consists of three attributes (shape type, border color, animal)
   - Shapes are generated in sets of three for matching
   - Special shapes have unique behaviors through components

2. **Physics**:
   - Unity's 2D physics engine handles collisions and gravity
   - Special physics for heavy/sticky shapes
   - "Sandfall" effect when shapes spawn

3. **Game Flow**:
   - Shape spawning with delay for cascading effect
   - Dynamic win/lose condition checking
   - Screen management for game states

### Key Scripts
- `LevelManager`: Controls game flow, spawning, and win/lose conditions
- `ActionBar`: Manages collected shapes and matching logic
- `Shape`: Base shape behavior and special type implementations
- `ShapeData`: ScriptableObjects defining shape properties
- `StickyShape`: Component for sticky shape behavior

## Setup Instructions
1. Clone the repository
2. Open in Unity 2021.3 or later
3. For shape generation:
   - Use the `ShapeDataCreator` editor tool (Tools menu)
   - Assign folders containing shape and animal sprites
   - Click "Generate ShapeData Assets"

## Requirements
- Unity 2021.3+
- Target platform: Android or iOS (configured in Build Settings)

## How to Play
1. Tap shapes to send them to the action bar
2. Match three identical shapes to clear them
3. Don't let the action bar fill completely (7 shapes)
4. Clear all shapes from the board to win
5. Use the reset button to reshuffle shapes