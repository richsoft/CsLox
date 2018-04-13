using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Exceptions
{
    class ReturnException : Exception
    {
        public object Value { get; }

        public ReturnException(object value) : base()
        {
            this.Value = value;
        }

    }
}
