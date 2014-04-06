using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.Runtime
{
    /// <summary>
    /// Each variable has a type associated with it
    /// </summary>
    public enum VariableType
    {
        String, Number, Matrix
    }

    /// <summary>
    /// Common interface for all variable types.
    /// </summary>
    public interface APLVariable: IComparable<APLVariable>
    {
        String ToString();
        VariableType GetType();
        Object Value
        {
            get;
            set;
        }
        String Name
        {
            get;
            set;
        }


        //  Simple Arithmetic Operators

        /// <summary>
        /// The built in add function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the built in function</returns>
        APLVariable Add(APLVariable arg);
        /// <summary>
        /// The built in add function. Monadic
        /// </summary>
        /// <returns>the result of the add function on this variable</returns>
        APLVariable Add();
        /// <summary>
        /// The built in subtract function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the built in function</returns>
        APLVariable Subtract(APLVariable arg);
        /// <summary>
        /// The built in subtract function. Monadic
        /// </summary>
        /// <returns>result of the subtract function on this variable</returns>
        APLVariable Subtract();
        /// <summary>
        /// The built in multiply function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the multiplication function</returns>
        APLVariable Multiply(APLVariable arg);
        /// <summary>
        /// The built in multily function. Monadic
        /// </summary>
        /// <returns>result of the multilication function on this variable</returns>
        APLVariable Multiply();
        /// <summary>
        /// The built in divide function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the division function</returns>
        APLVariable Divide(APLVariable arg);
        /// <summary>
        /// The built in divide function. Monadic
        /// </summary>
        /// <returns>result of the multiplication function on this variable</returns>
        APLVariable Divide();
        /// <summary>
        /// The built in modulus function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the modulus function</returns>
        APLVariable Modulus(APLVariable arg);
        /// <summary>
        /// The built in modulus function
        /// </summary>
        /// <returns>result of the modulus function</returns>
        APLVariable Modulus();
        /// <summary>
        /// The built in equals function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the equals function</returns>
        APLVariable Equals(APLVariable arg);
        /// <summary>
        /// The built in index function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the index function</returns>
        APLVariable Index(APLVariable arg);
        /// <summary>
        /// The built in index function
        /// </summary>
        /// <returns>result of the index function</returns>
        APLVariable Index();
        /// <summary>
        /// The built in find function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the find function</returns>
        APLVariable Find(APLVariable arg);
        /// <summary>
        /// The built in outer function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the outer function</returns>
        APLVariable Outer(APLVariable arg);
        /// <summary>
        /// The built in bigger function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the bigger function</returns>
        APLVariable Bigger(APLVariable arg);
        /// <summary>
        /// The built in bigger function
        /// </summary>
        /// <returns>result of the bigger function</returns>
        APLVariable Bigger();
        /// <summary>
        /// The built in smaller function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the smaller function</returns>
        APLVariable Smaller(APLVariable arg);
        /// <summary>
        /// The built in smaller function
        /// </summary>
        /// <returns>result of the smaller function</returns>
        APLVariable Smaller();
        /// <summary>
        /// The built in greater than function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the greater than function</returns>
        APLVariable GreaterThan(APLVariable arg);
        /// <summary>
        /// The built in greater than function
        /// </summary>
        /// <returns>result of the greater than function</returns>
        APLVariable GreaterThan();
        /// <summary>
        /// The built in greater than or equal to function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the greater than or equal to function</returns>
        APLVariable GreaterEqThan(APLVariable arg);
        /// <summary>
        /// The built in greater than or equal to function
        /// </summary>
        /// <returns>result of the greater than or equal to function</returns>
        APLVariable GreaterEqThan();
        /// <summary>
        /// The built in less than function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the less than function</returns>
        APLVariable LessThan(APLVariable arg);
        /// <summary>
        /// The built in less than function
        /// </summary>
        /// <returns>result of the less than function</returns>
        APLVariable LessThan();
        /// <summary>
        /// The built in less than or equal to function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the less than or equal to function</returns>
        APLVariable LessEqThan(APLVariable arg);
        /// <summary>
        /// The built in less than or equal to function
        /// </summary>
        /// <returns>result of the less than or equal to function</returns>
        APLVariable LessEqThan();

        //  Power Operators
        /// <summary>
        /// The built in power (^) function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the power (^) function</returns>
        APLVariable Power(APLVariable arg);
        //  Power Operators
        /// <summary>
        /// The built in power (^) function
        /// </summary>
        /// <returns>result of the power (^) function</returns>
        APLVariable Power();
        /// <summary>
        /// The built in log function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the log function</returns>
        APLVariable Log(APLVariable arg);
        /// <summary>
        /// The built in log function
        /// </summary>
        /// <returns>result of the log function</returns>
        APLVariable Log();
        //  Logarithm and Roots
        /// <summary>
        /// The built in nth root function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the nth root function</returns>
        APLVariable NRoot(APLVariable arg);
        /// <summary>
        /// The built in square root function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the square root function</returns>
        APLVariable Sqrt();

        /// <summary>
        /// The built in reshape function (p)
        /// </summary>
        /// <returns>result of the reshape function</returns>        
        APLVariable Size();
        /// <summary>
        /// The built in reshape function (p)
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the reshape function</returns>
        APLVariable Resize(APLVariable colCount);
        /// <summary>
        /// The built in factorial (!) function
        /// </summary>
        /// <returns>result of the factorial function</returns>
        APLVariable Factorial();
        /// <summary>
        /// The built in factorial (!) function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the factorial function</returns>
        APLVariable Factorial(APLVariable arg);
        /// <summary>
        /// The built in invert (~) function
        /// </summary>
        /// <returns>result of the invert function</returns>
        APLVariable Invert();
        /// <summary>
        /// The built in invert (~) function
        /// </summary>
        /// <param name="arg">diadic argument</param>
        /// <returns>result of the invert function</returns>
        APLVariable Invert(APLVariable arg);

    }

    
}
