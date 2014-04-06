using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL2011
{
    class APLInterpreter
    {
        //  Create a dictionary object to link to the defined functions
        private Dictionary<char, IAPLFunction> functions = new Dictionary<char, IAPLFunction>();
        private Dictionary<String, APLVariable> variables = new Dictionary<String, APLVariable>();
        private static APLInterpreter _instance;

        public APLInterpreter() {
            loadBuiltinFunctions();
        }

        private void loadBuiltinFunctions()
        {
            //  Load the Addition Function
            functions['+'] = new BuiltinFunctions.AddFunction();
            functions['x'] = new BuiltinFunctions.MultiplyFunction();
        }
        public APLVariable EvaluateExpression(String expression)
        {
            APLExpressionReader reader = new APLExpressionReader(expression, functions);
            List<APLVariable> vars = new List<APLVariable>();
            String curOperator = "";
            //  While hasNext
            while (reader.hasNext())
            {
                //  If the next type is a variable, then retrieve the next variable
                if (reader.nextType() == APLExpressionReader.APLExpressionType.Variable)
                {
                    vars.Add(reader.nextVariable());
                }
                else if (reader.nextType() == APLExpressionReader.APLExpressionType.Operator)
                {
                    if (curOperator == "")
                    {
                        curOperator = reader.nextOperator();
                    }
                    else
                    {
                        //  Otherwise we need to execute the previous function
                        //  Put the answer to that function in the vars 
                        //  And Reset the Operator
                        functions[curOperator[0]].setParams(vars);
                        APLVariable result = functions[curOperator[0]].Evaluate();
                        vars.Clear();
                        vars.Add(result);
                        Console.WriteLine("DEBUG: Result added to var stack: " + result.ToString());
                        curOperator = reader.nextOperator();
                    }
                }
             }
             if (!curOperator.Equals("") && vars.Count > 0)
             {
                 functions[curOperator[0]].setParams(vars);
                 APLVariable result = functions[curOperator[0]].Evaluate();
                 vars.Clear();
                 return result;
             }
            return APLVariable.FromDouble(0.0);
            }
            
            

        private bool isOperator(String op)
        {
            foreach (char f in functions.Keys)
            {
                if (op.Contains(f))
                    return true;
            }
            return false;
        }


    }
}

