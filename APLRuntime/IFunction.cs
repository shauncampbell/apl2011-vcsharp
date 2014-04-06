using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.Runtime
{
    public interface IFunction
    {
        APLVariable Execute(APLVariable arg1);
        APLVariable Execute(APLVariable arg1, APLVariable arg2);
    }
}
