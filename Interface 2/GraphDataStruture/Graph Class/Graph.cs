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
        //declare attributes
        public int numberOfDeletedVertices;
        private int numberOfVertices;
        private int iDVertex = 0; //this increments
        private List<Vertex> vertexSet; //Adjacency List 
        private List<Tuple<int, int, int>> listOfEdges = null; //list of edges in graph (u,v,weight)
        public string Name { get; set; }
        public Graph()
        {
            //initiliase attributes
            this.numberOfVertices = 0;
            this.numberOfDeletedVertices = 0;
            this.vertexSet = new List<Vertex>();
            this.listOfEdges = new List<Tuple<int, int, int>>();
            this.Name = "";
        }
    }
}

