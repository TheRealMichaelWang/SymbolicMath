using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolicMath.Simplify
{
    class Rule
    { 
        public Pattern InputPattern { get; private set; }
        public Pattern OutputPattern { get; private set; }

        public Rule(Pattern inputPattern, Pattern outputPattern)
        {
            this.InputPattern = inputPattern;
            this.OutputPattern = outputPattern;
        }

        public Node Apply(Node node, Rule[] Ruleset)
        {
            Dictionary<int, Node> res;
            if((res = TreeAnalyzer.PatternMatch(InputPattern,node)) != null)
            {
                return TreeAnalyzer.BuildTree(OutputPattern.Head, res, Ruleset); 
            }
            return null;
        }
    }

    class Rules
    {
        public static Rule[] CommonRules = new Rule[]
        {

            /*
             * Trigonometric Behavior Rules
             */

            //sin(x) * cos(x) = x
            new Rule(new Pattern(new UniaryOperatorNode(UniaryOperator.Sin, Pattern.Any1.Head) * new UniaryOperatorNode(UniaryOperator.Cos, Pattern.Any1.Head)), new Pattern(0.5 * new UniaryOperatorNode(UniaryOperator.Sin, 2 * Pattern.Any1.Head))),
            new Rule(new Pattern(new UniaryOperatorNode(UniaryOperator.Cos, Pattern.Any1.Head) * new UniaryOperatorNode(UniaryOperator.Sin, Pattern.Any1.Head)), new Pattern(0.5 * new UniaryOperatorNode(UniaryOperator.Sin, 2 * Pattern.Any1.Head))),

            /*
             * Exponential Rules (logarithmic)
             */

            // log(1 / a, b) = -log(a, b)
            new Rule(new Pattern(new BinaryOperatorNode(BinaryOperator.Log, Pattern.Any1.Head^-1, Pattern.Any2.Head)), new Pattern(-new BinaryOperatorNode(BinaryOperator.Log, Pattern.Any1.Head, Pattern.Any2.Head))),

            /*
             * Exponential Behavior Rules (non-logarithmic)
             */

            //a * a = a^2
            new Rule(Pattern.Any1 * Pattern.Any1, Pattern.Any1^2),

            //a ^ (b ^ c) = a ^ (b + c)
            new Rule(Pattern.Position1 ^ (Pattern.Position2 ^ Pattern.Position3), Pattern.Position1 ^ (Pattern.Position2 + Pattern.Position3)),
            new Rule((Pattern.Position1 ^ Pattern.Position2) ^ Pattern.Position3, Pattern.Position1 ^ (Pattern.Position2 + Pattern.Position3)),

            //a / (a ^ b) = a ^ (1 - b)
            new Rule(Pattern.Any1 / (Pattern.Any1 ^ Pattern.Position1), Pattern.Any1 ^ (1 - Pattern.Position1)),

            //(a ^ b) / a = a ^ (b - 1)
            new Rule((Pattern.Any1 ^ Pattern.Position1) / Pattern.Any1, Pattern.Any1 ^ (Pattern.Position1 - 1)),

            //(a ^ b) / (a ^ c) = a ^ (b - c)
            new Rule((Pattern.Any1 ^ Pattern.Position1) / (Pattern.Any1 ^ Pattern.Position2), Pattern.Any1 ^ (Pattern.Position1 - Pattern.Position2)),

            //a * (a ^ b) = (a ^ b + 1), (a ^ b) * a = a ^ (b + 1)
            new Rule(Pattern.Any1 * (Pattern.Any1 ^ Pattern.Position1), Pattern.Any1 ^ (Pattern.Position1 + 1)),
            new Rule((Pattern.Any1 ^ Pattern.Position1) * Pattern.Any1, Pattern.Any1 ^ (Pattern.Position1 + 1)),

            //(a ^ b) * (a ^ c) = a ^ (b + c)
            new Rule((Pattern.Any1 ^ Pattern.Position1) * (Pattern.Any1 ^ Pattern.Position2), Pattern.Any1 ^ (Pattern.Position1 + Pattern.Position2)),

            //1 / (a ^ b) = a ^ (-b)
            new Rule(1 / (Pattern.Position1 ^ Pattern.Position2), Pattern.Position1 ^ (-Pattern.Position2)),
            
            //1 / a = a^-1
            new Rule(1 / Pattern.Position1, Pattern.Position1^-1),

            //(a ^ -b) / c = c / (a ^ b)
            new Rule((Pattern.Position1 ^ -Pattern.Position2) / Pattern.Position3, Pattern.Position3 / (Pattern.Position1 ^ Pattern.Position2)),

            /*
             * Basic Addition and Subtraction Behavior Rules
             */

            //a + a = 2 * a
            new Rule(Pattern.Any1 + Pattern.Any1, 2 * Pattern.Any1),

            //a + (b * a) = (b + 1) * a
            new Rule(Pattern.Any1 + (Pattern.Position1 * Pattern.Any1), (Pattern.Position1 + 1) * Pattern.Any1),
            new Rule(Pattern.Any1 + (Pattern.Any1 * Pattern.Position1), (Pattern.Position1 + 1) * Pattern.Any1),
            new Rule((Pattern.Position1 * Pattern.Any1) + Pattern.Any1, (Pattern.Position1 + 1) * Pattern.Any1),
            new Rule((Pattern.Any1 * Pattern.Position1) + Pattern.Any1, (Pattern.Position1 + 1) * Pattern.Any1),
            
            //a - (b * a) = (1 - b) * a, (b * a) - a = (b - 1) * a
            new Rule(Pattern.Any1 - (Pattern.Position1 * Pattern.Any1), (1 - Pattern.Position1) * Pattern.Any1),
            new Rule(Pattern.Any1 - (Pattern.Any1 * Pattern.Position1), (1 - Pattern.Position1) * Pattern.Any1),
            new Rule((Pattern.Position1 * Pattern.Any1) - Pattern.Any1, (Pattern.Position1 - 1) * Pattern.Any1),
            new Rule((Pattern.Any1 * Pattern.Position1) - Pattern.Any1, (Pattern.Position1 - 1) * Pattern.Any1),

            //(b * a) + (c * a)
            new Rule((Pattern.Position1 * Pattern.Any1) + (Pattern.Position2 * Pattern.Any1), (Pattern.Position1 + Pattern.Position2) * Pattern.Any1),
            new Rule((Pattern.Position1 * Pattern.Any1) + (Pattern.Any1 * Pattern.Position2), (Pattern.Position1 + Pattern.Position2) * Pattern.Any1),
            new Rule((Pattern.Any1 * Pattern.Position1) + (Pattern.Position2 * Pattern.Any1), (Pattern.Position1 + Pattern.Position2) * Pattern.Any1),
            new Rule((Pattern.Any1 * Pattern.Position1) + (Pattern.Any1 * Pattern.Position2), (Pattern.Position1 + Pattern.Position2) * Pattern.Any1),
            
            //(b * a) - (c * a)
            new Rule((Pattern.Position1 * Pattern.Any1) - (Pattern.Position2 * Pattern.Any1), (Pattern.Position1 - Pattern.Position2) * Pattern.Any1),
            new Rule((Pattern.Position1 * Pattern.Any1) - (Pattern.Any1 * Pattern.Position2), (Pattern.Position1 - Pattern.Position2) * Pattern.Any1),
            new Rule((Pattern.Any1 * Pattern.Position1) - (Pattern.Position2 * Pattern.Any1), (Pattern.Position1 - Pattern.Position2) * Pattern.Any1),
            new Rule((Pattern.Any1 * Pattern.Position1) - (Pattern.Any1 * Pattern.Position2), (Pattern.Position1 - Pattern.Position2) * Pattern.Any1),

            /*
             * Basic Multiplication and Division Behavior Rules
             */
            
            // a / (b / c) = (a * c) / b, (a / b) / c = a / (b * c)
            new Rule(Pattern.Position1 / (Pattern.Position2 / Pattern.Position3), (Pattern.Position1 * Pattern.Position3) / Pattern.Position2),
            new Rule((Pattern.Position1 / Pattern.Position2) / Pattern.Position3, Pattern.Position1 / (Pattern.Position2 * Pattern.Position3)),

            // (a / b) + (a * c) = a * (b^-1 * c)
            new Rule((Pattern.Any1 / Pattern.Position1) + (Pattern.Any1 * Pattern.Position2), Pattern.Any1 * ((Pattern.Position1 ^ -1) * Pattern.Position2)),
            new Rule((Pattern.Any1 / Pattern.Position1) + (Pattern.Position2 * Pattern.Any1), Pattern.Any1 * ((Pattern.Position1 ^ -1) * Pattern.Position2)),
            new Rule((Pattern.Any1 * Pattern.Position2) + (Pattern.Any1 / Pattern.Position1), Pattern.Any1 * ((Pattern.Position1 ^ -1) * Pattern.Position2)),
            new Rule((Pattern.Position2 * Pattern.Any1) + (Pattern.Any1 / Pattern.Position1), Pattern.Any1 * ((Pattern.Position1 ^ -1) * Pattern.Position2))
        };
    }
}
