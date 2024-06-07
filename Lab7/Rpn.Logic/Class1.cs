using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rpn.Logic
{
    class Token { }
    class XVariable : Token
    {
        public override string ToString()
        {
            return "x";
        }
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

    class Variable : Token
    {
        public char Symbol { get; }
        public float Value { get; }

        public Variable(char symbol, float value)
        {
            Symbol = symbol;
            Value = value;
        }

        public override string ToString()
        {
            return Symbol.ToString();
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
            if (symbol == 'x') return new XVariable();
            return new Operation(symbol);
        }
    }

    public class RpnCalculate
    {
        private List<Token> rpn;
        private float xValue;

        public RpnCalculate(string expression, float x = 0)
        {
            xValue = x;
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
                else if (token is XVariable)
                {
                    numbers.Push(xValue);
                }
                else if (token is Operation op)
                {
                    var num1 = numbers.Pop();
                    var num2 = numbers.Pop();
                    var result = Calculate(op.Symbol, num2, num1); // Note the order of num2 and num1
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
                default: throw new Exception("Неизвестная операция");
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
                    if (buffer != string.Empty) tokens.Add(TokenCreator.Create(buffer));
                    if (symbol == 'x')
                    {
                        tokens.Add(new XVariable());
                    }
                    else
                    {
                        tokens.Add(TokenCreator.Create(symbol));
                    }
                    buffer = string.Empty;
                }
            }
            if (buffer != string.Empty) tokens.Add(TokenCreator.Create(buffer));
            return tokens;
        }

        private List<Token> ToRpn(List<Token> tokens)
        {
            List<Token> rpnTokens = new List<Token>();
            Stack<Operation> stack = new Stack<Operation>();
            foreach (Token token in tokens)
            {
                if (token is Number num)
                {
                    rpnTokens.Add(num);
                }
                else if (token is XVariable)
                {
                    rpnTokens.Add(token);
                }
                else if (token is Operation op)
                {
                    while (stack.Count > 0 && stack.Peek().Priority >= op.Priority)
                    {
                        rpnTokens.Add(stack.Pop());
                    }
                    stack.Push(op);
                }
                else if (token is Parenthesis parenthesis)
                {
                    if (!parenthesis.isClosing)
                    {
                        stack.Push(new Operation(parenthesis.ToString()[0]));
                    }
                    else
                    {
                        while (stack.Peek().Symbol != '(')
                        {
                            rpnTokens.Add(stack.Pop());
                        }
                        stack.Pop();
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
