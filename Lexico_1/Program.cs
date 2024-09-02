using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexico_1;

namespace prueba
{
    class Program
    {
        static void Main(string[] args) 
        {
            try
            {
                using(Lexico T = new Lexico())
                {
                    Lexico l = new Lexico();
                    while(!l.finArchivo())
                    {
                        l.nextToken();
                    }
                
                    /*T.setContenido("HOLA");
                    T.setClasificacion(Token.Tipos.Identificador);
                    Console.WriteLine(T.getContenido() + " = " + T.getClasificacion());

                    T.setContenido("123");
                    T.setClasificacion(Token.Tipos.Numero);
                    Console.WriteLine(T.getContenido() + " = " + T.getClasificacion());
                    */
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " +e.Message);
            }
        }
    }
}