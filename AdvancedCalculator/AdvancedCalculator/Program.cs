using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using static System.Console;

//Implementation of The Shunting Yard algorithm to convert infix notation
//to Reverse Polish Notation (RPN or simply postfix notation) and then
//calculate the expression with the RPN evaluation algorithm
namespace AdvancedCalculator
{

    class Program
    {

        static string[] operators = { "+", "-", "*", "/" };
        static string[] tokens = { };
        static Stack<string> ops = new Stack<string>();
        static Queue<string> values = new Queue<string>();

        static void Main(string[] args)
        {
            AdvancedCalculator();
        }

        private static void AdvancedCalculator()
        {
            string expression = " ";
            string splitString = " ";
            string[] RPN = { };
            string[] rpn = { };
            bool running = true;
            bool inputOK = false;
            bool firstrun = true;
            bool clear = false;

            while (running)
            {
                if (firstrun && !clear)
                {
                    ShowWelcomeScreen();
                }
                Write("Enter calculation: ");
                
                expression = ReadLine();
                inputOK = ParseInput(expression);

                if (expression.ToLower().Equals("q"))
                {
                    Write("Program has ended. ");
                    running = false;
                }
                else if (expression.ToLower().Equals("c"))
                {
                    Clear();
                    ShowWelcomeScreen();
                    clear = true;
                }
                else if (!inputOK)
                {
                    Clear();
                    WriteLine("Invalid input.");
                }
                else
                {
                    splitString = InsertSplitSign(expression);
                    tokens = splitString.Split("%");

                    RPN = ShuntingYard(tokens);
                    if (RPN != null)
                     {
                        try { rpn = CalculateRPN(RPN); }
                        catch (DivideByZeroException)
                        {
                           WriteLine("Divide by zero error.");
                           values.Clear();
                           firstrun = false;
                           continue;
                        }
                           SetCursorPosition(CursorLeft + 19 + expression.Length, CursorTop - 1);
                           WriteLine(" = " + rpn[0]);
                           firstrun = false;
                     }
                    else
                    {
                      Clear();
                      WriteLine("Invalid input: Mismatching parenthesis.");
                    }             
                }
            }
        }

        //Validating input string. Checks if there are other characters besides 
        //digits and operators
        private static bool ParseInput(string input)
        {
            char[] operators = { '+', '-', '*', '/', '(', ')', ',', ' ' };
            char[] inputarray = new char[input.Length];
            bool isOK = true;
            int i = 0;
            char c = ' ';
            inputarray = input.ToCharArray();

            while (isOK && i < inputarray.Length) 
            {
                c = inputarray[i++];
                if (!char.IsDigit(c) && !operators.Contains(c))
                {
                    isOK = false;
                }
            }
            return isOK;
        }

        private static void ShowWelcomeScreen()
        {
            WriteLine("--------------------------------------------");
            WriteLine("Advanced Calculator.\n");
            WriteLine("Enter mathematical expression in infix notation.");
            WriteLine("Program can handle operators {+,-,*,/} and tokens {(,)}.");
            WriteLine("End program with q. Clear history with c.");
            WriteLine("--------------------------------------------");
        }

        
        //Below is my source for evaluating RPN
        //https://www.dreamincode.net/forums/topic/35320-reverse-polish-notation-in-c%23/
        private static string[] CalculateRPN(string[] RPN)
        {
            Stack<string> rpnStack = new Stack<string>();
            
            for (int i = 0; i < RPN.Length; i++)
            {
                if (isNumber(RPN[i]))
                {
                    rpnStack.Push(RPN[i]);
                }
                else if (isOperator(RPN[i]))
                {
                    string op1 = rpnStack.Pop();
                    string op2 = rpnStack.Pop();

                    string result = PerformOperation(op1, op2, RPN[i]);
                    rpnStack.Push(result);
                }

            }
            return rpnStack.ToArray();
        }

