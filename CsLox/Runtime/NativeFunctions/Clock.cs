using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Runtime.NativeFunctions
{
    class Clock : ILoxCallable
    {
        private static readonly DateTime _unix_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public int Arity => 0;

        /// <summary>
        /// Return the number of seconds since UNIX epoch (1/1/1970)
        /// </summary>
        /// <param name="interpreter">The interpreter</param>
        /// <param name="arguments">The arguments</param>
        /// <returns>The UNIX time</returns>
        public object Call(Interpreter interpreter, IList<object> arguments)
        {
            return (double)((DateTime.UtcNow - _unix_epoch).TotalSeconds);
        }
    }
}
