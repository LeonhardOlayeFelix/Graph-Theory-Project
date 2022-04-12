using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Network
    {
        public bool IsInVertexList(int v)//function to check if a vertex exists
        {
            List<int> vertexList = GetListOfVertices();//get the list of vertices
            foreach (int vertex in vertexList)//loop through it
            {
                if (v == vertex)
                {
                    return true;//if a match is found return true
                }
            }
            return false;//otherwise return false
        }
        public List<int> GetAdjVertices(int vertex) //gets the adjacent vertices of a vertex
        {

            if (!IsInVertexList(vertex)) //make sure that the input vertex exists first.
                throw new ArgumentOutOfRangeException("Vertex does note exist");
            List<int> adjacentVertices = new List<int>();
            for (int i = 0; i < VertexSet.Count(); ++i)
            {
                if (VertexSet[i].GetVertexId() == vertex)
                {
                    foreach (Tuple<int, int> adjVertex in VertexSet[i].GetAdjVertices())
                    {
                        adjacentVertices.Add(adjVertex.Item1);
                    }
                }
            }
            return adjacentVertices;
        }
        public int GetEdgeWeight(int v1, int v2) //gets weight between to vertices
        {
            int weight = -1;
            foreach (Node vertex in VertexSet)
            {
                if (vertex.GetVertexId() == v1)
                {
                    weight = vertex.GetWeight(v2);
                }
            }
            return weight; //returns -1 if no edge was found
        }
        public List<Tuple<int, int, int>> GetListOfEdges() //item1: vertex1, Item2: Vertex2, Item3: Cost
        {
            return listOfEdges;
        }
        public List<Tuple<int, int, int>> GetListOfSortedEdges() //uses merge sort to get a list of sorted edges
        {
            List<Tuple<int, int, int>> unsortedEdges = GetListOfEdges();//get the unsorted list
            List<Tuple<int, int, int>> sortedEdges = MergeSort(unsortedEdges); //run the merge sort on the unsorted list
            return sortedEdges;
        }
        public List<Node> GetAdjacencyList()//gets the adjacency list of the graph
        {
            return VertexSet;
        }
        public int GetSumOfWeights()
        {
            int sum = 0;
            foreach (Node node in VertexSet) //loop through each node
            {
                foreach (Tuple<int, int> adjacentVertex in node.GetAdjVertices())//loop through each adj vertex
                {
                    sum += adjacentVertex.Item2; //loop through each tuple and get the weight, update sum
                }
            }
            return sum / 2; //because its counted each edge twice
        }
        public int GetNumberOfVertices()//retreives the number of vertices
        {
            return this.NumberOfVertices;
        }
        public List<int> GetListOfVertices()//retreives a list of vertices
        {
            List<int> ListOfVertices = new List<int>();
            foreach (Node node in VertexSet)//loop through the nodes
            {
                ListOfVertices.Add(node.GetVertexId());//add the ID of each node into the list.
            }
            return ListOfVertices;
        }
        public int GetValency(int v1)//get the valency of a node
        {
            if (IsInVertexList(v1))//first, make sure the vertex exists
            {
                foreach (Node node in VertexSet)//loop through each node
                {
                    if (node.GetVertexId() == v1) //when you find the match
                    {
                        return node.GetAdjVertices().Count();//return the number of adjacent vertices
                    }
                }
            }
            return -1;//if the vertex doesnt exist, return -1 as an indicator
        }
        public int GetSumValency()//get the sum of all of the valencies
        {
            int total = 0;
            List<int> vertexList = GetListOfVertices();
            for (int i = 0; i < vertexList.Count(); ++i)
            {
                total += GetValency(vertexList.ElementAt(i));//add the valency of each vertex to total by repetitvely calling getvalency() on each vertex
            }
            return total;
        }
        public string PrintValency(int v) //returns the string which represents the valency of vertex v
        {
            string stringToReturn = "";
            stringToReturn += "Valency of Vertex " + v.ToString() + ": " + GetValency(v).ToString() + "\n";
            return stringToReturn;
        }
        public string PrintAdjList() //returns the string which represents the adjacency list
        {
            string stringToReturn = "";
            List<Node> adjList = GetAdjacencyList();
            for (int vertex = 0; vertex < adjList.Count(); ++vertex)
            {
                List<Tuple<int, int>> adjSet = adjList.ElementAt(vertex).GetAdjVertices();
                foreach (var tuple in adjSet)
                {
                    stringToReturn += adjList.ElementAt(vertex).GetVertexId().ToString() + " --> " + tuple.Item1.ToString() + ", cost: " + tuple.Item2.ToString() + "\n";
                }
            }
            return stringToReturn;
        }
        public int GetMaxNodeID() //returns the highest Node ID
        {
            int maxId = -10;
            foreach (Node node in VertexSet)
            {
                if (node.GetVertexId() > maxId)
                {
                    maxId = node.GetVertexId();
                }
            }
            return maxId;
        }
        public List<List<int>> GetAdjacencyMatrix() //returns the adjacency matrix as a 2d list
        {
            int size = GetMaxNodeID() + 1; //need a size one greater since the nodes start from 0
            List<List<int>> adjMatrix = new List<List<int>>(); //where we will store the matrix
            for (int row = 0; row < size; ++row)
            {
                adjMatrix.Add(new List<int>());
                //set all entries to -1 first
                for (int column = 0; column < size; ++column)
                {
                    adjMatrix[row].Add(-1);
                }
            }
            foreach (Node node in VertexSet)
            {
                //intialise all entries which will have a connection
                foreach (Tuple<int, int> adjacentVertex in node.GetAdjVertices())
                {
                    adjMatrix[node.GetVertexId()][adjacentVertex.Item1] = adjacentVertex.Item2; //update the position in the matrix 
                    adjMatrix[adjacentVertex.Item1][node.GetVertexId()] = adjacentVertex.Item2;//both ways
                }
            }
            return adjMatrix;
        }
        public List<int> GetOddVertices() //returns a list of all the odd valency vertices (for route inspection)
        {
            List<int> oddVertices = new List<int>();
            foreach (Node vertex in VertexSet)
            {
                if (GetValency(vertex.GetVertexId()) % 2 == 1)
                {
                    oddVertices.Add(vertex.GetVertexId()); //once identified, append to list
                }
            }
            return oddVertices;
        }
        private bool IsValidEdge(int u, int v, bool[] isInMst) //Check if the passed in edge is available for use in the MST
        {
            if (u == v) //a vertex to itself is not valid
                return false;
            if (isInMst[u] == false && isInMst[v] == false) //exactly one of the vertices need to be in the MST so return false
                return false;
            else if (isInMst[u] == true && isInMst[v] == true)//exactly one of the vertices need to be in the MST so return false
                return false;
            return true;
        }
        public int GetNumberOfEdges()
        {
            return GetSumValency() / 2;
        }
        public bool ContainsCycle()
        {
            foreach (Node vertex in VertexSet)
            {
                if (DepthFirst(vertex.GetVertexId()).Item2) //Start from every node incase the graph is disconnected and only looks at one part
                {
                    return true; //if a cycle is found at anypoint, return true;
                }
            }
            return false; //if a cycle wasnt found, then return false;
        }
        public bool IsConnected() //returns true if the graph is connected, false if not
        {
            if (GetNumberOfVertices() == 1)
            {
                return true; //a graph with 1 node is connected
            }
            List<Tuple<int, int>> DFSresult = DepthFirst(GetMaxNodeID()).Item1;
            if (DFSresult.Count() == GetNumberOfVertices() - 1)
                return true; //if the result of DFS includes all of the vertices it is connected
            return false;
        }
        public int GetPathCost(List<int> path) //returns the cost of a path
        {
            int pathCost = 0;
            for (int i = 0; i < path.Count() - 1; ++i)
            {
                int edgeWeight = GetEdgeWeight(path[i], path[i + 1]); //get the weight between the vertices
                if (edgeWeight == -1) //it returns -1 if theres no edge
                {
                    pathCost = -1; //set cost to -1
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
            int numOddVertices = 0;
            foreach (Node vertex in VertexSet)
            {
                if (GetValency(vertex.GetVertexId()) % 2 == 1)
                {
                    numOddVertices += 1;
                }
            }
            if (numOddVertices == 2)
                return true;
            return false;
        }
        public bool IsEulerian() //returns true if the graph is eulerian
        {
            foreach (Node vertex in VertexSet)
            {
                if (GetValency(vertex.GetVertexId()) % 2 == 1)
                {
                    return false; //returns false if an odd vertex is found
                }
            }
            return true; //returns true since no odd vertices were found
        }
        private int GetMax(int a, int b) //gets the larger value
        {
            return (a > b) ? a : b;
        }
        private int GetMin(int a, int b)
        {
            return (a < b) ? a : b; //gets the smaller value
        }
    }
}
