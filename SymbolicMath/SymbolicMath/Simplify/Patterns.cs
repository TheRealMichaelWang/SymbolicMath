﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymbolicMath.Simplify;

namespace SymbolicMath
{
    partial class Node
    {
        internal abstract bool MatchPattern(Node pattern, Dictionary<int, Node> matchings);
    }

    partial class BinaryOperatorNode
    {
        internal override bool MatchPattern(Node pattern, Dictionary<int, Node> matchings)
        {
            if(pattern is BinaryOperatorNode)
            {
                BinaryOperatorNode binOp = pattern as BinaryOperatorNode;
                if(binOp.Operator != Operator)
                {
                    return false;
                }
                if(!Left.MatchPattern(binOp.Left,matchings) || !Right.MatchPattern(binOp.Right,matchings))
                {
                    return false;
                }
                return true;
            }
            else if(pattern is PatternNode)
            {
                PatternNode patternNode = pattern as PatternNode;
                if(patternNode.Type == PatternType.Any)
                {
                    if (!matchings.ContainsKey(patternNode.Key))
                    {
                        matchings[patternNode.Key] = this;
                        return true;
                    }
                    else
                    {
                        return matchings[patternNode.Key].Equals(this);
                    }
                }
            }
            return false;
        }
    }

    partial class UniaryOperatorNode
    {
        internal override bool MatchPattern(Node pattern, Dictionary<int, Node> matchings)
        {
            if(pattern is UniaryOperatorNode)
            {
                UniaryOperatorNode uniOp = pattern as UniaryOperatorNode;
                if(uniOp.Operator != Operator)
                {
                    return false;
                }
                if(!Left.MatchPattern(uniOp.Left, matchings))
                {
                    return false;
                }
                return true;
            }
            else if (pattern is PatternNode)
            {
                PatternNode patternNode = pattern as PatternNode;
                if (patternNode.Type == PatternType.Any)
                {
                    if (!matchings.ContainsKey(patternNode.Key))
                    {
                        matchings[patternNode.Key] = this;
                        return true;
                    }
                    else
                    {
                        return matchings[patternNode.Key].Equals(this);
                    }
                }
            }
            return false;
        }
    }

    partial class VariableNode
    {
        internal override bool MatchPattern(Node pattern, Dictionary<int, Node> matchings)
        {
            if(pattern is PatternNode)
            {
                PatternNode patternNode = pattern as PatternNode;
                if(patternNode.Type == PatternType.Any || patternNode.Type == PatternType.Variable)
                {
                    if (!matchings.ContainsKey(patternNode.Key))
                    {
                        matchings[patternNode.Key] = this;
                        return true;
                    }
                    else
                    {
                        return matchings[patternNode.Key].Equals(this);
                    }
                }
                return false;
            }
            else if(pattern is VariableNode)
            {
                return pattern.Equals(this);
            }
            return false;
        }
    }

    partial class NumberNode
    {
        internal override bool MatchPattern(Node pattern, Dictionary<int, Node> matchings)
        {
            if (pattern is PatternNode)
            {
                PatternNode patternNode = pattern as PatternNode;
                if (patternNode.Type == PatternType.Any || patternNode.Type == PatternType.Constant)
                {
                    if (!matchings.ContainsKey(patternNode.Key))
                    {
                        matchings[patternNode.Key] = this;
                        return true;
                    }
                    else
                    {
                        return matchings[patternNode.Key].Equals(this);
                    }
                }
                return false;
            }
            else if(pattern is NumberNode)
            {
                return pattern.Equals(this);
            }
            return false;
        }
    }
}

namespace SymbolicMath.Simplify
{
    enum PatternType
    { 
        Constant,
        Variable,
        Any
    }

