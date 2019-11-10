Unity Version 2018.2.3f1 (64-bit)

GAME FLOW
1. Start scene is "Main"
2. GameManager Line:38 is where the game flow starts. 
Game related flow like scene change is called from here.
3. UIManager - This holds all menu changes, scene changes function. 
UI transition is handled and called from here.

GAME CONFIGS
1. Configuration - Currently hold Grid size and other sprite references that are used in the game.
2. GameBalance - This holds AnimationCurves for balancing.
The way is used is, Time is considered as PlayerLevel and value over the respective PlayerLevel
eg. Active Enemy Speed Graph
At Time = 1, Value = 250 and at Time = 30, Value = 1100
This translates to - at PlayerLevel = 1, EnemySpeed = 250 and at PlayerLevel = 30, EnemySpeed = 1100