        private static string PerformOperation(string op1, string op2, string operation)
        {
            decimal value1 = 0;
            decimal value2 = 0;
            bool isValue1 = false;
            bool isValue2 = false;
            string result = " ";

            isValue1 = decimal.TryParse(op1, out value1);
            isValue2 = decimal.TryParse(op2, out value2);

            switch (operation)
            {
                case "+": 
                    result = Add(value2, value1);
                    break;
                case "-":
                    result = Subtract(value2, value1);
                    break;
                case "*":
                    result = Multiplicate(value2, value1);
                    break;
                case "/":
                     result = Divide(value2, value1);
                  break;
                default:
                    break;
            }
            return result;
        }

        private static string Divide(decimal value1, decimal value2)
        {
           
            decimal result = value1 / value2; 
  
            return result.ToString(); ;
        }

        private static string Multiplicate(decimal value1, decimal value2)
        {
            decimal result = value1 * value2;

            return result.ToString();
        }

        private static string Subtract(decimal value1, decimal value2)
        {
            decimal result = value1 - value2;

            return result.ToString();
        }

        private static string Add(decimal value1, decimal value2)
        {
            decimal result = value1 + value2;

            return result.ToString();
        }

        //Below is a link to my source for the algorithm
        //https://en.wikipedia.org/wiki/Shunting-yard_algorithm#The_algorithm_in_detail
        private static string[] ShuntingYard(string[] tokens)
        {
        
            for (int i = 0; i < tokens.Length; i++)
            {
                if (isNumber(tokens[i]))
                {
                    values.Enqueue(tokens[i]);
                }
                else if (isOperator(tokens[i]))
                {
                    while (ops.Count > 0 && !ops.Peek().Equals("(") &&
                        hasHigherOrEqualPrecedense(ops.Peek().ToString(), tokens[i]))
                    {
                       string s = ops.Pop().ToString();
                       values.Enqueue(s);
                    }
                        ops.Push(tokens[i]);  
                }
                else if (tokens[i].Equals("("))
                {
                    ops.Push(tokens[i]);
                }
                else if (tokens[i].Equals(")"))
                {
                    while (!ops.Peek().ToString().Equals("("))
                    {
                        string s = ops.Pop();
                        values.Enqueue(s);
                    }
                    if (ops.Count == 0)
                    {
                        return null;
                    }
                    else if (ops.Peek().ToString().Equals("("))
                    {
                        ops.Pop();
                    }
                }
            } 
            while (ops.Count > 0)
            {
                string s = ops.Pop().ToString();
                if (s.Equals("(") || s.Equals(")"))
                {
                    return null;
                }
                else
                    values.Enqueue(s);
            }
            return values.ToArray<string>();
        }
       

        private static int PrecedenceValue(string token)
        {
            int value = 0;

            switch (token){
                case "*": value = 3;
                    break;
                case "/": value = 3;
                    break;
                case "+": value = 2;
                    break;
                case "-": value = 2;
                    break;
                default: 
                    break;
            }
            return value;
        }


        private static bool hasHigherOrEqualPrecedense(string op1, string op2)
        {
            bool higherorequal = false;
            int value1 = 0;
            int value2 = 0;

            value1 = PrecedenceValue(op1);
            value2 = PrecedenceValue(op2);

            higherorequal = value1 >= value2;

            return higherorequal;
            
        }

        private static bool isOperator(string token)
        {
            bool isOperator = false;

            isOperator = operators.Contains(token);

            return isOperator;
        }

        private static bool isNumber(string token)
        {
            bool isNumeric = false;

            isNumeric = decimal.TryParse(token, out _);

            return isNumeric;
        }

        private static string InsertSplitSign(string expression)
        {
 
            var splitString = expression.Replace("+", "%+%").Replace("-", "%-%").
                Replace("*", "%*%").Replace("/", "%/%").Replace("(", "%(%").
                Replace(")", "%)%").Replace("%%", "%");
        
            return splitString;
        }
    }
}
