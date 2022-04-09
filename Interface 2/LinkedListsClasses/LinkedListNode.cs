using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2.LinkedListsClasses
{
    class LinkedListNode
    {
        public int data;
        public LinkedListNode next;

        public LinkedListNode(int v)
        {
            data = v;
            next = null;
        }
    }
}
