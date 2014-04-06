using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.SyntaxTreeGeneration;
using APL.Errors;

namespace APL.Runtime
{
    class APLMatrixVariable:APLVariable
    {
        private List<List<APLVariable>> _vals = new List<List<APLVariable>>();
        private int                     _cols = 0;

        /***
         * Common APLVariable Functions
         ***/

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">List of items in the array</param>
        /// <param name="columns">number of columns in the array (to calculate row amount)</param>
        public APLMatrixVariable(List<APLVariable> items, int columns)
        {
            if (items.Count % columns != 0)
                throw new APLRuntimeError("Invalid Matrix size: All rows and columns of a matrix must be the same size");
                
            _cols = columns;
            List<APLVariable> curRow = new List<APLVariable>();
            foreach(APLVariable v in items)
            {
                curRow.Add(v);
                if (curRow.Count == columns)
                {
                    _vals.Add(curRow);
                    curRow = new List<APLVariable>();
                }
            }
            if (curRow.Count != 0)
                _vals.Add(curRow);
        }

        /// <summary>
        /// Gets the variable type of this class
        /// </summary>
        /// <returns>VariableType.Matrix</returns>
        public new VariableType GetType()
        {
            return VariableType.Matrix;
        }

        /// <summary>
        /// The number of rows in this matrix
        /// </summary>
        public int Rows
        {
            get { return _vals.Count; }
        }

        /// <summary>
        /// The number of columns in this matrix
        /// </summary>
        public int Columns
        {
            get { return _cols; }
        }

        /// <summary>
        /// Add a row to this matrix
        /// </summary>
        /// <param name="v">list of values to add</param>
        public void AppendRow(List<APLVariable> v)
        {
            if (v.Count == _cols)
                _vals.Add(v);
            else
                throw new APLRuntimeError("Unable to add new row to matrix: All rows and columns of a matrix must be the same size");
        }

        /// <summary>
        /// Retrieve an element from this matrix
        /// </summary>
        /// <param name="row">row number of the element to extract (starts from 0)</param>
        /// <param name="column">column number of the element to extract (Starts from 0)</param>
        /// <returns>element specified by the row and column parameters</returns>
        public APLVariable this[int row, int column]
        {
            get
            {
                if (row >= _vals.Count || column >= _cols)
                    throw new APLRuntimeError("Invalid Matrix Item: The specified item does not exist within this matrix '" + row + "," + column + "'");

                return _vals[row][column];
            }
        }

        /// <summary>
        /// Return the number of items in this matrix
        /// </summary>
        public int Count
        {
            get { return _vals.Count*_cols; }
        }

        /// <summary>
        /// Provide raw access to the elements of this matrix
        /// </summary>
        public object Value
        {
            get
            {
                return _vals;
            }
            set
            {
                _vals = (List<List<APLVariable>>)value;
            }
        }

        /// <summary>
        /// Return the name of this matrix
        /// </summary>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Express this matrix as a string
        /// </summary>
        /// <returns>this matrix represented as a string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<APLVariable> row in _vals)
            {
                sb.Append("[");
                StringBuilder cb = new StringBuilder();
                foreach (APLVariable col in row)
                {
                    cb.Append(col.ToString() + " ");
                }
                string rowstring = cb.ToString().TrimEnd();
                sb.Append(rowstring);
                sb.Append("]\n");
            }
            return sb.ToString();

        }
        
        /***
         * Static factory method
         ***/

        /// <summary>
        /// Take a string and create a matrix from it
        /// </summary>
        /// <param name="value">matrix as represented by a string</param>
        /// <returns>new matrix object</returns>
        public static APLMatrixVariable Parse(string value)
        {
            List<APLVariable> result = new List<APLVariable>();
            string cutStrign = value.Substring(1, value.Length - 2);
            string[] values = cutStrign.Split(new char[] {' '});
            foreach (String s in values)
            {
                result.Add(APLVariableFactory.GetVariable(s));
            }
            return new APLMatrixVariable(result, result.Count);
        }

        /// <summary>
        /// Create a touple (matrix with 1 row and 2 columns)
        /// </summary>
        /// <param name="v1">first column value</param>
        /// <param name="v2">second column value</param>
        /// <returns>new matrix object</returns>
        public static APLMatrixVariable ParseTuple(double v1, double v2)
        {
            List<APLVariable> result = new List<APLVariable>();
            result.Add(APLNumberVariable.Parse(v1));
            result.Add(APLNumberVariable.Parse(v2));
            return new APLMatrixVariable(result,result.Count);

        }
        
        /***
         * Basic Arithmetic Operations
         ***/

