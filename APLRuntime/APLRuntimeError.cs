using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL.Errors
{
    /// <summary>
    /// Class to represent errors that occur whilst interpreting user code
    /// </summary>
    public class APLRuntimeError:Exception
    {
        private string _msg;
        public APLRuntimeError(string message)
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
