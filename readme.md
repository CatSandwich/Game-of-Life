# Game of Life
This is a very early prototype of a 1 on 1 game using Conway's Game of Life mechanics. 
Each player will place alive cells on their side of the battlefield and then the simulation will run. When a cell is turned alive, it will keep
the ownership of whichever player had the most cells around it. The winner will be one of the following (TBD):
- Whoever has the most amount of cells after X ticks
- Whoever has the most amount of cells at standstill
- Whoever has the most cells reach the opponents end zone

The two parts I'm proud of in this repository are the networking (this was my first semi-fuinctional prototype) and the 
[multithreadable game of life simulation](https://github.com/CatSandwich/Game-of-Life/tree/master/Server/Common/Simulation). This simulation runs asynchronously with 
exposed callbacks for ticks and simualtion end. It supports multiple players (cell owners) and is rather optimized. As it is fully C# with no dependencies, it runs successfully
in Unity and [on the server](https://github.com/CatSandwich/Game-of-Life/blob/master/Server/Plugin/Game/Room.cs#L61), allowing the client to run the simulation 
[with rendering through callbacks](https://github.com/CatSandwich/Game-of-Life/blob/master/Unity/Assets/Scripts/Game/Simulation/GameController.cs#L24). 

I would love to get back to this game at some point but life is busy.
