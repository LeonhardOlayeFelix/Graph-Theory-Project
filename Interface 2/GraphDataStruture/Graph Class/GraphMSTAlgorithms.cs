using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        public List<Tuple<int, int, int>> Kruskals() //uses Kruskals Algo for MST
        {
            List<Tuple<int, int, int>> listOfSortedEdges = GetListOfSortedEdges(); //use merge sort to get a list of the sorted edges.
            int successful = 0; //the amount of edges that were added without creating a cycle
            Graph mst = new Graph(); //create a temporary graph data structure
            for (int i = 0; i < GetMaxNodeID() + 1; ++i)//add the same amount of vertices as the current graph
            {
                mst.AddVertex(0, 0);
            }
            while (successful < GetListOfVertices().Count() - 1) //the mst is made when n - 1 successful edges have been added (n = num vertices)
            {
                Tuple<int, int, int> cheapestEdge = listOfSortedEdges.First();//choose the lowest cost edge (first element)
                listOfSortedEdges.RemoveAt(0); //update the list of sorted edges
                mst.AddEdge(cheapestEdge.Item1, cheapestEdge.Item2, cheapestEdge.Item3); //add that edge to the new graph
                if (mst.ContainsCycle())
                {
                    mst.RemoveEdge(cheapestEdge.Item1, cheapestEdge.Item2); //if that caused the new graph to have a cycle, then remove it
                }
                else
                {
                    successful += 1; //if it didnt cause a cycle, then increment successful
                }
            }
            List<Tuple<int, int, int>> mstEdges = mst.GetListOfSortedEdges(); //by here, the mst is made
            return mstEdges; //so return their sorted edges
        }
        private List<Tuple<int, int, int>> MergeSort(List<Tuple<int, int, int>> edges) //recursively splits the list into right and left until the size is 1
        {
            if (edges.Count() <= 1) { return edges; } //base case (where the size is 1)

            List<Tuple<int, int, int>> left = new List<Tuple<int, int, int>>(); //empty list for the left side
            List<Tuple<int, int, int>> right = new List<Tuple<int, int, int>>(); //empty list for the right side

            for (int i = 0; i < edges.Count(); i++) //splitting the list into two parts
            {
                if (i % 2 == 0)
                {
                    left.Add(edges[i]); //if i is even, add it to the left side
                }
                else
                {
                    right.Add(edges[i]); //if i is odd, add it to the right side
                }
            }
            left = MergeSort(left); //recursively repeat this process so you have lists with one element that will be merged and sorted
            right = MergeSort(right);
            return Merge(left, right); //merge these two lists while sorting them using the merge method
        }
        private List<Tuple<int, int, int>> Merge(List<Tuple<int, int, int>> left, List<Tuple<int, int, int>> right) //merges the lists in correct order
        {
            List<Tuple<int, int, int>> result = new List<Tuple<int, int, int>>(); //empty list which will store the sorted list
            while (NotEmpty(left) && NotEmpty(right)) //whilst both of the lists are non-empty
            {
                if (left.First().Item3 <= right.First().Item3) //we are comparing by the edge weight, so if left weight is less than right weight...
                {
                    UpdateResult(left, result); //see function definition
                }
                else
                {
                    UpdateResult(right, result); //see function definition
                }
            }

            //either the left or the right may still have elements in them so add them to the list
            while (NotEmpty(left)) //do this until the list is empty
            {
                UpdateResult(left, result); //see function definition
            }
            while (NotEmpty(right)) //do this until the list is empty
            {
                UpdateResult(right, result); //see function definition
            }
            return result; //return the sorted list
        }
        private void UpdateResult(List<Tuple<int, int, int>> LeftOrRight, List<Tuple<int, int, int>> result)
        {
            result.Add(LeftOrRight.First()); //add the first element we just compared (from left or right list)to be smaller to the result list
            LeftOrRight.RemoveAt(0); //then remove that first element from the list it was in (left or right list)
        }
        private bool NotEmpty(List<Tuple<int, int, int>> list)
        {
            return list.Count != 0; //returns true if the list is not empty
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
    }
}
