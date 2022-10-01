using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        public int[,] FloydWarshall()
        {
            int nV = GetMaxVertexID() + 1;
            int[,] matrix = new int[nV, nV];
            int i, j, k;
            for (i = 0; i < nV; i++)
                for (j = 0; j < nV; j++)
                    matrix[i, j] = (GetEdgeWeight(i, j) == -1) ? 10000 : GetEdgeWeight(i, j);
            for (k = 0; k < nV; k++)
            {
                for (i = 0; i < nV; i++)
                {
                    for (j = 0; j < nV; j++)
                    {
                        if (matrix[i, k] + matrix[k, j] < matrix[i, j] && i != j)
                        {
                            matrix[i, j] = matrix[i, k] + matrix[k, j];
                        }
                    }
                }
            }
            return matrix;
        }
        public string FloydWarshallStr()
        {
            int n = GetMaxVertexID() + 1;
            int[,] matrix = FloydWarshall();
            string table = "";
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (matrix[i, j] == 10000)
                        table += "0, ";
                    else
                        table += matrix[i, j] + ", ";
                }
                table += "\n";
            }
            return table;
        }
    }
}
