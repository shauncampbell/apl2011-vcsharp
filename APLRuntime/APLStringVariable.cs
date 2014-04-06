using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.Errors;
namespace APL.Runtime
{
    class APLStringVariable: APLVariable
    {
        private String _val;
        private String _name;

        public new VariableType GetType()
        {
            return VariableType.String;
        }
        public object Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = (String)value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public override string ToString()
        {
            return _val;
        }
        /// <summary>
        /// Parse a string object from a string
        /// </summary>
        /// <param name="value">stirng to parse</param>
        /// <returns>string variable</returns>
        public static APLStringVariable Parse(string value)
        {
            APLStringVariable s = new APLStringVariable();
            s.Value = value;
            return s;
        }

        /***
         * Basic Arithmetic Operations
         ***/

        //  +
        public APLVariable Add()
        {
            return APLStringVariable.Parse(_val.ToUpper());
        }                        //  Complete
        public APLVariable Add(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.String:
                    StringBuilder sb = new StringBuilder();
                    sb.Append(this.Value.ToString());
                    sb.Append(arg.Value.ToString());
                    return APLVariableFactory.GetVariable(sb.ToString());
                case VariableType.Number:
                    StringBuilder sb_n = new StringBuilder();
                    sb_n.Append(this.Value.ToString());
                    sb_n.Append(arg.Value.ToString());
                    return APLVariableFactory.GetVariable(sb_n.ToString());
                case VariableType.Matrix:
                    APLMatrixVariable val_m = (APLMatrixVariable)arg;
                    List<APLVariable> vars = new List<APLVariable>();
                    for (int i = 0; i < val_m.Rows; i++)
                        for (int j = 0; j < val_m.Columns; j++)
                            vars.Add(this.Add(val_m[i, j]));
                    return new APLMatrixVariable(vars, val_m.Columns);
                default:
                    throw new APLRuntimeError("Unsupported Arguments: The 'Add (+)' function does not support the arguments '" + this.GetType() + "' and '" + arg.GetType() + "'");
            }

        }         //  Complete
        //  -
        public APLVariable Subtract()
        {
            return APLStringVariable.Parse(_val.ToLower());
        }                   //  Complete
        public APLVariable Subtract(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    double argval = (double)arg.Value;
                    if (argval > this._val.Length)
                        throw new APLRuntimeError("A string cannot be cropped longer than its length");
                    return APLStringVariable.Parse(this._val.Substring(0, (int)(_val.Length-argval)));
                case VariableType.Matrix:
                    APLMatrixVariable vars = (APLMatrixVariable)arg;
                    List<APLVariable> n_vars = new List<APLVariable>();
                    for (int i = 0; i < vars.Rows; i++)
                        for (int j = 0; j < vars.Columns; j++)
                            n_vars.Add(this.Subtract(vars[i, j]));
                    return new APLMatrixVariable(n_vars, vars.Columns);
                case VariableType.String:
                    APLStringVariable sars = (APLStringVariable)arg;
                    if (!_val.Contains((string)sars.Value))
                        throw new APLRuntimeError("Cannot remove the string '" + sars.Value + "' from '" + _val + "'");
                    else
                        return APLStringVariable.Parse(_val.Replace((string)sars.Value, ""));
                default:
                    throw new APLRuntimeError("The 'Subtract (-)' function cannot process the arguments passed to it");
            }
        }    //  Complete
        //  x
        public APLVariable Multiply()
        {
            throw new APLRuntimeError("The unary function 'multiply (x)' does not function on strings");
        }                   //  Complete
        public APLVariable Multiply(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < (double)arg.Value; i++)
                        sb.Append(_val);
                    return APLStringVariable.Parse(sb.ToString());
                case VariableType.Matrix:
                    APLMatrixVariable mat = (APLMatrixVariable)arg;
                    List<APLVariable> vars = new List<APLVariable>();
                    for (int i = 0; i < mat.Rows; i++)
                        for (int j = 0; j < mat.Columns; j++)
                            vars.Add(this.Multiply(mat[i, j]));
                    return new APLMatrixVariable(vars, mat.Columns);
                default:
                    throw new APLRuntimeError("The binary function 'multiply (x)' does not function with two string arguments");
            }
        }    //  Complete
        //  %
        public APLVariable Divide()
        {
            List<APLVariable> vars = new List<APLVariable>();
            for (int i = 0; i < _val.Length; i++)
                vars.Add(APLStringVariable.Parse(_val[i].ToString()));
            return new APLMatrixVariable(vars,_val.Length);
        }                     //  Complete
        public APLVariable Divide(APLVariable arg)         // Complete
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    double len = (double)((APLNumberVariable)arg).Value;
                    List<APLVariable> vars = new List<APLVariable>();
                    int i = 0;
                    while (i < _val.Length) 
                    {
                        if (i < (_val.Length-len))
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int j = 0; j < len; j++)
                                sb.Append(_val[i + j]);
                            vars.Add(APLStringVariable.Parse(sb.ToString()));
                            i += (int)len;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int j = 0; j < (_val.Length - i); j++)
                                sb.Append(_val[i + j]);
                            vars.Add(APLStringVariable.Parse(sb.ToString()));
                            i = (_val.Length);
                        }
                    }
                    return new APLMatrixVariable(vars,vars.Count);
                default:
                    throw new APLRuntimeError("Arguments passed to the 'divide (%)' are not valid");
            }
        }
        //  |
        public APLVariable Modulus()
        {
            throw new APLRuntimeError("The unary function 'modulus (|)' does not function on strings.");
        }                    // Complete
        public APLVariable Modulus(APLVariable arg)
        {
            throw new APLRuntimeError("The unary binary 'modulus (|)' does not function on strings.");
        }     // Complete
        //  bigger
        public APLVariable Bigger()
        {
            throw new APLRuntimeError("The unary function 'bigger' does not function on strings.");
        }                       //  Complete
        public APLVariable Bigger(APLVariable arg)
        {
            return (this.CompareTo(arg) < 0) ? this : arg;
        }       //Complete
        //smaller
        public APLVariable Smaller()
        {
            throw new APLRuntimeError("The unary function 'smaller' does not function on strings.");
        }                   // Complete
        public APLVariable Smaller(APLVariable arg)
        {
            return (this.CompareTo(arg) > 0) ? this : arg;
        }   // Complete
        //  *
        public APLVariable Power(APLVariable arg)
        {
            throw new APLRuntimeError("The unary function 'power (*)' does not function on strings.");
        }
        public APLVariable Power()
        {
            throw new APLRuntimeError("The binary function 'power (*)' does not function on strings.");
        }
        //  log
        public APLVariable Log(APLVariable arg)
        {
            throw new APLRuntimeError("The unary function 'log' does not function on strings.");
        }
        public APLVariable Log()
        {
            throw new APLRuntimeError("The binary function 'log' does not function on strings.");
        }
        //  !
        public APLVariable Factorial()
        {
            throw new APLRuntimeError("The unary function 'factorial (!)' does not function on strings.");
        }
        public APLVariable Factorial(APLVariable arg)
        {
            throw new APLRuntimeError("The binary function 'factorial (!)' does not function on strings.");
        }
        //  ~
        public APLVariable Invert()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _val.Length; i++)
                if (Char.IsUpper(_val[i]))
                    sb.Append(Char.ToLower(_val[i]));
                else
                    sb.Append(Char.ToUpper(_val[i]));
            return APLStringVariable.Parse(sb.ToString());
        }
        public APLVariable Invert(APLVariable arg)
        {
            throw new APLRuntimeError("The binary function 'invert (~)' does not function on strings.");
        }

        //  index
        public APLVariable Index()
        {
            throw new APLRuntimeError("The unary function 'index (i)' does not function on strings.");
        }
        public APLVariable Index(APLVariable arg)
        {
            throw new APLRuntimeError("The binary function 'index (i)' does not function correctly with the arguments given");
        }
        //  find
        public APLVariable Find(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.String:
                    return APLNumberVariable.Parse(((string)((APLStringVariable)arg).Value).IndexOf(_val));
                case VariableType.Matrix:
                    APLMatrixVariable m = (APLMatrixVariable)arg;
                    List<APLVariable> vars = new List<APLVariable>();
                    for (int i = 0; i < m.Rows; i++)
                        for (int j = 0; j < m.Columns; j++)
                            vars.Add(this.Find(m[i, j]));
                    return new APLMatrixVariable(vars, m.Columns);
                default:
                    throw new APLRuntimeError("The binary function 'find' does notfunction correctly with the arguments given");
            }
        }
        //  p
        public APLVariable Resize(int colCount)
        {
            throw new NotImplementedException();
        }
        public APLVariable Resize(APLVariable colCount)
        {
            throw new APLRuntimeError("The binary function 'reshape (p)' does not function correctly with the arguments given");
        }
        public APLVariable Size()
        {
            return APLMatrixVariable.ParseTuple(1, this._val.Length);
        }
        //  =
        public APLVariable Equals(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.String:
                    List<APLVariable> string_s = new List<APLVariable>();
                    string arg_Var = (string)arg.Value;
                    int bigLength = ((this._val.Length > arg_Var.Length) ? this._val.Length : arg_Var.Length);
                    for (int i = 0; i < bigLength; i++)
                    {
                        if (i >= _val.Length || i >= arg_Var.Length)
                            string_s.Add(APLNumberVariable.Parse(0));
                        else if (_val[i].Equals(arg_Var[i]))
                            string_s.Add(APLNumberVariable.Parse(1));
                        else
                            string_s.Add(APLNumberVariable.Parse(0));
                    }
                    return new APLMatrixVariable(string_s, string_s.Count);
                default:
                    return Interpreter.APL_FALSE;
            }

        }
        //  >
        public APLVariable GreaterThan(APLVariable arg)
        {
            return (this.CompareTo(arg) > 0) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
        }
        public APLVariable GreaterThan()
        {
            throw new APLRuntimeError("The unary function 'greater than (>)' does not function on strings");
        }
        //  <
        public APLVariable LessThan(APLVariable arg)
        {
            return (this.CompareTo(arg) < 0) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
        }
        public APLVariable LessThan()
        {
            throw new APLRuntimeError("The unary function 'less than (<)' does not function on strings");
        }
        //  >=
        public APLVariable GreaterEqThan(APLVariable arg)
        {
            return (this.CompareTo(arg) >= 0) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
        }
        public APLVariable GreaterEqThan()
        {
            throw new APLRuntimeError("The unary function 'greater than equal (>=)' does not function on strings");
        }
        //  <=
        public APLVariable LessEqThan(APLVariable arg)
        {
            return (this.CompareTo(arg) <= 0) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
        }
        public APLVariable LessEqThan()
        {
            throw new APLRuntimeError("The unary function 'less than equal (<=)' does not function on strings");
        }

        /***
         * Unimplemented Stubs
         ***/
        public APLVariable NRoot(APLVariable arg)
        {
            throw new NotImplementedException();
        }
        public APLVariable Sqrt()
        {
            throw new NotImplementedException();
        }
        public APLVariable Outer(APLVariable arg)
        {
            throw new NotImplementedException();
        }


        public int CompareTo(APLVariable other)
        {
            return _val.CompareTo((string)other.Value);
        }



    }
}
