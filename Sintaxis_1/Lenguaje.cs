/*
REQUERIMIENTOS:
    math function
    while
    for
    excepcion read()
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

        private Variable.TipoDato maximoTipo;
        public Lenguaje(string nombre = "prueba.cpp") : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            // log.WriteLine("Constructor lenguaje");
            maximoTipo = Variable.TipoDato.Char;
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
                Variables(true);
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

        private void Variables(bool ejecuta)
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (getContenido())
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(ejecuta, t);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables(ejecuta);
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
        private void ListaIdentificadores(bool ejecuta, Variable.TipoDato t)
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
                        if ((r >= 'A' && r <= 'Z') 
                        || (r >= 'a' && r <= 'z') 
                        || (r >= '0' && r <= '9'))
                        {
                            l.Last().setValor(r); // Asignamos el último valor leído a la última variable detectada
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número o letra", log, linea, columna);
                        }

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
                    // match(";");
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
                ListaIdentificadores(ejecuta, t);
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
                While(ejecuta);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables(ejecuta);
            }
            else
            {
                Asignacion(ejecuta);
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
        private void Asignacion(bool ejecuta)
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
                    ListaIdentificadores(ejecuta, v.GetTipoDato()); // Ya se hace este procedimiento arriba así que simplemente obtenemos a través del método lo que necesitamos
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
                bool ejecutarElse = !ejecuta && ejecuta2; // Solo se ejecuta el else si el if no se ejecutó
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
        private void While(bool ejecuta)
        {
            
            // Console.WriteLine(getContenido());
            // Console.WriteLine("charTmp: " + charTmp);
            int lineTmp = linea;
            // Console.WriteLine("aWhile");
            match("while");
            int charTmp = characterCounter;
            // Console.WriteLine("dWhile");
            match("(");
            // Condicion();
            bool executeWhile = Condicion() && ejecuta;
            match(")");

            while (executeWhile)
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                // if (executeWhile)
                // {
                archivo.DiscardBufferedData();
                // Console.WriteLine(getContenido());
                archivo.BaseStream.Seek(charTmp, SeekOrigin.Begin);
                characterCounter = charTmp;
                linea = lineTmp;

                // Console.WriteLine(charTmp);
                // Console.WriteLine(lineTmp);
                // Console.WriteLine(characterCounter);
                // Console.WriteLine(getContenido());
                nextToken();
                Console.WriteLine(getContenido());
                executeWhile = Condicion() && ejecuta;
                match(")");
                // match("while");
                // }
            }
            // while (getContenido() != "{"){
            //     nextToken();
            // }
            if (getContenido() == "{")
            {
                BloqueInstrucciones(false);
            }
            else
            {
                Instruccion(false);
            }
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
       private void Do(bool ejecuta)
        {
            int charTmp = characterCounter - 3;
            int lineTmp = linea; //el condador de lineas
            bool executeDo;
            //es public en las lineas (QUITAR)
            do
            {
                match("do");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                match("while");
                match("(");
                // Condicion();
                executeDo = ejecuta && Condicion();
                match(")");
                match(";");
                if (executeDo)
                {
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(charTmp, SeekOrigin.Begin);
                    characterCounter = charTmp;
                    linea = lineTmp;
                    nextToken();
                    //     // Console.WriteLine(Contenido);
                }
            } while (executeDo);
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For(bool ejecuta)
        {
            // bool Asig = false;
            bool ejecutaFor;
            match("for");
            match("(");
            Asignacion(ejecuta);
            int charTmpCond = characterCounter;
            int lineaTmp = linea;
            match(";");

            ejecutaFor = Condicion() && ejecuta;
            int charTmpAsig = characterCounter;
            match(";");

            while (getContenido() != "{")
            {
                nextToken();
            }
            while (ejecutaFor)
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(charTmpAsig, SeekOrigin.Begin);
                characterCounter = charTmpAsig;
                linea = lineaTmp;
                nextToken();
                Asignacion(true);


                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(charTmpCond, SeekOrigin.Begin);
                characterCounter = charTmpCond;
                linea = lineaTmp;
                nextToken();
                ejecutaFor = Condicion() && ejecuta;

                while (getContenido() != "{")
                {
                    nextToken();
                }

            }
            if (getContenido() == "{")
            {
                BloqueInstrucciones(false);
            }
            else
            {
                Instruccion(false);
            }

        }
        //Consolee -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool ejecuta)
        {
            float valor;
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
            else if(getClasificacion() == Tipos.Identificador)
            {
                Variable? v = l.Find(var => var.getNombre() == getContenido());
                if (v == null)
                {
                    throw new Error("Sintaxis: La variable " + getContenido() + " no está definida", log, linea, columna);
                }
                else
                {
                    valor = v.getValor();
                    concatenaciones = valor.ToString();
                    match(Tipos.Identificador);
                    // asm.WriteLine($"\tPRINT_DEC 4,{v.getNombre}");
                }
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
            // string result = "";
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
            else if(getClasificacion() == Tipos.funcionMatematica){
                //modificar Contenido;
                string functionName = getContenido();
                match(Tipos.funcionMatematica);
                match("(");
                Expresion();
                match(")");
                float resultado = s.Pop();
                float mathResult = mathFunction(resultado, functionName);
                s.Push(mathResult);
            }
            
            else
            {

                match("(");
                Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
                bool huboCasteo = false;

                if (getClasificacion() == Tipos.TipoDato)
                {

                    switch (getContenido())
                    {

                        case "int": tipoCasteo = Variable.TipoDato.Int; break;
                        case "float": tipoCasteo = Variable.TipoDato.Float; break;

                    }

                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    huboCasteo = true;

                }
                Expresion();


                if (huboCasteo)
                {
                    maximoTipo = tipoCasteo;
                    // Console.WriteLine(Contenido);
                    if (maximoTipo == Variable.TipoDato.Int)
                    {
                        s.Push(s.Pop() % 256);
                        asm.WriteLine("\tpop eax");
                        asm.WriteLine("\tpush eax");
                    }
                    else if (maximoTipo == Variable.TipoDato.Float)
                    {
                        s.Push(s.Pop() % 35536);
                        asm.WriteLine("\tpop eax");
                        asm.WriteLine("\tpush eax");
                    }

                }
                match(")");
            }

        }
        private float mathFunction(float Valor, string nombre){
            switch(nombre){
                case "abs": return Math.Abs(Valor);
                case "pow": return (float)Math.Pow(Valor,2);
                case "sqrt": return (float) Math.Sqrt(Valor);//parseo
                case "floor": return (float)  Math.Floor(Valor);
                case "ceil": return (float) Math.Ceiling(Valor);
                case "exp": return  (float) Math.Exp(Valor);
                case "max": 
                if(0 <= Valor && Valor <=255){
                    return 255;
                }
                else if(266 <= Valor && Valor <= 65536){
                    return 65536;
                }
                break;
                case "log10": return (float) Math.Log10(Valor);
                case "log2": return (float) Math.Log2(Valor);
                case "rand": 
                Random rnd = new Random();
                return rnd.Next(0, (int)Valor);
                case "trunc": return (float) Math.Truncate(Valor);
                case "round": return (float) Math.Round(Valor);
            }
            return Valor;
        }
        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}