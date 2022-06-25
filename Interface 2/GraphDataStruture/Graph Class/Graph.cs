using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    [Serializable]
    public partial class Graph
    {
        public int NumberOfDeletedVertices; //number of vertices that have been deleted
        private int NumberOfVertices; //number of vertices in graph
        private int idOfNodetoAdd = 0; //makes sure that the ID of all nodes are unique, as it will be incremented
        private List<Node> VertexSet; //Adjacency List made up of vertices
        private List<Tuple<int, int, int>> listOfEdges = null; //list of edges in graph (u,v,weight)
        public string Name { get; set; } //name of graph
        public Graph() //constructor
        {
            this.NumberOfVertices = 0;
            this.NumberOfDeletedVertices = 0;
            this.VertexSet = new List<Node>();
            listOfEdges = new List<Tuple<int, int, int>>();
        }
    }
}

