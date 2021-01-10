using System;
using System.Collections.Generic;

namespace SymbolicMath.Simplify
{
    static partial class TreeAnalyzer
    {
        public enum LinearChildTag
        { 
            Keep,
            Inverted
        }

        public static void Sort(List<Node> nodes, SortLevel level)
        {
            nodes.Sort((a, b) => a.Hash(level).CompareTo(b.Hash(level)));
        }

        private static List<Tuple<Node, LinearChildTag>> InvertResults(List<Tuple<Node, LinearChildTag>> old)
        {
            List<Tuple<Node, LinearChildTag>> results = new List<Tuple<Node, LinearChildTag>>();
            foreach(Tuple<Node, LinearChildTag> oldResult in old)
            {
                switch (oldResult.Item2)
                {
                    case LinearChildTag.Keep:
                        results.Add(new Tuple<Node, LinearChildTag>(oldResult.Item1, LinearChildTag.Inverted));
                        break;
                    case LinearChildTag.Inverted:
                        results.Add(new Tuple<Node, LinearChildTag>(oldResult.Item1, LinearChildTag.Keep));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return results;
        }

        public static List<Tuple<Node, LinearChildTag>> LinearChildren(Node node, BinaryOperator @operator, BinaryOperator inverse)
        {
            List<Tuple<Node,LinearChildTag>> results = new List<Tuple<Node, LinearChildTag>>();
            if (node is BinaryOperatorNode)
            {
                BinaryOperatorNode binOp = node as BinaryOperatorNode;
                if (binOp.Operator == @operator || binOp.Operator == inverse)
                {
                    results.AddRange(LinearChildren(binOp.Left, @operator, inverse));
                    if (binOp.Operator == @operator)
                    {
                        results.AddRange(LinearChildren(binOp.Right, @operator, inverse));
                    }
                    else
                    {
                        results.AddRange(InvertResults(LinearChildren(binOp.Right, @operator, inverse)));
                    }
                }
                else
                {
                    results.Add(new Tuple<Node, LinearChildTag>(node,LinearChildTag.Keep));
                }
            }
            else if(node is UniaryOperatorNode)
            {
                UniaryOperatorNode uniOp = node as UniaryOperatorNode;
                if(uniOp.Operator == UniaryOperator.Negate && inverse == BinaryOperator.Subtract)
                {
                    results.AddRange(InvertResults(LinearChildren(uniOp.Left, @operator, inverse)));
                }
                else if(uniOp.Operator == UniaryOperator.Negate && @operator == BinaryOperator.Multiply)
                {
                    results.Add(new Tuple<Node, LinearChildTag>(1, LinearChildTag.Keep));
                    results.AddRange(LinearChildren(uniOp.Left, @operator, inverse));
                }
                else
                {
                    results.Add(new Tuple<Node, LinearChildTag>(uniOp, LinearChildTag.Keep));
                }
            }
            else
            {
                results.Add(new Tuple<Node, LinearChildTag>(node, LinearChildTag.Keep));
            }
            return results;
        }
    }
}
