using System;
using System.Collections.Generic;
using SymbolicMath.Simplify;

namespace SymbolicMath.OperatorExtensions
{
    static partial class Functions
    {
        public delegate Node BinaryEvaluation(Node left, Node right);
        public delegate Node UniaryEvaluation(Node node);

        public static readonly Dictionary<BinaryOperator, BinaryEvaluation> binaryEvalTable = new Dictionary<BinaryOperator, BinaryEvaluation>()
        {
            {BinaryOperator.Add, addf.Eval },
            {BinaryOperator.Subtract, subtractf.Eval },
            {BinaryOperator.Multiply, multiplyf.Eval },
            {BinaryOperator.Divide, dividef.Eval },
            {BinaryOperator.Power, powf.Eval },
            {BinaryOperator.Log, logf.Eval }
        };

        public static readonly Dictionary<UniaryOperator, UniaryEvaluation> uniaryEvalTable = new Dictionary<UniaryOperator, UniaryEvaluation>()
        {
            {UniaryOperator.Sin, sinf.Eval },
            {UniaryOperator.Cos, cosf.Eval },
            {UniaryOperator.Tan, tanf.Eval },
            {UniaryOperator.Abs, absf.Eval },
            {UniaryOperator.Negate, negatef.Eval }
        };

        public static bool IsOneNumber(Node left, Node right, Node number)
        {
            return left.Equals(number) || right.Equals(number);
        }

        public static Node GetAnotherNode(Node left, Node right, Node searchFor)
        {
            if (left.Equals(searchFor))
            {
                return right;
            }
            else if (right.Equals(searchFor))
            {
                return left;
            }
            throw new NotImplementedException();
        }
    }

    static partial class addf
    {
        public static Node Eval(Node left, Node right)
        {
            Node r1 = left.Eval();
            Node r2 = right.Eval();
            if (r1 is NumberNode && r2 is NumberNode)
            {
                return new NumberNode((r1 as NumberNode).Value + (r2 as NumberNode).Value);
            }
            else
            {
                if (Functions.IsOneNumber(r1, r2, 0))
                {
                    return Functions.GetAnotherNode(r1, r2, 0);
                }
                else
                {
                    return new BinaryOperatorNode(BinaryOperator.Add, r1, r2);
                }
            }
        }
    }

    static partial class subtractf
    {
        public static Node Eval(Node left, Node right)
        {
            Node r1 = left.Eval();
            Node r2 = right.Eval();
            if (r1 is NumberNode && r2 is NumberNode)
            {
                return new NumberNode((r1 as NumberNode).Value - (r2 as NumberNode).Value);
            }
            else if (r1.Equals(r2))
            {
                return 0;
            }
            else if (r2.Equals(0))
            {
                return r1;
            }
            else if (r1.Equals(0))
            {
                return new UniaryOperatorNode(UniaryOperator.Negate, r2);
            }
            else
            {
                return new BinaryOperatorNode(BinaryOperator.Subtract, r1, r2);
            }
        }
    }

    static partial class multiplyf
    {
        public static Node Eval(Node left, Node right)
        {
            Node r1 = left.Eval();
            Node r2 = right.Eval();
            if (r1 is NumberNode && r2 is NumberNode)
            {
                return new NumberNode((r1 as NumberNode).Value * (r2 as NumberNode).Value);
            }
            else if (Functions.IsOneNumber(r1, r2, 1))
            {
                return Functions.GetAnotherNode(r1, r2, 1);
            }
            else if (Functions.IsOneNumber(r1, r2, 0))
            {
                return 0;
            }
            else
            {
                return new BinaryOperatorNode(BinaryOperator.Multiply, r1, r2);
            }
        }
    }

    static partial class dividef
    {
        public static Node Eval(Node left, Node right)
        {
            Node r1 = left.Eval();
            Node r2 = right.Eval();
            if (r1 is NumberNode && r2 is NumberNode)
            {
                return new NumberNode((r1 as NumberNode).Value / (r2 as NumberNode).Value);
            }
            else if (r1.Equals(0))
            {
                return 0;
            }
            else if (r2.Equals(1))
            {
                return r1;
            }
            else
            {
                return new BinaryOperatorNode(BinaryOperator.Divide, left, right);
            }
        }
    }

    static partial class powf
    {
        public static Node Eval (Node left, Node right)
        {
            Node r1 = left.Eval();
            Node r2 = right.Eval();
            if (r1 is NumberNode && r2 is NumberNode)
            {
                return new NumberNode(Math.Pow((r1 as NumberNode).Value, (r2 as NumberNode).Value));
            }
            else if (r1.Equals(0) || r1.Equals(1))
            {
                return r1;
            }
            else if (r2.Equals(1))
            {
                return r1;
            }
            else if (r2.Equals(0))
            {
                return 1;
            }
            else
            {
                return new BinaryOperatorNode(BinaryOperator.Power, r1, r2);
            }
        }
    }

    static partial class logf
    {
        public static Node Eval(Node left, Node right)
        {
            Node r = left.Eval();
            Node @base = right.Eval();
            if (r is NumberNode && @base is NumberNode)
            {
                return new NumberNode(Math.Log((r as NumberNode).Value) / Math.Log((@base as NumberNode).Value));
            }
            else if (r.Equals(@base))
            {
                return 1;
            }
            else if (r.Equals(1))
            {
                return 0;
            }
            return new BinaryOperatorNode(BinaryOperator.Log, r, @base);
        }
    }

    static partial class sinf
    {
        public static Node Eval(Node node)
        {
            var r = node.Eval();
            if (r is NumberNode)
            {
                return new NumberNode(Math.Sin((r as NumberNode).Value));
            }
            return new UniaryOperatorNode(UniaryOperator.Sin, r);
        }
    }

    static partial class cosf
    {
        public static Node Eval(Node node)
        {
            var r = node.Eval();
            if (r is NumberNode)
            {
                return new NumberNode(Math.Cos((r as NumberNode).Value));
            }
            return new UniaryOperatorNode(UniaryOperator.Cos, r);
        }
    }

    static partial class tanf
    {
        public static Node Eval(Node node)
        {
            var r = node.Eval();
            if (r is NumberNode)
            {
                return new NumberNode(Math.Tan((r as NumberNode).Value));
            }
            return new UniaryOperatorNode(UniaryOperator.Tan, r);
        }
    }

    static partial class absf
    {
        public static Node Eval(Node node)
        {
            var r = node.Eval();
            if (r is NumberNode)
            {
                return new NumberNode(Math.Abs((r as NumberNode).Value));
            }
            return new UniaryOperatorNode(UniaryOperator.Abs, r);
        }
    }

    static partial class negatef
    {
        public static Node Eval(Node node)
        {
            var r = node.Eval();
            if (r is NumberNode)
            {
                return new NumberNode(-(r as NumberNode).Value);
            }
            return new UniaryOperatorNode(UniaryOperator.Negate, r);
        }
    }
}
