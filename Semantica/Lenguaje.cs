using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    Requerimeintos:
    1  Implementar set y get para la clase token
    2. Implementar parametros default en el constructor de archivo lexico
    3. Implementar linea y columna en los errores semanticos
    4. implementar maximo tipo en la asignacion, es decir, cuando se haga v.setvalor(r)
    5. Aplicar el casteo en el stack, si hubo casteo se hace pop se calcula el residuo de la division
*/
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
        }
        public Lenguaje(string name) : base(name)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
        }

        private void displayStack()
        {
            Console.WriteLine(" Contenido del Stack");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }
        private void displayList()
        {
            log.WriteLine("\nLista de varibles: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.getTipoDato()} {elemento.getValor()}");
            }
        }

        // Programa  -> Librerias? Variables? Main
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
            displayList();
        }

        // Librerias -> using ListaLibrerias; Librerias?
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

        // Variables -> tipo_dato Lista_identificadores; Variables?
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
            if (Contenido == "=")
            {
                match("=");
                Expresion();
            }
            match(";");

            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }

        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {

            //Console.WriteLine(Contenido);
            if (l.Find(Variable => Variable.getNombre() == Contenido) != null)
            {
                throw new Error("Sintaxis: la variable " + Contenido + " ya existe", log, linea, col);
            }
            l.Add(new Variable(t, Contenido));
            Variable? v = l.Find(variable => variable.getNombre() == Contenido);
            match(Tipos.Identificador);

            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
            else if (Contenido == "=")
            {
                match("=");

                if (Contenido == "Console")
                {

                    match("Console");
                    match(".");
                    switch (Contenido)
                    {
                        case "Read":
                            match("Read");
                            match("(");
                            match(")");
                            Console.Read();
                            v?.setValor(Console.Read());
                            break;
                        case "ReadLine":
                            match("ReadLine");
                            match("(");
                            match(")");
                            string? cont = Console.ReadLine();

                            if (!float.TryParse(cont, out float contNumero))
                            {
                                throw new Error("No se ingresó un numero");
                            }
                            v?.setValor(contNumero);
                            break;
                    }
                }
                else
                {
                    Expresion();
                    v?.setValor(s.Pop());
                }

            }
        }

        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool execute)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(execute);
            }
            else
            {
                match("}");
            }

        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool execute)
        {
            Instruccion(execute);
            if (Contenido != "}")
            {
                ListaInstrucciones(execute);
            }
            else
            {
                match("}");
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool execute)
        {
            if (Contenido == "Console")
            {
                console(execute);
            }
            else if (Contenido == "if")
            {
                If(execute);
            }
            else if (Contenido == "while")
            {
                While(execute);
            }
            else if (Contenido == "do")
            {
                Do(execute);
            }
            else if (Contenido == "for")
            {
                For(execute);
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
        // Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            maximoTipo = Variable.TipoDato.Char;
            // Console.WriteLine(Contenido);
            Variable? v = l.Find(variable => variable.getNombre() == Contenido);
            if (v == null)
            {
                throw new Error("La variable " + Contenido + " no está definida");
            }
            // Console.WriteLine(Contenido + " = "); 
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    switch (Contenido)
                    {
                        case "Read":
                            match("Read");
                            match("(");
                            match(")");
                            Console.Read();
                            v.setValor(Console.Read(), maximoTipo);
                            break;
                        case "ReadLine":
                            match("ReadLine");
                            match("(");
                            match(")");
                            string? cont = Console.ReadLine();

                            if (!float.TryParse(cont, out float contNumero))
                            {
                                throw new Error("No se ingresó un numero");
                            }
                            v.setValor(contNumero, maximoTipo);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("antes:" + maximoTipo);
                    Expresion();
                    Console.WriteLine("despues:" + maximoTipo); 
                    v.setValor(s.Pop(), maximoTipo);
                }

                // displayStack();	
            }
            else if (Clasificacion == Tipos.IncrementoTermino)
            {
                if (Contenido == "++")
                {
                    match("++");
                    float nuevoValor = v.getValor();
                    v.setValor(nuevoValor + 1);
                }
                else if (Contenido == "--")
                {
                    match("--");
                    float nuevoValor = v.getValor();
                    v.setValor(nuevoValor - 1);
                }
                else if (Contenido == "+=")
                {
                    match("+=");
                    Expresion();
                    // float nuevoValor = s.Pop();
                    v.setValor(v.getValor() + s.Pop());
                }
                else if (Contenido == "-=")
                {
                    match("-=");
                    Expresion();
                    float nuevoValor = v.getValor();

                    v.setValor(nuevoValor - s.Pop());
                }
            }
            else if (Clasificacion == Tipos.IncrementoFactor)
            {
                if (Contenido == "/=")
                {
                    match("/=");
                    Expresion();
                    float nuevoValor = v.getValor();
                    nuevoValor = nuevoValor / s.Pop();
                    v.setValor(nuevoValor);
                }
                else if (Contenido == "%=")
                {
                    match("%=");
                    Expresion();
                    float nuevoValor = v.getValor();
                    nuevoValor = nuevoValor % s.Pop();
                    v.setValor(nuevoValor);
                }
                else if (Contenido == "*=")
                {
                    match("*=");
                    Expresion();
                    float nuevoValor = v.getValor();
                    nuevoValor = nuevoValor * s.Pop();
                    v.setValor(nuevoValor);
                }



            }

        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        public void If(bool execute2)
        {

            match("if");
            match("(");
            bool execute = Condicion() && execute2;
            //Console.WriteLine(execute);
            match(")");

            if (Contenido == "{")
            {
                BloqueInstrucciones(execute);
            }
            else
            {
                Instruccion(execute);
            }

            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(!execute && execute2);
                }
                else
                {
                    Instruccion(!execute && execute2);
                }

            }

        }

        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor1 = s.Pop();
            string operador = Contenido;
            match(Tipos.OperadorRelacional);

            maximoTipo = Variable.TipoDato.Char;
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
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool execute)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {

                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        // Do -> do 
        // bloqueInstrucciones | intruccion 
        // while(Condicion); 
        private void Do(bool execute)
        {
            match("do");
            if (Contenido == "{")
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

        // For -> for(Asignacion; Condicion; Asignacion) 
        // BloqueInstrucciones | Intruccion
        private void For(bool execute)
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        // Console -> Console.(WriteLine|Write) (cadena concatenaciones?);
        public void console(bool execute)
        {
            match("Console");
            match(".");
            switch (Contenido)
            {
                case "WriteLine":
                    match("WriteLine");
                    match("(");

                    if (Contenido == ")")
                    {
                        match(")");
                        match(";");
                        if (execute)
                        {
                            Console.WriteLine();
                        }
                        // Console.WriteLine();
                        break;
                    }

                    string texto = "";

                    if (Clasificacion == Tipos.Identificador)
                    {
                        Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                        if (v == null)
                        {
                            throw new Error("La variable " + Contenido + " no está declarada");
                        }
                        // Console.Write(v.getValor());
                        texto += v.getValor();
                        match(Tipos.Identificador);
                    }
                    else
                    {
                        texto += Contenido.Trim('\"');
                        match(Tipos.Cadena);
                    }

                    if (Contenido == "+")
                    {
                        match("+");
                        texto += Concatenaciones();
                    }

                    if (execute)
                    {
                        Console.WriteLine(texto);
                    }

                    // Console.WriteLine(Contenido.Trim('\"')); 
                    match(")");
                    match(";");
                    break;
                case "Write":
                    match("Write");
                    match("(");
                    if (Contenido == ")")
                    {
                        match(")");
                        match(";");

                        break;
                    }
                    string texto2 = "";

                    if (Clasificacion == Tipos.Identificador)

                    {
                        Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                        if (v == null)
                        {
                            throw new Error("La variable " + Contenido + " no está declarada");
                        }
                        // Console.Write(v.getValor());
                        texto2 += v.getValor();
                        match(Tipos.Identificador);
                    }
                    else
                    {
                        texto2 += Contenido.Trim('\"');
                        match(Tipos.Cadena);
                    }

                    if (Contenido == "+")
                    {
                        match("+");
                        texto2 += Concatenaciones();
                    }

                    if(execute) {
                        Console.Write(texto2);
                    }

                    match(")");
                    match(";");
                    break;
                    /*
                    case "ReadLine":
                        match("ReadLine");
                        match("(");
                        match(")");

                        Console.ReadLine();
                        match(";");
                    break;

                    case "Read":
                        match("Read");
                        match("(");
                        match(")");

                        Console.Read();
                        match(";");
                    break;
                    */
            }



        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones 
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
        public void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {

            if (Clasificacion == Tipos.OperadorTermino)
            {
                string operador = Contenido;
                match(Tipos.OperadorTermino);
                Termino();
                // Console.Write(operador + " " );

                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador)
                {
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OperadorFactor)
            {
                string operador = Contenido;
                match(Tipos.OperadorFactor);
                Factor();
                // Console.Write(operador + " " );
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
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {

            if (Clasificacion == Tipos.Numero)
            {
                //si el tipo de dato del numero es mayor al tio actual, se cambia
                if(maximoTipo <Variable.valorToTipoDato(float.Parse(Contenido))){
                    maximoTipo = Variable.valorToTipoDato(float.Parse(Contenido));;
                }
                s.Push(float.Parse(Contenido));
                // Console.Write(Contenido + " " );
                match(Tipos.Numero);

            }
            else if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v == null)
                {
                    throw new Error("La variable " + Contenido + " no está declarada");
                }
                if(maximoTipo < v.getTipoDato()){
                    maximoTipo = v.getTipoDato();
                }
                
                s.Push(v.getValor());

                // Console.Write(Contenido + " " );
                match(Tipos.Identificador);
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
                    
                    /*
                    pop 
                    residuo de la devision dependiendo del tipo
                    push del residuo
                    *
                    if(Variable.valorToTipoDato(float.Parse(Contenido)) == Variable.TipoDato.Int){
                        s.Push(s.Pop()%256);
                    }
                    else if(Variable.valorToTipoDato(float.Parse(Contenido)) == Variable.TipoDato.Float){
                        s.Push(s.Pop()%35536);
                    }
                    */
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
        public string Concatenaciones()
        {
            string texto = "";
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v == null)
                {
                    throw new Error("La variable " + Contenido + " no está declarada");
                }
                texto += v.getValor();
                // Console.Write(v.getValor());
                match(Tipos.Identificador);
            }
            else
            {
                texto += Contenido.Trim('\"');
                match(Tipos.Cadena);
            }

            if (Contenido == "+")
            {
                match("+");
                texto += Concatenaciones();
            }

            return texto;
        }
    }

}
