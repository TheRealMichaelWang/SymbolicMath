using System;
using System.Collections.Generic;

namespace SymbolicMath
{
    public class Expression
    {
        public static implicit operator Expression(double value) => FromValue(value);
        public static implicit operator Expression(int value) => FromValue(Convert.ToDouble(value));

        public Node Head { get; internal set; }

        public static Expression FromVariable(string identifier)
        {
            return new Expression(new VariableNode(identifier));
        }

        public static Expression FromValue(double value)
        {
            return new Expression(new NumberNode(value));
        }

        public static Expression operator +(Expression a, Expression b)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Add, a.Head, b.Head).Simplify());
        }

        public static Expression operator -(Expression a, Expression b)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Subtract, a.Head, b.Head).Simplify());
        }

        public static Expression operator *(Expression a, Expression b)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Multiply, a.Head, b.Head).Simplify());
        }

        public static Expression operator /(Expression a, Expression b)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Divide, a.Head, b.Head).Simplify());
        }

        public static Expression operator ^(Expression a, Expression b)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Power, a.Head, b.Head).Simplify());
        }

        public static Expression operator -(Expression expression)
        {
            return new Expression(new UniaryOperatorNode(UniaryOperator.Negate, expression.Head));
        }

        public Expression(Node node)
        {
            this.Head = node;
        }

        public double Substitute(Dictionary<string, double> susbstituteValues)
        {
            return Head.Substitute(susbstituteValues);
        }

        public Expression Derive() => Derive("x");

        public Expression Derive(string variableIdentifier)
        {
            return new Expression(Head.Derive(variableIdentifier).Simplify());
        }

        public override string ToString()
        {
            return Head.ToString();
        }

        public override bool Equals(object obj)
        {
            if(obj is Expression)
            {
                return (obj as Expression).Head.Equals(Head);
            }
            return false;
        }
    }
}
