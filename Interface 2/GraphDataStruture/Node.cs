using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public class Node
    {
        private int VertexId; //The unique ID of the node
        private List<Tuple<int, int>> AdjSet; //Hashset - each element is a tuple containing 2 items - .Item1 represents adjacent vertex's ID, .Item2 represents Edge weight

        public Node(int vertexId) //constructor
        {
            this.VertexId = vertexId; //initialise vertex ID
            this.AdjSet = new List<Tuple<int, int>>(); //initialise the Hashset
        }
        public void AddEdge(int NeighbourIdIndex, int Weight) //Method to add edge
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
        public void RemoveEdge(int NeighbourIdIndex) //Method to remove edge from here to NeighbourID
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

        public bool EdgeExists(int NeighbourId) //function to check if an edge to neighbourID exists
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
        public int GetWeight(int NeighbourIdIndex) //function to get the weight of the edge connecting this Node and NeighbourIDIndex
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
        public List<Tuple<int, int>> GetAdjVertices() //function to retreive all the adjacent vertices to this node
        {
            return this.AdjSet; //returns the adjacency set
        }
        public int GetVertexId() //function to get the ID of this vertex
        {
            return this.VertexId; //retuns the ID of this vertex
        }
        public string coutHashSet()
        {
            string stringToCout = "";
            stringToCout = "Node" + VertexId.ToString() + ":\n";
            for (int i = 0; i < AdjSet.Count(); ++i)
            {
                stringToCout += "Neighbour: " + AdjSet.ElementAt(i).Item1.ToString() + ", Cost: " + AdjSet.ElementAt(i).Item2.ToString() + "\n";
            }
            return stringToCout;
        }
    }
}
