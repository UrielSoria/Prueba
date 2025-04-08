/*
REQUERIMIENTOS:
    1) Indicar en el error Léxico o sintáctico, el número de línea y caracter [DONE]
    2) En el log colocar el getNombre del archivo a compilar, la fecha y la hora [DONE]
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

namespace Emulador
{
    public class Lenguaje : Sintaxis
    {
        
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        
        public Lenguaje(string getNombre = "prueba.cpp") : base(getNombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
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
                log.WriteLine($"{elemento.getNombre} {elemento.getTipoDato} {elemento.Valor}");
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
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
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?

        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre == Contenido) != null)
            {
                throw new Error($"La variable {Contenido} ya existe", log, linea, columna);
            }
            l.Add(new Variable(t, Contenido));
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        int r = Console.Read();
                        l.Last().Valor =r; // Asignamos el último Valor leído a la última variable detectada
                    }
                    else
                    {
                        match("ReadLine");
                        string? r = Console.ReadLine();
                        if (float.TryParse(r, out float Valor))
                        {
                            l.Last().Valor = Valor;
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
                    l.Last().Valor = resultado;
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (Contenido != "}")
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
            if (Contenido != "}")
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
            if (Contenido == "Console")
            {
                console(ejecuta);
            }
            else if (Contenido == "if")
            {
                If(ejecuta);
            }
            else if (Contenido == "while")
            {
                While(ejecuta);
            }
            else if (Contenido == "do")
            {
                Do(ejecuta);
            }
            else if (Contenido == "for")
            {
                For(ejecuta);
            }
            else if (Clasificacion == Tipos.TipoDato)
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
            Variable? v = l.Find(variable => variable.getNombre == Contenido);
            if (v == null)
            {
                throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
            }
            //Console.Write(Contenido + " = ");
            match(Tipos.Identificador);
            if (Contenido == "++")
            {
                match("++");
                r = v.Valor + 1;
                v.Valor = r;
            }
            else if (Contenido == "--")
            {
                match("--");
                r = v.Valor - 1;
                v.Valor = (r);
            }
            else if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    ListaIdentificadores(v.getTipoDato); // Ya se hace este procedimiento arriba así que simplemente obtenemos a través del método lo que necesitamos
                }
                else
                {
                    Expresion();
                    r = s.Pop();
                    v.Valor = (r);
                }
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                r = v.Valor + s.Pop();
                v.Valor = (r);
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                r = v.Valor - s.Pop();
                v.Valor = (r);
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                r = v.Valor * s.Pop();
                v.Valor = (r);
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                r = v.Valor / s.Pop();
                v.Valor = (r);
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                r = v.Valor % s.Pop();
                v.Valor = (r);
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
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            if (Contenido == "else")
            {
                match("else");
                bool ejecutarElse = !ejecuta && ejecuta2; // Solo se ejecuta el else si el if no se ejecutó
                if (Contenido == "{")
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
            float Valor1 = s.Pop();
            string operador = Contenido;
            match(Tipos.OperadorRelacional);
            Expresion();
            float Valor2 = s.Pop();
            switch (operador)
            {
                case ">": return Valor1 > Valor2;
                case ">=": return Valor1 >= Valor2;
                case "<": return Valor1 < Valor2;
                case "<=": return Valor1 <= Valor2;
                case "==": return Valor1 == Valor2;
                default: return Valor1 != Valor2;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            Condicion();
            bool executeWhile = ejecuta && Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(executeWhile);
            }
            else
            {
                Instruccion(executeWhile);
            }
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do(bool ejecuta)
        {
            int charTmp = characterCounter -3;
            int lineTmp = linea; //el condador de lineas
            //es public en las lineas (QUITAR)
            
            do
            {
                Console.WriteLine("ins");
                match("do");
                if (Contenido == "{")
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
                bool executeDo = ejecuta && Condicion();
                match(")");
                match(";");
                if (executeDo)
                {
                    
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(charTmp, SeekOrigin.Begin);
                    characterCounter = charTmp;
                    linea = lineTmp;
                    nextToken();
                    // Console.WriteLine(Contenido);
                }
            } while (ejecuta && Condicion());
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            bool executeFor = ejecuta && Condicion();
            match(";");
            Asignacion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(executeFor);
            }
            else
            {
                Instruccion(executeFor);
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool ejecuta)
        {
            bool isWriteLine = false;
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
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
            if (Clasificacion == Tipos.Cadena)
            {
                concatenaciones = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            else
            {
                Variable? v = l.Find(var => var.getNombre == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
                }
                else
                {
                    concatenaciones = v.Valor.ToString();
                    match(Tipos.Identificador);
                }
            }
            if (Contenido == "+")
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
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre == Contenido);
                if (v != null)
                {
                    resultado = v.Valor.ToString(); // Obtener el Valor de la variable y convertirla
                }
                else
                {
                    throw new Error("La variable " + Contenido + " no está definida", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {
                resultado = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
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
            if (Clasificacion == Tipos.OperadorTermino)
            {
                string operador = Contenido;
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
            if (Clasificacion == Tipos.OperadorFactor)
            {
                string operador = Contenido;
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
            if (Clasificacion == Tipos.Numero)
            {
                s.Push(float.Parse(Contenido));
                //Console.Write(Contenido + " ");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida", log, linea, columna);
                }
                s.Push(v.Valor);
                //Console.Write(Contenido + " ");
                match(Tipos.Identificador);
            }
            //modificar getClasifiacion
            else if(Clasificacion == Tipos.FuncionMetematica){
                //modificar Contenido;
                string functionName = Contenido;
                match(Tipos.FuncionMetematica);
                match("(");
                Expresion();
                match(")");
                float resultado = s.Pop();
                float mathResult = mathFunction(resultado, functionName);
                s.Push(mathResult);
            }
            else
            {
                
                Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
                Boolean huboCasteo = false;
                match("(");
                if(Clasificacion == Tipos.TipoDato) {
                    switch(Contenido){
                        case"int": tipoCasteo = Variable.TipoDato.Int; break;
                        case"float": tipoCasteo = Variable.TipoDato.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    huboCasteo = true;
                }
                
                Expresion();
                if(huboCasteo){
                    maximoTipo = tipoCasteo;
                    
                    if(maximoTipo== Variable.TipoDato.Int){
                        s.Push(s.Pop()%256);
                    }
                    else if(maximoTipo == Variable.TipoDato.Float){
                        s.Push(s.Pop()%35536);
                    }
                }
                match(")");
            }
        }

        private float mathFunction(float Valor, string getNombre){
            switch(getNombre){
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
                // case "rand": return random
                case "trunc": return (float) Math.Truncate(Valor);
                case "round": return (float) Math.Round(Valor);
            }
            return Valor;
        }
        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}