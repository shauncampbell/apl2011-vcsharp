using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.SyntaxTreeGeneration;
using System.Collections;
using APL.Errors;
namespace APL.Runtime
{
    public class Interpreter
    {
        //  private instance variables
        private Hashtable          _vars             = new Hashtable();
        private Hashtable          _unaryFunctions   = new Hashtable(); 
        private Hashtable          _binaryFunctions  = new Hashtable();
        private Interpreter        _parent;
        private static Interpreter _instance;

        //  Constant values to dictate true and false
        public static APLVariable APL_TRUE  = APLVariableFactory.GetVariable("1");
        public static APLVariable APL_FALSE = APLVariableFactory.GetVariable("0");
        //  Error Messages
        public const string ERROR_INVALID_ARG_TYPE = "The argument types '{0}' and '{1}' passed to the function '{2}' are not acceptable.";
        public const string ERROR_INVALID_ARG_SIZE = "The dimensions of argument type '{0}' passed to the function '{1}' are not accetable.";
        public const string ERROR_INVALID_FUNC_ARG = "Too many arguments.Only unary and binary functions are supported.";
        /// <summary>
        /// Get the singleton instance of the primary interpreter
        /// </summary>
        /// <returns></returns>
        public static Interpreter GetInstance()
        {
            if (_instance == null)
                _instance = new Interpreter();

            return _instance;
        }

        /// <summary>
        /// Spawn a child instance of this interpreter
        /// Which will be used for interpreting local functions
        /// </summary>
        /// <returns>Child Interpreter instance</returns>
        public Interpreter SpawnChild()
        {
            return new Interpreter(this);
        }

        /// <summary>
        /// No argument constructor for creation of an interpreter
        /// </summary>
        private Interpreter()
        {
            _vars.Add("E", APLVariableFactory.GetVariable(Math.E.ToString()));
            _vars.Add("PI", APLVariableFactory.GetVariable(Math.PI.ToString()));
            _vars.Add("TRUE", APL_TRUE);
            _vars.Add("FALSE", APL_FALSE);
        }

        /// <summary>
        /// Create a new Interpreter Instance with a designated parent instance
        /// </summary>
        /// <param name="parent">parent interpreter instance</param>
        private Interpreter(Interpreter parent)
        {
            this._parent = parent;
            _vars.Add("E", APLVariableFactory.GetVariable(Math.E.ToString()));
            _vars.Add("PI", APLVariableFactory.GetVariable(Math.PI.ToString()));
            _vars.Add("TRUE", APL_TRUE);
            _vars.Add("FALSE", APL_FALSE);
        }

        //  Instance Methods

