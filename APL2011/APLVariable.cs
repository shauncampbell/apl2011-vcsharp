using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL2011
{
    class APLVariable
    {
        private List<List<double>> _vals = new List<List<double>>();
        private int _cols = 0, _rows = 0;
        public APLVariable(int columns, int rows)
        {
            _cols = columns;
            _rows = rows;
            for (int i = 0; i < rows; i++)
            {
                List<double> l = new List<double>(columns);
                for (int j = 0; j < columns; j++)
                {
                    l.Add(0.0);
                }
                _vals.Add(l);
            }
        }
        /*
         * Generate an APLVariable from a double
         */
        public static APLVariable FromDouble(double val)
        {
            APLVariable a = new APLVariable(1, 1);
            a.setValue(1, 1, val);
            return a;
        }
        /*
         * Generate an APLVariable from a List of Doubles
         */
        public static APLVariable FromList(List<double> vals)
        {
            APLVariable a = new APLVariable(vals.Count, 1);
            for(int x = 0; x < vals.Count; x++)
            {
                a.setValue(1+x, 1, vals[x]);
            }
            return a;
        }

        public int Rows
        {
            get { return _rows; }
        }
        public int Columns
        {
            get { return _cols; }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<double> l in _vals)
            {
                StringBuilder sbmin = new StringBuilder();
                foreach (double i in l)
                {
                    sbmin.Append(i + " ");
                }
                sb.AppendLine(sbmin.ToString());
            }
            return sb.ToString();
        }

        public double getValue(int x, int y)
        {
            return _vals[y-1][x-1];
        }
        public void setValue(int x, int y, double value)
        {
            _vals[y-1][x-1] = value;
        }
            
    }
}
