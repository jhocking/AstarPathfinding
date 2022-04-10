# AstarPathfinding
WebGL build at https://jhocking.itch.io/state-machine-ai

Here's a library for and demo of A* on a 2D grid. Many years ago I had programmed a pathfinding library in AS3 to use in Flash, and I decided to port it to C# for use in Unity. Click and drag the green and red squares in the interactive demo to see the pathfinding algorithm in action.

For many games (especially 3D games) you would be better off using the NavMesh pathfinding built into Unity, or you could use the A* Pathfinding Project. However those approaches aren't great choices for a simple 2D grid, plus this library is a good reference for you to learn how A* works.

I relied heavily on this tutorial when I was learning the algorithm:<br>
https://gist.github.com/jcward/45afd22560939aaae5c75e68f1e57505

---

Note that the demo code executes the pathfinding process on a separate thread, and wraps the threaded execution in a coroutine so that Unity will keep going while the process runs (plus it returns the pathfinding result back to the main thread). This library doesn't *have* to be used on new threads, but running pathfinding on separate threads helps your game's performance when there is lots of pathfinding going on.
