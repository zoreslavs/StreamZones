# Unity Scene Streaming

Additive scene streaming using Addressables with a simple grid-based controller.

## Unity version

Developed and tested with **Unity 2022.3.62f2 (LTS)**.  
Other 2022 LTS versions will likely work, but are not guaranteed.

## Scenes

- **Hub.unity** – main scene (run this one)
- **Zone_A.unity**, **Zone_B.unity** – loaded additively

## Controls

**WASD / Arrow keys** – move one grid cell per keypress

## Features

- Additive loading/unloading of zones via Addressables
- Enter/Exit radius hysteresis logic
- Minimal loading UI during async operations
- Gizmos showing Enter/Exit radii in Scene view
- Console logs for load/unload lifecycle

## Setup

- Open **Hub.unity**
- Adjust zone centers and radii in the Hub inspector
- Addressables setup included (local groups)