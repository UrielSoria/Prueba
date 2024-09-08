using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_01
{
    public class TipeFileMismatch : Exception
    {
        public TipeFileMismatch(string message, StreamWriter log) : base(message)
        {
            log.Write("Error" + message);
        }
    }
}