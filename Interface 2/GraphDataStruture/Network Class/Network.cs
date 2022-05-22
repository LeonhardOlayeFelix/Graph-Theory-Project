using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    [Serializable]
    public partial class Network
    {
        public int NumberOfDeletedVertices; //keeps track of the number of vertices that have been deleted
        private int NumberOfVertices; //keeps track of the number of vertices
        private int idOfNodetoAdd = 0; //makes sure that the ID of all nodes are unique, as it will be incremented
        private List<Node> VertexSet; //represents the adjacency list: A list containing data of class Node, where each Node contains a HashSet of type Tuple<int, int> 
                                      //which represents all of its adjacent vertices and the weight.
        private List<Tuple<int, int, int>> listOfEdges = null; //keeps track of all of the edges in the graph
        public string Name { get; set; }
        public Network() //constructor
        {
            this.NumberOfVertices = 0;
            this.NumberOfDeletedVertices = 0;
            this.VertexSet = new List<Node>();
            listOfEdges = new List<Tuple<int, int, int>>();
        }
    }
}

