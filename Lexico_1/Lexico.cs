using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/*

    Requerimiento 1: Sobrecargar el constructor lexico para que escriba como argumento
    el nombre del archivo
    Requerimiento 2: contador de lineas
*/

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;

        public Lexico()
        {
            linea = 1;
            //Como contar lineas c#
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if( File.Exists("prueba.cpp"))
            {
            archivo = new StreamReader("pueba.cpp");   
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
            
        }

        /*
        public Lexico(string nombre)
        {
            /*
                Si nombre = suma.cpp
                LOG = suma.log
                ASM = suma.asm
                y validar la extension del nombre del archivo(que tenga cpp a fuerzas)
                flie (como verificar el archivo de un string y cambiarlo)
            *
        }
        */
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            //castear a int porque read y peek regresan int
            //similar al metodo de las palabras
            //ejemplo 123+z
            //archivo.Read();
            //lee caracteres en el archivo
            //archivo.Peek();
            //hace lo mismo pero no avanza
            while(char.IsWhiteSpace(c=(char)archivo.Read()))
            {

            }
            if(char.IsLetter(c))
            {
                setClasificacion(Tipos.Identificador);
            }
            else if(char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
            }
            else
            {
                setClasificacion(Tipos.Caracter);
            }
            setContenido(buffer);
            log.WriteLine(getContenido() + " = " + getClasificacion());
        }
    }
}