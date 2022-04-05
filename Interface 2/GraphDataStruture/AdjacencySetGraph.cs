using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public class AdjacencySetGraph
    {
        private int NumberOfDeletedVertices; //keeps track of the number of vertices that have been deleted
        private int NumberOfVertices; //keeps track of the number of vertices
        private int idOfNodetoAdd = 0; //makes sure that the ID of all nodes are unique, as it will be incremented
        private List<Node> VertexSet; //represents the adjacency list: A hashset containing data of class Node, where each Node contains another HashSet of type Tuple<int, int> 
                           //which represents all of its adjacent vertices and the weight.
        public string Name { get; set; }

        public AdjacencySetGraph() //constructor
        {
            this.NumberOfVertices = 0;
            this.NumberOfDeletedVertices = 0;
            this.VertexSet = new List<Node>();
        }
        public void AddEdge(int v1, int v2, int weight = 0) //Makes a the input nodes adjacent to each other with a weight of the input
        {

            if (v1 == v2)
            {
                throw new ArgumentException("Cannot make a vertex adjacent to itself.");
            }
            if (!IsInVertexList(v1) || !IsInVertexList(v2)) //make sure that the vertex is actually a vertex.
            {
                throw new ArgumentException("Vertex does not exist.");
            }
            List<int> vertexList = GetListOfVertices(); //get a list of the current vertices
            int v1Index = vertexList.IndexOf(v1);//gets the index of the vertex in the vertices list incase some vertices have been deleted.
            int v2Index = vertexList.IndexOf(v2);//gets the index of the vertex in the vertices list incase some vertices have been deleted.
            this.VertexSet.ElementAt(v1Index).AddEdge(v2, weight); //add a tuple to each of the vertices, to represent that the vertices are adjacent to each other
            //this is where we should make the line appear on the screen from button v1 to button v2
            this.VertexSet.ElementAt(v2Index).AddEdge(v1, weight); //do it the other way aswell since the graph is undirected            
        }

        public bool IsInVertexList(int v)//function to check if a vertex actually exists.
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
        public void RemoveEdge(int v1, int v2)//function that deletes the tuple that says theres a connection
        {
            if (!IsInVertexList(v1) || !IsInVertexList(v2))//if the input vertex is not in the list
            {
                throw new ArgumentException("Vertex Does not exist");
            }
            else if (VertexSet.ElementAt(GetListOfVertices().IndexOf(v1)).EdgeExists(v2))//must use the index of v1 in the vertices list, incase any vertices have been deleted, as it will represent the ID too.
            {//also checks if a connection exists so we can remove it.
                List<int> vertexList = GetListOfVertices();//get list of vertices
                if (v1 == v2)
                {
                    throw new ArgumentException("An edge does not exist from itself to itself");
                }
                //else if (v1 >= this.NumberOfVertices + NumberOfDeletedVertices || v2 >= this.NumberOfVertices + NumberOfDeletedVertices || v1 < 0 || v2 < 0)//make sure that input vertex is in range
                //    throw new ArgumentOutOfRangeException("Vertex out of bounds"); ////not sure i even need this code
                int v1Index = vertexList.IndexOf(v1);
                int v2Index = vertexList.IndexOf(v2);
                this.VertexSet.ElementAt(v1Index).RemoveEdge(v2);//pass in the index since some of the vertices may have been deleted
                //this is where we should make the line disappear on the screen from button v1 to button v2
                this.VertexSet.ElementAt(v2Index).RemoveEdge(v1); //undirected graph
            }
            else
            {
                throw new ArgumentException($"No edge exists from {v1} to {v2}");
            }
        }
        public List<Tuple<int, int>> GetAdjVertices(int vertex) //gets the adjacent vertices of a vertex
        {

            if (!IsInVertexList(vertex)) //make sure that the input vertex exists first.
                throw new ArgumentOutOfRangeException("Vertex does note exist");
            return this.VertexSet.ElementAt(GetListOfVertices().IndexOf(vertex)).GetAdjVertices();
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
            return weight;
        }
        public List<Node> GetAdjacencyList()//gets the adjacency list of the graph
        {
            return VertexSet;
        }
        public void AddVertex(int amountToAdd = 1)//adds a vertex to the graph
        {
            for (int i = 0; i < amountToAdd; ++i)
            {
                VertexSet.Add(new Node(idOfNodetoAdd));//add a node to the hashset containing nodes
                idOfNodetoAdd += 1;//incrememnt id so all of the IDs are unique
                NumberOfVertices += 1;//update number of vertices
            }

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
        public int GetValency(int v1)//get the valency of each node (number of edges that are coming out of it.)
        {
            if (IsInVertexList(v1))//first, make sure the input vertex exists
            {
                foreach (Node node in VertexSet)
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
        public void RemoveVertex(int vertexToRemove)//remove a vertex
        {

            if (!IsInVertexList(vertexToRemove)) //make sure that the vertex exists first
            {
                throw new ArgumentOutOfRangeException("This vertex does not exist");
            }
            else //if it does...
            {
                for (int i = 0; i < NumberOfVertices; ++i)
                {
                    if (VertexSet.ElementAt(i).GetVertexId() == vertexToRemove)
                    {

                        //first remove all of the edges coming INTO the vertex
                        foreach (Node node in VertexSet)//loops through node
                        {
                            foreach (Tuple<int, int> tuple in node.GetAdjVertices())//loop through the hashset of tuples in within each node
                            {
                                if (tuple.Item1 == vertexToRemove)//if this is true, then an edge coming into the vertex has been found
                                {
                                    RemoveEdge(node.GetVertexId(), vertexToRemove); //removes said edge

                                    break;//MUST since you cannot edit a tuple after its been editted inside of a loop
                                }
                            }
                        }
                        VertexSet.Remove(VertexSet.ElementAt(i)); //now delete the vertex, since all incoming edges have been discarded
                        NumberOfDeletedVertices += 1;//update the number of deleted vertices
                        NumberOfVertices -= 1;//update the number of vertices
                        break;
                    }
                    else if (i == NumberOfVertices - 1)
                    {
                        throw new ArgumentException("This vertex does not exist");
                    }
                }
            }
        }
        public string coutHashSet(int vertex) //returns the string of the adjacency list
        {
            string stringToReturn = "";
            foreach (Node node in VertexSet)
            {
                if (node.GetVertexId() == vertex)
                {
                    stringToReturn = node.coutHashSet();
                }
            }
            return stringToReturn;
        }
        public string coutValency(int v) 
        {
            string stringToReturn = "";
            stringToReturn += "Valency of Vertex " + v.ToString() + ": " + GetValency(v).ToString() + "\n";
            return stringToReturn;
        }
        public string coutAdjList()
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
        public int GetMaxNodeID()
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
            int size = GetMaxNodeID() + 1;
            List<List<int>> adjMatrix = new List<List<int>>();
            for (int row = 0; row < size; ++row)
            {
                adjMatrix.Add(new List<int>());
                //set all entries to -1
                for (int column = 0; column < size; ++column)
                {
                    adjMatrix[row].Add(-1);
                }
            }
            foreach(Node node in VertexSet)
            {
                //intialise all entries which will have a connection
                foreach (Tuple<int,int> adjacentVertex in node.GetAdjVertices())
                {
                    adjMatrix[node.GetVertexId()][adjacentVertex.Item1] = adjacentVertex.Item2;
                    adjMatrix[adjacentVertex.Item1][node.GetVertexId()] = adjacentVertex.Item2;
                }
            }
            return adjMatrix;
        }
        public int selectMinVertex(List<int> value, List<bool> processed, int size) //gets the vertex that has the lowest value
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
                int minVertex = selectMinVertex(value, processed, size);
                processed[minVertex] = true;
                for (int column = 0; column < size; ++column)
                {
                    //checks for a present edge by comparing it with -1
                    //makes sure its not processed
                    //checks if it provides a lowered cost
                    if (adjMat[minVertex][column] != -1 && processed[column] == false && value[minVertex] != 2147483647 && (value[minVertex] + adjMat[minVertex][column] < value[column]))
                    {
                        value[column] = value[minVertex] + adjMat[minVertex][column];
                        parent[column] = minVertex;
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
                cost += GetEdgeWeight(path.ElementAt(i), path.ElementAt(i + 1));
            }
            return Tuple.Create(path, cost);
        }
        
        public List<int> GetOddVertices() //returns a list of odd valency vertices
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
        static bool IsValidEdge(int u, int v, bool[] isInMst) //Check if the passed in edge is available for use in the MST
        {
            if (u == v) //a vertex to itself is not valid
                return false;
            if (isInMst[u] == false && isInMst[v] == false) //exactly one of the vertices need to be in the MST so return false
                return false;
            else if (isInMst[u] == true && isInMst[v] == true)//exactly one of the vertices need to be in the MST so return false
                return false;
            return true;
        }
        public bool IsConnected()
        {
            if (GetNumberOfVertices() == 1)
            {
                return true;
            }
            List<Tuple<int, int, int>> primsRVal = Prims();
            if (primsRVal.Count == 0)
            {
                return false;
            }
            return true;
        }
        public List<Tuple<int, int, int>> Prims() //returns the MST as a tuple(vertex, vertex, cost)
        {
            List<List<int>> adjMatrix = GetAdjacencyMatrix();
            List<Tuple<int, int, int>> mstPath = new List<Tuple<int, int, int>>();//return value
            bool[] isInMST = new bool[adjMatrix.Count()];//represents a value for each vertex which says if its in the MST
            isInMST[VertexSet.ElementAt(0).GetVertexId()] = true;//first existing vertex is in MST
            int numVertex = adjMatrix.Count;
            int countedEdges = 0; //to know when to stop the loop
            int mstTotal = 0;
            int timeslooped = 0;
            while (countedEdges < GetNumberOfVertices() - 1) //we stop when there is one more vertices than edges
            {
                timeslooped += 1;
                if (timeslooped > GetNumberOfVertices())//if the number of times looped is >= the num of vertices, we are in an endless loop so halt this process
                {
                    return new List<Tuple<int, int, int>>();
                }
                int minimum = int.MaxValue; //set to the highest value
                int a = -1; //set to a value which wont be used
                int b = -1; //set to a value which wont be used
                for (int i = 0; i < numVertex; i++)
                {
                    for (int j = 0; j < numVertex; j++)
                    {
                        if (adjMatrix[i][j] < minimum && adjMatrix[i][j] != -1) //if the cost is -1, there is no edge
                        {
                            if (IsValidEdge(i, j, isInMST))
                            {
                                minimum = adjMatrix[i][j]; //loop through adjMatrix and set the minimum once a valid edge is found
                                a = i;
                                b = j;
                            }
                        }
                    }
                }
                if (a != -1 && b != -1) //if a and b are -1, then an edge was not found
                {
                    mstPath.Add(Tuple.Create(GetMin(a, b), GetMax(a, b), minimum)); //add this edge to the return value
                    countedEdges++;//increment the edge to stop infinite looping
                    mstTotal += minimum; //update the total cost
                    isInMST[a] = true; //update a
                    isInMST[b] = true;//update b
                }
            }
            return mstPath;
        }

        public int GetPathCost(List<int> path)
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

