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
        /// completes kruskals algorithm, returning a list of edges within the minimum spannig tree tuple(vertex, vertex, cost)
        /// </summary>
        /// <returns></returns>
        public Tuple<List<Tuple<int, int, int>>, int> Kruskals() 
        {
            //uses Kruskals Algo for MST
            List<Tuple<int, int, int>> listOfSortedEdges = GetListOfSortedEdges();
            Graph mst = new Graph();
            for (int i = 0; i < GetMaxVertexID() + 1; i++)
            {
                mst.AddVertex(0,0);
                if (!IsInVertexList(i))
                {
                    mst.RemoveVertex(i);
                }
            }
            int successful = 0;
            for (int i = 0; i < listOfSortedEdges.Count(); ++i)
            {
                if (successful == mst.GetNumberOfVertices()) { break; }
                mst.AddEdge(listOfSortedEdges[i].Item1, listOfSortedEdges[i].Item2, listOfSortedEdges[i].Item3);
                if (mst.ContainsCycle())
                {
                    mst.RemoveEdge(listOfSortedEdges[i].Item1, listOfSortedEdges[i].Item2);
                }
                else if (!mst.ContainsCycle())
                {
                    successful++;
                }
            }
            List<Tuple<int, int, int>> mstEdges = mst.GetListOfSortedEdges();
            int cost = mst.GetSumOfWeights();
            return new Tuple<List<Tuple<int, int, int>>, int>(mstEdges, cost);
        }
        private List<Tuple<int, int, int>> MergeSort(List<Tuple<int, int, int>> edges) 
        {
            //recursively splits the list into right and left until the size is 1
            if (edges.Count() <= 1) 
            {
                //base case (where the size is 1)
                return edges; 
            } 

            List<Tuple<int, int, int>> left = new List<Tuple<int, int, int>>(); 
            List<Tuple<int, int, int>> right = new List<Tuple<int, int, int>>(); 

            for (int i = 0; i < edges.Count(); i++) 
            {
                //split list into two, using odd and even indexes 
                if (i % 2 == 0)
                {
                    left.Add(edges[i]); 
                }
                else
                {
                    right.Add(edges[i]);
                }
            }
            //recursion
            left = MergeSort(left); 
            right = MergeSort(right);

            //return merged and sorted list
            return Merge(left, right); 
        }
        private List<Tuple<int, int, int>> Merge(List<Tuple<int, int, int>> left, List<Tuple<int, int, int>> right) 
        {
            //merges the lists in increasing order
            List<Tuple<int, int, int>> result = new List<Tuple<int, int, int>>();
            while (NotEmpty(left) && NotEmpty(right)) 
            {
                //loop whilst both of the lists are non-empty
                if (left.First().Item3 <= right.First().Item3)
                {
                    //comparing by edge weights
                    UpdateResult(left, result);
                }
                else
                {
                    UpdateResult(right, result);
                }
            }

            //either left or right may still have elements so add to list
            while (NotEmpty(left))
            {
                UpdateResult(left, result);
            }
            while (NotEmpty(right)) 
            {
                UpdateResult(right, result);
            }
            return result; //return the sorted list
        }
        private void UpdateResult(List<Tuple<int, int, int>> LeftOrRight, List<Tuple<int, int, int>> result)
        {
            //add first element to the result list
            result.Add(LeftOrRight.First());
            //remove that first element from the list it was in
            LeftOrRight.RemoveAt(0); 
        }
        private bool NotEmpty(List<Tuple<int, int, int>> list)
        {
            //returns true if the list is not empty
            return list.Count != 0; 
        }
        /// <summary>
        /// Completes prims algorithm, returning a list of edges within the minimum spanning tree tuple(vertex, vertex, cost)
        /// </summary>
        /// <param name="startVertex"> The Vertex the minimum spanning tree starts from</param>
        /// <returns></returns>
        public List<Tuple<int, int, int>> Prims(int startVertex = -1) 
        {
            //uses Prims Algo for MST
            List<List<int>> adjMatrix = GetAdjacencyMatrix2();
            List<Tuple<int, int, int>> mstPath = new List<Tuple<int, int, int>>();
            //represents a value for each vertex which says if its in the MST
            bool[] isInMST = new bool[adjMatrix.Count()];
            if (startVertex == -1 || !IsInVertexList(startVertex))
            {
                //if start vertex wasnt specified choose first vertex as start
                isInMST[vertexSet.ElementAt(0).GetVertexId()] = true;
            }
            else 
            {
                //if a start vertex was specified
                for (int i = 0; i < vertexSet.Count(); ++i)
                {
                    if (vertexSet.ElementAt(i).GetVertexId() == startVertex)
                    {
                        isInMST[vertexSet.ElementAt(i).GetVertexId()] = true;
                    }
                }
            }

            int numVertex = adjMatrix.Count;
            int countedEdges = 0; //to know when to stop the loop
            int mstTotal = 0;
            int timeslooped = 0;
            while (countedEdges < GetNumberOfVertices() - 1) 
            {
                //keep going until n-1 edges have been added
                timeslooped += 1;
                if (timeslooped > GetNumberOfVertices())
                {
                    //if the number of times looped is >= the num of vertices,
                    //we are in an endless loop so halt this process
                    return new List<Tuple<int, int, int>>();
                }
                int minimum = int.MaxValue;
                int a = -1; 
                int b = -1;
                for (int i = 0; i < numVertex; i++)
                {
                    for (int j = 0; j < numVertex; j++)
                    {
                        if (adjMatrix[i][j] < minimum && adjMatrix[i][j] != -1) 
                        {
                            //if the cost is -1, there is no edge
                            if (IsValidEdge(i, j, isInMST))
                            {
                                //loop through adjMatrix and set the minimum once a valid edge is found
                                minimum = adjMatrix[i][j]; 
                                a = i;
                                b = j;
                            }
                        }
                    }
                }
                if (a != -1 && b != -1) 
                {
                    //if a and b are -1, then an edge was not found
                    mstPath.Add(Tuple.Create(GetMin(a, b), GetMax(a, b), minimum));
                    countedEdges++;
                    mstTotal += minimum;
                    isInMST[a] = true; 
                    isInMST[b] = true;
                }
            }
            return mstPath;
        }
    }
}
