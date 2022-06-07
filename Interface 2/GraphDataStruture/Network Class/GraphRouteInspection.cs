using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public partial class Graph
    {
        public Tuple<List<Tuple<int, int>>, int> RInspStartAndEnd(int startVertex, int endVertex) //item1 is the edges to repeat, item2 is the cost of repition
        {
            if (!IsInVertexList(startVertex) || !IsInVertexList(endVertex))
            {
                throw new Exception("Input vertex does not exist.");
            }
            else if (GetValency(startVertex) % 2 == 0 || GetValency(endVertex) % 2 == 0)
            {
                throw new Exception("Both the start vertex and end vertex need to have an odd valency");
            }
            List<int> oddVertices = GetOddVertices();//get a list of all the odd vertices
            oddVertices.Remove(startVertex); //the start and end vertex dont need to be made even
            oddVertices.Remove(endVertex);
            return GetOptimalCombination(oddVertices);
        }
        public Tuple<List<Tuple<int, int>>, int> RInspStartAtEnd() //item1 is the edges to repeat, item2 is the cost of repition
        {
            List<int> oddVertices = GetOddVertices();//get a list of all the odd vertices
            return GetOptimalCombination(oddVertices);
        }
        public Tuple<List<Tuple<int, int>>, int> GetOptimalCombination(List<int> oddVertices)
        {
            List<List<List<int>>> combinations = Partition(oddVertices); //partition the odd vertices into pairs
            List<List<Tuple<List<int>, int>>> CombinationsCost = new List<List<Tuple<List<int>, int>>>(); //example: [[#path, cost],[#path, cost]]
            for (int i = 0; i < combinations.Count(); ++i) //loop through each combination
            {
                CombinationsCost.Add(new List<Tuple<List<int>, int>>()); //create a new element so we can add the cost and path below
                for (int j = 0; j < combinations[i].Count(); ++j)
                {
                    CombinationsCost[i].Add(DijkstrasAlgorithmShort(combinations[i][j][0], combinations[i][j][1])); //populate the element just made
                }
            }
            int index = selectMinPairing(CombinationsCost); //returns the index of the lowest cost pairing
            //Console.WriteLine(k); ////checking
            List<Tuple<int, int>> edgesToRepeat = new List<Tuple<int, int>>(); //the edges that will need to be repeated
            List<Tuple<List<int>, int>> optimalCombo = new List<Tuple<List<int>, int>>();
            try
            {
                optimalCombo = CombinationsCost[index];//get the combination that had the lowest cost
            }
            catch
            {
                return null; //incase anything goes wrong
            };
            int total = 0;//the cost
            foreach (Tuple<List<int>, int> pathAndCost in optimalCombo)
            {
                total += pathAndCost.Item2;//update the cost
                List<int> path = pathAndCost.Item1; //get the path e.g (1,2,3,4,5)
                for (int i = 0; i < path.Count() - 1; ++i)
                {
                    edgesToRepeat.Add(Tuple.Create(GetMin(path[i], path[i + 1]), GetMax(path[i], path[i + 1])));//get the path as different edges, e.g.
                                                                                                                //(1,2),(2,3),(3,4),(4,5);
                }
            }
            return Tuple.Create(edgesToRepeat, total);
        }
        private int selectMinPairing(List<List<Tuple<List<int>, int>>> combinations) //returns the index of the lowest cost combination for route inspection
        {
            int min = int.MaxValue; //initialise the lowest cost to the minimum value
            int index = 0;//set the index to 0
            for (int i = 0; i < combinations.Count(); ++i)
            {
                int temp = 0; //set temp so it restarts every time

                for (int j = 0; j < combinations[i].Count(); ++j)
                {
                    temp += combinations[i][j].Item2; //add the cost of the combination temp
                }
                //Console.WriteLine(temp); ////checking
                if (temp < min)//if the temp was a lower cost...
                {
                    index = i;//set the new index
                    min = temp;//replace min
                }

            }
            return index;//return the index that the lowest combination is at
        }
        private static List<List<int>> AddListAtoListB(List<List<int>> listA, List<List<int>> listB) //adds two dimensional lists together for route inspection pair partitioning
        {
            List<List<int>> ResultTwoDimList = new List<List<int>>() { };
            for (int i = 0; i < listA.Count; ++i)
            {
                ResultTwoDimList.Add(listA[i]); //add onto the end of resulttwodimlist
            }
            for (int i = 0; i < listB.Count; ++i)
            {
                ResultTwoDimList.Add(listB[i]);//add onto the end of resulttwodimlist
            }
            return ResultTwoDimList;
        }
        private static List<int> AddListAtoListB(List<int> listA, List<int> listB) //adding lists together - for route inspection partitioning
        {
            List<int> listResult = new List<int>() { };
            for (int i = 0; i < listA.Count; ++i)
            {
                listResult.Add(listA[i]); //add onto the end of listresult
            }
            for (int i = 0; i < listB.Count; ++i)
            {
                listResult.Add(listB[i]); //add onto the end of listresult
            }
            return listResult;
        }
        private static List<int> SliceList(List<int> list, int start, int end) //cuts a list at specified interval and returns cut list
        {

            end = (end == -1) ? list.Count : end;  //passing in -1 means the end of the list
            start = (start == -1) ? 0 : start; //passing in -1 means the start of the list
            List<int> sublist = new List<int>() { };
            for (int i = start; i < end; ++i)
            {
                sublist.Add(list[i]); //takes the list at the specified positions
            }
            return sublist;
        } //c# version of slicing
        private static List<List<List<int>>> Partition(List<int> a) //returns all the possible combinations in a 3 dimensional list
        {
            if (a.Count == 2) //if its two items, theres only one combination
            {
                List<List<List<int>>> temp = new List<List<List<int>>>() { new List<List<int>>() { new List<int>() { a[0], a[1] } } };
                return temp;
            }
            List<List<List<int>>> ret = new List<List<List<int>>>() { }; //return value
            for (int i = 1; i < a.Count; ++i)
            {
                List<List<int>> p1 = new List<List<int>>() { new List<int>() { a[0], a[i] } }; //split apart: [0,1] [2,3,4,5]
                List<int> rem = AddListAtoListB(SliceList(a, 1, i), SliceList(a, i + 1, -1)); //generate combinations of [2,3,4,5] recursively
                List<List<List<int>>> res = Partition(rem);
                foreach (var ri in res)
                {
                    ret.Add(AddListAtoListB(p1, ri)); //add that combination to [0,1]
                }
            }
            return ret;
        }
    }
}
