using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.Errors
{
    /// <summary>
    /// Class that represents an error thrown whilst parsing user code
    /// </summary>
    public class APLParseError : Exception
    {
        private string _msg = "";

        public APLParseError(string message)
        {
            _msg = message;
        }

        public override string Message
        {
            get
            {
                return _msg;
            }
        }
    }
}
