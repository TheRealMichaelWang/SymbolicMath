using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymbolicMath.Simplify;

namespace SymbolicMath
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression x = Expression.FromVariable("x");
            Expression y = Expression.FromVariable("y");
            Expression z = Expression.FromVariable("z");
            //Expression expr = MathLib.Sine(x) + x + y + MathLib.Sine(y) + (2 * x);
            //Node simp = expr.Head.Simplify(2);

            //Expression expr = 1 / MathLib.Root(x, 2);
            //Console.WriteLine(expr.Derive("x"));

            //Expression numerator = 3 * x;
            //Expression denominator = 3 * (x^2);
            //Expression quotient = (numerator / denominator);
            //numerator.Head.Simplify(2);

            Expression expression = MathLib.Log(x ^ -1, y);
            Console.Write(expression.Derive());
            ;
        }
    }
}
