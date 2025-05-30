using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Generador
{
    public class Error : Exception
    {
        public Error(string message) : base("Error " + message) { }
        public Error(string message, StreamWriter log) : base(message)
        {
            log.WriteLine("Error: " + message);
        }        
        public Error(string message, StreamWriter log, int linea ) : base(message + " en la linea: " + linea)
        {
            log.WriteLine("Error: " + message + " en la linea: " + linea);
        }
        public Error(string message, StreamWriter log, int linea, int col) : base(message + " en la linea: " + linea + " columna: " + col)
        {
            log.WriteLine("Error: " + message + " en la linea: " + linea + " en la col: " + col);
        }
    }
}
    