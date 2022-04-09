using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public class UDLinkedList
    {
        public int Count; //keep track of and maintain the number of elements in the list
        UDLinkedListNode head; //each list has a head
        public UDLinkedList()
        {
            head = null; //at first the head will be null;
            Count = 0; //and the count will be 0
        }
        public void PushFront(Tuple<int, int> data) //push an element onto the front of a list
        {
            UDLinkedListNode node = new UDLinkedListNode(data);
            node.next = head;//this nodes link will now be this lists head
            head = node; //this lists head will now be this node
            Count++; //update count
        }
        public Tuple<int, int> PopFront() //pop an element off the front of a list, return what was popped
        {
            Tuple<int, int> value = head.data; //first get the head data so we can return it
            UDLinkedListNode node = head; //create a reference to the current head
            head = head.next;//set the head to be the node after
            node = null; //set the old head to null
            Count--;//update count
            return value;
        }
        public void PrintList() //for testing
        {
            UDLinkedListNode runner = head;
            while (runner != null)
            {
                Console.Write(runner.data + ", ");
                runner = runner.next;
            }
            Console.Write("\n");
        }
        public bool Contains(int value) //check whether a value is in any of the elements ITEM1 slot
        {
            UDLinkedListNode runner = head;
            while (runner != null) //loop until you reach the end of the list
            {
                if (runner.data.Item1 == value) //if a match is found
                {
                    return true;//return true;
                }
                runner = runner.next; //update runner
            }
            return false;
        }
    }
}
