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
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " +e.Message);
            }
            
            
        }
    }
}