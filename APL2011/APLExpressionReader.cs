using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL2011
{
    class APLExpressionReader
    {
        private String _expression = "";
        private String[] _expressionparts;
        private Dictionary<char, IAPLFunction> _functions;
        private int _curPos = -1;
        public enum APLExpressionType
        {
            Variable, Operator, Function
        }

        public APLExpressionReader(String expression, Dictionary<char, IAPLFunction> functions)
        {
            _expression = expression;
            _expressionparts = _expression.Split(new char[] { ' ' });
            _functions = functions;
            _curPos = _expressionparts.Length - 1;
        }

        private bool isOperator(String op)
        {
            foreach (char f in _functions.Keys)
            {
                if (op.Contains(f))
                    return true;
            }
            return false;
        }
        public APLExpressionType nextType()
        {
            try
            {
                if (isOperator(_expressionparts[_curPos]))
                {
                    return APLExpressionType.Operator;
                }
                else
                {
                    return APLExpressionType.Variable;
                }
            }
            catch (IndexOutOfRangeException ioe)
            {
                throw new Exception("No more things to retrieve!");
            }
        }

        public APLVariable nextVariable()
        {
            if (_curPos > -1)
            {
                if (nextType() == APLExpressionType.Variable)
                {
                    List<double> nums = new List<double>();
                    nums.Add(Double.Parse(_expressionparts[_curPos--]));
                    while (_curPos > -1 && nextType() == APLExpressionType.Variable)
                    {
                        nums.Add(Double.Parse(_expressionparts[_curPos--]));
                    }
                    APLVariable a = APLVariable.FromList(nums);
                    return a;
                }
                
            }
            
                throw new Exception("Next Line is not a Variable!");
           
        }

        public String nextOperator()
        {
            if (nextType() == APLExpressionType.Operator)
            {
                return _expressionparts[_curPos--];
            }
            else
            {
                throw new Exception("Next Line is not an Operator!");
            }
        }

        public bool hasNext()
        {
            Console.WriteLine("Current Position: " + _curPos);
            return (_curPos > -1);
        }

    }
}
