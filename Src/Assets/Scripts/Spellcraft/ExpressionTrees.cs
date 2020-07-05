using UnityEngine;
using System.Linq.Expressions;
using System;

public class ExpressionTrees : MonoBehaviour
{
    private void Start()
    {
        this.LambdaExpression<int, int>( null,
            new Node<int>()
            {
                Value = 12,
            },
            new Node<int>()
            {
                Value = 13,
            }
        );
    }

    public void BinaryExpression<T1, T2>(BinaryExpressionRoot root, Node<T1> one, Node<T2> two)
    {
        BinaryExpression exp = Expression.MakeBinary(ExpressionType.Add, Expression.Constant(42, typeof(int)), Expression.Constant(12, typeof(int))); 
        //Funk<int, int, int> some = exp.
    }

    public void LambdaExpression<T1, T2>(BinaryExpressionRoot root, Node<T1> nOne, Node<T2> nTwo)
    {
        MemberExpression one = Expression.Property(Expression.Constant(nOne, typeof(Node<T1>)), nameof(Node<T1>.Value));
        MemberExpression two = Expression.Property(Expression.Constant(nTwo, typeof(Node<T2>)), nameof(Node<T2>.Value));

        var some = Expression.Lambda(Expression.Add(one, two));

        Func<int> oneThreeHill = (Func<int>)some.Compile();
        Debug.Log(oneThreeHill());
    }

    public class BinaryExpressionRoot
    {

    }

    public class Node<T>
    {
        public NodeType Type { get; set; }

        public T Value { get; set; }

        public double Multy { get; set; }
    }

    public enum NodeType
    {
        Const,
        Variable,
    }

    public void Block()
    {
        // Creating a parameter expression.  
        ParameterExpression value = Expression.Parameter(typeof(int), "value");

        // Creating an expression to hold a local variable.
        ParameterExpression result = Expression.Parameter(typeof(int), "result");

        // Creating a label to jump to from a loop.  
        LabelTarget label = Expression.Label(typeof(int));

        // Creating a method body.  
        BlockExpression block = Expression.Block(
            // Adding a local variable.  
            new[] { result },
            // Assigning a constant to a local variable: result = 1  
            Expression.Assign(result, Expression.Constant(1)),
            // Adding a loop.  
            Expression.Loop(
                // Adding a conditional block into the loop.  
                Expression.IfThenElse(
                    // Condition: value > 1  
                    Expression.GreaterThan(value, Expression.Constant(1)),
                    // If true: result *= value --  
                    Expression.MultiplyAssign(result, Expression.PostDecrementAssign(value)),
                    // If false, exit the loop and go to the label.  
                    Expression.Break(label, result)
                ),
            // Label to jump to.  
            label
            )
        );

        // Compile and execute an expression tree.  
        int factorial = Expression.Lambda<Func<int, int>>(block, value).Compile()(5);

        Debug.Log(factorial);
    }
}

