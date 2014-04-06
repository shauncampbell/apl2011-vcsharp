using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL2011
{
    interface IAPLFunction
    {
        String Name
        {
            get;
        }

        APLVariable Evaluate();
        void setParams(List<APLVariable> paramsList);

    }
}