        /// <summary>
        /// Interpret an APL Command
        /// </summary>
        /// <param name="b">Tree to interpret</param>
        /// <returns></returns>
        public APLVariable InterpretLine(STree b)
        {
            try
            {
                //  Select an action based on the type of the top node
                switch (b.Type)
                {
                    case NodeType.Literal:
                        return APLVariableFactory.GetVariable(b);

                    case NodeType.Matrix:
                        List<APLVariable> elements = new List<APLVariable>();
                        foreach (STree child in b.Children)
                            elements.Add(InterpretLine(child));
                        return new APLMatrixVariable(elements, elements.Count);

                    case NodeType.VariableDeclaration:
                        if (b.Count == 2)
                            SetVariable(b.Item(0).Value, InterpretLine(b.Item(1)));
                        return APLVariableFactory.GetVariable("");

                    case NodeType.FunctionDeclaration:
                        try
                        {
                            if (b.Count == 3)
                            {
                                UnaryFunction uf = new UnaryFunction(b.Item(1).Value);
                                uf.Expression = b.Item(2);
                                _unaryFunctions.Add(b.Item(0).Value, uf);
                                return APLVariableFactory.GetVariable("");
                            }
                            else if (b.Count == 4)
                            {
                                BinaryFunction bf = new BinaryFunction(b.Item(1).Value, b.Item(2).Value);
                                bf.Expression = b.Item(3);
                                _binaryFunctions.Add(b.Item(0).Value, bf);
                                return APLVariableFactory.GetVariable("");
                            }

                        }
                        catch (ArgumentException ex)
                        {
                            throw new APLRuntimeError(ERROR_INVALID_FUNC_ARG);
                        }
                        return APLVariableFactory.GetVariable("");

                    case NodeType.VariableName:
                        return GetVariable(b.Value);

                    case NodeType.FunctionCall:
                        if (b.Count == 2)
                            return CallBinaryFunction(b.Value, InterpretLine(b.Item(0)), InterpretLine(b.Item(1)));
                        else if (b.Count == 1)
                            return CallUnaryFunction(b.Value, InterpretLine(b.Item(0)));
                        break;

                    case NodeType.Operator:
                        return CallOperator(b);

                    case NodeType.LetExpression:
                        Interpreter i = new Interpreter();
                        foreach (String s in _vars.Keys)
                            i.SetVariable(s, (APLVariable)_vars[s]);
                        i.SetVariable(b.Item(0).Value, InterpretLine(b.Item(1)));
                        return i.InterpretLine(b.Item(2));

                    case NodeType.ConditionalExpression:
                        if (b.Count == 2)
                        {
                            APLVariable v = InterpretLine(b.Item(0));
                            switch (v.GetType())
                            {
                                case VariableType.Number:
                                    return (v.Value.Equals(APL_TRUE.Value)) ? InterpretLine(b.Item(1)) : APLVariableFactory.GetVariable("");

                                case VariableType.Matrix:
                                    STree vfunc = new STree(NodeType.FunctionCall, "+");
                                    APLMatrixVariable mat_var = (APLMatrixVariable)v;
                                    for (int j = 0; j < mat_var.Rows; j++)
                                        for (int k = 0; k < mat_var.Columns; k++)
                                            if (!mat_var[j, k].Value.Equals(APL_FALSE.Value))
                                                return InterpretLine(b.Item(1));
                                    return APLVariableFactory.GetVariable("");
                            }

                        }
                        else if (b.Count == 3)
                        {
                            APLVariable v = InterpretLine(b.Item(0));
                            switch (v.GetType())
                            {
                                case VariableType.Number:
                                    return (v.Value.Equals(APL_FALSE.Value)) ? InterpretLine(b.Item(2)) : InterpretLine(b.Item(1));
                                case VariableType.Matrix:
                                    STree vfunc = new STree(NodeType.FunctionCall, "+");
                                    APLMatrixVariable mat_var = (APLMatrixVariable)v;
                                    for (int j = 0; j < mat_var.Rows; j++)
                                        for (int k = 0; k < mat_var.Columns; k++)
                                            if (!mat_var[j, k].Value.Equals(APL_FALSE.Value))
                                                return InterpretLine(b.Item(1));
                                    return InterpretLine(b.Item(2));
                            }

                        }
                        break;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new APLRuntimeError("Malformed Expression");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new APLRuntimeError("Malformed Expression");
            }
            return APLVariableFactory.GetVariable("");
        }

        /// <summary>
        /// Set the value of a variable local to this interpreter instance
        /// </summary>
        /// <param name="varName">variable identifier</param>
        /// <param name="value">value to set the variable to</param>
        public void SetVariable(string varName, APLVariable value)
        {
            //  Check for reserved variable names
            if (varName.Equals("TRUE") || varName.Equals("FALSE") || varName.Equals("PI") || varName.Equals("E"))
                throw new APLRuntimeError("Unable to set variable values for reserved variables");

            if (_vars.ContainsKey(varName))
                _vars[varName] = value;
            else
                _vars.Add(varName, value);
        }

        /// <summary>
        /// Gets the value of a variable in this interpreter or in parent processes
        /// </summary>
        /// <param name="varName">identifier of the variable to retrieve</param>
        /// <returns>value of the variable requested</returns>
        public APLVariable GetVariable(string varName)
        {
            if (_vars.ContainsKey(varName))
                return (APLVariable)_vars[varName];
            else if (_parent != null)
                return _parent.GetVariable(varName);
            else
                throw new APLRuntimeError("Variable not defined: The variable '" + varName + "' has been used before it has been defined");
        }

        /// <summary>
        /// Call a Unary function with name funcName
        /// </summary>
        /// <param name="funcName">name of the unary function to call</param>
        /// <param name="arg1">argument to pass to the function</param>
        /// <returns>the answer to the function</returns>
        public APLVariable CallUnaryFunction(string funcName, APLVariable arg1)
        {
            switch (funcName)
            {
                case "+":                       //  Complete
                    return arg1.Add();  
                case "-":                       //  Complete
                    return arg1.Subtract();
                case "x":                       //  Complete
                    return arg1.Multiply();
                case "%":                       //  Complete
                    return arg1.Divide();
                case "mod":                     //  Complete
                    return arg1.Modulus();
                case "smaller":                 //  Complete
                    return arg1.Smaller();
                case "bigger":                  //  Complete
                    return arg1.Bigger();
                case ">":                       //  Complete
                    return arg1.GreaterThan();
                case "<":                       //  Complete
                    return arg1.LessThan();
                case "^":                       //  Complete
                    return arg1.Power();
                case "log":                     //  Complete
                    return arg1.Log();
                case "?":                       //  Complete
                     return Deal(arg1);
                case "sorta":   
                     APLMatrixVariable c = (APLMatrixVariable)arg1;
                     List<APLVariable> flatUnsorted = new List<APLVariable>();
                     for (int k = 0; k < c.Rows; k++)
                         for (int j = 0; j < c.Columns; j++)
                             flatUnsorted.Add(c[k,j]);
                    flatUnsorted.Sort(delegate(APLVariable v1, APLVariable v2) { return ((double)v1.Value).CompareTo((double)v2.Value);});
                    return new APLMatrixVariable(flatUnsorted, c.Columns);
                case "p":                       //  Complete
                    return arg1.Size();
                case "i":                       //  Complete
                    return arg1.Index();
                case "!":                       //  Complete
                    return arg1.Factorial();
                case "~":                       //  Complete
                    return arg1.Invert();
                default:
                    if (!_unaryFunctions.ContainsKey(funcName))
                        throw new APLRuntimeError("Function not defined : The function '" + funcName + "' has been used before it has been defined.");
                    Interpreter i = Interpreter.GetInstance().SpawnChild();
                    UnaryFunction   uf  = (UnaryFunction)_unaryFunctions[funcName];
                    i.SetVariable(uf.Argument1, arg1);
                    return i.InterpretLine(uf.Expression);                    
            }
        }
       
        /// <summary>
        /// Call a Binary function with name funcName
        /// </summary>
        /// <param name="funcName">name of the binary function to call</param>
        /// <param name="arg1">argument to pass to the function</param>
        /// <param name="arg2">second argument to pass to the function</param>
        /// <returns>the answer to the function</returns>
        public APLVariable CallBinaryFunction(string funcName, APLVariable arg1, APLVariable arg2)
        {
            try
            {
                switch (funcName)
                {
                    case "+":
                        return arg1.Add(arg2);
                    case "-":
                        return arg1.Subtract(arg2);
                    case "x":
                        return arg1.Multiply(arg2);
                    case "%":
                        return arg1.Divide(arg2);
                    case "mod":
                        return arg1.Modulus(arg2);
                    case "!":
                        return arg1.Factorial(arg2);
                    case "bigger":
                        return arg1.Bigger(arg2);
                    case "smaller":
                        return arg1.Smaller(arg2);
                    case "^":
                        return arg1.Power(arg2);
                    case "log":
                        return arg1.Log(arg2);
                    case "=":
                        return arg1.Equals(arg2);
                    case "p":
                        return arg2.Resize(arg1);
                    case "?":
                        return Deal(arg1, arg2);
                    case ">":
                        return arg1.GreaterThan(arg2);
                    case "<":
                        return arg1.LessThan(arg2);
                    case "<=":
                        return arg1.LessEqThan(arg2);
                    case ">=":
                        return arg1.GreaterEqThan(arg2);
                    case "$":
                        if (arg1.GetType() == VariableType.Number && arg2.GetType() == VariableType.Matrix)
                            return ((APLMatrixVariable)arg2)[0, Int32.Parse(arg1.ToString())];
                        else if (arg1.GetType() == VariableType.Matrix && arg2.GetType() == VariableType.Matrix)
                            return ((APLMatrixVariable)arg2)[Int32.Parse(((APLMatrixVariable)arg1)[0, 0].ToString()), Int32.Parse(((APLMatrixVariable)arg1)[0, 1].ToString())];
                        else
                            throw new APLRuntimeError("Unable to extract elements from incompatible arguments");
                    case "i":
                        return arg1.Index(arg2);
                    case "find":
                        return arg1.Find(arg2);
                    case "~":
                        return arg1.Invert(arg2);
                    default:
                        if (!_binaryFunctions.ContainsKey(funcName))
                            throw new APLRuntimeError("Unable to call an undefined function '" + funcName + "'");

                        Interpreter interpreter = this.SpawnChild();
                        BinaryFunction   bf  = (BinaryFunction)_binaryFunctions[funcName];
                        interpreter.SetVariable(bf.Argument1, arg1);
                        interpreter.SetVariable(bf.Argument2, arg2);
                        return interpreter.InterpretLine(bf.Expression); 
                }
            }
            catch (FormatException ex)
            {
                throw new APLRuntimeError("Unsupported Arguments: The 'range (:)' function does not support the arguments '" + arg1.GetType() + "' and '" + arg2.GetType() + "'");
            }
            
        }

        /// <summary>
        /// Call an Operator function
        /// </summary>
        /// <param name="b">Tree containing the operator to execute</param>
        /// <returns>value of the expression</returns>
        public APLVariable CallOperator(STree b)
        {
            Console.WriteLine(b.ToString());
            switch (b.Value)
            {
                case "/":
                    return Reduce(b);
                case "\\":
                    return Scan(b);
                case ".":
                    return Dot(b);
                default:
                            throw new APLRuntimeError("The operator '" + b.Value + "' does not exist or does not support the arguments passed to it");
            }
            
        }

        /// <summary>
        /// Generate a set of random numbers
        /// </summary>
        /// <param name="l">left hand argument</param>
        /// <param name="r">right hand argument</param>
        /// <returns>value of the deal function</returns>
        public APLVariable Deal(APLVariable l, APLVariable r)
        {
            Random ra = new Random();
            switch (l.GetType())
            {
                case VariableType.Number:
                    List<APLVariable> vars = new List<APLVariable>();
                    int ncount = Int32.Parse(l.Value.ToString());
                    switch (r.GetType())
                    {
                        case VariableType.Number:
                            int nmax = Int32.Parse(r.Value.ToString());
                            int[] vals = new int[ncount];

                            for (int i = 0; i < ncount; i++)
                            {
                                int ranno = ra.Next(nmax);
                                if (ncount < nmax)
                                    while (vals.Contains(ranno))
                                        ranno = ra.Next(nmax);
                                vals[i] = ranno;
                                vars.Add(APLVariableFactory.GetVariable(ranno.ToString()));
                            }
                            return new APLMatrixVariable(vars, vars.Count);
                        case VariableType.Matrix:
                            APLMatrixVariable nmat = (APLMatrixVariable)r;
                            int maxValue = nmat.Columns * nmat.Rows;
                            for (int i=0; i < ncount; i++)
                            {
                                int randNo = ra.Next(maxValue);
                                vars.Add(nmat[(randNo/nmat.Columns),(randNo%nmat.Columns)]);
                            }
                            return new APLMatrixVariable(vars, vars.Count);
                        case VariableType.String:
                            APLStringVariable nstr = (APLStringVariable)r;
                            StringBuilder sb = new StringBuilder();
                            int maxLValue = nstr.Value.ToString().Length;
                            for (int i=0; i < ncount; i++)
                            {
                                int randNo = ra.Next(maxLValue-1);
                                sb.Append(nstr.Value.ToString()[randNo]);
                            }
                            return APLVariableFactory.GetVariable(sb.ToString());
                    }
                    break;
            }
            throw new APLRuntimeError("Unsupported Arguments: The 'Deal (?)' function does not support the arguments '" + l.GetType() + "' and '" + r.GetType() + "'");
            
        }

        /// <summary>
        /// Monadic function generate a single random number
        /// </summary>
        /// <param name="r">monadic argument</param>
        /// <returns>value of the deal function</returns>
        public APLVariable Deal(APLVariable r)
        {
            Random ra = new Random();
            switch (r.GetType())
            {
                case VariableType.Number:
                    int ncount = Int32.Parse(r.Value.ToString());
                    return APLVariableFactory.GetVariable(ra.Next(ncount).ToString());
                case VariableType.Matrix:
                    APLMatrixVariable nmat = (APLMatrixVariable)r;
                    int maxValue = nmat.Columns * nmat.Rows;
                    int randNo = ra.Next(maxValue);
                    return nmat[(randNo / nmat.Columns), (randNo % nmat.Columns)];
                case VariableType.String:
                    APLStringVariable nstr = (APLStringVariable)r;
                    StringBuilder sb = new StringBuilder();
                    int maxLValue = nstr.Value.ToString().Length;
                    int randsNo = ra.Next(maxLValue - 1);
                    sb.Append(nstr.Value.ToString()[randsNo]);
                    return APLVariableFactory.GetVariable(sb.ToString());
            }
            throw new APLRuntimeError("Unsupported Arguments: The 'Deal (?)' function does not support the arguments '" + r.GetType() + "'");
            
        }

        //  Operators
        /// <summary>
        /// Call the dot operator
        /// </summary>
        /// <param name="b">The stree containing the operator arguments</param>
        /// <returns>value of the expression</returns>
        public APLVariable Dot(STree b)
        {
            if (b.Children.Count < 4)
                throw new APLRuntimeError("The '.' operator requires two functions, and two parameters as arguments");
            STree leftVal = b.Children[2];
            STree rightVal = b.Children[3];
            STree leftFun = b.Children[0];
            STree rightFun = b.Children[1];

            APLVariable left = this.InterpretLine(leftVal);
            APLVariable right = this.InterpretLine(rightVal);

            if (left.GetType() != VariableType.Matrix || right.GetType() != VariableType.Matrix)
                throw new APLRuntimeError("The '.' operator requires two vectors as arguments.");

            APLMatrixVariable left_m = (APLMatrixVariable)left;
            APLMatrixVariable right_m = (APLMatrixVariable)right;

            List<APLVariable> output = new List<APLVariable>();
            
            //  Check if we want the outer product
            if (leftFun.Value.Equals("@"))
            {
                if ((left_m.Columns > 1 && left_m.Rows > 1) || (right_m.Columns > 1 && left_m.Rows > 1))
                    throw new APLRuntimeError("The '@.' operator requires two vectors as arguments.");

                int left_max    = (left_m.Columns   > left_m.Rows   )  ? left_m.Columns    : left_m.Rows;
                int right_max   = (right_m.Columns  > right_m.Rows  )  ? right_m.Columns   : right_m.Rows;
                bool left_ver   = (left_m.Rows      > left_m.Columns);
                bool right_ver  = (right_m.Rows     > right_m.Columns);

                List<APLVariable> vars = new List<APLVariable>();
                for (int i = 0; i < left_max; i++)
                {
                    for (int j = 0; j < right_max; j++)
                    {
                        vars.Add(this.CallBinaryFunction(rightFun.Value, (left_ver ? left_m[i, 0] : left_m[0, i]), (right_ver ? right_m[j, 0] : right_m[0,j])));
                    }
                }

                return new APLMatrixVariable(vars, right_max);

            }

            if (left_m.Rows != right_m.Columns)
                throw new APLRuntimeError("The '.' operator requires an 'm x n' matrix and an 'n x p' matrix as valid arguments.");

            //  Otherwise do the inner product
            for (int i = 0; i < left_m.Rows; i++)
            {
                List<APLVariable> vars = new List<APLVariable>();
                for (int j=0; j < left_m.Columns; j++)
                {
                    vars.Add(this.CallBinaryFunction(rightFun.Value, left_m[i, j], right_m[j, i]));
                }
                
                APLVariable currentVal = null;
                for (int k = 0; k < vars.Count; k++)
                {
                    if (currentVal == null)
                        currentVal = vars[k];
                    else
                        currentVal = this.CallBinaryFunction(leftFun.Value, currentVal, vars[k]);
                }

                output.Add(currentVal);
            }
            return new APLMatrixVariable(output, output.Count);
        }

        /// <summary>
        /// Call the reduce operator
        /// </summary>
        /// <param name="b">The stree containing the operator arguments</param>
        /// <returns>value of the expression</returns>
        public APLVariable Reduce(STree b)
        {
            //  There are always two arguments to the reduction operator
            if (b.Children.Count != 2)
                throw new APLRuntimeError("The arguments passed to the 'reduce (/)' operator are not valid");

            //  If the left hand side is a function, the we reduce
            //  If not then we expand

            STree leftArg   = b.Children[0];
            STree rightArg  = b.Children[1];

            if (leftArg.Type == NodeType.FunctionName && rightArg.Type == NodeType.Matrix)
            {
                APLMatrixVariable matrix = (APLMatrixVariable)this.InterpretLine(rightArg);
                APLVariable currentValue = null;
                List<APLVariable> v = new List<APLVariable>();
                for (int i = matrix.Rows - 1; i >= 0; i--)
                {
                    for (int j = matrix.Columns - 1; j >= 0; j--)
                    {
                        if (currentValue == null)
                            currentValue = matrix[i, j];
                        else
                            currentValue = this.CallBinaryFunction(leftArg.Value, currentValue, matrix[i, j]);
                    }
                    v.Add(currentValue);
                }

                if (v.Count > 1)
                    return new APLMatrixVariable(v, v.Count);
                else
                    return v[0];
                
            }
            else
            {
                throw new APLRuntimeError("The arguments passed to the 'reduce (/)' operator are not valid");
            }
        }

        /// <summary>
        /// Call the scan operator 
        /// </summary>
        /// <param name="b">The stree containing the operator arguments</param>
        /// <returns>value of the expression</returns>
        public APLVariable Scan(STree b)
        {
            //  There are always two arguments to the reduction operator
            if (b.Children.Count != 2)
                throw new APLRuntimeError("The arguments passed to the 'scan (\\)' operator are not valid");

            //  If the left hand side is a function, the we scan
            //  If not then we compress

            STree leftArg = b.Children[0];
            STree rightArg = b.Children[1];

            if (leftArg.Type == NodeType.FunctionName && rightArg.Type == NodeType.Matrix)
            {
                APLMatrixVariable matrix = (APLMatrixVariable)this.InterpretLine(rightArg);
                APLVariable currentValue = null;
                List<APLVariable> v = new List<APLVariable>();
                for (int i = matrix.Rows - 1; i >= 0; i--)
                {
                    for (int j = matrix.Columns - 1; j >= 0; j--)
                    {
                        if (currentValue == null)
                            currentValue = matrix[i, j];
                        else
                            currentValue = this.CallBinaryFunction(leftArg.Value, currentValue, matrix[i, j]);
                        v.Add(currentValue);
                    }
                }

                if (matrix.Rows > 1)
                    return new APLMatrixVariable(v, matrix.Rows);
                else
                    return new APLMatrixVariable(v, v.Count);

            }
            else
            {
                throw new APLRuntimeError("The arguments passed to the 'scan (\\)' operator are not valid");
            }
        }

        /// <summary>
        /// Call the compress operator
        /// </summary>
        /// <param name="b">The arguments to the left of the operator</param>
        /// <param name="c">The arguments to the right of the operator</param>
        /// <returns></returns>
        public APLVariable Compress(STree b, STree c)
        {
            APLVariable left = this.InterpretLine(b);
            APLVariable right = this.InterpretLine(c);

            if (left.GetType() != VariableType.Matrix)
                throw new APLRuntimeError("The Compress operation requires a matrix argument");
            APLMatrixVariable left_m = (APLMatrixVariable)left;
            switch (right.GetType())
            {
                case VariableType.Matrix:

                    APLMatrixVariable right_m = (APLMatrixVariable)right;
                    if (left_m.Rows != right_m.Rows || left_m.Columns != right_m.Columns)
                        throw new APLRuntimeError("Matrices sizes incorrect");
                    List<APLVariable> vars = new List<APLVariable>();
                    for (int i = 0; i < left_m.Rows; i++)
                        for (int j = 0; j < left_m.Columns; j++)
                            if (left_m[i, j].Value.Equals(Interpreter.APL_TRUE.Value))
                                vars.Add(right_m[i, j]);
                    return new APLMatrixVariable(vars, vars.Count);
                case VariableType.String:
                    APLStringVariable right_s = (APLStringVariable)right;
                    if (left_m.Columns != ((string)right.Value).Length)
                        throw new APLRuntimeError("Matrix specified on left must be the same length as the string on the right");
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < left_m.Columns; i++)
                        if (left_m[0, i].Value.Equals(Interpreter.APL_TRUE.Value))
                            sb.Append(((string)right_s.Value).Substring(i, 1));
                    return APLStringVariable.Parse(sb.ToString());
                default:
                    throw new APLRuntimeError("The arguments to the 'Compress (\\)' operator are not valid");
            }
        }

        //  Not yet implemented
        public APLVariable Expand(STree b, STree c)
        {
            return null;
        }

    }
}
