using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
{
    public class Sintaxis : Lexico
    {
       
        public Sintaxis(string name) : base(name)
        {
            nextToken();
        }
        public void match(string contenido){
            if (contenido == Contenido){
                nextToken();
            }
            else{
                 throw new Error("Sintaxis: se espera un: " + contenido, log, linea, col);
            }
        }
        public void match(Tipos tipo){
            if (tipo == Clasificacion){
                nextToken();
            }
            else{
                throw new Error("Sintaxis: se espera un tipo: " + tipo, log, linea, col);
            }
        }
    }
}