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
        /// completes the route inspection algorithm which takes a start vertex and an end vertex returning a list of edges to repeat
        /// and the total cost of repitition as tuple(list(vertex, vertex), cost)
        /// </summary>
        /// <param name="startVertex">the vertex where the algorithm will choose as its start point</param>
        /// <param name="endVertex">the vertex where the algorithm will choose as its end point</param>
        /// <returns></returns>
        public Tuple<List<Tuple<int, int>>, int> RInspStartAndEnd(int startVertex, int endVertex) 
        {
            //Route inspection starting and ending at same vertex
            if (!IsInVertexList(startVertex) || !IsInVertexList(endVertex))
            {
                //error handling - existence
                throw new Exception("Input vertex does not exist.");
            }
            else if (GetValency(startVertex) % 2 == 0 || GetValency(endVertex) % 2 == 0)
            {
                //error handling - validation
                throw new Exception("Both the start vertex and end vertex need to have an odd valency");
            }
            List<int> oddVertices = GetOddVertices();
            //the start and end vertex dont need to be made even
            oddVertices.Remove(startVertex); 
            oddVertices.Remove(endVertex);
            return GetOptimalCombination(oddVertices);
        }
        /// <summary>
        /// completes the route inspection algorithm where the start vertex and end vertex are the same so a start vertex is not
        /// required. returns a list of edges to repeat and the total cost of repitition as tuple(list(vertex, vertex), cost)
        /// </summary>
        /// <returns></returns>
        public Tuple<List<Tuple<int, int>>, int> RInspStartAtEnd() 
        {
            //Route inspection starting and ending at same vertex
            List<int> oddVertices = GetOddVertices();
            return GetOptimalCombination(oddVertices);
        }
        /// <summary>
        /// finds the optimal combination of edges to repeat for the route inspection algorithm
        /// </summary>
        /// <param name="oddVertices">A list of all the vertices with an odd valency</param>
        /// <returns></returns>
        private Tuple<List<Tuple<int, int>>, int> GetOptimalCombination(List<int> oddVertices)
        {

            //partition the odd vertices into pairs, 3d list
            List<List<List<int>>> combinations = Partition(oddVertices);

            //populate with combinations list and costs example: [[#path, cost],[#path, cost]]
            List<List<Tuple<List<int>, int>>> CombinationsCost = new List<List<Tuple<List<int>, int>>>(); 

            for (int i = 0; i < combinations.Count(); ++i) 
            {
                //loop through each combination

                //create a new element so we can add the cost and path below
                CombinationsCost.Add(new List<Tuple<List<int>, int>>()); 
                for (int j = 0; j < combinations[i].Count(); ++j)
                {
                    //populate the element just made with cheapest way of connecting vertices
                    CombinationsCost[i].Add(DijkstrasAlgorithmShort(combinations[i][j][0], combinations[i][j][1]));
                }
            }
            //index of the lowest cost pairing
            int index = selectMinPairing(CombinationsCost); 
           
            List<Tuple<int, int>> edgesToRepeat = new List<Tuple<int, int>>();
            List<Tuple<List<int>, int>> optimalCombo = new List<Tuple<List<int>, int>>();
            try
            {
                //get the combination that had the lowest cost
                optimalCombo = CombinationsCost[index];
            }
            catch
            {
                //error handling
                return null; 
            };
            int cost = 0;
            foreach (Tuple<List<int>, int> pathAndCost in optimalCombo)
            {
                //evaluate the cost of repitition and the edges to repeat
                cost += pathAndCost.Item2;
                List<int> path = pathAndCost.Item1;
                for (int i = 0; i < path.Count() - 1; ++i)
                {
                    edgesToRepeat.Add(Tuple.Create(GetMin(path[i], path[i + 1]), GetMax(path[i], path[i + 1])));//get the path as different edges, e.g.
                                                                                                                //(1,2),(2,3),(3,4),(4,5);
                }
            }
            return Tuple.Create(edgesToRepeat, cost);
        }
        /// <summary>
        /// chooses the cheapest out of all of the possible combination of edges in the route inspection algorithm
        /// </summary>
        /// <param name="combinations"></param>
        /// <returns></returns>
        private int selectMinPairing(List<List<Tuple<List<int>, int>>> combinations) 
        {
            //returns the index of the lowest cost combination for route inspection
            int min = int.MaxValue;
            int index = 0;
            for (int i = 0; i < combinations.Count(); ++i)
            {
                //reinitialise temp in each loop
                int temp = 0;

                for (int j = 0; j < combinations[i].Count(); ++j)
                {
                    //add the cost of the combination to temp
                    temp += combinations[i][j].Item2; 
                }
                if (temp < min)
                {
                    //if the temp was a lower cost...
                    index = i;
                    min = temp;
                }

            }
            return index;//return the index that the lowest combination is at
        }
        private static List<List<T>> AddListAtoListB<T>(List<List<T>> listA, List<List<T>> listB) 
        {
            //add two dimensional lists together for route inspection
            List<List<T>> ResultTwoDimList = new List<List<T>>() { };
            for (int i = 0; i < listA.Count; ++i)
            {
                ResultTwoDimList.Add(listA[i]);
            }
            for (int i = 0; i < listB.Count; ++i)
            {
                ResultTwoDimList.Add(listB[i]);
            }
            return ResultTwoDimList;
        }
        private static List<T> AddListAtoListB<T>(List<T> listA, List<T> listB)
        {
            //add one dimensional lists together for route inspection
            List<T> listResult = new List<T>() { };
            for (int i = 0; i < listA.Count; ++i)
            {
                listResult.Add(listA[i]); 
            }
            for (int i = 0; i < listB.Count; ++i)
            {
                listResult.Add(listB[i]); 
            }
            return listResult;
        }
        /// <summary>
        /// A c# version of the python slicing function on lists. Splits a list into parts using specified starting and end points.
        /// </summary>
        /// <param name="list">The list that will be spliced</param>
        /// <param name="start">where to start splicing from. passing in -1 indicates the start of the list</param>
        /// <param name="end">where to end splicing from. passing in -1 indicates the end of the list</param>
        /// <returns></returns>
        private static List<T> SliceList<T>(List<T> list, int start, int end) 
        {
            //cuts a list at specified interval and returns cut list
            end = (end == -1) ? list.Count : end;  //passing in -1 for end means the end of the list
            start = (start == -1) ? 0 : start; //passing in -1 for start means the start of the list
            List<T> sublist = new List<T>() { };
            for (int i = start; i < end; ++i)
            {
                sublist.Add(list[i]);
            }
            return sublist;
        }
        /// <summary>
        /// Takes a list of type T and returns all of the different ways they can be paired together using recursion
        /// </summary>
        /// <param name="list">The list that will be partitioned</param>
        /// <returns></returns>
        private static List<List<List<T>>> Partition<T>(List<T> list) 
        {
            //returns all the possible combinations in a 3 dimensional list
            if (list.Count == 2) //base case - two items have one combination
            {
                List<List<List<T>>> temp = new List<List<List<T>>>() { new List<List<T>>() { new List<T>() { list[0], list[1] } } };
                return temp;
            }
            List<List<List<T>>> ret = new List<List<List<T>>>() { }; //return value
            for (int i = 1; i < list.Count; ++i)
            {
                //split apart: [0,1] [2,3,4,5]
                List<List<T>> p1 = new List<List<T>>() { new List<T>() { list[0], list[i] } };

                //generate combinations of [2,3,4,5] recursively
                List<T> temp = AddListAtoListB(SliceList(list, 1, i), SliceList(list, i + 1, -1)); 

                List<List<List<T>>> result = Partition(temp);
                foreach (var combo in result)
                {
                    ret.Add(AddListAtoListB(p1, combo)); //add those combination to [0,1]
                }
            }
            return ret;
        }
    }
}
