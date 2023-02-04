# strategy-game

## project structure

### Frontend

- used latest Unity LTS Version 2021.3.18f1

### Backend

- I have shortly considered doing a simple backend to store user data but ditched that idea in favour of game code (thats why there is a frontend folder)

### Pipeline

- used GitHub Actions to build APK (can be downloaded via the following link ADD_LINK)

## decisions

### Perspective

I have chosen a top-down perspective as that would be the easiest for me to produce; To save time on camera handling, I just draw the map as big as the screen is. A little constaint is that a little part is behind the UI or might be hidden outside of the map in the worst case. With building for Landscape mode only, I prevent that the map looks very awkward in portrait mode.

### Grid

To make it as easy as possible, I have just implemented a simple grid which is mostly the turning point for all mechanics of the game (Map, Placements and Units). 

### Tests

Due to the prototyping character and the short timeframe I have decided to not do tests at all

### Comments

Only where I found it usesful; The rest of the code is simple and should be self-explanatory

### Used Ressources

- Unity 2D Sprite package for Sprite editing
- Kenny's Assets (Art Collection of assets)
- Priority Queue Unity Package From: https://erdiizgi.com/data-structure-for-games-priority-queue-for-unity-in-c/
- Used A* Algorithm from https://www.redblobgames.com/pathfinding/a-star/introduction.html (converted from pseudo-code)

### further decisions and directions taken

- In a real world scenario (also on prototyping), I would go for an existing dependency injection framework like Zenject. Due to the time constraints I wanted to make that quick and dirty, still prevented circular dependencies, but if needed, services are just thrown into the constructor
- Concept phase. In this scenario I didn't plan ahead and decided to do it as pragmatic as possible. This caused some changes of direction and also more dirty code as it would have caused with an initial short planning. Still I would go again this way to save as much time as possible as the scenario also pointed in that direction

## challenges

- being out from Unity for a while and coming from a central tech department with Unity, I needed some warm-up time to get into again
- as usual with pet-projects or game development in general....cutting down on ideas (see ideas section)
- stopping to develop and leave unfinished stuff behind :D

## ideas (in first instance or for further iterations)

- my first idea, also to stick to existing games (tycoon/trains) was to combine the game mode with trains. A main station would serve as player hub where the initial train will be created
- waggons could be build eiter in additional buildings along the track or the main buidling (the idea for additional buildings could be to upgrade waggons on demand along the track)
- waggons can change purpose along the track depending on the needs; with conditions, a defensive wagon at the start can be converted to an attack or support (slowing down enemies, buffing other waggons) along the way
- track-builder trains could be build to bring more variation and strategy in, those track builders do one branch and continue from there; the enemy would need to decide if it is worth to counter on the opposite site
- due to the longer onboarding phase (getting into games development again), I have changed direction and decided to go for a more simpler prototype closer to the basics of the requirements

## concept and working mode of current prototype

## what could be build on top / next steps

- AI in general; only focussed more on the basic player behaviour (on some parts with enemy in mind, but due to time constraints no enemy AI for now)
- Duplicated code; there is some parts just duplicated to have code quickly available; that should be removed
- as during the development, especially the units grew, a proper state machine would have made sense; that should be one of the next steps
- multiplayer (optional; could be made fun with more variations in AI as well)
- improve AI; more AI variations to create more challenge once the player got used
- rock, paper, scissor; counter units with the right answer
- more unit types (which also means more investment in game setup; so game design can easier work with possible unit/building types/maps); could be done locally via Scriptable Objects or
via Backend
- path finding; the path finding is extremely simple (like if you click on adding a worker twice, the worker didn't signup for a tree yet, so both would go to the same)
- animations; pretty static at the moment
- better pathfinding for more interesting maps (blockers; maybe blockers like stones would require specific units to remove them)
- More use of SOs (better upgrading of Abilities, etc.)
- the currency handling is super simplified and hardcoded; that would also be a change to centralice a Wallet System, also for later monetisation

# given

## mission (with comments on what has been achieved/how its working)

- A building which creates some units, they move around toward an area and are deleted, explode or die  
    - the player and some enemy starts with a base; this base automatically creates units. They automatically move towards the enemy. Further buildings can be placed to create more units; the enemy will also build further buildings
- Have a worker collect a resource and increment a simple UI element to indicate the amount collected 
    - workers can be build from gold earned by killing enemies; those workers will search for trees, harvest them and bring the ressources back to the base; you will see an increase of wood one earned; ressourced will expire, so new trees should be build
- Make a UI to manage a few different resources, then use those resources to place/construct a building in the environment, and update the resources 
    - wood and gold is available as ressources and harvested through trees and by killing enemies
- Construct multiple buildings of the same type but don’t let them build on top of each other, bonus points if you decrement resources for doing so 
    - you can build further barracks to create more units, barracks cost wood
- Implement a spell or “ability” attack on a group of baddies, visibly affect them, and expose the ability to modify a parameter of the attack – maybe radius or intensity?  
    - all parameters of the own units ability can be upgraded with gold
- Any combination of the above. We just made this stuff up.  

## contraints

- Please use Unity 
- This should work on mobile (you’ll be producing an APK)   
- Store assets/plugins are fine, but you should do some programming to bring this together – we specifically want to know how you code and think.  
- Try to spend no more than 4 hours on this after you’ve set up your dev environment and really start working – we don’t want to tank your whole day! Sloppy is OK because we’ll be asking you how to improve your solution afterwards.  
- We don’t care how it looks art-wise. Don’t spend more time on this than you have to.  