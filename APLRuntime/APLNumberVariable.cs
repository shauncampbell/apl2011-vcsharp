using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.Errors;
using APL.SyntaxTreeGeneration;

namespace APL.Runtime
{
    public class APLNumberVariable:APLVariable
    {

        private double _val;
        private String _name;

        /***
         * Common APLVariable Functions
         ***/
        public new VariableType GetType()
        {
            return VariableType.Number;
        }
        public object Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = (double)value;
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
            return _val.ToString();
        }

        /***
         * Static factory method
         ***/
        /// <summary>
        /// Create a new number object from a string reresentation
        /// </summary>
        /// <param name="s">string to parse</param>
        /// <returns>variable as object</returns>
        public static APLNumberVariable Parse(String s)
        {
            APLNumberVariable v = new APLNumberVariable();
            v.Value = Double.Parse(s);
            return v;
        }
        /// <summary>
        /// Create a new number object from a number
        /// </summary>
        /// <param name="d">double to parse</param>
        /// <returns>variable as object</returns>
        public static APLNumberVariable Parse(double d)
        {
            APLNumberVariable v = new APLNumberVariable();
            v.Value = d;
            return v;
        }
        
        /***
         * Basic Arithmetic Operations
         ***/

        //  +
        public APLVariable Add(APLVariable arg)
        {
            VariableType tArg1 = this.GetType();
            VariableType tArg2 = arg.GetType();

            switch (tArg2)
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    return APLNumberVariable.Parse((n_arg1 + n_arg2));

                case VariableType.Matrix:
                    double m_arg1 = (double)this.Value;
                    APLMatrixVariable array = (APLMatrixVariable)arg;
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < array.Rows; i++)
                        for (int j = 0; j < array.Columns; j++)
                            result.Add(array[i,j].Add(this));
                    return new APLMatrixVariable(result, array.Columns);

                case VariableType.String:
                    double s_arg1 = (double)this.Value;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(s_arg1);
                    sb.Append(((APLStringVariable)arg).Value);
                    return APLVariableFactory.GetVariable(sb.ToString());

