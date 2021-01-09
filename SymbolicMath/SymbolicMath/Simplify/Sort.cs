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
            List<Node> children = TreeAnalyzer.LinearChildren(this, isSum ? BinaryOperator.Add : BinaryOperator.Multiply, isSum ? BinaryOperator.Subtract : BinaryOperator.Divide);
            List<List<Node>> groups = TreeAnalyzer.groupByHash(children, level);
            List<Node> groupedChildren = new List<Node>();
            foreach(List<Node> group in groups)
            {
                groupedChildren.Add(TreeAnalyzer.MultiHang(group, isSum ? BinaryOperator.Add : BinaryOperator.Multiply));
            }
            return TreeAnalyzer.MultiHang(groupedChildren, isSum ? BinaryOperator.Add : BinaryOperator.Multiply);
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
        public static List<List<Node>> groupByHash(List<Node> nodes, SortLevel level)
        {
            Dictionary<string, List<Node>> dict = new Dictionary<string, List<Node>>();
            foreach(Node node in nodes)
            {
                string hash = node.Hash(level);
                if(!dict.ContainsKey(hash))
                {
                    dict.Add(hash, new List<Node>());
                }
                dict[hash].Add(node);
            }
            List<List<Node>> toreturn = new List<List<Node>>();
            List<string> keys = dict.Keys.ToList();
            keys.Sort();
            keys.Reverse();
            foreach (string key in keys)
            {
                toreturn.Add(dict[key]);
            }
            return toreturn;
        }

        public static Node MultiHang(List<Node> nodes, BinaryOperator @operator)
        {
            if(nodes.Count == 1)
            {
                return nodes[0];
            }
            return new BinaryOperatorNode(@operator, nodes[0], MultiHang(nodes.GetRange(1, nodes.Count - 1), @operator));
        }
    }
}