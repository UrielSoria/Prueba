using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generador
{
    public class Sintaxis : Lexico
    {
        public Sintaxis(string name = "gram.txt") : base(name)
        {
            nextToken();
        }
        public void match(string contenido){
            if (contenido == Contenido){
                nextToken();
            }
            else{
                 throw new Error("Sintaxis: se espera un: " + contenido, log, linea);
            }
        }
        public void match(Tipos tipo){
            if (tipo == Clasificacion){
                nextToken();
            }
            else{
                throw new Error("Sintaxis: se espera un tipo: " + tipo, log, linea);
            }
        }
    }
}