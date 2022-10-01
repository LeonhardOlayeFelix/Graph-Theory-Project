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
        /// Runs the Dijkstras algorithm on this graph instance
        /// </summary>
        /// <param name="startVertex">Start Vertex</param>
        /// <param name="endVertex">Start Vertex</param>
        /// <returns>Returns the shortest path as a list and the cost of that path</returns>
        public Tuple<List<int>, int> DijkstrasAlgorithmShort(int startVertex, int endVertex) 
        {
            //returns shortest path as tuple 1) path, 2) cost
            if (!IsInVertexList(startVertex))
            {
                //error handling - existence
                throw new Exception("Start vertex does not exist.");
            }
            else if (!IsInVertexList(endVertex))
            {
                //error handling - existence
                throw new Exception("End vertex does not exist.");
            }
            //uses the adjacency matrix to do the algorithm
            List<List<int>> adjMat = GetAdjacencyMatrix2();

            List<int> path = new List<int>();
            int size = adjMat.ElementAt(0).Count();

            //each vertex has a parent node which is where the shortest path came from
            List<int> parent = new List<int>();
            //keeps track of the cost above each vertex
            List<int> value = new List<int>();
            //keeps track of which vertices are marked as permanent
            List<bool> processed = new List<bool>();
            for (int i = 0; i < size; ++i)
            {
                //-1 indicates no parent 
                parent.Add(-1);
            }
            for (int i = 0; i < size; ++i)
            {
                //none are processed yet
                processed.Add(false);
            }
            for (int i = 0; i < size; ++i)
            {
                //populate with maximum number
                value.Add(2147483647);
            }
            //begin with startVertex
            parent[startVertex] = -1; 
            value[startVertex] = 0;
            for (int row = 0; row < size - 1; ++row)
            {
                //choose the next minimum vertex and mark it as processed
                int minVertex = selectLowestVertex(value, processed, size);
                processed[minVertex] = true;
                for (int column = 0; column < size; ++column)
                {
                    //checks for a present edge by comparing it with -1
                    if (adjMat[minVertex][column] != -1 && processed[column] == false && value[minVertex] != 2147483647 && (value[minVertex] + adjMat[minVertex][column] < value[column]))
                    {
                        //makes sure its not processed
                        //makes sure its cheaper
                        value[column] = value[minVertex] + adjMat[minVertex][column];
                        parent[column] = minVertex;
                    }
                }
            }
            int currentVertex = endVertex;
            path.Add(endVertex);
            while (currentVertex != startVertex)
            {
                try
                {
                    //gets the path in reverse order using the parent child relationship
                    path.Add(parent[currentVertex]);
                    currentVertex = parent[currentVertex]; 
                }
                catch (ArgumentOutOfRangeException)
                {
                    //error handling
                    return null;
                }
            }
            //reverse the order
            path.Reverse();
            int cost = 0;
            for (int i = 0; i < path.Count() - 1; ++i)
            {
                //get the cost of the path
                cost += GetEdgeWeight(path.ElementAt(i), path.ElementAt(i + 1)); 
            }
            return Tuple.Create(path, cost);
        }
        /// <summary>
        /// Selects the next lowest cost, unprocessed vertex for dijkstras algorithm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="processed"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private int selectLowestVertex(List<int> value, List<bool> processed, int size) 
        {
            //returns the vertex that has the lowest value, that is not already processed
            int minimum = 2147483647;
            int vertex = 0;
            for (int i = 0; i < size; ++i)
            {
                if (processed[i] == false && value[i] < minimum)
                {
                    vertex = i;
                    minimum = value[i];
                }
            }
            return vertex;
        }
    }
}
