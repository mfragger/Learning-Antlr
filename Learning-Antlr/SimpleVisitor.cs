using Antlr4.Runtime.Misc;
using Learning_Antlr.ANTLR;
using System.Runtime.InteropServices.ObjectiveC;

namespace Learning_Antlr
{
    internal class SimpleVisitor : SimpleBaseVisitor<object?>
    {
        private Dictionary<string, object?> Variables { get; } = new();

        public SimpleVisitor()
        {
            Variables["Write"] = new Func<object?[], object?>(Write);
        }

        private object? Write(object?[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
            return null;
        }

        public override object? VisitFunctionCall([NotNull] SimpleParser.FunctionCallContext context)
        {
            var name = context.IDENTIFIER().GetText();
            var args = context.expression().Select(e => Visit(e)).ToArray();

            if (!Variables.TryGetValue(name, out var value))
            {
                throw new Exception($"Function {name} is not defined.");
            }
            if (!(Variables[name] is Func<object?[], object?> func))
            {
                throw new Exception($"Variable {name} is not a function");
            }
            return func(args);
        }

        public override object? VisitAssignment([NotNull] SimpleParser.AssignmentContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            var value = Visit(context.expression());

            Variables[varName] = value;

            return null;
        }

        public override object? VisitIdentifierExpression([NotNull] SimpleParser.IdentifierExpressionContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            if (!Variables.ContainsKey(varName))
            {
                throw new Exception($"Variable {varName} is not defined.");
            }
            return Variables[varName];
        }

        public override object? VisitAdditiveExpression([NotNull] SimpleParser.AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.addOp().GetText();
            return op switch
            {
                "+" => Add(left, right),
                "-" => Subtract(left, right),
                "*" => Multiply(left, right),
                _ => throw new NotImplementedException()
            };
        }

        private object? Multiply(object? left, object? right)
        {
            if (left is int l && right is int r)
            {
                return l * r;
            }
            if (left is float lf && right is float rf)
            {
                return lf * rf;
            }
            if (left is int lint && right is float rfloat)
            {
                return lint * rfloat;
            }
            if (left is float lfloat && right is float rint)
            {
                return lfloat * rint;
            }
            throw new Exception($"Cannot multiply values types {left?.GetType()} and {right?.GetType()}.");
        }

        private object? Add(object? left, object? right)
        {
            if (left is int l && right is int r)
            {
                return l + r;
            }
            if (left is float lf && right is float rf)
            {
                return lf + rf;
            }
            if (left is int lint && right is float rfloat)
            {
                return lint + rfloat;
            }
            if (left is float lfloat && right is float rint)
            {
                return lfloat + rint;
            }
            if (left is string || right is string)
            {
                return $"{left}{right}";
            }
            throw new Exception($"Cannot add values types {left?.GetType()} and {right?.GetType()}.");
        }

        private object? Subtract(object? left, object? right)
        {
            if (left is int l && right is int r)
            {
                return l - r;
            }
            if (left is float lf && right is float rf)
            {
                return lf - rf;
            }
            if (left is int lint && right is float rfloat)
            {
                return lint - rfloat;
            }
            if (left is float lfloat && right is float rint)
            {
                return lfloat - rint;
            }
            throw new Exception($"Cannot subtract values types {left?.GetType()} and {right?.GetType()}.");
        }

        public override object? VisitConstant([NotNull] SimpleParser.ConstantContext context)
        {
            if (context.INTEGER() != null)
            {
                return int.Parse(context.INTEGER().GetText());
            }
            if (context.FLOAT() != null)
            {
                return float.Parse(context.FLOAT().GetText());
            }
            if (context.STRING() != null)
            {
                return context.GetText()[1..^1];
            }
            if (context.BOOL() != null)
            {
                return context.GetText() == "true";
            }
            if (context.NULL == null)
            {
                return null;
            }
            throw new NotImplementedException();
        }
        public override object? VisitWhileBlock([NotNull] SimpleParser.WhileBlockContext context)
        {
            Func<object?, bool> condition = context.WHILE().GetText() == "while" ? IsTrue : IsFalse;


            if (condition(Visit(context.expression())))
            {
                while (condition(Visit(context.expression())))
                {
                    Visit(context.block());
                }
            }
            else
            {
                Visit(context.elseIfBlock());
            }

            return null;
        }

        public bool IsFalse(object? value) => !IsTrue(value);

        public bool IsTrue(object? value)
        {
            if (value is bool b)
            {
                return b;
            }
            throw new Exception("Value is not a boolean");
        }

        public override object? VisitComparisonExpression([NotNull] SimpleParser.ComparisonExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.compareOp().GetText();

            return op switch
            {
                "==" => left == right,
                "!=" => left != right,
                ">" => IsGreaterThan(left, right),
                "<" => IsLessThan(left, right),
            };

        }
        private bool IsGreaterThan(object? left, object? right)
        {
            if (left is int l && right is int r)
            {
                return l > r;
            }
            if (left is float lf && right is float rf)
            {
                return lf > rf;
            }
            if (left is int lInt && right is float rFloat)
            {
                return lInt > rFloat;
            }
            if (left is float lFloat && right is int rInt)
            {
                return lFloat > rInt;
            }

            throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}.");
        }

        private bool IsLessThan(object? left, object? right)
        {
            if (left is int l && right is int r)
            {
                return l < r;
            }
            if (left is float lf && right is float rf)
            {
                return lf < rf;
            }
            if (left is int lInt && right is float rFloat)
            {
                return lInt < rFloat;
            }
            if (left is float lFloat && right is int rInt)
            {
                return lFloat < rInt;
            }

            throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}.");
        }
    }
}