                default:
                    throw new APLRuntimeError("The arguments passed to the '+' function are invalid");
            }
        }
        public APLVariable Add()
        {
            return this;
        }
        //  -
        public APLVariable Subtract(APLVariable arg)
        {
            VariableType tArg1 = this.GetType();
            VariableType tArg2 = arg.GetType();
            switch (tArg2)
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    return APLNumberVariable.Parse(n_arg1 - n_arg2);
                case VariableType.Matrix:
                    double m_arg1 = (double)this.Value;
                    APLMatrixVariable array = (APLMatrixVariable)arg;
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < array.Rows; i++)
                        for (int j = 0; j < array.Columns; j++)
                            result.Add(this.Subtract(array[i,j]));
                    return new APLMatrixVariable(result, array.Columns);
                default:
                    throw new APLRuntimeError("The arguments passed to the '-' function are invalid");
            }

        }
        public APLVariable Subtract()
        {
            return APLNumberVariable.Parse(-1 * _val);
        }
        //  x
        public APLVariable Multiply(APLVariable arg)
        {
            VariableType tArg1 = this.GetType();
            VariableType tArg2 = arg.GetType();

            switch (tArg2)
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    return APLNumberVariable.Parse((n_arg1 * n_arg2));

                case VariableType.Matrix:
                    double m_arg1 = (double)this.Value;
                    APLMatrixVariable array = (APLMatrixVariable)arg;
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < array.Rows; i++)
                        for (int j = 0; j < array.Columns; j++)
                            result.Add(this.Multiply(array[i,j]));
                    return new APLMatrixVariable(result, array.Columns);

                case VariableType.String:
                    double s_arg1 = (double)this.Value;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < s_arg1; i++)
                        sb.Append(((APLStringVariable)arg).Value);
                    return APLVariableFactory.GetVariable(sb.ToString());

                default:
                    throw new APLRuntimeError("The arguments passed to the 'x' function are invalid");
            }

        }
        public APLVariable Multiply()
        {
            if (_val > 0)
                return APLNumberVariable.Parse(1);
            else if (_val < 0)
                return APLNumberVariable.Parse (- 1);
            else
                return APLNumberVariable.Parse(0);
        }
        //  %
        public APLVariable Divide(APLVariable arg)
        {
            VariableType tArg1 = this.GetType();
            VariableType tArg2 = arg.GetType();

            switch (tArg2)
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    if (n_arg2 == 0)
                        throw new APLRuntimeError("The function 'divide (%)' is not capable of dividing by 0");
                    return APLNumberVariable.Parse((n_arg1 / n_arg2));

                case VariableType.Matrix:
                    double m_arg1 = (double)this.Value;
                    APLMatrixVariable array = (APLMatrixVariable)arg;
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < array.Rows; i++)
                        for (int j = 0; j < array.Columns; j++)
                            result.Add(this.Divide(array[i,j]));
                    return new APLMatrixVariable(result, array.Columns);

                default:
                    throw new APLRuntimeError("The arguments passed to the '%' function are invalid");
            }
        }
        public APLVariable Divide()
        {
            return APLNumberVariable.Parse(1 / this._val);
        }
        //  |
        public APLVariable Modulus(APLVariable arg)
        {
            VariableType tArg1 = this.GetType();
            VariableType tArg2 = arg.GetType();

            switch (tArg2)
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    if (n_arg2 == 0)
                        throw new APLRuntimeError("The function 'modulus (|)' is not capable of dividing by 0");

                    return APLNumberVariable.Parse((n_arg1 % n_arg2));

                case VariableType.Matrix:
                    double m_arg1 = (double)this.Value;
                    APLMatrixVariable array = (APLMatrixVariable)arg;
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < array.Rows; i++)
                        for (int j = 0; j < array.Columns; j++)
                            result.Add(this.Modulus(array[i,j]));
                    return new APLMatrixVariable(result, array.Columns);

                default:
                    throw new APLRuntimeError("The arguments passed to the '|' function are invalid");
            }
        }
        public APLVariable Modulus()
        {
            double v = (double)this.Value;
            return APLNumberVariable.Parse(v < 0 ? (-1 * v) : v);
        }
        //  bigger
        public APLVariable Bigger(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    return APLNumberVariable.Parse((n_arg1 > n_arg2 ? n_arg1 : n_arg2));

                default:
                    throw new APLRuntimeError("The arguments passed to the 'bigger' function are invalid");
            }
        }
        public APLVariable Bigger()
        {
            return APLNumberVariable.Parse(Math.Ceiling(_val));
        }
        //  smaller
        public APLVariable Smaller(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    double n_arg1 = (double)this.Value;
                    double n_arg2 = (double)arg.Value;
                    return APLNumberVariable.Parse((n_arg1 < n_arg2 ? n_arg1 : n_arg2));

                default:
                    throw new APLRuntimeError("The arguments passed to the 'smaller' function are invalid");
            }
        }
        public APLVariable Smaller()
        {
            return APLNumberVariable.Parse(Math.Floor(_val));
        }
        //  *
        public APLVariable Power(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    return APLNumberVariable.Parse(Math.Pow(_val, (double)arg.Value));
                case VariableType.Matrix:
                    APLMatrixVariable m_box = (APLMatrixVariable)arg;
                    List<APLVariable> m_vars = new List<APLVariable>();
                    for (int i = 0; i < m_box.Rows; i++)
                        for (int j = 0; j < m_box.Columns; j++)
                            m_vars.Add(this.Power(m_box[i, j]));
                    return new APLMatrixVariable(m_vars, m_box.Columns);
                default:
                    throw new APLRuntimeError("Invalid arguments to the 'power (*)' function");
            }
        }
        public APLVariable Power()
        {
            return APLNumberVariable.Parse(Math.Pow(Math.E, _val));
        }
        //  log
        public APLVariable Log(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    return APLNumberVariable.Parse(Math.Log(_val, (double)arg.Value));
                case VariableType.Matrix:
                    APLMatrixVariable m_box = (APLMatrixVariable)arg;
                    List<APLVariable> m_vars = new List<APLVariable>();
                    for (int i = 0; i < m_box.Rows; i++)
                        for (int j = 0; j < m_box.Columns; j++)
                            m_vars.Add(this.Log(m_box[i, j]));
                    return new APLMatrixVariable(m_vars, m_box.Columns);
                default:
                    throw new APLRuntimeError("Invalid arguments to the 'log' function");
            }

        }
        public APLVariable Log()
        {
            return APLNumberVariable.Parse(Math.Log(_val, Math.E));
        }
        //  !
        public APLVariable Factorial(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    double n = (double)arg.Value;
                    double n_fact = 1;
                    for (int i = 1; i < n; i++)
                        n_fact *= i;

                    double r = _val;
                    double r_fact = 1;
                    for (int i = 1; i < r; i++)
                        r_fact *= i;

                    double n_r_fact = 1;
                    for (int i = 1; i < (n - r); i++)
                        n_r_fact *= i;

                    return APLNumberVariable.Parse(n_fact / ((r_fact * n_r_fact)));
                case VariableType.Matrix:
                    APLMatrixVariable val_m = (APLMatrixVariable)arg;
                    List<APLVariable> vars = new List<APLVariable>();
                    for (int i = 0; i < val_m.Rows; i++)
                        for (int j = 0; j < val_m.Columns; j++)
                            vars.Add(this.Factorial(val_m[i, j]));
                    return new APLMatrixVariable(vars, val_m.Columns);
                default:
                    throw new NotImplementedException();
            }
            

        }
        public APLVariable Factorial()
        {
            double factout = 0;
            for (int i = 0; i < (_val); i++)
            {
                factout += i;
            }
            return APLVariableFactory.GetVariable(factout.ToString());
        }
        //  ~
        public APLVariable Invert(APLVariable arg)
        {
            throw new NotImplementedException();
        }
        public APLVariable Invert()
        {
            if (_val == 0 || _val == 1)
                return APLNumberVariable.Parse((_val == 0) ? 1 : 0);
            else
                return this.Subtract();
        }

        /***
         * Array Operations
         ***/

        //  i
        public APLVariable Index(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Matrix:
                    List<APLVariable> vars_m    = new List<APLVariable>();
                    APLMatrixVariable m         = (APLMatrixVariable)arg;
                    return m[0, (int)_val];
  
                default:
                    throw new APLRuntimeError("The arguments passed to the '%' function are invalid");
            }
        }
        public APLVariable Index()
        {
            List<APLVariable> vars = new List<APLVariable>();
            int val = (int) _val;
            for (int i = 1; i <= val; i++)
                vars.Add(APLNumberVariable.Parse(i));
            return new APLMatrixVariable(vars, val);
        }
        //  find
        public APLVariable Find(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Matrix:
                    APLMatrixVariable var_m = (APLMatrixVariable)arg;
                    for (int i = 0; i < var_m.Rows; i++)
                        for (int j = 0; j < var_m.Columns; j++)
                            if (var_m[i, j].Value.Equals(this.Value))
                                return APLMatrixVariable.ParseTuple(i, j);
                    return APLNumberVariable.Parse(-1);
                default:
                    throw new APLRuntimeError("Arguments passed to the 'find' function are not valid");
            }
        }
        //p
        public APLVariable Resize(APLVariable colCount)
        {
            switch (colCount.GetType())
            {
                case VariableType.Number:
                    try
                    {
                        List<APLVariable> vars = new List<APLVariable>();
                        int ncols = Int32.Parse(colCount.Value.ToString());
                        for (int i = 0; i < ncols; i++)
                            vars.Add(this);
                        return new APLMatrixVariable(vars, ncols);
                    }
                    catch (FormatException ex)
                    {
                        throw new APLRuntimeError(String.Format(Interpreter.ERROR_INVALID_ARG_TYPE, VariableType.Number.ToString(), "Resize (p)"));
                    }
                case VariableType.Matrix:
                    try
                    {
                        APLMatrixVariable m = (APLMatrixVariable)colCount;
                        List<APLVariable> vars = new List<APLVariable>();
                        if (m.Columns != 2)
                            throw new APLRuntimeError(String.Format(Interpreter.ERROR_INVALID_ARG_SIZE, VariableType.Matrix.ToString(), "Resize (p)"));

                        int ncols = Int32.Parse(m[0, 0].Value.ToString());
                        int nrows = Int32.Parse(m[0, 1].Value.ToString());

                        for (int i = 0; i < nrows; i++)
                            for (int j = 0; j < ncols; j++)
                                vars.Add(this);
                        return new APLMatrixVariable(vars, ncols);
                    }
                    catch (FormatException ex)
                    {
                        throw new APLRuntimeError("The parameters passed to the function 'resize (p)' are invalid");
                    }
                default:
                    throw new APLRuntimeError("The parameters passed to the function 'resize (p)' are invalid");
            }
        }
        public APLVariable Size()
        {
            return APLMatrixVariable.ParseTuple(1, 1);
        }
        // =
        public APLVariable Equals(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Matrix:
                    return arg.Equals(this);
                case VariableType.Number:
                    return (((double)this.Value) == ((double)arg.Value)) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
                default:
                    return Interpreter.APL_FALSE;
            }
        }
        // >
        public APLVariable GreaterThan(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    return ((_val > (double)arg.Value)) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
                case VariableType.Matrix:
                    APLMatrixVariable m_matrix = (APLMatrixVariable)arg;
                    List<APLVariable> m_vals = new List<APLVariable>();
                    for (int i = 0; i < m_matrix.Rows; i++)
                        for (int j = 0; j < m_matrix.Columns; j++)
                            m_vals.Add(this.GreaterThan(m_matrix[i, j]));
                    return new APLMatrixVariable(m_vals, m_matrix.Columns);

                default:
                    throw new NotImplementedException();
            }
        }
        public APLVariable GreaterThan()
        {
            return this;
        }
        //  <
        public APLVariable LessThan(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    return ((_val < (double)arg.Value)) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
                case VariableType.Matrix:
                    APLMatrixVariable m_matrix = (APLMatrixVariable)arg;
                    List<APLVariable> m_vals = new List<APLVariable>();
                    for (int i = 0; i < m_matrix.Rows; i++)
                        for (int j = 0; j < m_matrix.Columns; j++)
                            m_vals.Add(this.LessThan(m_matrix[i, j]));
                    return new APLMatrixVariable(m_vals, m_matrix.Columns);

                default:
                    throw new NotImplementedException();
            }            
        }
        public APLVariable LessThan()
        {
            return this;
        }
        //  >=
        public APLVariable GreaterEqThan(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    return ((_val >= (double)arg.Value)) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
                case VariableType.Matrix:
                    APLMatrixVariable m_matrix = (APLMatrixVariable)arg;
                    List<APLVariable> m_vals = new List<APLVariable>();
                    for (int i = 0; i < m_matrix.Rows; i++)
                        for (int j = 0; j < m_matrix.Columns; j++)
                            m_vals.Add(this.GreaterEqThan(m_matrix[i, j]));
                    return new APLMatrixVariable(m_vals, m_matrix.Columns);

                default:
                    throw new NotImplementedException();
            }
        }
        public APLVariable GreaterEqThan()
        {
            throw new NotImplementedException();
        }
        //  <=
        public APLVariable LessEqThan(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    return (_val <=((double)arg.Value)) ? Interpreter.APL_TRUE : Interpreter.APL_FALSE;
                case VariableType.Matrix:
                    APLMatrixVariable m_matrix = (APLMatrixVariable)arg;
                    List<APLVariable> m_vals = new List<APLVariable>();
                    for (int i = 0; i < m_matrix.Rows; i++)
                        for (int j = 0; j < m_matrix.Columns; j++)
                            m_vals.Add(this.LessEqThan(m_matrix[i, j]));
                    return new APLMatrixVariable(m_vals, m_matrix.Columns);

                default:
                    throw new NotImplementedException();
            }
        }
        public APLVariable LessEqThan()
        {
            throw new NotImplementedException();
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
            return APLVariableFactory.GetVariable(Math.Sqrt(Double.Parse(this.Value.ToString())).ToString());
        }
        public APLVariable Outer(APLVariable arg)
        {
            throw new NotImplementedException();
        }






        public int CompareTo(APLVariable other)
        {
            switch (other.GetType())
            {
                case VariableType.Number:
                    double other_val = (double)other.Value;
                    return _val.CompareTo(other_val);
                default:
                    return -1;
        
            }
        }
    }
}
