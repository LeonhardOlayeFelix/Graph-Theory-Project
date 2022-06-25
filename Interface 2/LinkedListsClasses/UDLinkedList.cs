using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    class UDLinkedList
    {
        public int Count;
        UDLinkedListNode head; //each list has a head
        public UDLinkedList()
        {
            //at first the head will be null;
            head = null; 
            Count = 0; 
        }
        public void Push(Tuple<int, int> data) 
        {
            //push an element onto the front of a stack
            UDLinkedListNode node = new UDLinkedListNode(data);
            node.next = head;
            head = node;
            Count++;
        }
        public Tuple<int, int> Pop() 
        {
            //pop an element off the top of the stack
            if (head == null)
            {
                //if theres nothing in the list return null
                return null; 
            }
            if (head.next == null)
            {
                //if there was one item in the list, also return null
                return null;
            }

            UDLinkedListNode secondLast = head;
            while (secondLast.next.next != null)
            {
                //finds the second to last item
                secondLast = secondLast.next;
            }
            Count--;
            Tuple<int, int> value = secondLast.next.data;
            secondLast.next = null;
            return value;
        }
        public Tuple<int, int> DeQueue() 
        {
            //take an element off the front of a queue
            Tuple<int, int> value = head.data; 
            UDLinkedListNode node = head;
            //set the head to be the node after
            head = head.next;
            //set the old head to null
            node = null; 
            Count--;
            return value;
        }
        public void EnQueue(Tuple<int, int> data) 
        {
            //add element to back of queue
            UDLinkedListNode newNode = new UDLinkedListNode(data);
            if (head == null)  
            {
                //if the list was empty make the head the item
                head = new UDLinkedListNode(data); 
                Count++;
                return;
            }

            newNode.next = null; 

            UDLinkedListNode last = head;
            while (last.next != null)
            {
                last = last.next;
            }
            last.next = newNode;
            Count++;
            return;
        }
        public bool Contains(int value) 
        {
            //check whether a value is in any of the elements ITEM1 slot
            UDLinkedListNode runner = head;
            while (runner != null) 
            {
                //loop until reach the end of the list
                if (runner.data.Item1 == value) 
                {
                    //if a match is found return true
                    return true;
                }
                runner = runner.next;
            }
            return false;
        }
    }
}
