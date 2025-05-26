using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Data.Common;
using System.IO.Compression;
using Microsoft.VisualBasic;

namespace Generador
{
    public class Lexico : Token, IDisposable
    {
        protected StreamReader archivo;
        public static StreamWriter log = null!;
        public static StreamWriter Languaje = null!;
        public StreamWriter asm;
        public static int linea = 1;
        protected int charactercounter;
        const int F = -1;
        const int E = -2;
        public static int columna = 1;

        int[,] TRAND = {
            {0,  1,  2, 10,  4, 10, 10, 10, 10, 10, 10}, // 0
            {F,  1,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 1
            {F,  F,  F,  3,  F,  F,  F,  F,  F,  F,  F}, // 2
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 3
            {F,  F,  F,  F,  F,  5,  6,  7,  8,  9,  F}, // 4
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 5
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 6
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 7
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 8
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 9
            {F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F}, // 10
        };

        public Lexico(string fileName)
        {
            string fileNameWithoutExtension;
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            log = new StreamWriter(fileNameWithoutExtension + ".log");
            log.AutoFlush = true;
            Languaje = new StreamWriter("./generado/Lenguaje.cpp");
            Languaje.AutoFlush = true;
            if (File.Exists(fileName))
            {
                archivo = new StreamReader(fileName);
                if (Path.GetExtension(fileName).ToLower() == ".txt")
                {
                    asm = new StreamWriter(fileNameWithoutExtension + ".asm");
                    asm.AutoFlush = true;
                }
                else
                {
                    throw new Error("El tipo de archivo no es correcto, se esperaba (.txt) ", log);
                }
                log.WriteLine("Archivo: " + fileName);
                DateTime fecha = DateTime.Now;
                log.Write("//Generado: " + fecha.ToString("d"));
                log.WriteLine(" Hora de Compilación: " + fecha.ToString("HH:mm:ss"));
            }
            else
            {
                throw new Error("El archivo " + fileName + " no existe", log);
            }
            // Console.WriteLine("Archivo: " + fileName); 
        }
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }

        private int Columna(char c)
        {
            //int columna;

            if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.IsLetter(c))
            {
                return 1;
            }
            else if (c == '-')
            {
                return 2;
            }
            else if (c == '>')
            {
                return 3;
            }
            else if (c == '\\')
            {
                return 4;
            }
            else if (c == ';')
            {
                return 5;
            }
            else if (c == '?')
            {
                return 6;
            }
            else if (c == '|')
            {
                return 7;
            }
            else if (c == '(')
            {
                return 8;
            }
            else if (c == ')')
            {
                return 9;
            }
            else
            {
                return 10;
            }
            
        }
        private void Clasifica(int estado)
        {
            
            switch (estado)
            {
                case 1: Clasificacion = Tipos.SNT; break;
                case 2: Clasificacion = Tipos.ST; break;
                case 3: Clasificacion = Tipos.Produce; break;
                case 4: Clasificacion = Tipos.ST; break;
                case 5: Clasificacion = Tipos.FinProduccion; break;
                case 6: Clasificacion = Tipos.Optativo; break;
                case 7: Clasificacion = Tipos.OR; break;
                case 8: Clasificacion = Tipos.InicioAgrupacion; break;
                case 9: Clasificacion = Tipos.CierreAgrupacion; break;
                case 10: Clasificacion = Tipos.ST; break;
            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            int estado = 0;
            while (estado >= 0)
            {
                c = (char)archivo.Peek();
                estado = TRAND[estado, Columna(c)];
                Clasifica(estado);
                if (estado >= 0)
                {
                    archivo.Read();
                    charactercounter++;
                    if (c == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                    else
                    {
                        columna++;
                    }
                    if (estado > 0)
                    {
                        buffer += c;
                    }
                    else
                    {
                        buffer = "";
                    }
                }
            }
            if (estado == E)
            {
                
            }
            Contenido = buffer;
            if (Clasificacion == Tipos.SNT && char.IsLower(Contenido[0]))
            {
                Clasificacion = Tipos.ST;
            }
            
            if (!finArchivo())
            {
                log.WriteLine(Contenido + " = " + Clasificacion);
            }
        }
        protected bool isClasification(string name)
        {
            switch (name)
            {
                case "Identificador":
                case "Numero":
                case "Caracter":
                case "FinSentencia":
                case "InicioBloque":
                case "FinBloque":
                case "OperadorTermino":
                case "OperadorTernario":
                case "OperadorFactor":
                case "Incremen toTermino":
                case "IncrementoFactor":
                case "Flecha":
                case "Asignacion":
                case "OperadorRelacional":
                case "OperadorLogico":
                case "Puntero":
                case "Cadena":
                case "TipoDato":
                case "PalabraReservada":
                case "FuncionMatematica":
                    return true;
            }

            return false;
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}
/*

Expresión Regular: Método Formal que a través de una secuencia de caracteres que define un PATRÓN de búsqueda

a) Reglas BNF 
b) Reglas BNF extendidas
c) Operaciones aplicadas al lenguaje

----------------------------------------------------------------

OAL

1. Concatenación simple (·)
2. Concatenación exponencial (Exponente) 
3. Cerradura de Kleene (*)
4. Cerradura positiva (+)
5. Cerradura Epsilon (?)
6. Operador OR (|)
7. Paréntesis ( y )

L = {A, B, C, D, E, ... , Z | a, b, c, d, e, ... , z}

D = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

1. L.D
    LD
    >=

2. L^3 = LLL
    L^3D^2 = LLLDD
    D^5 = DDDDD
    =^2 = ==

3. L* = Cero o más letras
    D* = Cero o más dígitos

4. L+ = Una o más letras
    D+ = Una o más dígitos

5. L? = Cero o una letra (la letra es optativa-opcional)

6. L | D = Letra o dígito
    + | - = más o menos

7. (L D) L? (Letra seguido de un dígito y al final una letra opcional)

Producción gramátical

Clasificación del Token -> Expresión regular

Identificador -> L (L | D)*

Número -> D+ (.D+)? (E(+|-)? D+)?
FinSentencia -> ;
InicioBloque -> {
FinBloque -> }
OperadorTernario -> ?

Puntero -> ->

OperadorTermino -> + | -
IncrementoTermino -> ++ | += | -- | -=

Término+ -> + (+ | =)?
Término- -> - (- | = | >)?

OperadorFactor -> * | / | %
IncrementoFactor -> *= | /= | %=

Factor -> * | / | % (=)?

OperadorLogico -> && | || | !

NotOpRel -> ! (=)?

Asignación -> =

AsgOpRel -> = (=)?

OperadorRelacional -> > (=)? | < (> | =)? | == | !=

Cadena -> "c*"
Carácter -> 'c' | #D* | Lamda

----------------------------------------------------------------

Autómata: Modelo matemático que representa una expresión regular a través de un GRAFO, 
para una maquina de estado finito, para una máquina de estado finito que consiste en 
un conjunto de estados bien definidos:

- Un estado inicial 
- Un alfabeto de entrada 
- Una función de transición 

*/