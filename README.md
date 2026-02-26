# ModularFramework

## Features

### Core Architecture
- **Service Locator Pattern**: This is one of the optimazied patern between, DI systems and full Singelton base structure. Easy to use for light weight games and can be replaced with DI structures for complicated games.
- **SignalBus**: An event system to prevent coupling between core systems and ingame structures.
- **SaveLoadService**: This is the main data management system. In order to use Web based or time consuming IO operations in the future all public functions are coded as async. Each minigame and other services can be integrated to this service via implementing its data class and adding its name to DataKey enum.
- **PoolingService**: A generic pooling system to handle each poolable objects used in any games.
- **CurrencyService**: This is a centralized Coin system. For the sake of this project, only the Coin is added. This can be extended to organize more currency types. This is the one of the services which uses SaveLoadService.
- **PanelService**: A Panel Management System which operates each panel in the system. In order to add new panels, a panel prefeb should be created which has a script extends the BasePanel. BasePanel has a basic animation option. A queue, more animation options and more hide and show options can be added. 
- **HapticService**: A facade for Hactic library. It can be enabled/disabled. Status data is handled via SaveLoadService.
- **AudioService**: A centralized tool for managing sfxes. For this project, only one time effects added and only management option is mute and unmute. For more complex version, volume control and ambiant music options also can be added. Status data is handled via SaveLoadService.
- **SceneController**: A facade for SceneManager of Unity which is also manages the loading panel.
- **MiniGameService**: The structure which seperates base system and games. Each game is loaded via a MiniGameLoader. In order to give maximum elasticity this system matches mini games with scenes. 
- **InventoryService**: An Inventory system added for further use. None of the example games is required this but it is used for most cases. Items are included via ItemCollection and ownership data is integrated with SaveLoadService.


## Minigames

### TicTacToe
A basic Tic Tac Toe game is created.
    - Very basic Minigame loader
    - Integrated to Base Systems (Currency, Audio etc..)

### Tower Defense
A basic tower defense game is created to demonstrate system's ability to manage several games.
    - More complex Minigame loader
    - It has its own data. This is one of the few changes in the base systems.
    - Enums added for panels and poolable objects.
    - There are several scriptable objects for balancing both tower, upgrades and enemies.
        - EnemyCollection
        - TowerConfig
    - Pool is used for enemies and projectiles.


## Design Decisions
- I also noted to add a Level Management system but none of the sample games required it so, omitted it. Also Minigame transition can be handle as a level progression but this feature can be added if it is required.
- I kept the scope of each service simple in order to add several and most used systems, however, each system has enough foundation to be extended easily.
- I added a navigation scene to simulate landing scene in most games and use this opportunity to navigate between different games. In an actual game this scene can be a landing scene or level selection scene.
- There are some duplications between ui elements. It's because i wanted to both handle each mini game seperatly and give a overall estetic to project.