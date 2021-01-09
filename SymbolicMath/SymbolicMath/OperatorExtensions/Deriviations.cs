using System;
using System.Collections.Generic;

namespace SymbolicMath.OperatorExtensions
{
    /*
     * I got all of this derivative rules from mathisfun.com
     */ 
    static partial class Functions
    {
        public delegate Node BinaryDeriviationFunction(Node left, Node right, string variableIdentifier);
        public delegate Node UniaryDerivationFunction(Node node, string variableIdentifier);

        public static readonly Dictionary<BinaryOperator, BinaryDeriviationFunction> binaryDeriveTable = new Dictionary<BinaryOperator, BinaryDeriviationFunction>()
        {
            {BinaryOperator.Add, addf.Derive },
            {BinaryOperator.Subtract, subtractf.Derive },
            {BinaryOperator.Multiply, multiplyf.Derive },
            {BinaryOperator.Divide, dividef.Derive },
            {BinaryOperator.Power, powerf.Derive }
        };

        public static readonly Dictionary<UniaryOperator, UniaryDerivationFunction> uniaryDeriveTable = new Dictionary<UniaryOperator, UniaryDerivationFunction>()
        {

        };
    }

    static partial class addf
    {
        //(a + b)' = a' + b'
        public static Node Derive(Node left, Node right, string variabelIdentifier)
        {
            return new BinaryOperatorNode(BinaryOperator.Add, left.Derive(variabelIdentifier), right.Derive(variabelIdentifier));
        }
    }

    static partial class subtractf
    {
        //(a + b)' = a' - b'
        public static Node Derive(Node left, Node right, string variabelIdentifier)
        {
            return new BinaryOperatorNode(BinaryOperator.Subtract, left.Derive(variabelIdentifier), right.Derive(variabelIdentifier));
        }
    }

    static partial class multiplyf
    { 
        //(a * b)' = (a' * b + b' * a)
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            return new BinaryOperatorNode(BinaryOperator.Add, new BinaryOperatorNode(BinaryOperator.Multiply, left.Derive(variableIdentifier), right), new BinaryOperatorNode(BinaryOperator.Multiply, right.Derive(variableIdentifier), left));
        }
    }

    static partial class dividef
    {
        // (a / b)' = (a' * b - b' * a) / b^2
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            return new BinaryOperatorNode(BinaryOperator.Divide,
            new BinaryOperatorNode(BinaryOperator.Subtract, new BinaryOperatorNode(BinaryOperator.Multiply, left.Derive(variableIdentifier), right), new BinaryOperatorNode(BinaryOperator.Multiply, right.Derive(variableIdentifier), left)), new BinaryOperatorNode(BinaryOperator.Power, right, 2));
        }
    }
    
    static partial class powerf
    {
        // (a ^ b)' = e ^ (ln(a) * b) * (a' * b / a + ln(a) * b')
        // (a ^ const)' = const * a ^ (const - 1)
        // (const ^ b)' = e^b * b'
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            if(right is NumberNode)
            {
                var cons = (right as NumberNode).Value - 1;
                return new BinaryOperatorNode(BinaryOperator.Multiply, right, new BinaryOperatorNode(BinaryOperator.Multiply, new BinaryOperatorNode(BinaryOperator.Power, left, cons), left.Derive(variableIdentifier)));
            }
            else if(left is NumberNode)
            {
                new BinaryOperatorNode(BinaryOperator.Multiply, new BinaryOperatorNode(BinaryOperator.Log, left, Math.E), new BinaryOperatorNode(BinaryOperator.Multiply, new BinaryOperatorNode(BinaryOperator.Power, left, right), right.Derive(variableIdentifier)));
            }
            return new BinaryOperatorNode(BinaryOperator.Multiply, new BinaryOperatorNode(BinaryOperator.Power, left, right), new BinaryOperatorNode(BinaryOperator.Multiply, new BinaryOperatorNode(BinaryOperator.Divide, right, new BinaryOperatorNode(BinaryOperator.Add, left, new BinaryOperatorNode(BinaryOperator.Log, left, Math.E))), new BinaryOperatorNode(BinaryOperator.Multiply, left.Derive(variableIdentifier), right.Derive(variableIdentifier))));
        }
    }

    static partial class logf
    {
        // log(a, b) = (ln(a) / ln(b))' = (ln(a)' * ln(b) - ln(a) * ln(b)') / ln(b)^2 = (a' / a * ln(b) - ln(a) * b' / b) / ln(b)^2
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            return new BinaryOperatorNode(BinaryOperator.Divide, new BinaryOperatorNode(BinaryOperator.Subtract, new BinaryOperatorNode(BinaryOperator.Divide, left.Derive(variableIdentifier), new BinaryOperatorNode(BinaryOperator.Multiply, left, new BinaryOperatorNode(BinaryOperator.Log, right, Math.E))), new BinaryOperatorNode(BinaryOperator.Multiply, new BinaryOperatorNode(BinaryOperator.Log, left, Math.E), new BinaryOperatorNode(BinaryOperator.Divide, right.Derive(variableIdentifier), right))), new BinaryOperatorNode(BinaryOperator.Power, new BinaryOperatorNode(BinaryOperator.Log, right, Math.E), 2));
        }
    }
}
