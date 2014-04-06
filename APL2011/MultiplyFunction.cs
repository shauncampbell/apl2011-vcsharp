using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL2011;

namespace APL2011.BuiltinFunctions
{
    class MultiplyFunction : APLFunction, IAPLFunction
    {
        public MultiplyFunction()
            : base()
        {
        }
        public String Name
        {
            get { return "x"; }
        }
        public new APLVariable Evaluate()
        {
            try
            {
                APLVariable curVariable = null;
                foreach (APLVariable v in _params)
                {
                    if (curVariable == null)
                    {
                        curVariable = v;
                    }
                    else
                    {
                        curVariable = multiplyMatrices(v, curVariable);
                    }
                }
                return curVariable;
            }
            catch (IncompatibleMatrixSizeException i)
            {
                throw new Exception("Multiply Function: Cannot add incompatible matrix sizes");
            }
        }

        private APLVariable multiplyMatrices(APLVariable a, APLVariable b)
        {
            if (a.Columns != b.Rows)
                throw new IncompatibleMatrixSizeException();

            APLVariable v = new APLVariable(b.Columns, a.Rows);

            //  Loop through each element of the new matrix
            for (int i = 1; i <= v.Rows; i++)
            {
                for (int j = 1; j <= v.Columns; j++)
                {
                    v.setValue(j, i, getMatrixCellValue(i,j,a,b));
                }
            }
            return v;
        }

        private double getMatrixCellValue(int row, int col, APLVariable a, APLVariable b)
        {
            double val = 0;
            for (int x = 1; x <= a.Columns; x++)
            {
                val += a.getValue(x, row) * b.getValue(col, x);
            }
            return val;
        }
    }
}
