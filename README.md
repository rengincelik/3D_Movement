# 3D Movement System 🎮

A comprehensive Unity movement framework with modular input mapping, DOTween animations, and vehicle interaction system.

## 📦 Package Contents

### Player Movement System
- **Modular Input-Movement Bridge**: Map any input to any movement type
- **Multiple Movement Types**: Velocity, AddForce, MovePosition, Torque, etc.
- **Camera-Relative Movement**: FPS, TPS, Top-Down support
- **Custom Property Drawers**: Visual inspector configuration
- **State Machine**: Extensible player/vehicle state system

### Object Animation System (DOTween)
- **ScriptableObject-Based**: Reusable movement configurations
- **Movement Types**: Move, Jump, Rotate, Path following
- **Custom Editor**: Visual setup with Gizmos
- **Loop Control**: Restart, PingPong, infinite loops

### Vehicle System
- **Mount/Dismount**: Seamless vehicle interaction
- **Controller Manager**: Hot-swap between player/vehicle controls
- **Interaction Zones**: Trigger-based entry system

## 🚀 Quick Start

### Player Movement
1. Add `MovementController` to player
2. Configure `InputForceBridge` in inspector
3. Assign Input Actions from Input System
4. Set camera reference for camera-relative movement

### Object Animation
1. Create `MovementDataSO` (Right-click → ScriptableObjects → MovementDataSO)
2. Configure animation type (Move, Jump, Path, etc.)
3. Add `MovementController` to object
4. Assign the SO and press Play

### Vehicle System
1. Add `VehicleInteraction` to vehicle
2. Set mount/exit points
3. Configure `ActiveMovementControllerManager` singleton
4. Press E to enter/exit vehicles

## 🔧 Requirements

- Unity 2021.3+
- Unity Input System Package
- DOTween (Free or Pro)

## 📖 Full Documentation

👉 [View on Portfolio](https://rengincelik.github.io/RenginCelik/projects/Mini_Dev_Kit/)

---

© 2025 Rengin Çelik
