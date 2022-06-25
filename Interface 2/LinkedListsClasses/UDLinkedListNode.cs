using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public class UDLinkedListNode
    {
        public Tuple<int, int> data; //each node has a piece of data
        public UDLinkedListNode next;//and the node that it points to

        public UDLinkedListNode(Tuple<int, int> v) //constructor
        {
            data = v;
            next = null;
        }
    }
}