        // +
        public APLVariable Add(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            result.Add(this[i,j].Add(arg));
                    return new APLMatrixVariable(result, this.Columns);
            
                case VariableType.Matrix:
                    APLMatrixVariable m_array = (APLMatrixVariable)arg;
                    if (m_array.Columns != this.Columns || m_array.Rows != this.Rows)
                        throw new APLRuntimeError("Matrix arguments to '+' must be of the same size");
                    List<APLVariable> m_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            m_result.Add(this[i,j].Add(m_array[i,j]));
                    return new APLMatrixVariable(m_result,this.Columns);

                case VariableType.String:
                    List<APLVariable> s_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            s_result.Add(this[i,j].Add(arg));
                    return new APLMatrixVariable(s_result, this.Columns);
                default:
                    throw new APLRuntimeError("Arguments to '+' are invalid");
            }
            
        }
        public APLVariable Add()
        {
            return this;
        }
        // -
        public APLVariable Subtract(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            result.Add(this[i,j].Subtract(arg));
                    return new APLMatrixVariable(result, this.Columns);
            
                case VariableType.Matrix:
                    APLMatrixVariable m_array = (APLMatrixVariable)arg;
                    if (m_array.Columns != this.Columns || m_array.Rows != this.Rows)
                        throw new APLRuntimeError("Matrix arguments to '-' must be of the same size");
                    List<APLVariable> m_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            m_result.Add(this[i,j].Subtract(m_array[i,j]));
                    return new APLMatrixVariable(m_result,this.Columns);

                case VariableType.String:
                    List<APLVariable> s_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            s_result.Add(this[i,j].Subtract(arg));
                    return new APLMatrixVariable(s_result, this.Columns);
                default:
                    throw new APLRuntimeError("Arguments to '-' are invalid");
            }
        }
        public APLVariable Subtract()
        {
            List<APLVariable> var = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    var.Add(this[i, j].Subtract());
            return new APLMatrixVariable(var, this.Columns);
        }
        //  x
        public APLVariable Multiply(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Matrix:
                    APLMatrixVariable m_array = (APLMatrixVariable)arg;
                    if (m_array.Columns != this.Columns || m_array.Rows != this.Rows)
                        throw new APLRuntimeError("Matrix arguments to 'x' must be of the same size");
                    List<APLVariable> m_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            m_result.Add(this[i,j].Multiply(m_array[i,j]));
                    return new APLMatrixVariable(m_result,this.Columns);

                default:
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            result.Add(this[i,j].Multiply(arg));
                    return new APLMatrixVariable(result, this.Columns);
            }
        }
        public APLVariable Multiply()
        {
            List<APLVariable> var = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    var.Add(this[i, j].Multiply());
            return new APLMatrixVariable(var, this.Columns);
        }
        //  %
        public APLVariable Divide(APLVariable arg)
        {
            switch (arg.GetType())
            {           
                case VariableType.Matrix:
                    APLMatrixVariable m_array = (APLMatrixVariable)arg;
                    if (m_array.Columns != this.Columns || m_array.Rows != this.Rows)
                        throw new APLRuntimeError("Matrix arguments to '%' must be of the same size");
                    List<APLVariable> m_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            m_result.Add(this[i,j].Divide(m_array[i,j]));
                    return new APLMatrixVariable(m_result,this.Columns);

                default:
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            result.Add(this[i, j].Divide(arg));
                    return new APLMatrixVariable(result, this.Columns);
            }
        }
        public APLVariable Divide()
        {
            List<APLVariable> var = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    var.Add(this[i, j].Divide());
            return new APLMatrixVariable(var, this.Columns);
        }
        // |
        public APLVariable Modulus(APLVariable arg)
        {
            switch (arg.GetType())
            {
                default:
                    List<APLVariable> result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            result.Add(this[i, j].Modulus(arg));
                    return new APLMatrixVariable(result, this.Columns);

                case VariableType.Matrix:
                    APLMatrixVariable m_array = (APLMatrixVariable)arg;
                    if (m_array.Columns != this.Columns || m_array.Rows != this.Rows)
                        throw new APLRuntimeError("Matrix arguments to '|' must be of the same size");
                    List<APLVariable> m_result = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            m_result.Add(this[i, j].Modulus(m_array[i, j]));
                    return new APLMatrixVariable(m_result, this.Columns);
            }
        }
        public APLVariable Modulus()
        {
            List<APLVariable> var = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    var.Add(this[i, j].Modulus());
            return new APLMatrixVariable(var, this.Columns);
        }
        //  bigger
        public APLVariable Bigger(APLVariable arg)
        {
            throw new NotImplementedException();
        }
        public APLVariable Bigger()
        {
            List<APLVariable> vars = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    vars.Add(this[i, j].Bigger());
            return new APLMatrixVariable(vars, this.Columns);
        }
        //  smaller
        public APLVariable Smaller(APLVariable arg)
        {
            throw new NotImplementedException();
        }
        public APLVariable Smaller()
        {
            List<APLVariable> vars = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    vars.Add(this[i, j].Smaller());
            return new APLMatrixVariable(vars, this.Columns);
        }
        //  biggest
        public APLVariable GreaterThan()
        {
            double biggest = -1;
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    if (biggest == -1 || (double)this[i,j].Value > biggest )
                        biggest = (double)this[i, j].Value;                      
                }
            }
            return APLNumberVariable.Parse(biggest);
        }
        public APLVariable GreaterThan(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    List<APLVariable> m_number = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            m_number.Add(this[i, j].GreaterThan(arg));
                    return new APLMatrixVariable(m_number, this.Columns);
                default:
                    throw new APLRuntimeError("GT E");
            }
            
        }
        
        /***
         * Basic Algebraic Operations
         ***/
        public APLVariable Index(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Matrix:
                    List<APLVariable> vars_m = new List<APLVariable>();
                    APLMatrixVariable m = (APLMatrixVariable)arg;
                    if (this.Rows != 1 || this.Columns != 2)
                        throw new APLRuntimeError("Unable to index into the matrix with the given parameters");

                    int r = (int)(double)this[0,0].Value;
                    int c = (int)(double)this[0,1].Value;
                    return m[r, c];        
                default:
                    throw new APLRuntimeError("Unable to index into the matrix with the given parameters");
            }
        }
        public APLVariable Index()
        {
            throw new APLRuntimeError("The 'i' function does not operate on matrices");
        }

        //  @
        public APLVariable Outer(APLVariable arg)
        {
            return null;
        }
        //  TODO:   Implement Matrix Power 
        public APLVariable Power(APLVariable arg)
        {
            VariableType tArg1 = this.GetType();
            VariableType tArg2 = arg.GetType();

            switch (tArg2)
            {
                case VariableType.Number:
                    List<APLVariable> nout = new List<APLVariable>();
                    for (int a = 0; a < this.Rows; a++)
                        for (int b = 0; b < this.Columns; b++)
                            nout.Add(this[a, b].Power(arg));
                    return new APLMatrixVariable(nout, this.Columns);
                case VariableType.Matrix:
                    APLMatrixVariable matArg = (APLMatrixVariable)arg;
                    if (matArg.Rows != this.Rows)
                        throw new APLRuntimeError("Unable to multiply matrices: Matrices must be of compatible dimensions in order to use the 'Multiply' function");
                    List<APLVariable> mout = new List<APLVariable>();
                    for (int a = 0; a < this.Rows; a++)
                        for (int b = 0; b < this.Columns; b++)
                            mout.Add(this[a, b].Power(matArg[a,b]));
                    return new APLMatrixVariable(mout, this.Columns);
                default:
                    throw new APLRuntimeError("Unsupported Arguments: The 'Multiply (*)' function does not support the arguments '" + this.GetType() + "' and '" + arg.GetType() + "'");
            }
        }
        //  TODO:   Implement
        public APLVariable Power()
        {
            throw new NotImplementedException();
        }
        //  TODO:   Implement
        public APLVariable NRoot(APLVariable arg)
        {
            throw new NotImplementedException();
        }
        
        public APLVariable Sqrt()
        {
            List<APLVariable> values = new List<APLVariable>();
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    values.Add(this[i, j].Sqrt());
            return new APLMatrixVariable(values, this.Columns);
        }
        //  TODO:   Implement
        public APLVariable Log(APLVariable arg)
        {
            throw new NotImplementedException();
        }
        //  TODO:   Implement
        public APLVariable Log()
        {
            throw new NotImplementedException();
        }
        public APLVariable Factorial()
        {
            List<APLVariable> vars = new List<APLVariable>();

            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    vars.Add(this[i, j].Factorial());
            return new APLMatrixVariable(vars, this.Columns);

        }
        /// <summary>
        /// Resize a matrix by adjusting the column count
        /// </summary>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public APLVariable Resize(APLVariable colCount)
        {
            switch (colCount.GetType())
            {
                /***
                 * When resizing an array with a numeric argument X
                 * The array is resized so that each row has X columns
                 ***/
                case VariableType.Number:
                    try
                    {
                        //  Set up internal variables for this case
                        int                 total = this.Rows * this.Columns;
                        int                 v     = Int32.Parse(colCount.Value.ToString());
                        List<APLVariable>   vals  = new List<APLVariable>();

                        //  Check that the resulting matrix will be of an allowed size
                        if ((this.Rows*this.Columns) % v != 0)
                            throw new APLRuntimeError("The parameters passed to the '#' function produce an invalid matrix size");

                        //  Add each variable to a new list
                        foreach (List<APLVariable> i in _vals)
                        {
                            foreach (APLVariable j in i)
                            {
                                vals.Add(j);
                            }
                        }

                        return new APLMatrixVariable(vals, (int)v);
                    }
                    catch (FormatException ex)
                    {
                        //  Catch non integer parameters (ie. decimal points)
                        throw new APLRuntimeError("The parameters passed to the '#' are invalid");
                    }
                /***
                 * When resizing an array with a array argument X
                 * The array is resized so that each row has X[0] columns
                 * and that X[1] rows exist
                 ***/
                case VariableType.Matrix:
                   APLMatrixVariable m = (APLMatrixVariable)colCount;
                   //Check that the argument has only 2 items in it

                   if (m.Columns != 2)
                       throw new APLRuntimeError("The '#' function only accepts two dimensions as an argument");

                   try
                   {
                       int drows = Int32.Parse(m[0, 1].Value.ToString());
                       int dcols = Int32.Parse(m[0, 0].Value.ToString());
                       
                       //   Check that the matrix will be of the correct size
                       if ((_vals[0].Count / dcols) != drows)
                           throw new APLRuntimeError("The parameters passed to the '#' function produce an invalid matrix size");

                       List<APLVariable> mvals = new List<APLVariable>();
                       foreach (List<APLVariable> i in _vals)
                       {
                           foreach (APLVariable j in i)
                           {
                               mvals.Add(j);
                           }
                       }
                       return new APLMatrixVariable(mvals, dcols);
                   }
                   catch (FormatException ex)
                   {
                       //   Catch non integer array elements in argument
                       throw new APLRuntimeError("The parameters passed to the '#' are invalid");
                   }
                default:
                   throw new APLRuntimeError("The parameters passed to the '#' are invalid");
            }
        }

        /// <summary>
        /// Check if a matrix equals another matrix
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public APLVariable Equals(APLVariable arg)
        {
            switch (arg.GetType())
            {
                case VariableType.Number:
                    List<APLVariable> vars = new List<APLVariable>();
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            vars.Add(this[i, j].Equals(arg));
                    return new APLMatrixVariable(vars, this.Columns);
                case VariableType.Matrix:
                    APLMatrixVariable var = (APLMatrixVariable)arg;
                    if (this.Columns != var.Columns || this.Rows != var.Rows)
                        throw new APLRuntimeError("The '=' function requires matrix arguments to be of the same size");

                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            if (this[i, j].Equals(var[i, j]).Value.Equals(Interpreter.APL_FALSE.Value))
                                return Interpreter.APL_FALSE;
                    return Interpreter.APL_TRUE;
                default:
                    return Interpreter.APL_FALSE;
            }
           
        }

        /// <summary>
        /// Return the size of the matrix as a touple
        /// </summary>
        /// <returns></returns>
        public APLVariable Size()
        {
            return APLMatrixVariable.ParseTuple(this.Rows, this.Columns);
            
        }

        public APLVariable Find(APLVariable arg)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(APLVariable other)
        {
            int val = 0;
            switch (other.GetType())
            {
                case VariableType.Matrix:
                    APLMatrixVariable m_var = (APLMatrixVariable) other;
                    if (m_var.Columns != this.Columns || m_var.Rows != this.Rows)
                        return -1;
                    for (int i = 0; i < this.Rows; i++)
                        for (int j = 0; j < this.Columns; j++)
                            val += this[i, j].CompareTo(m_var[i, j]);
                    return val;
                default:
                    return -1;
            }
        }


        public APLVariable Factorial(APLVariable arg)
        {
            throw new NotImplementedException();
        }


        public APLVariable Invert()
        {
            List<APLVariable> vals = new List<APLVariable>();

            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    vals.Add(this[i, j].Invert());
            return new APLMatrixVariable(vals, this.Columns);
        }

        public APLVariable Invert(APLVariable arg)
        {
            throw new NotImplementedException();
        }


        public APLVariable GreaterEqThan(APLVariable arg)
        {
            throw new NotImplementedException();
        }

        public APLVariable GreaterEqThan()
        {
            throw new NotImplementedException();
        }

        public APLVariable LessThan(APLVariable arg)
        {
            throw new NotImplementedException();
        }

        public APLVariable LessThan()
        {
            throw new NotImplementedException();
        }

        public APLVariable LessEqThan(APLVariable arg)
        {
            throw new NotImplementedException();
        }

        public APLVariable LessEqThan()
        {
            throw new NotImplementedException();
        }
    }
}
