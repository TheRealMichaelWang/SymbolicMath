using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymbolicMath.Simplify;

namespace SymbolicMath
{
    using SortLevel = SortLevel;

    public partial class Node
    { 
        internal string Hash(SortLevel sortLevel)
        {
            if(this is NumberNode)
            {
                return sortLevel == SortLevel.High ? string.Empty : (this as NumberNode).Value.ToString();
            }
            else if(this is VariableNode)
            {
                return "var+" + (this as VariableNode).Identifier;
            }
            else if(this is UniaryOperatorNode)
            {
                return (this as UniaryOperatorNode).Operator + (sortLevel == SortLevel.Low ? "+" + Left.Hash(sortLevel) : "");
            }
            else if(this is BinaryOperatorNode)
            {
                string toret = sortLevel == SortLevel.Low ? (this as BinaryOperatorNode).Operator + "+" : string.Empty;
                string lefthash;
                if((lefthash = (this as BinaryOperatorNode).Left.Hash(sortLevel)) != string.Empty)
                {
                    toret += lefthash;
                }
                string righthash;
                if((righthash = (this as BinaryOperatorNode).Right.Hash(sortLevel)) != string.Empty)
                {
                    if(lefthash == string.Empty)
                    {
                        toret += righthash;
                    }
                    else
                    {
                        toret += "+" + righthash;
                    }
                }
                return toret;
            }
            else if(this is PatternNode)
            {
                return (this as PatternNode).Type.ToString() + ((sortLevel == SortLevel.Low) ? string.Empty : (this as PatternNode).Key.ToString());
            }
            throw new NotImplementedException();
        }

        internal Node Sort(SortLevel level)
        {
            if(Left != null)
            {
                Left = Left.Sort(level);
            }
            if(Right != null)
            {
                Right = Right.Sort(level);
            }
            if(!(this is BinaryOperatorNode))
            {
                return Clone() as Node;
            }
            BinaryOperatorNode binOp = this as BinaryOperatorNode;
            if(binOp.Operator != BinaryOperator.Add && binOp.Operator != BinaryOperator.Subtract && binOp.Operator != BinaryOperator.Multiply && binOp.Operator != BinaryOperator.Divide)
            {
                return Clone() as Node;  
            }
            bool isSum = binOp.Operator == BinaryOperator.Add || binOp.Operator == BinaryOperator.Subtract;
            List<Tuple<Node, TreeAnalyzer.LinearChildTag>> children = TreeAnalyzer.LinearChildren(this, isSum ? BinaryOperator.Add : BinaryOperator.Multiply, isSum ? BinaryOperator.Subtract : BinaryOperator.Divide);
            List<List<Tuple<Node, TreeAnalyzer.LinearChildTag>>> groups = TreeAnalyzer.groupByHash(children, level);
            List<Tuple<Node, TreeAnalyzer.LinearChildTag>> groupedChildren = new List<Tuple<Node, TreeAnalyzer.LinearChildTag>>();
            foreach(List<Tuple<Node, TreeAnalyzer.LinearChildTag>> group in groups)
            {
                groupedChildren.Add(TreeAnalyzer.internalMultihang(group, isSum ? BinaryOperator.Add : BinaryOperator.Multiply, isSum ? BinaryOperator.Subtract : BinaryOperator.Divide));
            }
            return TreeAnalyzer.MultiHang(groupedChildren, isSum ? BinaryOperator.Add : BinaryOperator.Multiply, isSum ? BinaryOperator.Subtract : BinaryOperator.Divide);
        }
    }
}

namespace SymbolicMath.Simplify
{
    enum SortLevel
    {
        High,
        Low,
        Medium
    }

    static partial class TreeAnalyzer
    {
        public static List<List<Tuple<Node, TreeAnalyzer.LinearChildTag>>> groupByHash(List<Tuple<Node, TreeAnalyzer.LinearChildTag>> nodes, SortLevel level)
        {
            Dictionary<string, List<Tuple<Node, TreeAnalyzer.LinearChildTag>>> dict = new Dictionary<string, List<Tuple<Node, TreeAnalyzer.LinearChildTag>>>();
            foreach(Tuple<Node, TreeAnalyzer.LinearChildTag> node in nodes)
            {
                string hash = node.Item1.Hash(level);
                if(!dict.ContainsKey(hash))
                {
                    dict.Add(hash, new List<Tuple<Node, TreeAnalyzer.LinearChildTag>>());
                }
                dict[hash].Add(node);
            }
            List<List<Tuple<Node, TreeAnalyzer.LinearChildTag>>> toreturn = new List<List<Tuple<Node, TreeAnalyzer.LinearChildTag>>>();
            List<string> keys = dict.Keys.ToList();
            keys.Sort();
            foreach (string key in keys)
            {
                toreturn.Add(dict[key]);
            }
            return toreturn;
        }

        public static Node MultiHang(List<Tuple<Node, LinearChildTag>> nodes, BinaryOperator @operator, BinaryOperator inverse)
        {
            Tuple<Node, LinearChildTag> result = internalMultihang(nodes, @operator, inverse);
            if(result.Item2 == LinearChildTag.Keep)
            {
                return result.Item1;
            }
            switch (inverse)
            {
                case BinaryOperator.Subtract: return new UniaryOperatorNode(UniaryOperator.Negate, result.Item1);
                case BinaryOperator.Divide: return new BinaryOperatorNode(BinaryOperator.Divide, result.Item1, -1);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Tuple<Node, LinearChildTag> internalMultihang(List<Tuple<Node,LinearChildTag>> nodes, BinaryOperator @operator, BinaryOperator inverse)
        {
            if(nodes.Count == 1)
            {
                return nodes[0];
            }
            Tuple<Node, LinearChildTag> left = nodes[0];
            nodes.RemoveAt(0);
            Tuple<Node, LinearChildTag> right = internalMultihang(nodes, @operator, inverse);
            if(left.Item2 == LinearChildTag.Keep && right.Item2 == LinearChildTag.Keep)
            {
                return new Tuple<Node, LinearChildTag>(new BinaryOperatorNode(@operator, left.Item1, right.Item1), LinearChildTag.Keep);
            }
            else if(left.Item2 == LinearChildTag.Keep && right.Item2 == LinearChildTag.Inverted)
            {
                return new Tuple<Node, LinearChildTag>(new BinaryOperatorNode(inverse, left.Item1, right.Item1), LinearChildTag.Keep);
            }
            else if(left.Item2 == LinearChildTag.Inverted && right.Item2 == LinearChildTag.Keep)
            {
                return new Tuple<Node, LinearChildTag>(new BinaryOperatorNode(inverse, right.Item1, left.Item1), LinearChildTag.Keep);
            }
            else if(left.Item2 == LinearChildTag.Inverted && right.Item2 == LinearChildTag.Inverted)
            {
                return new Tuple<Node, LinearChildTag>(new BinaryOperatorNode(inverse, left.Item1, right.Item1), LinearChildTag.Inverted);
            }
            throw new NotImplementedException();
        }
    }
}