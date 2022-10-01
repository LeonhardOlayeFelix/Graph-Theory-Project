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
        /// 
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
        /// Parametrically arranges a graph into a shape using properties of a circle
        /// </summary>
        /// <param name="center"> the coordinates of the place the vertices will revolve around</param>
        /// <param name="numInnerVertices">the number of vertices on the inner shape</param>
        /// <param name="radiusIncrement"></param>
        public void ArrangeGraph(MyPoint center, int numInnerVertices, int radiusIncrement)
        {
            int numberOfVerticesArranged = 0;
            int index = 0;
            int radius;
            List<int> ListVertices = GetListOfVertices();
            double numRadii = Math.Ceiling((double)GetNumberOfVertices() / (double)numInnerVertices);
            for (int i = 1; i < numRadii + 1; ++i)
            {
                radius = radiusIncrement * i;//after each shape has been made, increase the radius by the same amount
                double angle = 2.0 / (double)numInnerVertices * Math.PI;
                for (int j = 1; j < numInnerVertices + 1; ++j)
                {
                    if (numberOfVerticesArranged == GetNumberOfVertices())
                    {
                        break;
                    }
                    double theta = angle * j; //angle away from the positive real x axis

                    //center (a,b) means new x coordinate = a + cos(theta) and y coordinate = b + sin(theta)
                    MyPoint arrangedPoint = new MyPoint(center.X + radius * Math.Cos(theta), center.Y + radius * Math.Sin(theta));
                    int vertexID = ListVertices[index++];
                    SetCoordinate(vertexID, arrangedPoint);
                    numberOfVerticesArranged++;
                }
                numInnerVertices += 2;
            }
        }
        public void SetCoordinate(int vertexID, MyPoint mypoint)
        {
            foreach (Vertex vertex in vertexSet)
            {
                if (vertex.GetVertexId() == vertexID)
                {
                    vertex.Position = mypoint;
                }
            }
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
        /// <summary>
        /// Returns the weight on an edge between vertices
        /// </summary>
        /// <param name="vertex1">The Vertex that is on one end of the edge</param>
        /// <param name="vertex2">The vertex that is on the other end of the edge</param>
        /// <returns></returns>
        public int GetEdgeWeight(int vertex1, int vertex2)
        {
            //returns weight on an edge
            int weight = -1;
            foreach (Vertex vertex in vertexSet)
            {
                if (vertex.GetVertexId() == vertex1)
                {
                    weight = vertex.GetWeight(vertex2);
                }
            }
            //return -1 if there was no edge
            return weight;
        }
        /// <summary>
        /// Returns a list of all the edges in the graph
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int, int>> GetListOfEdges()
        {
            //returns list of edges
            return listOfEdges;
        }
        /// <summary>
        /// returns a merge sorted list of all the edges in the graph
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int, int>> GetListOfSortedEdges()
        {
            //merge sorts list of edges
            List<Tuple<int, int, int>> unsortedEdges = GetListOfEdges();
            List<Tuple<int, int, int>> sortedEdges = MergeSort(unsortedEdges);
            return sortedEdges;
        }
        /// <summary>
        /// returns the adjacency list
        /// </summary>
        /// <returns></returns>
        public List<Vertex> GetAdjacencyList()
        {
            //returns adjacency list
            return vertexSet;
        }
        /// <summary>
        /// returns the sum of all the weights in the graph
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns the number of vertices in the graph
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfVertices()
        {
            //returns the number of vertices
            return this.numberOfVertices;
        }
        /// <summary>
        /// returns the list of vertices in the graph
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns the valency of a vertex
        /// </summary>
        /// <param name="vertex1"></param>
        /// <returns></returns>
        public int GetValency(int vertex1)
        {
            //returns the valency of a vertex
            if (!IsInVertexList(vertex1))
            {
                //error handling - existence
                throw new ArgumentException("this vertex does not exist.");
            }
            else
            {
                foreach (Vertex node in vertexSet)
                {
                    //loop through adjacency list to find right vertex
                    if (node.GetVertexId() == vertex1)
                    {
                        return node.GetAdjVertices().Count();
                    }
                }
            }
            //flag
            return -1;
        }
        /// <summary>
        /// returns the total valency of the graph
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns the adjacency list as a output-able string
        /// </summary>
        /// <returns></returns>
        public string PrintAdjList()
        {
            //returns adjacency list as string
            string stringToReturn = "";
            List<int> listOfVertices = GetListOfVertices();
            for (int i = 0; i < listOfVertices.Count(); ++i)
            {
                stringToReturn += listOfVertices[i] + ": ";
                foreach (int vertex in GetAdjVertices(listOfVertices[i]))
                {
                    stringToReturn += "{" + vertex + "; " + GetEdgeWeight(i, vertex) + "},";
                }
                stringToReturn += "\n";
            }
            return stringToReturn;
        }
        /// <summary>
        /// returns the vertex with the highest ID
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns the adjacency matrix as a 2 dimensional list
        /// </summary>
        /// <returns></returns>
        public int[,] GetAdjacencyMatrix()
        {
            int nV = GetMaxVertexID() + 1;
            int[,] matrix = new int[nV, nV];
            int i, j;
            for (i = 0; i < nV; i++)
                for (j = 0; j < nV; j++)
                    matrix[i, j] = GetEdgeWeight(i, j);
            return matrix;
        }
        public List<List<int>> GetAdjacencyMatrix2() //returns the adjacency matrix as a 2d list
        {
            int size = GetMaxVertexID() + 1; //need a size one greater since the nodes start from 0
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
            foreach (Vertex vertex in vertexSet)
            {
                //intialise all entries which will have a connection
                foreach (Tuple<int, int> adjacentVertex in vertex.GetAdjVertices())
                {
                    adjMatrix[vertex.GetVertexId()][adjacentVertex.Item1] = adjacentVertex.Item2; //update the position in the matrix 
                    adjMatrix[adjacentVertex.Item1][vertex.GetVertexId()] = adjacentVertex.Item2;//both ways
                }
            }
            return adjMatrix;
        }
        public string PrintAdjMatrix()
        {
            int nV = GetMaxVertexID() + 1;
            int[,] matrix = new int[nV, nV];
            int i, j;
            for (i = 0; i < nV; i++)
                for (j = 0; j < nV; j++)
                    matrix[i, j] = GetEdgeWeight(i, j);
            int n = GetMaxVertexID() + 1;
            string table = "";
            for (i = 0; i < n; ++i)
            {
                for (j = 0; j < n; ++j)
                {
                    if (matrix[i, j] == -1)
                        table += "-, ";
                    else
                        table += matrix[i, j] + ", ";
                }
                table += "\n";
            }
            return table;
        }
        /// <summary>
        /// returns all of the vertices whose valency is odd
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns true if the passed in edge is available for use in the minimum spanning tree
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="isInMst"></param>
        /// <returns></returns>
        private bool IsValidEdge(int vertex1, int vertex2, bool[] isInMst)
        {
            //Check if the passed in edge is available for use in the MST
            if (vertex1 == vertex2)
            {
                //vertex to itself is not valid
                return false;
            }
            if (isInMst[vertex1] == false && isInMst[vertex2] == false)
            {
                //exactly one of the vertices need to be in the MST so return false
                return false;
            }
            else if (isInMst[vertex1] == true && isInMst[vertex2] == true)
            {
                //exactly one of the vertices need to be in the MST so return false
                return false;
            }
            return true;
        }
        /// <summary>
        /// return the number of edges in the graph
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfEdges()
        {
            //returns number of edges in graph
            return GetSumValency() / 2;
        }
        /// <summary>
        /// returns true if the graph contains a cycle
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns true if the graph is connected
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns the cost of a walk
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// <summary>
        /// returns true if the graph is semu eulerian
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns true if the graph is eulerian
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// returns larger number of a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int GetMax(int a, int b)
        {
            return (a > b) ? a : b;
        }
        /// <summary>
        /// returns smaller number of a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int GetMin(int a, int b)
        {
            return (a < b) ? a : b;
        }
    }
}

