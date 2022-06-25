using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        public Tuple<List<Tuple<int, int>>, List<int>> BreadthFirst(int startNode)//returns a Tuple :
                                                                               //Item1: List of tuples (item1 vertex, and item 2 its parent)
                                                                               //Item2: Traversal Order List
        /*Step 1: Choose any one node randomly, to start traversing.
        Step 2: Visit its adjacent unvisited node.
        Step 3: Mark it as visited in the boolean array and display it.
        Step 4: Insert the visited node into the queue.
        Step 5: If there is no adjacent node, remove the first node from the queue.
        Step 6: Repeat the above steps until the queue is empty.*/
        {
            UDLinkedList queue = new UDLinkedList(); //create an instance of the linked list for the queue
            List<Tuple<int, int>> visited = new List<Tuple<int, int>>(); //intialise return value item
            List<int> outputList = new List<int>();//initialise traversal order return value item
            visited.Add(Tuple.Create(startNode, -1));//start node doesnt have a parent so we can set it to -1 and mark it as visited
            outputList.Add(startNode); //add the node to the output list
            List<int> adjNodes = GetAdjVertices(startNode); //get the nodes that are adjacent to that start node
            for (int i = 0; i < adjNodes.Count(); ++i)
            {
                queue.EnQueue(Tuple.Create(adjNodes[i], startNode)); //add all the adjacent nodes to the queue
            }
            while (queue.Count != 0)//do this until the stack is empty
            {
                Tuple<int, int> topOfQueue = queue.DeQueue(); //take and save the vertex at the front of the list
                visited.Add(topOfQueue); //mark this vertex as visited
                int parentNode = topOfQueue.Item1; //the parent of the next-generated adj vertices will be item1 of this vertex
                outputList.Add(parentNode);
                adjNodes = GetAdjVertices(parentNode);//get the adjacent vertices
                for (int i = 0; i < adjNodes.Count(); ++i)//loop through the adjacent vertices
                {
                    if (!VertexVisited(visited, adjNodes[i]) && !queue.Contains(adjNodes[i]))//if not visited and not already in queue
                        queue.EnQueue(Tuple.Create(adjNodes[i], parentNode));//append them onto the stack
                }
            }
            visited.RemoveAt(0); //remove (startvertex, -1) from the list since its not an edge
            return Tuple.Create(visited, outputList); //return the ordered edges and 
        }
        public Tuple<List<Tuple<int, int>>, bool, List<int>> DepthFirst(int startNode) //returns a Tuple :
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
            List<Tuple<int, int>> visited = new List<Tuple<int, int>>(); //initialise return value
            List<int> outputList = new List<int>();//initialise traversal order return value item
            visited.Add(Tuple.Create(startNode, -1)); //start node doesnt have a parent so we can set it to -1 and mark it as visited
            outputList.Add(startNode);
            List<int> adjNodes = GetAdjVertices(startNode); //get the nodes that are adjacent to that start node
            UDLinkedList stack = new UDLinkedList(); //create an instance of the linked list for the stack
            for (int i = 0; i < adjNodes.Count(); ++i)
            {
                stack.Push(Tuple.Create(adjNodes[i], startNode)); //add all the adjacent nodes to the stack
            }
            bool cycleExists = false;
            while (stack.Count != 0) //do this until the stack is empty
            {
                Tuple<int, int> topOfStack = stack.DeQueue(); //take and save the vertex at the top of the starck
                visited.Add(topOfStack); //mark this vertex as visited
                int parentNode = topOfStack.Item1; //the parent of the next-generated adj vertices will be item1 of this vertex
                adjNodes = GetAdjVertices(parentNode);//get the adj vertices
                outputList.Add(parentNode);
                for (int i = 0; i < adjNodes.Count(); ++i)//loop through all these nodes
                {
                    if (stack.Contains(adjNodes[i]))
                    {
                        cycleExists = true; //if an unvisited node appears more than one time in the stack, then there is a cycle
                    }
                    if (!VertexVisited(visited, adjNodes[i]) && !stack.Contains(adjNodes[i])) //if not visited and not already in stack
                        stack.Push(Tuple.Create(adjNodes[i], parentNode)); //push them to the front of the stack
                }
            }
            visited.RemoveAt(0); //remove (startvertex, -1) from the list since its not an edge
            return Tuple.Create(visited, cycleExists, outputList); //return the ordered edges, the cycle and the traversal order
        }
        private bool VertexVisited(List<Tuple<int, int>> visited, int node) //returns true if a vertex has been visited
        {
            for (int i = 0; i < visited.Count(); ++i)
            {
                if (visited[i].Item1 == node)
                {
                    return true; //if there is a match found in the list, return true;
                }
            }
            return false;//other wise return false;
        }
    }
}
