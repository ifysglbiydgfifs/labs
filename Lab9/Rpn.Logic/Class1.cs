using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rpn.Logic
{
    abstract class Token
    {
        public new abstract string ToString();
    }

    class Parenthesis : Token
    {
        public bool IsClosing { get; set; }
        public int Priority => 0;

        public Parenthesis(char ch)
        {
            if (ch != '(' && ch != ')')
                throw new ArgumentException("This isn't valid parenthesis");
            IsClosing = ch == ')';
        }

        public override string ToString()
        {
            return IsClosing ? ")" : "(";
        }
    }

    class Number : Token
    {
        public float Value { get; }

        public Number(string str)
        {
            Value = float.Parse(str, CultureInfo.InvariantCulture);
        }

        public Number(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static Number operator +(Number a, Number b) => new Number(a.Value + b.Value);
        public static Number operator -(Number a, Number b) => new Number(a.Value - b.Value);
        public static Number operator *(Number a, Number b) => new Number(a.Value * b.Value);
        public static Number operator /(Number a, Number b) => new Number(a.Value / b.Value);
        public static Number operator ^(Number a, Number b) => new Number((float)Math.Pow(a.Value, b.Value));
    }

    class Variable : Token
    {
        public char Symbol { get; }
        public float Value { get; }

        public Variable(char symbol, float value = 0)
        {
            Symbol = symbol;
            Value = value;
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }

    abstract class Operation : Token
    {
        public abstract string Name { get; }
        public abstract int Priority { get; }
        public abstract bool IsFunction { get; }
        public abstract int ArgsAmount { get; }

        public abstract Number Execute(params Number[] numbers);

        public override string ToString()
        {
            return Name;
        }
    }

    class Plus : Operation
    {
        public override string Name => "+";
        public override int Priority => 1;
        public override bool IsFunction => false;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => numbers[0] + numbers[1];
    }

    class Minus : Operation
    {
        public override string Name => "-";
        public override int Priority => 1;
        public override bool IsFunction => false;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => numbers[0] - numbers[1];
    }

    class Multiply : Operation
    {
        public override string Name => "*";
        public override int Priority => 2;
        public override bool IsFunction => false;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => numbers[0] * numbers[1];
    }

    class Div : Operation
    {
        public override string Name => "/";
        public override int Priority => 2;
        public override bool IsFunction => false;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => numbers[0] / numbers[1];
    }

    class Pow : Operation
    {
        public override string Name => "^";
        public override int Priority => 3;
        public override bool IsFunction => false;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => numbers[0] ^ numbers[1];
    }

    class Log : Operation
    {
        public override string Name => "log";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => new Number((float)Math.Log(numbers[1].Value, numbers[0].Value));
    }

    class Sqrt : Operation
    {
        public override string Name => "sqrt";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 1;

        public override Number Execute(params Number[] numbers) => new Number((float)Math.Sqrt(numbers[0].Value));
    }

    class Rt : Operation
    {
        public override string Name => "rt";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 2;

        public override Number Execute(params Number[] numbers) => new Number((float)Math.Pow(numbers[1].Value, 1.0 / numbers[0].Value));
    }

    class Sin : Operation
    {
        public override string Name => "sin";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 1;

        public override Number Execute(params Number[] numbers) => new Number((float)Math.Sin(numbers[0].Value));
    }

    class Cos : Operation
    {
        public override string Name => "cos";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 1;

        public override Number Execute(params Number[] numbers) => new Number((float)Math.Cos(numbers[0].Value));
    }

    class Tg : Operation
    {
        public override string Name => "tg";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 1;

        public override Number Execute(params Number[] numbers)
        {
            if (Math.Abs(numbers[0].Value - Math.PI / 2) < 0.0001 || Math.Abs(numbers[0].Value + Math.PI / 2) < 0.0001)
            {
                throw new ArgumentException("The tangent function is undefined at pi/2 and -pi/2.");
            }
            return new Number((float)Math.Tan(numbers[0].Value));
        }
    }

    class Ctg : Operation
    {
        public override string Name => "ctg";
        public override int Priority => 3;
        public override bool IsFunction => true;
        public override int ArgsAmount => 1;

        public override Number Execute(params Number[] numbers)
        {
            if (Math.Abs(numbers[0].Value - Math.PI / 2) < 0.0001 || Math.Abs(numbers[0].Value + Math.PI / 2) < 0.0001)
            {
                throw new ArgumentException("The cotangent function is undefined at pi/2 and -pi/2.");
            }
            return new Number(1.0f / (float)Math.Tan(numbers[0].Value));
        }
    }


    static class TokenCreator
    {
        private static List<Operation> _availableOperations;

        public static Token Create(string str)
        {
            if (char.IsDigit(str.First()) || str.First() == '.')
                return new Number(str);
            if (str.Length == 1 && char.IsLetter(str.First()))
                return new Variable(str.First());
            return CreateOperation(str);
        }

        public static Token Create(char ch, List<char> variableNames)
        {
            if (variableNames.Contains(ch))
                return new Variable(ch);
            if (ch == '(' || ch == ')')
                return new Parenthesis(ch);
            return CreateOperation(ch.ToString());
        }

        private static Operation CreateOperation(string name)
        {
            var operation = FindName(name);
            if (operation == null)
            {
                throw new ArgumentException($"Unknown operation {name}");
            }
            return operation;
        }

        private static Operation FindName(string name)
        {
            if (_availableOperations == null)
            {
                var parent = typeof(Operation);
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                var types = allAssemblies.SelectMany(x => x.GetTypes());
                var inheritingTypes = types.Where(t => parent.IsAssignableFrom(t) && !t.IsAbstract).ToList();
                _availableOperations = inheritingTypes.Select(type => (Operation)Activator.CreateInstance(type)).ToList();
            }
            return _availableOperations.FirstOrDefault(op => op.Name.Equals(name));
        }
    }

    public class RpnCalculate
    {
        private List<Token> rpn;
        private float xValue;
        private readonly List<char> _variableNames;

        public RpnCalculate(string expression, float x = 0)
        {
            xValue = x;
            _variableNames = new List<char> { 'x' };
            var tokens = Tokenize(expression);
            var parsedTokens = ConvertToRpn(tokens);
            rpn = parsedTokens;
        }

        public float Calculate()
        {
            var stack = new Stack<Number>();

            foreach (var token in rpn)
            {
                if (token is Number number)
                {
                    stack.Push(number);
                }
                else if (token is Variable variable)
                {
                    stack.Push(new Number(xValue));
                }
                else if (token is Operation operation)
                {
                    var args = new List<Number>();
                    for (int i = 0; i < operation.ArgsAmount; i++)
                    {
                        args.Add(stack.Pop());
                    }
                    args.Reverse();
                    var result = operation.Execute(args.ToArray());
                    stack.Push(result);
                }
            }

            return stack.Pop().Value;
        }

        private List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            var numberBuffer = "";
            var variableBuffer = "";
            foreach (var ch in expression)
            {
                if (char.IsDigit(ch) || ch == '.')
                {
                    numberBuffer += ch;
                }
                else
                {
                    if (numberBuffer.Length > 0)
                    {
                        tokens.Add(new Number(numberBuffer));
                        numberBuffer = "";
                    }
                    if (char.IsLetter(ch))
                    {
                        variableBuffer += ch;
                    }
                    else
                    {
                        if (variableBuffer.Length > 0)
                        {
                            tokens.Add(TokenCreator.Create(variableBuffer));
                            variableBuffer = "";
                        }
                        tokens.Add(TokenCreator.Create(ch, _variableNames));
                    }
                }
            }

            if (numberBuffer.Length > 0)
            {
                tokens.Add(new Number(numberBuffer));
            }
            if (variableBuffer.Length > 0)
            {
                tokens.Add(TokenCreator.Create(variableBuffer));
            }

            return tokens;
        }

        private List<Token> ConvertToRpn(List<Token> tokens)
        {
            var output = new List<Token>();
            var stack = new Stack<Token>();

            foreach (var token in tokens)
            {
                if (token is Number || token is Variable)
                {
                    output.Add(token);
                }
                else if (token is Operation operation)
                {
                    while (stack.Count > 0 && stack.Peek() is Operation topOperation &&
                        (operation.IsFunction || topOperation.Priority >= operation.Priority))
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(operation);
                }
                else if (token is Parenthesis parenthesis)
                {
                    if (!parenthesis.IsClosing)
                    {
                        stack.Push(parenthesis);
                    }
                    else
                    {
                        while (stack.Count > 0 && !(stack.Peek() is Parenthesis))
                        {
                            output.Add(stack.Pop());
                        }
                        if (stack.Count == 0)
                        {
                            throw new ArgumentException("Mismatched parentheses");
                        }
                        stack.Pop();
                    }
                }
            }

            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top is Parenthesis)
                {
                    throw new ArgumentException("Mismatched parentheses");
                }
                output.Add(top);
            }

            return output;
        }
    }
}
