using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Network
    {
        public void AddVertex(double x, double y)//adds a vertex to the graph
        {
                VertexSet.Add(new Node(idOfNodetoAdd, x, y));//add a node to the hashset containing nodes
                idOfNodetoAdd += 1;//incrememnt id so all of the IDs are unique
                NumberOfVertices += 1;//update number of vertices
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
            listOfEdges.Add(Tuple.Create(v1, v2, weight)); //update list of edges
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
                int weight = GetEdgeWeight(v1, v2);
                this.VertexSet.ElementAt(v1Index).RemoveEdge(v2);//pass in the index since some of the vertices may have been deleted
                this.VertexSet.ElementAt(v2Index).RemoveEdge(v1); //undirected graph
                listOfEdges.Remove(Tuple.Create(v1, v2, weight)); //update list of edges
                listOfEdges.Remove(Tuple.Create(v2, v1, weight)); //update this way incase it was saved like this
            }
            else
            {
                throw new ArgumentException($"No edge exists from {v1} to {v2}");
            }
        }
        
    }
}
