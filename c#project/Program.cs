using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba
{
    class Program
    {
        static void Main(string[] args) 
        {
            using (Lecturas L = new Lecturas("prueba.cpp"))
            {
                //L.Encrypt2('o');
                //Console.WriteLine(L.primerCaracter());
                while(!L.finArchivo()){
                    L.palabra();

                }
            }
        }
    }
}