using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    class UDLinkedList
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
        public void Append(Tuple<int, int> data)
        {
            UDLinkedListNode newNode = new UDLinkedListNode(data);
            if (head == null)  //if the list was empty
            {
                head = new UDLinkedListNode(data); //make the head the item
                Count++;//incremement count
                return;//stop execution
            }

            newNode.next = null; //if the list wasnt empty, get the 

            UDLinkedListNode last = head;
            while (last.next != null)
            {
                last = last.next;
            }
            last.next = newNode;
            Count++;
            return;
        }
        public Tuple<int, int> Pop()
        {
            if (head == null)
                return null; //if theres nothing in the list return null
            if (head.next == null)//if there was one item in the list, also return null
                return null;

            UDLinkedListNode secondLast = head; 
            while (secondLast.next.next != null) //finds the second to last item since .next.next is 2 items away
                secondLast = secondLast.next; //update secondLast
            Count--;//decrement the count
            Tuple<int, int> value = secondLast.next.data; //to return what was popped
            secondLast.next = null;//set the last item to null
            return value; //return what was popped
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
