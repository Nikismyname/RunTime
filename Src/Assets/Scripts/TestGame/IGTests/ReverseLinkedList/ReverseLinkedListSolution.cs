
    public class ReverseLinkedListSolution
    {        
        public RTLinkedList Solve(RTLinkedList input)
        {
            this.Rec(input.First, null, input); 
            return input;
        }

        public void Rec(RTLinkedList.Node current, RTLinkedList.Node prev, RTLinkedList input)
        {
            if(current == null)
            {
                return; 
            }

            if(current.Next == null)
            {
                input.First = current; 
            }

            var oldNext = current.Next; 
            current.Next = prev;

            this.Rec(oldNext, current, input);
        }
    }
