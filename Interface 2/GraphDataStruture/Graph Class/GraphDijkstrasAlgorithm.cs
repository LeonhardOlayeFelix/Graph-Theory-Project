using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        public Tuple<List<int>, int> DijkstrasAlgorithmShort(int startVertex, int endVertex) //returns tuple 1) path, 2) cost
        {
            if (!IsInVertexList(startVertex))
            {
                throw new Exception("Start vertex does not exist.");
            }
            else if (!IsInVertexList(endVertex))
            {
                throw new Exception("End vertex does not exist.");
            }
            List<int> path = new List<int>(); //a list which will contain the path
            List<List<int>> adjMat = GetAdjacencyMatrix(); //uses the adjacency matrix to do the algorithm
            int size = adjMat.ElementAt(0).Count(); //size of adj matrix columns
            List<int> parent = new List<int>(); //each node has a parent node which is where the shortest path came from
            List<int> value = new List<int>(); //keeps track of the cost of each path
            List<bool> processed = new List<bool>();//to keep track of which nodes are marked as permanent
            for (int i = 0; i < size; ++i)
            {
                parent.Add(-1); //populate parent
            }
            for (int i = 0; i < size; ++i)
            {
                processed.Add(false); //populate processed
            }
            for (int i = 0; i < size; ++i)
            {
                value.Add(2147483647);//populate with maximum number
            }
            parent[startVertex] = -1; //begin with source node
            value[startVertex] = 0;
            for (int row = 0; row < size - 1; ++row)
            {
                //choose the next minimum vertex and mark it as processed
                int minVertex = selectLowestVertex(value, processed, size);
                processed[minVertex] = true;
                for (int column = 0; column < size; ++column)
                {
                    //checks for a present edge by comparing it with -1
                    //makes sure its not processed
                    //checks if it provides a lowered cost
                    if (adjMat[minVertex][column] != -1 && processed[column] == false && value[minVertex] != 2147483647 && (value[minVertex] + adjMat[minVertex][column] < value[column]))
                    {
                        value[column] = value[minVertex] + adjMat[minVertex][column];
                        parent[column] = minVertex; //if the case then use it in the shortest path
                    }
                }
            }
            int currentVertex = endVertex;
            path.Add(endVertex);//add the vertex to
            while (currentVertex != startVertex)
            {
                try
                {
                    path.Add(parent[currentVertex]);
                    currentVertex = parent[currentVertex]; //gets the path in reverse order using the parent child relationship
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
            //so reverse the order
            path.Reverse();
            int cost = 0;
            for (int i = 0; i < path.Count() - 1; ++i)
            {
                cost += GetEdgeWeight(path.ElementAt(i), path.ElementAt(i + 1)); //get the cost of the shortest path
            }
            return Tuple.Create(path, cost);
        }
        private int selectLowestVertex(List<int> value, List<bool> processed, int size) //gets the vertex that has the lowest value, that is not already processed
        {
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
