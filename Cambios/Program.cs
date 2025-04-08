using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;



namespace Emulador
{
    class Program
    {
        static void Main(string[] args) 
        {
            try
            {
                
                using(Lenguaje  L = new("prueba.cpp"))
                {
                    /*while(!T.finArchivo())
                    {
                        T.nextToken();
                    }
                    */
                    L.Programa();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " +e.Message);
            }    
        }
    }
}