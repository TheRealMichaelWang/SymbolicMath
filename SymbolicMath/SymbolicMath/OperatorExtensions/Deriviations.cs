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
            {BinaryOperator.Power, powerf.Derive },
            {BinaryOperator.Log, logf.Derive }
        };

        public static readonly Dictionary<UniaryOperator, UniaryDerivationFunction> uniaryDeriveTable = new Dictionary<UniaryOperator, UniaryDerivationFunction>()
        {
            {UniaryOperator.Sin, sinf.Derive },
            {UniaryOperator.Cos, cosf.Derive },
            {UniaryOperator.Tan, tanf.Derive },
            {UniaryOperator.Negate, negatef.Derive }
        };
    }

    static partial class addf
    {
        //(a + b)' = a' + b'
        public static Node Derive(Node left, Node right, string variabelIdentifier)
        {
            return left.Derive(variabelIdentifier) + right.Derive(variabelIdentifier);
        }
    }

    static partial class subtractf
    {
        //(a + b)' = a' - b'
        public static Node Derive(Node left, Node right, string variabelIdentifier)
        {
            return left.Derive(variabelIdentifier) - right.Derive(variabelIdentifier);
        }
    }

    static partial class multiplyf
    { 
        //(a * b)' = (a' * b + b' * a)
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            return (left.Derive(variableIdentifier) * right) + (right.Derive(variableIdentifier) * left);
        }
    }

    static partial class dividef
    {
        // (a / b)' = (a' * b - b' * a) / b^2
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            return ((left.Derive(variableIdentifier) * right) - (right.Derive(variableIdentifier) * left)) / (right ^ 2);
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
                return (right * (left ^ cons)) * left.Derive(variableIdentifier);
            }
            else if(left is NumberNode)
            {
                return (left ^ right) * new BinaryOperatorNode(BinaryOperator.Log, left, Math.E) * right.Derive(variableIdentifier);
            }
            return (left ^ right) * (left.Derive(variableIdentifier) * right / left + (new BinaryOperatorNode(BinaryOperator.Log, left, Math.E) * right.Derive(variableIdentifier)));
        }
    }

    static partial class logf
    {
        // log(a, b) = (ln(a) / ln(b))' = (ln(a)' * ln(b) - ln(a) * ln(b)') / ln(b)^2 = (a' / a * ln(b) - ln(a) * b' / b) / ln(b)^2
        public static Node Derive(Node left, Node right, string variableIdentifier)
        {
            return ((left.Derive(variableIdentifier) / left * new BinaryOperatorNode(BinaryOperator.Log, right, Math.E)) - (right.Derive(variableIdentifier) / right * new BinaryOperatorNode(BinaryOperator.Log, left, Math.E))) / (new BinaryOperatorNode(BinaryOperator.Log, right, Math.E) ^ 2);
        }
    }

    static partial class sinf
    { 
        //sin(a)' = cos(a) * a'
        public static Node Derive(Node node, string variableIdentifier)
        {
            return new UniaryOperatorNode(UniaryOperator.Cos, node) * node.Derive(variableIdentifier);
        }
    }

    static partial class cosf
    {
        //sin(a)' = -sin(a) * a'
        public static Node Derive(Node node, string variableIdentifer)
        {
            return (-new UniaryOperatorNode(UniaryOperator.Sin, node)) * node.Derive(variableIdentifer);
        }
    }

    static partial class tanf
    {
        //tan(a) = cos(a)^-2 * a'
        public static Node Derive(Node node, string variableIdentifier)
        {
            return (new UniaryOperatorNode(UniaryOperator.Cos, node) ^ 2) * node.Derive(variableIdentifier);
        }
    }

    static partial class negatef
    {
        public static Node Derive(Node node, string variableIdentifier)
        {
            return multiplyf.Derive(-1, node, variableIdentifier);
        }
    }
}
