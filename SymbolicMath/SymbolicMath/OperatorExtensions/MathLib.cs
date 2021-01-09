using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolicMath
{
    public class MathLib
    {
        public static Expression Sine(Expression expression)
        {
            return new Expression(new UniaryOperatorNode(UniaryOperator.Sin, expression.Head));
        }

        public static Expression Cosine(Expression expression)
        {
            return new Expression(new UniaryOperatorNode(UniaryOperator.Cos, expression.Head));
        }

        public static Expression Tangent(Expression expression)
        {
            return new Expression(new UniaryOperatorNode(UniaryOperator.Tan, expression.Head));
        }

        public static Expression Abs(Expression expression)
        {
            return new Expression(new UniaryOperatorNode(UniaryOperator.Abs, expression.Head));
        }

        public static Expression Log(Expression expression, Expression @base)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Log, expression.Head, @base.Head));
        }

        public static Expression Pow(Expression expression, Expression root)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Power, expression.Head, @root.Head));
        }

        public static Expression Root(Expression @base, Expression @root)
        {
            return new Expression(new BinaryOperatorNode(BinaryOperator.Power, @base.Head, new BinaryOperatorNode(BinaryOperator.Divide,1, @root.Head)));
        }
    }
}
