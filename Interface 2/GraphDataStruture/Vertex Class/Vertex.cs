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

        //list of neighbours - tuple of 2 items, Item1: adjacent vertex's ID, Item2 represents Edge weight
        private List<Tuple<int, int>> AdjSet; 
        public MyPoint Position { get; set; } //position on canvas
        public Vertex(int vertexId, double x, double y) 
        {
            //initialise attributes
            this.VertexId = vertexId; 
            this.AdjSet = new List<Tuple<int, int>>(); 
            Position = new MyPoint(x, y);
        }
        public void AddEdge(int NeighbourIdIndex, int Weight) 
        {
            //adds connections from this vertex to other vertices
            if (EdgeExists(NeighbourIdIndex)) 
            {
                //check if edge already exists so we can replace it
                foreach (Tuple<int, int> tuple in AdjSet) 
                {
                    //loop through all neighbours
                    if (tuple.Item1 == NeighbourIdIndex) 
                    {
                        //remove connection
                        RemoveEdge(NeighbourIdIndex);
                        break; 
                    }
                }
            }
            //create new connection
            this.AdjSet.Add(Tuple.Create(NeighbourIdIndex, Weight)); 
        }
        public void RemoveEdge(int NeighbourIdIndex)
        {
            //remove connection from this vertex to other vertices
            Tuple<int, int> tupleToRemove = null; 
            foreach (Tuple<int, int> tuple in AdjSet) 
            {
                //loop through neighbours
                if (tuple.Item1 == NeighbourIdIndex) 
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

        public bool EdgeExists(int NeighbourId) 
        {
            //checks if a connection from this vertex to the passed in vertex exists
            foreach (Tuple<int, int> tuple in AdjSet) 
            {
                //loop through this vertices neighbours
                if (tuple.Item1 == NeighbourId) 
                {
                    //if an edge is found return true
                    return true;
                }
            }
            return false;
        }
        public int GetWeight(int NeighbourIdIndex) 
        {
            //Method to get weight from this vertex to the passed in vertex
            foreach (var tuple in AdjSet)
            {
                //loop through neighbours
                if (tuple.Item1 == NeighbourIdIndex) 
                {
                    //if an edge is found return the weight
                    return tuple.Item2;
                }
            }
            return -1; 
        }
        public List<Tuple<int, int>> GetAdjVertices() 
        {
            //returns all this vertex's neighbors
            return this.AdjSet;
        }
        public int GetVertexId() 
        {
            //retuns the ID of this vertex
            return this.VertexId; 
        }
    }
}
