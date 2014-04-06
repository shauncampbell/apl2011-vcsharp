using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL2011;
namespace APL2011.BuiltinFunctions
{
    class AddFunction: APLFunction, IAPLFunction
    {
        public String Name
        {
            get { return "+"; }
        }
        public AddFunction()
            : base()
        {
        }

        public APLVariable Evaluate()
        {

            try
            {
                APLVariable curVariable = null;
                _params.Reverse();
                foreach (APLVariable v in _params)
                {
                    Console.WriteLine("Debug: " + v.ToString());
                    if (curVariable == null)
                    {
                        curVariable = v;
                    }
                    else
                    {
                        curVariable = addMatrices(v, curVariable);
                    }
                }
                return curVariable;
            }
            catch (IncompatibleMatrixSizeException i)
            {
                throw new Exception("Add Function: Cannot add two matrices of differing size");
            }
        }

        private APLVariable addMatrices(APLVariable a, APLVariable b)
        {
            if (a.Rows != b.Rows && a.Columns != b.Columns)
                throw new IncompatibleMatrixSizeException();
            
            APLVariable v = new APLVariable(a.Columns, a.Rows);
            for (int i = 1; i <= a.Columns; i++)
            {
                for (int j = 1; j <= a.Rows; j++)
                {
                    v.setValue(i, j, a.getValue(i, j) + b.getValue(i, j));
                }
            }
            return v;
        }

    }
}
