using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    [Serializable]
    public class Node
    {
        private int VertexId; //The unique ID of the node
        private List<Tuple<int, int>> AdjSet; //Hashset - each element is a tuple containing 2 items - .Item1 represents adjacent vertex's ID, .Item2 represents Edge weight
        public MyPoint Position { get; set; } //a point class instance which represents its position on the canvas
        public Node(int vertexId, double x, double y) //constructor
        {
            this.VertexId = vertexId; //initialise vertex ID
            this.AdjSet = new List<Tuple<int, int>>(); //initialise the Hashset
            Position = new MyPoint(x, y);
        }
        public void AddEdge(int NeighbourIdIndex, int Weight) //A method which adds connections from this vertex to other vertices
        {
            if (EdgeExists(NeighbourIdIndex)) //check if edge exists so we can replace it
            {
                foreach (Tuple<int, int> tuple in AdjSet) //loop through all tuples
                {
                    if (tuple.Item1 == NeighbourIdIndex) //if a match is found, that means a connection exists
                    {
                        RemoveEdge(NeighbourIdIndex); //if edge already exists, remove the edge
                        break; //very important to break when found: cant loop over a tuple again after its been editted in the same loop.
                    }
                }
            }
            this.AdjSet.Add(Tuple.Create(NeighbourIdIndex, Weight));//now replace the connection with the updated connection
        }
        public void RemoveEdge(int NeighbourIdIndex) //Method to remove the connection from this vertex to the passed in vertexID
        {
            Tuple<int, int> tupleToRemove = null; //make a temporary variable to store the tuple we need to remove
            foreach (Tuple<int, int> tuple in AdjSet) //loop through the adjacenct vertices
            {
                if (tuple.Item1 == NeighbourIdIndex) //if the vertex we want to remove matches the vertex in the tuple...
                {
                    tupleToRemove = tuple; //update the temporary variable - we cant edit a tuple inside of a running loop, it breaks
                }
            }
            if (tupleToRemove != null) //make sure we dont try and remove something thats not in the tuple
            {
                AdjSet.Remove(tupleToRemove);//remove the whole tuple from the adjacency set
            }
        }

        public bool EdgeExists(int NeighbourId) //method to check if a connection from this vertex to the passed in vertex exists
        {
            foreach (Tuple<int, int> tuple in AdjSet) // loop through this nodes adjacent Vertices
            {
                if (tuple.Item1 == NeighbourId) //if an edge has been found...
                {
                    return true;//return true for edge existing
                }
            }
            return false;//return false for no edge existing
        }
        public int GetWeight(int NeighbourIdIndex) //Method to get the cost of connection from this vertex to the passed in vertex
        {
            foreach (var tuple in AdjSet)//loop through the Adjacenct Vertices
            {
                if (tuple.Item1 == NeighbourIdIndex) //if theres an edge found here
                {
                    return tuple.Item2; //return the weight which is the second item in the tuple
                }
            }
            return -1; //if we get to this point, then NeighbourIdIndex was not a valid vertex as it wasnt found, so return -1
        }
        public List<Tuple<int, int>> GetAdjVertices() //Method to retreive all this vertex's neighbors
        {
            return this.AdjSet; //returns the adjacency set
        }
        public int GetVertexId() //Method to get the ID of this vertex
        {
            return this.VertexId; //retuns the ID of this vertex
        }
    }
}
