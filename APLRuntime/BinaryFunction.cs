using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.SyntaxTreeGeneration;
using APL.Runtime;

namespace APL.Runtime
{
    class BinaryFunction
    {
        private STree _expression;
        private string _arg1;
        private string _arg2;

        public BinaryFunction(string arg1, string arg2)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public string Argument1
        {
            get { return _arg1; }
            set { _arg1 = value; }
        }

        public string Argument2
        {
            get { return _arg2; }
            set { _arg2 = value; }
        }

        public STree Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }
    }
}
