using System;
using System.Collections.Generic;

namespace SymbolicMath.Simplify
{
    static partial class TreeAnalyzer
    {
        public static void Sort(List<Node> nodes, SortLevel level)
        {
            nodes.Sort((a, b) => a.Hash(level).CompareTo(b.Hash(level)));
            nodes.Reverse();
        }

        public static List<Node> LinearChildren(Node node, BinaryOperator @operator, BinaryOperator inverse)
        {
            List<Node> results = new List<Node>();
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
                        List<Node> invertedResults = LinearChildren(binOp.Right, @operator, inverse);
                        foreach (Node inverted in invertedResults)
                        {
                            switch (@operator)
                            {
                                case BinaryOperator.Add:
                                    results.Add(new UniaryOperatorNode(UniaryOperator.Negate, inverted));
                                    break;
                                case BinaryOperator.Multiply:
                                    results.Add(new BinaryOperatorNode(BinaryOperator.Power, inverted, -1));
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    }
                }
                else
                {
                    results.Add(node);
                }
            }
            else if(node is UniaryOperatorNode)
            {
                UniaryOperatorNode uniOp = node as UniaryOperatorNode;
                if(uniOp.Operator == UniaryOperator.Negate)
                {
                    List<Node> unrefinedResults = LinearChildren(uniOp.Left, @operator, inverse);
                    if(@operator == BinaryOperator.Multiply)
                    {
                        results.Add(-1);
                        results.AddRange(unrefinedResults);
                    }
                    else
                    {
                        results.Add(uniOp);
                    }
                }
                else
                {
                    results.Add(uniOp);
                }
            }
            else
            {
                results.Add(node);
            }
            return results;
        }
    }
}
