using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        /// <summary>
        /// completes breadth first search on graph returning: a list of vertices visited and their parents, along with the output list
        /// returns as tuple(list(tuple(vertex, parent), outputlist)
        /// </summary>
        /// <param name="startNode">where the traversal starts from</param>
        /// <returns></returns>
        public Tuple<List<Tuple<int, int>>, List<int>> BreadthFirst(int startNode)
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
        /// <summary>
        /// complete Depth first search on graph returning: a list of vertices visited and their parents, True if the graph contains a 
        /// cycle, and the output list
        /// </summary>
        /// <param name="startNode">where the traversal starts from</param>
        /// <returns></returns>
        public Tuple<List<Tuple<int, int>>, bool, List<int>> DepthFirst(int startNode)
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
        /// <summary>
        /// returns true if a vertex has already been visited during a traversal
        /// </summary>
        /// <param name="visited"></param>
        /// <param name="node"></param>
        /// <returns></returns>
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
