using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        public Tuple<List<Tuple<int, int>>, List<int>> BreadthFirst(int startNode)
        //returns a Tuple :
        //Item1: List of tuples (item1 vertex, and item 2 its parent)
        //Item2: Traversal Order List

        /*Step 1: Choose any one vertex randomly, to start traversing.
        Step 2: Visit its adjacent unvisited vertex.
        Step 3: Mark it as visited and display it.
        Step 4: Insert the visited vertex into the queue.
        Step 5: If there is no adjacent vertex, remove the first vertex from the queue.
        Step 6: Repeat the above steps until the queue is empty.*/
        {
            UDLinkedList queue = new UDLinkedList();
            List<Tuple<int, int>> visited = new List<Tuple<int, int>>();
            List<int> outputList = new List<int>();
            visited.Add(Tuple.Create(startNode, -1));//start vertex doesnt have a parent so we can set it to -1 and mark it as visited
            outputList.Add(startNode);
            List<int> adjNodes = GetAdjVertices(startNode); //get the vertices that are adjacent to that start vertex
            for (int i = 0; i < adjNodes.Count(); ++i)
            {
                //add all the adjacent vertices to the queue
                queue.EnQueue(Tuple.Create(adjNodes[i], startNode)); 
            }
            while (queue.Count != 0)
            {
                //repeat steps this until the queue is empty
                Tuple<int, int> topOfQueue = queue.DeQueue(); 
                visited.Add(topOfQueue);
                int parentNode = topOfQueue.Item1; 
                outputList.Add(parentNode);
                adjNodes = GetAdjVertices(parentNode);
                for (int i = 0; i < adjNodes.Count(); ++i)
                {
                    if (!VertexVisited(visited, adjNodes[i]) && !queue.Contains(adjNodes[i]))
                    {
                        queue.EnQueue(Tuple.Create(adjNodes[i], parentNode));
                    }
                }
            }
            //remove (startvertex, -1) from the list since its not an edge
            visited.RemoveAt(0); 
            return Tuple.Create(visited, outputList);
        }
        public Tuple<List<Tuple<int, int>>, bool, List<int>> DepthFirst(int startNode)
        //returns a Tuple :
        //Item1: List of tuples (item1 vertex, and item 2 its parent)
        //Item2: bool which represents whether a cycle was found
        //Item3: Traversal Order List

        /*Step 1: Start by putting any one of the graph's vertices 
                  on top of a stack.
          Step 2: Take the top item of the stack and add it to the visited list.
          Step 3: Create a list of that vertex's adjacent nodes. Add the ones 
                  which aren't in the visited list to the top of the stack.
          Step 4: Keep repeating steps 2 and 3 until the stack is empty.*/
        {
            UDLinkedList stack = new UDLinkedList();
            List<Tuple<int, int>> visited = new List<Tuple<int, int>>();
            List<int> outputList = new List<int>();
            visited.Add(Tuple.Create(startNode, -1)); //start vertex doesnt have a parent so we can set it to -1 and mark it as visited
            outputList.Add(startNode);
            List<int> adjNodes = GetAdjVertices(startNode); //get the vertex that are adjacent to that start node
            for (int i = 0; i < adjNodes.Count(); ++i)
            {
                stack.Push(Tuple.Create(adjNodes[i], startNode)); //add all the adjacent vertex to the stack
            }
            bool cycleExists = false;
            while (stack.Count != 0) 
            {
                //repeat this until the stack is empty
                Tuple<int, int> topOfStack = stack.DeQueue();
                visited.Add(topOfStack); 
                int parentNode = topOfStack.Item1; 
                adjNodes = GetAdjVertices(parentNode);
                outputList.Add(parentNode);
                for (int i = 0; i < adjNodes.Count(); ++i)
                {
                    if (stack.Contains(adjNodes[i]))
                    {
                        cycleExists = true; 
                    }
                    if (!VertexVisited(visited, adjNodes[i]) && !stack.Contains(adjNodes[i]))
                    {
                        stack.Push(Tuple.Create(adjNodes[i], parentNode)); 
                    }
                }
            }
            //remove (startvertex, -1) from the list since its not an edge
            visited.RemoveAt(0); 
            return Tuple.Create(visited, cycleExists, outputList);
        }
        private bool VertexVisited(List<Tuple<int, int>> visited, int node) 
        {
            //returns true if a vertex has been visited
            for (int i = 0; i < visited.Count(); ++i)
            {
                if (visited[i].Item1 == node)
                {
                    return true; 
                }
            }
            return false;
        }
    }
}
