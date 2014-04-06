using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APL.SyntaxTreeGeneration;
using APL.Errors;
namespace APL.Runtime
{
    class UserDefinedFunction : IFunction
    {
        private string _arg1;
        private string _arg2;
        private STree _definition;
        public UserDefinedFunction(STree definition, string arg1, string arg2)
        {
            _definition = definition;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public UserDefinedFunction(STree definition, string arg1)
        {
            _definition = definition;
            _arg1 = arg1;
        }

        public APLVariable Execute(APLVariable arg1)
        {
            if (_arg2 != null)
                throw new APLRuntimeError("Unsupported Function argument(s): Unary functions require one argument");
            Interpreter i = Interpreter.GetInstance().SpawnChild();
            i.SetVariable(_arg1, arg1);
            return i.InterpretLine(_definition);
        }

        public APLVariable Execute(APLVariable arg1, APLVariable arg2)
        {
            if (arg2 == null)
                throw new APLRuntimeError("Unsupported Function argument(s): Binary functions require two arguments");
            Interpreter i = Interpreter.GetInstance().SpawnChild();
            i.SetVariable(_arg1, arg1);
            i.SetVariable(_arg2, arg2);
            return i.InterpretLine(_definition);
        }
    }
}
