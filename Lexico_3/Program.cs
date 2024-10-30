using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpreadsheetLight;


namespace Lexico_3
{
    class Program
    {
        static void Main(string[] args) 
        {
            try
            {
                bool matOrExcel = false;
                using(Lexico T = new Lexico())
                {
                    while(!T.finArchivo())
                    {
                        T.nextToken(matOrExcel);
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