using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.SyntaxTreeGeneration;
using APL.Errors;
namespace APL.Runtime
{
    class APLVariableFactory
    {
        /// <summary>
        /// Given a variable as a string determine its type
        /// </summary>
        /// <param name="val">string to try and parse</param>
        /// <returns>the vavriable type of the object</returns>
        public static VariableType GetVariableType(string val)
        {
            if (val.Length > 0)
            {
                Char c = val[0];
                if (Char.IsDigit(c))
                    return VariableType.Number;
                else if (c.Equals(' '))
                    return VariableType.String;
                else if (c.Equals('-'))
                    return VariableType.Number;
                else if (c.Equals('['))
                    return VariableType.Matrix;
                else if (c.Equals("\"") || Char.IsLetter(c))
                    return VariableType.String;
            }
            else if (val.Length == 0)
            {
                return VariableType.String;
            }
            throw new APLRuntimeError("Unsupported Variable Type: Unable to identify the variable type of data '" + val + "'");
        }

        /// <summary>
        /// Parse an STree into a variable
        /// </summary>
        /// <param name="val">stree to read</param>
        /// <returns>variable</returns>
        public static APLVariable GetVariable(STree val)
        {
            switch (val.Type)
            {
                case NodeType.Literal:
                    try
                    {
                        return APLNumberVariable.Parse(val.Value);
                    }
                    catch (FormatException ex)
                    {
                        try
                        {
                            return APLStringVariable.Parse(val.Value);
                        }
                        catch (FormatException fex)
                        {
                            throw new APLRuntimeError("Unsupported Variable Type: Unable to identify the variable type of data '" + val + "'");
                        }
                    }
                case NodeType.Matrix:
                    return GetArray(val);
                //case NodeType.VariableName:
                //    return Interpreter.GetInstance().GetVariable(val.Value);
                //case NodeType.FunctionCall:
                //    return Interpreter.GetInstance().InterpretLine(val);
                default:
                    throw new APLRuntimeError("Unsupported Variable Type: Unable to identify the variable type of data '" + val + "'");
            }
            
        }
        /// <summary>
        /// Parse a string into a variable
        /// </summary>
        /// <param name="val">string to read</param>
        /// <returns>variable</returns>
        public static APLVariable GetVariable(string val)
        {
            switch (GetVariableType(val))
            {
                case VariableType.Number:
                    try
                    {
                        return APLNumberVariable.Parse(val);
                    }
                    catch (FormatException ex)
                    {
                        try
                        {
                            return APLStringVariable.Parse(val);
                        }
                        catch (FormatException fex)
                        {
                            throw new APLRuntimeError("Unsupported Variable Type: Unable to identify the variable type of data '" + val + "'");
                        }
                    }
                case VariableType.String:
                    return APLStringVariable.Parse(val);
                case VariableType.Matrix:
                    return APLMatrixVariable.Parse(val);
            }
            throw new APLRuntimeError("Unsupported Variable Type: Unable to identify the variable type of data '" + val + "'");
        }
        /// <summary>
        /// Parse an STree into an APLMatrixVariable
        /// </summary>
        /// <param name="val">stree to read</param>
        /// <returns>variable</returns>
        public static APLVariable GetArray(STree b)
        {
            List<APLVariable> v = new List<APLVariable>();
            foreach (STree child in b.Children)
                v.Add(APLVariableFactory.GetVariable(child.Value));
            
            APLMatrixVariable a = new APLMatrixVariable(v,b.Children.Count);
            return a;
        }

    }
}
