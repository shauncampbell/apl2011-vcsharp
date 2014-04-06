using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.Runtime;
using APL.SyntaxTreeGeneration;

namespace APL
{
    class UnaryFunction
    {
        private STree _expression;
        private string _arg1;

        public UnaryFunction(string arg1)
        {
            _arg1 = arg1;
        }

        public string Argument1
        {
            get { return _arg1; }
            set { _arg1 = value; }
        }

        public STree Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }
    }
}
