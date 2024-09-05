using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Lexico_01
{
    class Program
    {
        static void Main(string[] args) 
        {
            
            
            try
            {
                using(Lexico T = new Lexico())
                {
                   
                    while(!T.finArchivo())
                    {
                        T.nextToken();
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