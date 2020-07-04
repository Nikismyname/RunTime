//using System.Collections;
//using System.Collections.Generic;


//public class RTLinkedList : IEnumerable<int>
//{
//    public Node First { get; set; }

//    public void Add(int val)
//    {
//        if (this.First == null)
//        {
//            this.First = new Node(val);
//            return;
//        }

//        var currentNode = First;
//        while (true)
//        {
//            if (currentNode.Next == null)
//            {
//                currentNode.Next = new Node(val);
//                break;
//            }
//            currentNode = currentNode.Next;
//        }
//    }

//    public IEnumerator<int> GetEnumerator()
//    {
//        var currentNode = this.First;

//        while (currentNode != null)
//        {
//            yield return currentNode.Value;
//            currentNode = currentNode.Next;
//        }
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return GetEnumerator();
//    }

//    public class Node
//    {
//        public Node(int value)
//        {
//            this.Value = value;
//        }
//        public int Value { get; set; }
//        public Node Next { get; set; }
//    }
//}


