# Copilot Instructions for ModularFramework

## Overview
- **Unity-based Modular Game Framework**: This project is structured for Unity, using a modular, service-oriented architecture to support multiple games (e.g., TowerDefense, TicTacToe).
- **Service Locator Pattern**: Centralized access to core systems via `SystemLocator` (see `Assets/Scripts/SystemLocator/SystemLocator.cs`). This pattern balances between full dependency injection and singletons for lightweight, decoupled services.
- **SignalBus Event System**: Decouples communication between systems and game logic. Use `SignalBus.Instance` for event publishing/subscribing (see `Assets/Scripts/SignalBus/SignalBus.cs`).

## Key Services (all accessible via `SystemLocator.Instance`):
- `SaveLoadService`: Async data management, extensible for new data types via `DataKey` enum.
- `PoolingService`: Generic object pooling for reusable game objects.
- `CurrencyService`: Centralized coin/currency management, depends on `SaveLoadService`.
- `PanelService`: UI panel management; extend by creating prefabs with scripts inheriting `BasePanel`.
- `HapticService` & `AudioService`: Centralized feedback systems, state managed via `SaveLoadService`.
- `SceneController`: Handles scene transitions and loading panels.

## Project Structure
- **Scripts**: All core logic in `Assets/Scripts/` (organized by system: `SystemLocator`, `SignalBus`, `PanelSystem`, etc.).
- **MiniGames**: Add new games under `Assets/TicTacToe/`, `Assets/TowerDefense/`, etc.
- **Prefabs/Resources**: UI and game object assets.
- **ThirdParty**: External Unity plugins (e.g., Array2DEditor).

## Developer Workflows
- **Build**: Use Unity Editor's build system. No custom CLI build scripts.
- **Testing**: Manual playtesting in Unity Editor; no automated test suite present.
- **Debugging**: Use Unity's Play mode and Console. Services are accessible via the Inspector for runtime debugging.

## Conventions & Patterns
- **Service Access**: Always use `SystemLocator.Instance.[Service]` for core systems.
- **Event Communication**: Use `SignalBus` for decoupled messaging.
- **Async Data**: All data operations in `SaveLoadService` are async for future web/IO extensibility.
- **Extending Panels**: Inherit from `BasePanel` and register new panels in `PanelService`.
- **Adding MiniGames**: Place new game logic/assets in a dedicated folder and integrate with core services as needed.

## Integration Points
- **Unity Packages**: Managed via `Packages/manifest.json`.
- **External Plugins**: Place in `Assets/ThirdParty/`.
- **DOTween**: Used for tweening/animation (see `DOTween.Modules.csproj`).

## References
- [README.md](../README.md) — for high-level architecture and service descriptions.
- [SystemLocator.cs](../Assets/Scripts/SystemLocator/SystemLocator.cs) — for service registration and access patterns.
- [SignalBus.cs](../Assets/Scripts/SignalBus/SignalBus.cs) — for event system usage.

---

**For AI agents:**
- Follow the service locator and event bus patterns for all new systems.
- Reference and extend existing services and panels as examples.
- Keep all new code modular and decoupled, using async patterns for IO/data.
