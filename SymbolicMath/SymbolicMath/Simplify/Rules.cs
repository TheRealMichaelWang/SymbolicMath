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
            new Rule(Pattern.Any1 / (Pattern.Any2 / Pattern.Any3),Pattern.Any1 * Pattern.Any2 / Pattern.Any3),
            new Rule(Pattern.Any1 * (Pattern.Any2 / Pattern.Any3), (Pattern.Any1 * Pattern.Any2) / Pattern.Any3),
            new Rule(Pattern.Var1 * Pattern.Constant1, Pattern.Constant1 * Pattern.Var1),
            new Rule(Pattern.Constant1 + Pattern.Var1, Pattern.Var1 + Pattern.Constant1),

            new Rule((Pattern.Constant1 * Pattern.Var1) + (Pattern.Constant2 * Pattern.Var1),(Pattern.Constant1 + Pattern.Constant2) * Pattern.Var1),

            new Rule((Pattern.Constant1 * Pattern.Var1) - (Pattern.Constant2 * Pattern.Var1),(Pattern.Constant1 - Pattern.Constant2) * Pattern.Var1),

            new Rule((Pattern.Any1 * Pattern.Any2) + (Pattern.Any1 * Pattern.Any3), Pattern.Any1 * (Pattern.Any2 + Pattern.Any3)),
            new Rule((Pattern.Any1 * Pattern.Any2) + (Pattern.Any3 * Pattern.Any1), Pattern.Any1 * (Pattern.Any2 + Pattern.Any3)),
            new Rule((Pattern.Any2 * Pattern.Any2) + (Pattern.Any1 * Pattern.Any3), Pattern.Any1 * (Pattern.Any2 + Pattern.Any3)),
            new Rule((Pattern.Any2 * Pattern.Any2) + (Pattern.Any3 * Pattern.Any1), Pattern.Any1 * (Pattern.Any2 + Pattern.Any3)),
            new Rule(Pattern.Any1 + (Pattern.Any1 * Pattern.Any2), Pattern.Any1 * (Pattern.Any2 + 1)),
            new Rule(Pattern.Any1 + (Pattern.Any2 * Pattern.Any1), Pattern.Any1 * (Pattern.Any2 + 1)),
            new Rule((Pattern.Any1 * Pattern.Any2) + Pattern.Any1, Pattern.Any1 * (Pattern.Any2 + 1)),
            new Rule((Pattern.Any2 * Pattern.Any1) + Pattern.Any1, Pattern.Any1 * (Pattern.Any2 + 1)),
            new Rule(Pattern.Any1 + Pattern.Any1, 2 * Pattern.Any1),

            new Rule((Pattern.Any1 * Pattern.Any2) - (Pattern.Any1 * Pattern.Any3), Pattern.Any1 * (Pattern.Any2 - Pattern.Any3)),
            new Rule((Pattern.Any1 * Pattern.Any2) - (Pattern.Any3 * Pattern.Any1), Pattern.Any1 * (Pattern.Any2 - Pattern.Any3)),
            new Rule((Pattern.Any2 * Pattern.Any2) - (Pattern.Any1 * Pattern.Any3), Pattern.Any1 * (Pattern.Any2 - Pattern.Any3)),
            new Rule((Pattern.Any2 * Pattern.Any2) - (Pattern.Any3 * Pattern.Any1), Pattern.Any1 * (Pattern.Any2 - Pattern.Any3)),
            new Rule(Pattern.Any1 - (Pattern.Any1 * Pattern.Any2), Pattern.Any1 * (1 - Pattern.Any2)),
            new Rule(Pattern.Any1 - (Pattern.Any2 * Pattern.Any1), Pattern.Any1 * (1 - Pattern.Any2)),
            new Rule((Pattern.Any1 * Pattern.Any2) - Pattern.Any1, Pattern.Any1 * (Pattern.Any2 - 1)),
            new Rule((Pattern.Any2 * Pattern.Any1) - Pattern.Any1, Pattern.Any1 * (Pattern.Any2 - 1)),

            new Rule((Pattern.Any1 / Pattern.Any2) + (Pattern.Any1 * Pattern.Any3),Pattern.Any1 * (1 / (Pattern.Any2 + Pattern.Any3))),
            new Rule((Pattern.Any1 / Pattern.Any2) + (Pattern.Any3 * Pattern.Any1),Pattern.Any1 * (1 / (Pattern.Any2 + Pattern.Any3))),
            new Rule((Pattern.Any1 * Pattern.Any2) + (Pattern.Any1 / Pattern.Any3),Pattern.Any1 * (Pattern.Any2 / (1 + Pattern.Any3))),
            new Rule((Pattern.Any2 * Pattern.Any1) + (Pattern.Any1 / Pattern.Any3),Pattern.Any1 * (Pattern.Any2 / (1 + Pattern.Any3))),
            new Rule(Pattern.Any1 + (Pattern.Any1 / Pattern.Any2),Pattern.Any1 * (1 + (1 / Pattern.Any2))),
            new Rule((Pattern.Any1 / Pattern.Any2) + Pattern.Any1,Pattern.Any1 * (1 + (1 / Pattern.Any2))),
            
            new Rule(Pattern.Var1 * Pattern.Var1, Pattern.Var1^2),

            new Rule(Pattern.Var1 * (Pattern.Any1^Pattern.Any2),(Pattern.Any1^Pattern.Any2) * Pattern.Var1),

            new Rule((Pattern.Any1^Pattern.Any2) * Pattern.Any1, Pattern.Any1^(Pattern.Any2 + 1)),
            new Rule(Pattern.Any1 * (Pattern.Any1^Pattern.Any2), Pattern.Any1^(Pattern.Any2 + 1)),

            new Rule((Pattern.Any1^Pattern.Any1)^Pattern.Any3, Pattern.Any1^(Pattern.Any1 + Pattern.Any2)),

            new Rule((Pattern.Any1^Pattern.Any2) * (Pattern.Any1^Pattern.Any3),Pattern.Any1^(Pattern.Any2 + Pattern.Any3)),

            new Rule((Pattern.Any1^Pattern.Any2) / (Pattern.Any1^Pattern.Any3),Pattern.Any1^(Pattern.Any2 - Pattern.Any3)),

            new Rule(Pattern.Any1 / (Pattern.Any1 ^ Pattern.Any2), Pattern.Any1 ^ (1 - Pattern.Any2)),
            new Rule((Pattern.Any1 ^ Pattern.Any2) / Pattern.Any1, Pattern.Any1 ^ (Pattern.Any2 - 1)),

            new Rule(Pattern.Constant1 ^ new BinaryOperatorNode(BinaryOperator.Log,Pattern.Any1.Head,Pattern.Constant1.Head), Pattern.Any1),

            new Rule((Pattern.Constant1 * Pattern.Var1) * Pattern.Constant2, (Pattern.Constant1 * Pattern.Constant2) * Pattern.Var1),
            new Rule(Pattern.Constant2 * (Pattern.Constant1 * Pattern.Var1), (Pattern.Constant1 * Pattern.Constant2) * Pattern.Var1),

            new Rule((Pattern.Var1 + Pattern.Constant1) + Pattern.Constant2, Pattern.Var1 + (Pattern.Constant1 + Pattern.Constant2)),
            new Rule(Pattern.Constant2 +(Pattern.Var1 + Pattern.Constant1), Pattern.Var1 + (Pattern.Constant1 + Pattern.Constant2)),
            
            new Rule(Pattern.Any1 * (Pattern.Any1 * Pattern.Any2), (Pattern.Any1 * Pattern.Any1) * Pattern.Any2),
            new Rule(Pattern.Any1 * (Pattern.Any2 * Pattern.Any1), (Pattern.Any1 * Pattern.Any1) * Pattern.Any2),
            new Rule((Pattern.Any1 * Pattern.Any2) * Pattern.Any1, (Pattern.Any1 * Pattern.Any1) * Pattern.Any2),
            new Rule((Pattern.Any2 * Pattern.Any1) * Pattern.Any1, (Pattern.Any1 * Pattern.Any1) * Pattern.Any2),

            new Rule((Pattern.Any1 ^ Pattern.Any3) * (Pattern.Any1 * Pattern.Any2), (Pattern.Any1^(Pattern.Any3+1)) * Pattern.Any2)
        };
    }
}