    class Pattern
    {
        public static Pattern operator +(Pattern a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Add, a.Head, b.Head).Simplify(7));
        public static Pattern operator +(Pattern a, Node b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Add, a.Head, b).Simplify(7));
        public static Pattern operator +(Node a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Add, a, b.Head).Simplify(7));
        public static Pattern operator -(Pattern a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Subtract, a.Head, b.Head).Simplify(7));
        public static Pattern operator -(Pattern a, Node b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Subtract, a.Head, b).Simplify(7));
        public static Pattern operator -(Node a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Subtract, a, b.Head).Simplify(7));
        public static Pattern operator *(Pattern a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Multiply, a.Head, b.Head).Simplify(7));
        public static Pattern operator *(Pattern a, Node b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Multiply, a.Head, b).Simplify(7));
        public static Pattern operator *(Node a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Multiply, a, b.Head).Simplify(7));
        public static Pattern operator /(Pattern a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Divide, a.Head, b.Head).Simplify(7));
        public static Pattern operator /(Pattern a, Node b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Divide, a.Head, b).Simplify(7));
        public static Pattern operator /(Node a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Divide, a, b.Head).Simplify(7));
        public static Pattern operator ^(Pattern a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Power, a.Head, b.Head).Simplify(7));
        public static Pattern operator ^(Pattern a, Node b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Power, a.Head, b).Simplify(7));
        public static Pattern operator ^(Node a, Pattern b) => new Pattern(new BinaryOperatorNode(BinaryOperator.Power, a, b.Head).Simplify(7));

        public static readonly Pattern Any1 = new Pattern(new PatternNode(01, PatternType.Any));
        public static readonly Pattern Any2 = new Pattern(new PatternNode(02, PatternType.Any));
        public static readonly Pattern Any3 = new Pattern(new PatternNode(03, PatternType.Any));
        public static readonly Pattern Any4 = new Pattern(new PatternNode(04, PatternType.Any));
        public static readonly Pattern Var1 = new Pattern(new PatternNode(11, PatternType.Variable));
        public static readonly Pattern Var2 = new Pattern(new PatternNode(12, PatternType.Variable));
        public static readonly Pattern Var3 = new Pattern(new PatternNode(13, PatternType.Variable));
        public static readonly Pattern Constant1 = new Pattern(new PatternNode(21, PatternType.Constant));
        public static readonly Pattern Constant2 = new Pattern(new PatternNode(22, PatternType.Constant));
        public static readonly Pattern Constant3 = new Pattern(new PatternNode(23, PatternType.Constant));

        public Node Head { get; private set; }

        public Pattern(Node head)
        {
            this.Head = head;
        }
    }

    partial class PatternNode : Node
    {
        public int Key { get; private set; }
        public PatternType Type { get; private set; }

        public PatternNode(int key, PatternType type)
        {
            this.Key = key;
            this.Type = type;
        }
    }

    static partial class TreeAnalyzer
    {
        public static Dictionary<int, Node> PatternMatch(Pattern pattern, Node node)
        {
            Dictionary<int, Node> result = new Dictionary<int, Node>();
            if(!node.MatchPattern(pattern.Head, result))
            {
                return null;
            }
            return result;
        }

        public static void Simplify(Rule[] Ruleset, ref Node node)
        {
            string hash;
            HashSet<string> replaced = new HashSet<string>();
            while (!replaced.Contains(hash = node.Hash(SortLevel.Low)))
            {
                replaced.Add(hash);
                foreach (Rule rule in Ruleset)
                {
                    Node newnode;
                    if ((newnode = rule.Apply(node, Ruleset)) != null)
                    {
                        node = newnode;
                    }
                }
            }
        }

        public static Node BuildTree(Node node, Dictionary<int, Node> matchings, Rule[] Ruleset)
        {
            if(node is PatternNode)
            {
                PatternNode patternNode = node as PatternNode;
                return matchings[patternNode.Key];
            }
            else if(node is UniaryOperatorNode)
            {
                UniaryOperatorNode uniOp = node as UniaryOperatorNode;
                Node left = BuildTree(uniOp.Left, matchings, Ruleset);
                Simplify(Ruleset, ref left);
                return new UniaryOperatorNode(uniOp.Operator, left);
            }
            else if(node is BinaryOperatorNode)
            {
                BinaryOperatorNode binOp = node as BinaryOperatorNode;
                Node right = BuildTree(binOp.Left, matchings, Ruleset);
                Simplify(Ruleset, ref right);
                Node left = BuildTree(binOp.Right, matchings, Ruleset);
                Simplify(Ruleset, ref left);
                return new BinaryOperatorNode(binOp.Operator, right, left);
            }
            return node.Clone() as Node;
        }
    }
}
