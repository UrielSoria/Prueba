using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;



namespace Generador
{
    class Program
    {
        static void Main(string[] args) 
        {
            try
            {

                using (Lenguaje L = new("gram.txt"))
                {
                    // while(L)
                    L.AvanzaEnElArchivo();
                    foreach (var metodo in L.Metodos)
                    // {
                    //     Console.WriteLine($"Metodo: {metodo.Key} - Valor: {metodo.Value}");
                    // }
                    L.Genera();
                    
                    // while(!L.finArchivo())
                    // {
                    //     L.nextToken();
                    // }

                    // L.Programa();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " +e.Message);
            }    
        }
    }
}