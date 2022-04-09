using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2.LinkedListsClasses
{
    class LinkedList
    {
        public int Count;
        LinkedListNode head;
        public LinkedList()
        {
            head = null;
            Count = 0;
        }
        public void PushFront(int data)
        {
            LinkedListNode node = new LinkedListNode(data);
            node.next = head;
            head = node;
            Count++;
        }
        public void PopFront()
        {
            LinkedListNode node = head;
            head = head.next;   
            node = null;
            Count--;
        }
        public void PrintList()
        {
            LinkedListNode runner = head;
            while (runner != null)
            {
                Console.WriteLine(runner.data);
                runner = runner.next;
            }
        }
    }
}
