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
                   //new Lexico("C:/Users/uriso/C#/Lexico_01/prueba.cpp");
                    
                    while(!T.finArchivo())
                    {
                        T.nextToken();
                    }
                    T.contarLineas();
                   
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " +e.Message);
            }
            
            
        }
    }
}