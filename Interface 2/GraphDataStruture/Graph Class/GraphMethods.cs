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
        /// Returns the Instance of a vertex given its ID
        /// </summary>
        /// <param name="vertexID">ID of the vertex</param>
        /// <returns></returns>
        public Vertex GetVertex(int vertexID)
        {
            //returns Vertex instance given an ID
            for (int i = 0; i < vertexSet.Count(); ++i)
            {
                if (vertexSet[i].GetVertexId() == vertexID)
                {
                    return vertexSet[i];
                }
            }
            return null;
        }
        /// <summary>
        /// Returns true if the specified vertex is a part of the graph
        /// </summary>
        /// <param name="vertexID"></param>
        /// <returns></returns>
        public bool IsInVertexList(int vertexID)
        {
            //returns true if vertex exists
            List<int> vertexList = GetListOfVertices();
            foreach (int vertex in vertexList)
            {
                if (vertexID == vertex)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns the adjacent vertices of a specified vertex
        /// </summary>
        /// <param name="vertexID"></param>
        /// <returns></returns>
        public List<int> GetAdjVertices(int vertexID)
        {
            //returns list of neighbours of a vertex
            if (!IsInVertexList(vertexID))
            {
                //error handling - existence
                throw new ArgumentOutOfRangeException("Vertex does note exist");
            }
            List<int> adjacentVertices = new List<int>();
            for (int i = 0; i < vertexSet.Count(); ++i)
            {
                if (vertexSet[i].GetVertexId() == vertexID)
                {
                    foreach (Tuple<int, int> neighbour in vertexSet[i].GetAdjVertices())
                    {
                        adjacentVertices.Add(neighbour.Item1);
                    }
                }
            }
            return adjacentVertices;
        }
        public int GetEdgeWeight(int v1, int v2)
        {
            //returns weight on an edge
            int weight = -1;
            foreach (Vertex vertex in vertexSet)
            {
                if (vertex.GetVertexId() == v1)
                {
                    weight = vertex.GetWeight(v2);
                }
            }
            //return -1 if there was no edge
            return weight;
        }
        public List<Tuple<int, int, int>> GetListOfEdges()
        {
            //returns list of edges
            return listOfEdges;
        }
        public List<Tuple<int, int, int>> GetListOfSortedEdges()
        {
            //merge sorts list of edges
            List<Tuple<int, int, int>> unsortedEdges = GetListOfEdges();
            List<Tuple<int, int, int>> sortedEdges = MergeSort(unsortedEdges);
            return sortedEdges;
        }
        public List<Vertex> GetAdjacencyList()
        {
            //returns adjacency list
            return vertexSet;
        }
        public int GetSumOfWeights()
        {
            //returns the sum of weights
            int sum = 0;
            foreach (Vertex node in vertexSet)
            {
                //loop through adjacency list
                foreach (Tuple<int, int> adjacentVertex in node.GetAdjVertices())
                {
                    //loop through each vertex's neighbours
                    sum += adjacentVertex.Item2; //add weight to sum
                }
            }
            //each edge is counted twice
            return sum / 2;
        }
        public int GetNumberOfVertices()
        {
            //returns the number of vertices
            return this.numberOfVertices;
        }
        public List<int> GetListOfVertices()
        {
            //returns list of vertices
            List<int> ListOfVertices = new List<int>();
            foreach (Vertex node in vertexSet)
            {
                //loop through adjacency list
                ListOfVertices.Add(node.GetVertexId());
            }
            return ListOfVertices;
        }
        public int GetValency(int v1)
        {
            //returns the valency of a vertex
            if (!IsInVertexList(v1))
            {
                //error handling - existence
                throw new ArgumentException("this vertex does not exist.");
            }
            else
            {
                foreach (Vertex node in vertexSet)
                {
                    //loop through adjacency list to find right vertex
                    if (node.GetVertexId() == v1)
                    {
                        return node.GetAdjVertices().Count();
                    }
                }
            }
            //flag
            return -1;
        }
        public int GetSumValency()
        {
            //return the sum of all of the valencies
            int total = 0;
            List<int> vertexList = GetListOfVertices();
            for (int i = 0; i < vertexList.Count(); ++i)
            {
                //call getvalency on each vertex
                total += GetValency(vertexList.ElementAt(i));
            }
            return total;
        }
        public string PrintAdjList()
        {
            //returns adjacency list as string
            string stringToReturn = "";
            List<int> listOfVertices = GetListOfVertices();
            for (int i = 0; i < listOfVertices.Count(); ++i)
            {
                stringToReturn += listOfVertices[i] + ": " + string.Join(", ", GetAdjVertices(listOfVertices[i])) + "\n";
            }
            return stringToReturn;
        }
        public int GetMaxVertexID()
        {
            //returns the highest Vertex ID
            int maxId = -10;
            foreach (Vertex vertex in vertexSet)
            {
                if (vertex.GetVertexId() > maxId)
                {
                    maxId = vertex.GetVertexId();
                }
            }
            return maxId;
        }
        public List<List<int>> GetAdjacencyMatrix()
        {
            //returns adjacency matrix
            int size = GetMaxVertexID() + 1;
            List<List<int>> adjMatrix = new List<List<int>>();
            for (int row = 0; row < size; ++row)
            {
                adjMatrix.Add(new List<int>());
                //set all entries to -1 first
                for (int column = 0; column < size; ++column)
                {
                    adjMatrix[row].Add(-1);
                }
            }
            foreach (Vertex node in vertexSet)
            {
                //intialise all entries which have a connection
                foreach (Tuple<int, int> adjacentVertex in node.GetAdjVertices())
                {
                    adjMatrix[node.GetVertexId()][adjacentVertex.Item1] = adjacentVertex.Item2;
                    adjMatrix[adjacentVertex.Item1][node.GetVertexId()] = adjacentVertex.Item2;
                }
            }
            return adjMatrix;
        }
        public List<int> GetOddVertices()
        {
            //returns odd valency vertices
            List<int> oddVertices = new List<int>();
            foreach (Vertex vertex in vertexSet)
            {
                //loop through adjacency list
                if (GetValency(vertex.GetVertexId()) % 2 == 1)
                {
                    oddVertices.Add(vertex.GetVertexId());
                }
            }
            return oddVertices;
        }
        private bool IsValidEdge(int u, int v, bool[] isInMst)
        {
            //Check if the passed in edge is available for use in the MST
            if (u == v)
            {
                //vertex to itself is not valid
                return false;
            }
            if (isInMst[u] == false && isInMst[v] == false)
            {
                //exactly one of the vertices need to be in the MST so return false
                return false;
            }
            else if (isInMst[u] == true && isInMst[v] == true)
            {
                //exactly one of the vertices need to be in the MST so return false
                return false;
            }
            return true;
        }
        public int GetNumberOfEdges()
        {
            //returns number of edges in graph
            return GetSumValency() / 2;
        }
        public bool ContainsCycle()
        {
            //returns true if the graph contains a cycle
            foreach (Vertex vertex in vertexSet)
            {
                //Check from every vertex incase the graph is disconnected
                if (DepthFirst(vertex.GetVertexId()).Item2)
                {
                    //item 2 is true if the graph has a cycle
                    return true;
                }
            }
            return false;
        }
        public bool IsConnected()
        {
            //returns true if the graph is connected
            if (GetNumberOfVertices() == 1)
            {
                //a graph with 1 vertex is connected
                return true;
            }
            List<Tuple<int, int>> DFSresult = DepthFirst(GetMaxVertexID()).Item1;
            if (DFSresult.Count() == GetNumberOfVertices() - 1)
            {
                //if the result of DFS includes all of the vertices it is connected
                return true;
            }
            return false;
        }
        public int GetWalkCost(List<int> path)
        {
            //returns cost of a walk
            int pathCost = 0;
            for (int i = 0; i < path.Count() - 1; ++i)
            {
                int edgeWeight = GetEdgeWeight(path[i], path[i + 1]);
                if (edgeWeight == -1)
                {
                    pathCost = -1;
                    break;
                }
                else
                {
                    pathCost += edgeWeight;
                }
            }
            return pathCost;
        }
        public bool IsSemiEulerian()
        {
            //returns true if the graph is semi eulerian
            int numOddVertices = 0;
            foreach (Vertex vertex in vertexSet)
            {
                if (GetValency(vertex.GetVertexId()) % 2 == 1)
                {
                    numOddVertices += 1;
                }
            }
            if (numOddVertices == 2)
            {
                return true;
            }
            return false;
        }
        public bool IsEulerian()
        {
            //returns true if the graph is eulerian
            foreach (Vertex vertex in vertexSet)
            {
                if (GetValency(vertex.GetVertexId()) % 2 == 1)
                {
                    return false;
                }
            }
            return true;
        }
        private int GetMax(int a, int b)
        {
            return (a > b) ? a : b;
        }
        private int GetMin(int a, int b)
        {
            return (a < b) ? a : b;
        }
    }
}

