using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL2011
{
    abstract class APLFunction
    {
        protected String _name, _helpmsg;
        protected bool _isExtended = false;
        protected List<APLVariable> _params;

        public APLFunction()
        {
         
        }

        public void setParams(List<APLVariable> paramsList)
        {
            _params = paramsList;
        }

        /*
         * Never used must override!
         */

        

    }
}
