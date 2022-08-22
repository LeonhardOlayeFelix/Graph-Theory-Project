using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    [Serializable]
    public class Vertex
    {
        private int VertexId; //The unique ID of the vertex

        /// <summary>
        /// This vertex's list of neighbours tuple(vertexID, cost)
        /// </summary>
        private List<Tuple<int, int>> AdjSet; 
        /// <summary>
        /// Position of the vertex in the canvas
        /// </summary>
        public MyPoint Position { get; set; } 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vertexId">Unique ID for the vertex to be assigned</param>
        /// <param name="x">x coordinate of vertex</param>
        /// <param name="y">y coordinate of vertex</param>
        public Vertex(int vertexId, double x, double y) 
        {
            //initialise attributes
            this.VertexId = vertexId; 
            this.AdjSet = new List<Tuple<int, int>>(); 
            Position = new MyPoint(x, y);
        }
        /// <summary>
        /// Creates a connection from this vertex to the passed in vertex
        /// </summary>
        /// <param name="vertex">The vertex that this vertex will connect to. pass in the position of the vertex in the vertex list</param>
        /// <param name="Weight">The weight that the edge will hold</param>
        public void AddEdge(int vertex, int Weight) 
        {
            //adds connections from this vertex to other vertices
            if (EdgeExists(vertex)) 
            {
                //check if edge already exists so we can replace it
                foreach (Tuple<int, int> tuple in AdjSet) 
                {
                    //loop through all neighbours
                    if (tuple.Item1 == vertex) 
                    {
                        //remove connection
                        RemoveEdge(vertex);
                        break; 
                    }
                }
            }
            //create new connection
            this.AdjSet.Add(Tuple.Create(vertex, Weight)); 
        }
        /// <summary>
        /// Removes a connection from this vertex to the passed in vertex
        /// </summary>
        /// <param name="vertex">the vertex that this vertex will be disconnected from. pass in the position of the vertex in the 
        /// vertex list</param>
        public void RemoveEdge(int vertex)
        {
            //remove connection from this vertex to other vertices
            Tuple<int, int> tupleToRemove = null; 
            foreach (Tuple<int, int> tuple in AdjSet) 
            {
                //loop through neighbours
                if (tuple.Item1 == vertex) 
                {
                    //if the connection is found update the temporary variable
                    tupleToRemove = tuple; 
                }
            }
            if (tupleToRemove != null) 
            {
                //remove the whole tuple from the adjacency set
                AdjSet.Remove(tupleToRemove);
            }
        }
        /// <summary>
        /// returns true if an edge exists from this vertex to the specfied vertex
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool EdgeExists(int vertex) 
        {
            //checks if a connection from this vertex to the passed in vertex exists
            foreach (Tuple<int, int> tuple in AdjSet) 
            {
                //loop through this vertices neighbours
                if (tuple.Item1 == vertex) 
                {
                    //if an edge is found return true
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// returns the weight between this vertex and the specified vertex
        /// </summary>
        /// <param name="vertex">Pass in the position of the vertex in the vertex list</param>
        /// <returns></returns>
        public int GetWeight(int vertex) 
        {
            //Method to get weight from this vertex to the passed in vertex
            foreach (var tuple in AdjSet)
            {
                //loop through neighbours
                if (tuple.Item1 == vertex) 
                {
                    //if an edge is found return the weight
                    return tuple.Item2;
                }
            }
            return -1; 
        }
        /// <summary>
        /// returns a list of vertices tuple(vertex, cost) that are all connected to this vertex
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int>> GetAdjVertices() 
        {
            //returns all this vertex's neighbors
            return this.AdjSet;
        }
        /// <summary>
        /// returns the ID of this vertex
        /// </summary>
        /// <returns></returns>
        public int GetVertexId() 
        {
            return this.VertexId; 
        }
    }
}
