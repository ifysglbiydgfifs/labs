using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rpn.Logic
{
    class Token
    {
    }

    class Parenthesis : Token
    {
        public bool isClosing { get; set; }
        public int Priority = 0;

        public Parenthesis(char symbol)
        {
            if (symbol != '(' && symbol != ')')
                throw new ArgumentException("This isn't a valid parenthesis");
            isClosing = symbol == ')';
        }

        public override string ToString()
        {
            return isClosing ? ")" : "(";
        }
    }

    class Number : Token
    {
        public float Value { get; }

        public Number(string str)
        {
            Value = float.Parse(str, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static float operator +(Number a, Number b)
        {
            return a.Value + b.Value;
        }

        public static float operator -(Number a, Number b)
        {
            return a.Value - b.Value;
        }

        public static float operator *(Number a, Number b)
        {
            return a.Value * b.Value;
        }

        public static float operator /(Number a, Number b)
        {
            return a.Value / b.Value;
        }
    }

    class Operation : Token
    {
        public char Symbol { get; }
        public int Priority { get; }

        public Operation(char symbol)
        {
            Symbol = symbol;
            Priority = GetPriority(symbol);
        }

        private int GetPriority(char symbol)
        {
            Dictionary<char, int> priorities = new Dictionary<char, int>
            {
                { '+', 1 },
                { '-', 1 },
                { '*', 2 },
                { '/', 2 },
                { '(', 0 }
            };
            return priorities[symbol];
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }

    static class TokenCreator
    {
        public static Token Create(string str)
        {
            return new Number(str);
        }

        public static Token Create(char symbol)
        {
            if (symbol == '(' || symbol == ')') return new Parenthesis(symbol);
            return new Operation(symbol);
        }
    }

    public class RpnCalculate
    {
        private List<Token> rpn;

        public RpnCalculate(string expression)
        {
            rpn = ToRpn(Parse(expression));
        }

        public float Calculate()
        {
            return CalculateExpression();
        }

        private float CalculateExpression()
        {
            Stack<float> numbers = new Stack<float>();
            foreach (Token token in rpn)
            {
                if (token is Number num)
                {
                    numbers.Push(num.Value);
                }
                else if (token is Operation op)
                {
                    var num2 = numbers.Pop();
                    var num1 = numbers.Pop();
                    var result = Calculate(op.Symbol, num1, num2);
                    numbers.Push(result);
                }
            }

            return numbers.Pop();
        }

        private float Calculate(char op, float num1, float num2)
        {
            switch (op)
            {
                case '+': return num1 + num2;
                case '-': return num1 - num2;
                case '*': return num1 * num2;
                case '/': return num1 / num2;
                default: throw new Exception("Unknown operation");
            }
        }

        private List<Token> Parse(string input)
        {
            input = input.Replace(" ", string.Empty);
            List<Token> tokens = new List<Token>();
            string buffer = string.Empty;
            foreach (char symbol in input)
            {
                if (char.IsDigit(symbol) || symbol == '.' || symbol == ',')
                {
                    buffer += symbol;
                }
                else
                {
                    if (buffer != string.Empty)
                    {
                        tokens.Add(TokenCreator.Create(buffer));
                        buffer = string.Empty;
                    }
                    tokens.Add(TokenCreator.Create(symbol));
                }
            }
            if (buffer != string.Empty) tokens.Add(TokenCreator.Create(buffer));
            return tokens;
        }

        private List<Token> ToRpn(List<Token> tokens)
        {
            List<Token> rpnTokens = new List<Token>();
            Stack<Token> stack = new Stack<Token>();
            foreach (Token token in tokens)
            {
                if (token is Number num)
                {
                    rpnTokens.Add(num);
                }
                else if (token is Operation op)
                {
                    while (stack.Count > 0 && stack.Peek() is Operation stackOp && stackOp.Priority >= op.Priority)
                    {
                        rpnTokens.Add(stack.Pop());
                    }
                    stack.Push(op);
                }
                else if (token is Parenthesis parenthesis)
                {
                    if (!parenthesis.isClosing)
                    {
                        stack.Push(parenthesis);
                    }
                    else
                    {
                        while (stack.Count > 0 && !(stack.Peek() is Parenthesis))
                        {
                            rpnTokens.Add(stack.Pop());
                        }
                        stack.Pop(); // Pop the opening parenthesis
                    }
                }
            }

            while (stack.Count > 0)
            {
                rpnTokens.Add(stack.Pop());
            }

            return rpnTokens;
        }
    }
}
