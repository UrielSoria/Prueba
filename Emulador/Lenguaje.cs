/*
REQUERIMIENTOS:
    1) Indicar en el error Léxico o sintáctico, el número de línea y caracter [DONE]
    2) En el log colocar el nombre del archivo a compilar, la fecha y la hora [DONE]
    3)  Agregar el resto de asignaciones [DONE]
            Asignacion -> 
            Id = Expresion
            Id++
            Id--
            Id IncrementoTermino Expresion
            Id IncrementoFactor Expresion
            Id = Console.Read()
            Id = Console.ReadLine()
    4) Emular el Console.Write() & Console.WriteLine() [DONE] 
    5) Emular el Console.Read() & Console.ReadLine() [DONE]

NUEVOS REQUERIMIENTOS:
    1) Concatenación [DONE]
    2) Inicializar una variable desde la declaración [DONE]
    3) Evaluar las expresiones matemáticas [DONE]
    4) Levantar una excepción si en el Console.(Read | ReadLine) no ingresan números [DONE]
    5) Modificar la variable con el resto de operadores (Incremento de factor y termino) [DONE]
    6) Implementar el else [DONE]
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            log.WriteLine("Constructor lenguaje");
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            log.WriteLine("Constructor lenguaje");
        }

        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }

        private void displayLista()
        {
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.GetTipoDato()} {elemento.getValor()}");
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            displayLista();
        }
        //Librerias -> using ListaLibrerias; Librerias?

        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?

        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (getContenido())
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre() == getContenido()) != null)
            {
                throw new Error($"La variable {getContenido()} ya existe", log, linea, columna);
            }
            l.Add(new Variable(t, getContenido()));
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");
                    if (getContenido() == "Read")
                    {
                        match("Read");
                        int r = Console.Read();
                        l.Last().setValor(r); // Asignamos el último valor leído a la última variable detectada
                    }
                    else
                    {
                        match("ReadLine");
                        string? r = Console.ReadLine();
                        if (float.TryParse(r, out float valor))
                        {
                            l.Last().setValor(valor);
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número ", log, linea, columna);
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    // Como no se ingresó un número desde el Console, entonces viene de una expresión matemática
                    Expresion();
                    float resultado = s.Pop();
                    l.Last().setValor(resultado);
                }
            }
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }

        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "Console")
            {
                console(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        //Asignacion -> Identificador = Expresion; (DONE)
        /*
        Id++ (DONE)
        Id-- (DONE)
        Id IncrementoTermino Expresion (DONE)
        Id IncrementoFactor Expresion (DONE)
        Id = Console.Read() (DONE)
        Id = Console.ReadLine() (DONE)
        */
        private void Asignacion()
        {
            float r;
            Variable? v = l.Find(variable => variable.getNombre() == getContenido());
            if (v == null)
            {
                throw new Error("Sintaxis: La variable " + getContenido() + " no está definida", log, linea, columna);
            }
            //Console.Write(getContenido() + " = ");
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                r = v.getValor() + 1;
                v.setValor(r);
            }
            else if (getContenido() == "--")
            {
                match("--");
                r = v.getValor() - 1;
                v.setValor(r);
            }
            else if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    ListaIdentificadores(v.GetTipoDato()); // Ya se hace este procedimiento arriba así que simplemente obtenemos a través del método lo que necesitamos
                }
                else
                {
                    Expresion();
                    r = s.Pop();
                    v.setValor(r);
                }
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                r = v.getValor() + s.Pop();
                v.setValor(r);
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
                r = v.getValor() - s.Pop();
                v.setValor(r);
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();
                r = v.getValor() * s.Pop();
                v.setValor(r);
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();
                r = v.getValor() / s.Pop();
                v.setValor(r);
            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();
                r = v.getValor() % s.Pop();
                v.setValor(r);
            }
            //displayStack();
        }
        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta = Condicion() && ejecuta2;
            //Console.WriteLine(ejecuta);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            if (getContenido() == "else")
            {
                match("else");
                bool ejecutarElse = !ejecuta; // Solo se ejecuta el else si el if no se ejecutó
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecutarElse);
                }
                else
                {
                    Instruccion(ejecutarElse);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            float valor1 = s.Pop();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float valor2 = s.Pop();
            switch (operador)
            {
                case ">": return valor1 > valor2;
                case ">=": return valor1 >= valor2;
                case "<": return valor1 < valor2;
                case "<=": return valor1 <= valor2;
                case "==": return valor1 == valor2;
                default: return valor1 != valor2;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool ejecuta)
        {
            bool isWriteLine = false;
            match("Console");
            match(".");
            if (getContenido() == "WriteLine")
            {
                match("WriteLine");
                isWriteLine = true;
            }
            else
            {
                match("Write");
            }
            match("(");
            string concatenaciones = "";
            if (getClasificacion() == Tipos.Cadena)
            {
                concatenaciones = getContenido().Trim('"');
                match(Tipos.Cadena);
            }
            if (getContenido() == "+")
            {
                match("+");
                concatenaciones += Concatenaciones();  // Se acumula el resultado de las concatenaciones
            }
            match(")");
            match(";");
            if (ejecuta)
            {
                if (isWriteLine)
                {
                    Console.WriteLine(concatenaciones);
                }
                else
                {
                    Console.Write(concatenaciones);
                }
            }
        }
        // Concatenaciones -> Identificador|Cadena ( + concatenaciones )?
        private string Concatenaciones()
        {
            string resultado = "";
            if (getClasificacion() == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == getContenido());
                if (v != null)
                {
                    resultado = v.getValor().ToString(); // Obtener el valor de la variable y convertirla
                }
                else
                {
                    throw new Error("La variable " + getContenido() + " no está definida", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            else if (getClasificacion() == Tipos.Cadena)
            {
                resultado = getContenido().Trim('"');
                match(Tipos.Cadena);
            }
            if (getContenido() == "+")
            {
                match("+");
                resultado += Concatenaciones();  // Acumula el siguiente fragmento de concatenación
            }
            return resultado;
        }
        //Main -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "*": s.Push(n2 * n1); break;
                    case "/": s.Push(n2 / n1); break;
                    case "%": s.Push(n2 % n1); break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                s.Push(float.Parse(getContenido()));
                //Console.Write(getContenido() + " ");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == getContenido());
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + getContenido() + " no está definida", log, linea, columna);
                }
                s.Push(v.getValor());
                //Console.Write(getContenido() + " ");
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}