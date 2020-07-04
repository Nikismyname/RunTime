//using System;
//using System.Reflection;

//public static class ReverseLinkedListTest
//{
//    public static bool Test(object classInstance, MethodInfo method)
//    {
//        const int count = 30;

//        var random = new Random();

//        var vals = new int[count];
//        for (int i = 0; i < count; i++)
//        {
//            vals[i] = random.Next(0, 100);
//        }

//        var expectedVals = new int[count];

//        Array.Copy(vals, expectedVals, count);
//        Array.Reverse(expectedVals);

//        var list = new RTLinkedList();

//        for (int i = 0; i < count; i++)
//        {
//            list.Add(vals[i]);
//        }

//        list = (RTLinkedList)method.Invoke(classInstance, new object[] { list });

//        var currentNode = list.First;
//        for (int i = 0; i < count; i++)
//        {
//            if (currentNode == null)
//            {
//                return false;
//            }

//            var val = expectedVals[i];
//            if (val != currentNode.Value)
//            {
//                return false;
//            }

//            currentNode = currentNode.Next;
//        }

//        return true;
//    }
//}
