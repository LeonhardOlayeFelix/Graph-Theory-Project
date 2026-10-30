# Graph Theory Simulator

A Windows desktop app for building graphs and watching graph algorithms run on them step by step. It was designed as a teaching tool: students and teachers have accounts, and teachers can organise classes and set graph assignments.

Built with **C#** and **WPF** on .NET Framework 4.8.

https://github.com/user-attachments/assets/deef9162-0990-4304-8802-04de74aa60fa

## Features

### Graph editor
- Add, move and delete vertices and weighted edges on a canvas
- Randomly generate edge weights within a chosen range
- Customise vertex size, shape, colours and labels
- Save and load graphs

### Algorithms
Each algorithm is animated on the graph, with an adjustable step speed.

| Category | Algorithms |
| --- | --- |
| Traversal | Breadth-first search, depth-first search |
| Shortest path | Dijkstra's algorithm, Floyd–Warshall |
| Minimum spanning tree | Prim's algorithm, Kruskal's algorithm |
| Route inspection | Chinese Postman problem, for routes that return to the start or end at a different vertex |

The app also checks graph properties such as connectivity, cycles, vertex degrees and whether a graph is Eulerian or semi-Eulerian, and can display the adjacency list and adjacency matrix.

### Accounts and classes
- Separate student and teacher accounts, plus a guest mode
- Teachers can create classes, add and remove students, and set graph assignments with due dates
- Tracks each student's activity, such as assignments submitted and algorithms run

## Screenshots

![App view 1](https://github.com/user-attachments/assets/23ab8911-6fce-45e2-80ad-f489f5f5e72d)
![App view 2](https://github.com/user-attachments/assets/7f33b158-7400-4338-94a3-1d702bf38f38)

## How It Works

- **Graph data structure:** a custom `Graph` class stores the graph as an adjacency list of `Vertex` objects. The algorithms are split across partial-class files by category.
- **Custom linked list:** a hand-written linked list serves as the queue for breadth-first search and the stack for depth-first search.
- **Database:** user accounts, classes, assignments and saved-graph records are stored in a Microsoft Access database, which the app creates on first run.
- **Saving:** graphs are serialised to binary files and linked to their owner in the database.

## Building

Windows only. Requires **Visual Studio** with the .NET desktop development workload.

1. Clone the repo and open `Interface 2.sln`.
2. Restore NuGet packages (Extended WPF Toolkit and Syncfusion).
3. Set the platform to **x86**. The app uses the Jet OLE DB provider for its database, which is only available to 32-bit apps.
4. Build and run.

## Acknowledgements

[Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit), [Syncfusion WPF](https://www.syncfusion.com/wpf-controls)
