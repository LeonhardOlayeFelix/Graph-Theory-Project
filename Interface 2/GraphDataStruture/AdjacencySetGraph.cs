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
        private List<Node> VertexSet; //represents the adjacency list: A list containing data of class Node, where each Node contains another HashSet of type Tuple<int, int> 
                                      //which represents all of its adjacent vertices and the weight.
        public string Name { get; set; }

        public AdjacencySetGraph() //constructor
        {
            this.NumberOfVertices = 0;
            this.NumberOfDeletedVertices = 0;
            this.VertexSet = new List<Node>();
        }
        public void AddEdge(int v1, int v2, int weight = 0) //Makes the input nodes adjacent to each other with a weight of the input
        {

            if (v1 == v2)
            {
                throw new ArgumentException("Cannot make a vertex adjacent to itself.");
            }
            if (!IsInVertexList(v1) || !IsInVertexList(v2)) //make sure that the vertex exists.
            {
                throw new ArgumentException("Vertex does not exist.");
            }
            List<int> vertexList = GetListOfVertices(); //get a list of the current vertices
            int v1Index = vertexList.IndexOf(v1);//gets the index of the vertex in the vertices list incase some vertices have been deleted.
            int v2Index = vertexList.IndexOf(v2);//gets the index of the vertex in the vertices list incase some vertices have been deleted.
            this.VertexSet.ElementAt(v1Index).AddEdge(v2, weight); //adds the edge using the AddEdge Method that the Nodes have
            this.VertexSet.ElementAt(v2Index).AddEdge(v1, weight); //Does it both ways since this is an undirected graph           
        }

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
        public void RemoveEdge(int v1, int v2)//function that deletes a connection
        {
            if (!IsInVertexList(v1) || !IsInVertexList(v2))//if the input vertex is not in the list
            {
                throw new ArgumentException("Vertex Does not exist");
            }
            else if (VertexSet.ElementAt(GetListOfVertices().IndexOf(v1)).EdgeExists(v2))//must use the index of v1 in the vertices list, incase any vertices have been deleted, as it will represent the ID too.
            {//need to make sure the edge exists first too
                List<int> vertexList = GetListOfVertices();//get list of vertices
                if (v1 == v2)
                {
                    throw new ArgumentException("An edge does not exist from itself to itself");
                }
                int v1Index = vertexList.IndexOf(v1);
                int v2Index = vertexList.IndexOf(v2);
                this.VertexSet.ElementAt(v1Index).RemoveEdge(v2);//pass in the index since some of the vertices may have been deleted
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
            return weight; //returns -1 if no edge was found
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
                            foreach (Tuple<int, int> tuple in node.GetAdjVertices())//loop through the hashset of tuples within each node
                            {
                                if (tuple.Item1 == vertexToRemove)//if the tuples item1 is the same as the vertex to remove, then its an edge that needs to be deleted
                                {
                                    RemoveEdge(node.GetVertexId(), vertexToRemove); //remove said edge
                                    break;//MUST do this since you cannot edit a tuple after its been editted inside of a loop
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
        public string coutValency(int v) //returns the string which represents the valency of vertex v
        {
            string stringToReturn = "";
            stringToReturn += "Valency of Vertex " + v.ToString() + ": " + GetValency(v).ToString() + "\n";
            return stringToReturn;
        }
        public string coutAdjList() //returns the string which represents the adjacency list
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
        private int selectMinVertex(List<int> value, List<bool> processed, int size) //gets the vertex that has the lowest value, that is not already processed
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
                cost += GetEdgeWeight(path.ElementAt(i), path.ElementAt(i + 1)); //get the cost of the shortest path
            }
            return Tuple.Create(path, cost);
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
        public bool IsConnected() //returns true if the graph is connected, false if not
        {
            if (GetNumberOfVertices() == 1)
            {
                return true; //a graph with 1 node is connected
            }
            List<Tuple<int, int, int>> primsRVal = Prims(); //if Prim's algorithm returns an empty list, then it means its not connected
            if (primsRVal.Count == 0)
            {
                return false;
            }
            return true; //if it gets here, the graph is connected
        }
        public List<Tuple<int, int, int>> Prims(int startVertex = -1) //returns the MST as a tuple(vertex, vertex, cost)
        {
            List<List<int>> adjMatrix = GetAdjacencyMatrix();
            List<Tuple<int, int, int>> mstPath = new List<Tuple<int, int, int>>();//return value
            bool[] isInMST = new bool[adjMatrix.Count()];//represents a value for each vertex which says if its in the MST
            if (startVertex == -1 || !IsInVertexList(startVertex)) //if a start vertex wasnt specified or if it was an invalid vertex
            {
                isInMST[VertexSet.ElementAt(0).GetVertexId()] = true;//first existing vertex is in MST
            }
            else //if a start vertex was specified
            {
                for (int i = 0; i < VertexSet.Count(); ++i)
                {
                    if (VertexSet.ElementAt(i).GetVertexId() == startVertex)
                    {
                        isInMST[VertexSet.ElementAt(i).GetVertexId()] = true;
                    }
                }
            }

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
        private static List<int> AddListAtoListB(List<int> listA, List<int> listB) //adding lists together - for route inspection partitioning
        {
            List<int> listResult = new List<int>() { };
            for (int i = 0; i < listA.Count; ++i)
            {
                listResult.Add(listA[i]); //add onto the end of listresult
            }
            for (int i = 0; i < listB.Count; ++i)
            {
                listResult.Add(listB[i]); //add onto the end of listresult
            }
            return listResult;
        }
        private static List<List<int>> AddListAtoListB(List<List<int>> listA, List<List<int>> listB) //adds two dimensional lists together for route inspection pair partitioning
        {
            List<List<int>> ResultTwoDimList = new List<List<int>>() { };
            for (int i = 0; i < listA.Count; ++i)
            {
                ResultTwoDimList.Add(listA[i]); //add onto the end of resulttwodimlist
            }
            for (int i = 0; i < listB.Count; ++i)
            {
                ResultTwoDimList.Add(listB[i]);//add onto the end of resulttwodimlist
            }
            return ResultTwoDimList;
        }
        private static List<int> SliceList(List<int> list, int start, int end) //cuts a list at specified interval and returns cut list
        {

            end = (end == -1) ? list.Count : end;  //passing in -1 means the end of the list
            start = (start == -1) ? 0 : start; //passing in -1 means the start of the list
            List<int> sublist = new List<int>() { };
            for (int i = start; i < end; ++i)
            {
                sublist.Add(list[i]); //takes the list at the specified positions
            }
            return sublist;
        } //c# version of slicing
        private static void OutputList(List<List<List<int>>> threedimlist)//output 3D list, for testing purposes
        {
            Console.Write("[");
            for (int i = 0; i < threedimlist.Count; ++i)
            {
                Console.Write("[");
                for (int j = 0; j < threedimlist[i].Count; ++j)
                {
                    Console.Write("(");
                    for (int k = 0; k < threedimlist[i][j].Count; ++k)
                    {
                        if (k == threedimlist[i][j].Count - 1)
                            Console.Write("{0}", threedimlist[i][j][k]);
                        else
                            Console.Write("{0},", threedimlist[i][j][k]);
                    }
                    if (j == threedimlist[i].Count - 1)
                        Console.Write(")");
                    else
                        Console.Write("),");
                }
                if (i == threedimlist.Count - 1)
                    Console.Write("]");
                else
                    Console.WriteLine("],");
            }
            Console.WriteLine("]");
        }
        private static List<List<List<int>>> Partition(List<int> a) //returns all the possible combinations in a 3 dimensional list
        {
            if (a.Count == 2) //if its two items, theres only one combination
            {
                List<List<List<int>>> temp = new List<List<List<int>>>() { new List<List<int>>() { new List<int>() { a[0], a[1] } } };
                return temp;
            }
            List<List<List<int>>> ret = new List<List<List<int>>>() { }; //return value
            for (int i = 1; i < a.Count; ++i)
            {
                List<List<int>> p1 = new List<List<int>>() { new List<int>() { a[0], a[i] } }; //split apart: [0,1] [2,3,4,5]
                List<int> rem = AddListAtoListB(SliceList(a, 1, i), SliceList(a, i + 1, -1)); //generate combinations of [2,3,4,5] recursively
                List<List<List<int>>> res = Partition(rem);
                foreach (var ri in res)
                {
                    ret.Add(AddListAtoListB(p1, ri)); //add that combination to [0,1]
                }
            }
            return ret;
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
        public Tuple<List<Tuple<int, int>>, int> GetOptimalCombination(List<int> oddVertices)
        {
            List<List<List<int>>> combinations = Partition(oddVertices); //partition the odd vertices into pairs
            List<List<Tuple<List<int>, int>>> CombinationsCost = new List<List<Tuple<List<int>, int>>>(); //example: [[#path, cost],[#path, cost]]
            for (int i = 0; i < combinations.Count(); ++i) //loop through each combination
            {
                CombinationsCost.Add(new List<Tuple<List<int>, int>>()); //create a new element so we can add the cost and path below
                for (int j = 0; j < combinations[i].Count(); ++j)
                {
                    CombinationsCost[i].Add(DijkstrasAlgorithmShort(combinations[i][j][0], combinations[i][j][1])); //populate the element just made
                }
            }
            int index = selectMinPairing(CombinationsCost); //returns the index of the lowest cost pairing
            //Console.WriteLine(k); ////checking
            List<Tuple<int, int>> edgesToRepeat = new List<Tuple<int, int>>(); //the edges that will need to be repeated
            List<Tuple<List<int>, int>> optimalCombo = new List<Tuple<List<int>, int>>();
            try
            {
                optimalCombo = CombinationsCost[index];//get the combination that had the lowest cost
            }
            catch
            {
                return null; //incase anything goes wrong
            };
            int total = 0;//the cost
            foreach (Tuple<List<int>, int> pathAndCost in optimalCombo)
            {
                total += pathAndCost.Item2;//update the cost
                List<int> path = pathAndCost.Item1; //get the path e.g (1,2,3,4,5)
                for (int i = 0; i < path.Count() - 1; ++i)
                {
                    edgesToRepeat.Add(Tuple.Create(GetMin(path[i], path[i + 1]), GetMax(path[i], path[i + 1])));//get the path as different edges, e.g.
                                                                                                                //(1,2),(2,3),(3,4),(4,5);
                }
            }
            return Tuple.Create(edgesToRepeat, total);
        }
        public Tuple<List<Tuple<int, int>>, int> RInspStartAtEnd() //item1 is the edges to repeat, item2 is the cost of repition
        {
            List<int> oddVertices = GetOddVertices();//get a list of all the odd vertices
            return GetOptimalCombination(oddVertices);
        }
        public Tuple<List<Tuple<int, int>>, int> RInspStartAndEnd(int startVertex, int endVertex) //item1 is the edges to repeat, item2 is the cost of repition
        {
            if (!IsInVertexList(startVertex) || !IsInVertexList(endVertex))
            {
                throw new Exception("Input vertex does not exist.");
            }
            else if (GetValency(startVertex) % 2 == 0 || GetValency(endVertex) % 2 == 0)
            {
                throw new Exception("Both the start vertex and end vertex need to have an odd valency");
            }
            List<int> oddVertices = GetOddVertices();//get a list of all the odd vertices
            oddVertices.Remove(startVertex); //the start and end vertex dont need to be made even
            oddVertices.Remove(endVertex);
            return GetOptimalCombination(oddVertices);
        }
        private int selectMinPairing(List<List<Tuple<List<int>, int>>> combinations) //returns the index of the lowest cost combination for route inspection
        {
            int min = int.MaxValue; //initialise the lowest cost to the minimum value
            int index = 0;//set the index to 0
            for (int i = 0; i < combinations.Count(); ++i)
            {
                int temp = 0; //set temp so it restarts every time

                for (int j = 0; j < combinations[i].Count(); ++j)
                {
                    temp += combinations[i][j].Item2; //add the cost of the combination temp
                }
                //Console.WriteLine(temp); ////checking
                if (temp < min)//if the temp was a lower cost...
                {
                    index = i;//set the new index
                    min = temp;//replace min
                }

            }
            return index;//return the index that the lowest combination is at
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

