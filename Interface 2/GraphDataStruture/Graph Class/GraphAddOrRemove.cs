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
        /// Adds a vertex to the graph, also designating it a position
        /// </summary>
        /// <param name="x">x coordinate of vertex</param>
        /// <param name="y">y coordinate of vertex</param>
        public void AddVertex(double x, double y)
        {
            //adds a vertex to the adjacency list
            vertexSet.Add(new Vertex(iDVertex, x, y));
            iDVertex += 1;
            numberOfVertices += 1;
        }
        /// <summary>
        /// Removes a specified vertex from the graph
        /// </summary>
        /// <param name="vertexToRemove">The Vertex that will be removed from the graph</param>
        public void RemoveVertex(int vertexToRemove)
        {
            //remove a vertex from the graph
            if (!IsInVertexList(vertexToRemove)) 
            {
                //if the passed in vertex doesnt exist (error handling)
                throw new ArgumentOutOfRangeException("This vertex does not exist");
            }
            else 
            {
                //if the passed in vertex does exist
                for (int i = 0; i < numberOfVertices; ++i)
                {
                    if (vertexSet.ElementAt(i).GetVertexId() == vertexToRemove)
                    {
                        //first remove all of the edges coming into the vertex
                        foreach (Vertex vertex in vertexSet)
                        {
                            //looping through adjacency set
                            foreach (Tuple<int, int> tuple in vertex.GetAdjVertices())
                            {
                                //loop through each vertex's Neighbours
                                if (tuple.Item1 == vertexToRemove)
                                {
                                    //if an edge was found, remove it using another method
                                    RemoveEdge(vertex.GetVertexId(), vertexToRemove); 
                                    break;
                                }
                            }
                        }
                        //now all edges coming into vertex are discarded, delete vertex
                        vertexSet.Remove(vertexSet.ElementAt(i)); 
                        numberOfDeletedVertices += 1;
                        numberOfVertices -= 1;
                        break;
                    }
                    else if (i == numberOfVertices - 1)
                    {
                        //error handling
                        throw new ArgumentException("This vertex does not exist");
                    }
                }
            }
        }
        /// <summary>
        /// Creates a connection between two specified vertices known as an edge which holds a specified weight
        /// </summary>
        /// <param name="v1">Vertex on one end of the edge</param>
        /// <param name="v2">Vertex on the other end of the edge</param>
        /// <param name="weight">Weight on edge</param>
        public void AddEdge(int v1, int v2, int weight = 0, bool dashed = false)
        {
            //adds an edge between passed in vertices
            if (v1 == v2)
            {
                //error handling - no looped connections
                throw new ArgumentException("Cannot make a vertex adjacent to itself.");
            }
            if (!IsInVertexList(v1) || !IsInVertexList(v2)) 
            {
                //error handling - check for existence
                throw new ArgumentException("Vertex does not exist.");
            }
            //Create connection here
            List<int> vertexList = GetListOfVertices();
            listOfEdges.Remove(Tuple.Create(v1, v2, GetEdgeWeight(v1, v2)));
            listOfEdges.Remove(Tuple.Create(v2, v1, GetEdgeWeight(v1, v2))); //remove an edge that may already be present
            listOfEdges.Add(Tuple.Create(v1, v2, weight));

            //index of vertex in list - some vertices could be deleted.
            int v1Index = vertexList.IndexOf(v1);
            int v2Index = vertexList.IndexOf(v2);

            //Update the vertices instances themselves
            this.vertexSet.ElementAt(v1Index).AddEdge(v2, weight); 
            this.vertexSet.ElementAt(v2Index).AddEdge(v1, weight); 
            if (dashed)
            {
                listOfDashedEdges.Remove(Tuple.Create(v1, v2, GetEdgeWeight(v1, v2)));
                listOfDashedEdges.Remove(Tuple.Create(v2, v1, GetEdgeWeight(v1, v2))); //remove an edge that may already be present
                listOfDashedEdges.Add(Tuple.Create(v1, v2, weight));
                listOfDashedEdges.Add(Tuple.Create(v2, v1, weight));
            }
        }
        /// <summary>
        /// Removes an exisiting connection between two specified vertices
        /// </summary>
        /// <param name="v1">Vertex on one end of the edge</param>
        /// <param name="v2">Vertex on other end of the edge</param>
        public void RemoveEdge(int v1, int v2)
        {
            //removes an edge between vertices
            if (!IsInVertexList(v1) || !IsInVertexList(v2))
            {
                //error handling - existence
                throw new ArgumentException("Vertex Does not exist");
            }
            else if (!vertexSet.ElementAt(GetListOfVertices().IndexOf(v1)).EdgeExists(v2))
            {
                //error handling - existence
                throw new ArgumentException($"No edge exists from {v1} to {v2}");
            }
            else if (v1 == v2)
            {
                //error handling - no looped edges
                throw new ArgumentException("An edge does not exist from itself to itself");
            }
            else
            {
                //removes edge here
                List<int> vertexList = GetListOfVertices();
                int v1Index = vertexList.IndexOf(v1);
                int v2Index = vertexList.IndexOf(v2);
                int weight = GetEdgeWeight(v1, v2);
                //update the vertices instances themselves
                this.vertexSet.ElementAt(v1Index).RemoveEdge(v2); 
                this.vertexSet.ElementAt(v2Index).RemoveEdge(v1);

                //update list of edges
                listOfEdges.Remove(Tuple.Create(v1, v2, weight));

                //incase vertices were saved the other way
                listOfEdges.Remove(Tuple.Create(v2, v1, weight));

                //update list of edges
                listOfDashedEdges.Remove(Tuple.Create(v1, v2, weight));

                //incase vertices were saved the other way
                listOfDashedEdges.Remove(Tuple.Create(v2, v1, weight));
            }
        }
        
    }
}
