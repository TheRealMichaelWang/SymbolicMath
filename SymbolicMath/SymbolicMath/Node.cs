using System;
using System.Collections.Generic;
using SymbolicMath.OperatorExtensions;
using SymbolicMath.Simplify;

namespace SymbolicMath
{
    public enum UniaryOperator
    {
        Sin,
        Cos,
        Tan,
        Abs,
        Negate
    }

    public enum BinaryOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Power,
        Log
    }

    public abstract partial class Node : ICloneable
    {
        public static Node operator +(Node a, Node b) => new BinaryOperatorNode(BinaryOperator.Add, a, b);
        public static Node operator -(Node a, Node b) => new BinaryOperatorNode(BinaryOperator.Subtract, a, b);
        public static Node operator *(Node a, Node b) => new BinaryOperatorNode(BinaryOperator.Multiply, a, b);
        public static Node operator /(Node a, Node b) => new BinaryOperatorNode(BinaryOperator.Divide, a, b);
        public static Node operator ^(Node a, Node b) => new BinaryOperatorNode(BinaryOperator.Power, a, b);
        public static Node operator -(Node a) => new UniaryOperatorNode(UniaryOperator.Negate, a);

        public static implicit operator Node(double value) => new NumberNode(value);
        public static implicit operator Node(int value) => new NumberNode(Convert.ToDouble(value));

        public bool IsLeaf
        {
            get
            {
                return Left == null && Right == null;
            }
        }

        public Node Left;
        public Node Right;

        public Node()
        {
            Left = null;
            Right = null;
        }

        public Node(Node left, Node right)
        {
            this.Left = left;
            this.Right = right;
        }

        public abstract double Substitute(Dictionary<string, double> substituteValues);
        public abstract Node Eval();
        public abstract Node Simplify(int level);
        public abstract Node Derive(string variableIdentifier);
        public abstract object Clone();
        public abstract override bool Equals(object obj);
        public abstract override string ToString();

        public Node Derive(VariableNode variableNode) => Derive(variableNode.Identifier);
        public Node Simplify() => Simplify(1);
    }

    public partial class NumberNode : Node
    { 
        public double Value { get; internal set; }

        public NumberNode(double value):base()
        {
            this.Value = value;
        }

        public override double Substitute(Dictionary<string,double> substituteValues)
        {
            return Value;
        }

        public override Node Eval()
        {
            return this;
        }

        public override Node Simplify(int level)
        {
            return this;
        }

        public override Node Derive(string variableIdentifier)
        {
            return 0;
        }

        public override object Clone()
        {
            return new NumberNode(Value);
        }

        public override bool Equals(object obj)
        {
            if(obj is NumberNode)
            {
                return (obj as NumberNode).Value == Value;
            }
            else if(obj is int)
            {
                return Convert.ToDouble((int)obj) == Value;
            }
            else if(obj is double)
            {
                return (double)obj == Value;
            }
            return false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public partial class VariableNode : Node
    {
        public string Identifier { get; internal set; }

        public VariableNode(string identifier):base()
        {
            this.Identifier = identifier;
        }

        public override double Substitute(Dictionary<string, double> substituteValues)
        {
            return substituteValues[Identifier];
        }

        public override Node Eval()
        {
            return this;
        }

        public override Node Simplify(int level)
        {
            return this;
        }

        public override Node Derive(string variableIdentifier)
        {
            if(variableIdentifier == Identifier)
            {
                return 1;
            }
            return 0;
        }

        public override object Clone()
        {
            return new VariableNode(Identifier);
        }

        public override bool Equals(object obj)
        {
            if(obj is VariableNode)
            {
                return (obj as VariableNode).Identifier == Identifier;
            }
            return false;
        }

        public override string ToString()
        {
            return Identifier.ToString();
        }
    }

    public partial class BinaryOperatorNode : Node
    {
        public BinaryOperator Operator { get; internal set; }

        public BinaryOperatorNode(BinaryOperator @operator, Node left, Node right):base(left,right)
        {
            this.Operator = @operator;
        }

        public override double Substitute(Dictionary<string, double> substituteValues)
        {
            switch (Operator)
            {
                case BinaryOperator.Add:
                    return Left.Substitute(substituteValues) + Right.Substitute(substituteValues);
                case BinaryOperator.Subtract:
                    return Left.Substitute(substituteValues) - Right.Substitute(substituteValues);
                case BinaryOperator.Multiply:
                    return Left.Substitute(substituteValues) * Right.Substitute(substituteValues);
                case BinaryOperator.Divide:
                    return Left.Substitute(substituteValues) / Right.Substitute(substituteValues);
                case BinaryOperator.Power:
                    return Math.Pow(Left.Substitute(substituteValues), Right.Substitute(substituteValues));
                case BinaryOperator.Log:
                    return Math.Log(Left.Substitute(substituteValues)) / Math.Log(Right.Substitute(substituteValues));
                default:
                    throw new NotImplementedException();
            }
        }

        public override Node Eval()
        {
            return Functions.binaryEvalTable[Operator](Left, Right);
        }

        public override Node Simplify(int level)
        {
            Node res = this.Eval();
            for (int i = 0; i < level; i++)
            {
                switch (i)
                {
                    case 0:
                        res = res.Sort(SymbolicMath.Simplify.SortLevel.High);
                        break;
                    case 2:
                        res = res.Sort(SymbolicMath.Simplify.SortLevel.Medium);
                        break;
                    case 4:
                        res = res.Sort(SymbolicMath.Simplify.SortLevel.Low);
                        break;
                }
                res = res.Eval();
                TreeAnalyzer.Simplify(Rules.CommonRules, ref res);
            }
            return res.Eval();
        }

        public override Node Derive(string variableIdentifier)
        {
            return Functions.binaryDeriveTable[Operator](Left, Right, variableIdentifier);
        }

        public override object Clone()
        {
            return new BinaryOperatorNode(Operator, Left.Clone() as Node, Right.Clone() as Node);
        }

        public override string ToString()
        {
            switch (Operator)
            {
                case BinaryOperator.Add:
                    return "(" + Left.ToString() + " + " + Right.ToString() + ")";
                case BinaryOperator.Subtract:
                    return "(" + Left.ToString() + " - " + Right.ToString() + ")";
                case BinaryOperator.Multiply:
                    return "(" + Left.ToString() + " * " + Right.ToString() + ")";
                case BinaryOperator.Divide:
                    return "(" + Left.ToString() + " / " + Right.ToString() + ")";
                case BinaryOperator.Power:
                    return "(" + Left.ToString() + " ^ " + Right.ToString() + ")";
                case BinaryOperator.Log:
                    return "(" + Left.ToString() + " LOG " + Right.ToString() + ")";
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is BinaryOperatorNode)
            {
                BinaryOperatorNode binaryOperatorNode = obj as BinaryOperatorNode;
                if(binaryOperatorNode.Operator != Operator)
                {
                    return false;
                }
                if(!binaryOperatorNode.Left.Equals(Left))
                {
                    return false;
                }
                if(!binaryOperatorNode.Right.Equals(Right))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    public partial class UniaryOperatorNode : Node
    {
        public UniaryOperator Operator { get; private set; }
        
        public UniaryOperatorNode(UniaryOperator @operator, Node arg1):base(arg1,null)
        {
            this.Operator = @operator;
        }

        public override double Substitute(Dictionary<string, double> substituteValues)
        {
            switch (Operator)
            {
                case UniaryOperator.Abs:
                    return Math.Abs(Left.Substitute(substituteValues));
                case UniaryOperator.Sin:
                    return Math.Sin(Left.Substitute(substituteValues));
                case UniaryOperator.Cos:
                    return Math.Cos(Left.Substitute(substituteValues));
                case UniaryOperator.Tan:
                    return Math.Tan(Left.Substitute(substituteValues));
                case UniaryOperator.Negate:
                    return -Left.Substitute(substituteValues);
                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            switch (Operator)
            {
                case UniaryOperator.Abs:
                    return "abs(" + Left.ToString() + ")";
                case UniaryOperator.Sin:
                    return "sin(" + Left.ToString() + ")";
                case UniaryOperator.Cos:
                    return "cos(" + Left.ToString() + ")";
                case UniaryOperator.Tan:
                    return "tan(" + Left.ToString() + ")";
                case UniaryOperator.Negate:
                    return "-(" + Left.ToString() + ")";
                default:
                    throw new NotImplementedException();
            }
        }

        public override Node Eval()
        {
            return Functions.uniaryEvalTable[Operator](Left);
        }

        public override Node Simplify(int level)
        {
            return new UniaryOperatorNode(Operator, Left.Simplify(level));
        }

        public override Node Derive(string variableIdentifier)
        {
            return Functions.uniaryDeriveTable[Operator](Left, variableIdentifier);
        }

        public override object Clone()
        {
            return new UniaryOperatorNode(Operator,Left.Clone() as Node);
        }

        public override bool Equals(object obj)
        {
            if(obj is UniaryOperatorNode)
            {
                UniaryOperatorNode uniaryOperatorNode = (UniaryOperatorNode)obj;
                if(uniaryOperatorNode.Operator != Operator)
                {
                    return false;
                }
                if(!uniaryOperatorNode.Left.Equals(Left))
                {
                    return false;
                }
            }
            return false;
        }
    }
}

namespace SymbolicMath.Simplify
{
    partial class PatternNode
    {
        public override double Substitute(Dictionary<string, double> substituteValues)
        {
            throw new NotImplementedException();
        }

        public override Node Eval()
        {
            return this;
        }

        public override Node Simplify(int level)
        {
            throw new NotImplementedException();
        }

        public override Node Derive(string variableIdentifier)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            return new PatternNode(Key, Type);
        }

        public override bool Equals(object obj)
        {
            if(obj is PatternNode)
            {
                return (obj as PatternNode).Key == Key && (obj as PatternNode).Type == Type;
            }
            return false;
        }

        public override string ToString()
        {
            return "pat" + Key;
        }

        internal override bool MatchPattern(Node pattern, Dictionary<int, Node> matchings)
        {
            throw new NotImplementedException();
        }
    }
}