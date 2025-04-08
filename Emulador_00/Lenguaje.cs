using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    Requerimeintos:
    1  Excepcion e el Read
    2 segunda asignacion del for debe ejecuatarse despues de 
    bloque instrucciones o instruccion
    3. Programar MathFunction
    4. Programar el for
    5. Programar el while

*/
namespace Emulador
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        
        public Lenguaje(string name = "prueba.cpp") : base(name)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
        }

        // private void displayStack()
        // {
        //     Console.WriteLine(" Contenido del Stack");
        //     foreach (float elemento in s)
        //     {
        //         Console.WriteLine(elemento);
        //     }
        // }
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
                    v?.setValor(s.Pop(), maximoTipo);
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
                                throw new Error("Semantico: No se ingresó un numero");
                            }
                            v.setValor(contNumero, maximoTipo);
                            break;
                    }
                }
                else
                {
                    // Console.WriteLine("antes:" + maximoTipo);
                    Expresion();
                    // Console.WriteLine("despues:" + maximoTipo); 
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
                    v.setValor(nuevoValor, maximoTipo);
                }
                else if (Contenido == "*=")
                {
                    match("*=");
                    Expresion();
                    float nuevoValor = v.getValor();
                    nuevoValor = nuevoValor * s.Pop();
                    v.setValor(nuevoValor, maximoTipo);
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
            bool executeWhile = execute && Condicion();
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
        // Do -> do 
        // bloqueInstrucciones | intruccion 
        // while(Condicion); 
        private void Do(bool execute)
        {
            int charTmp = characterCounter - 3;
            int lineTmp = linea;
            do{
                match("do");

                if (Contenido == "{")
                {
                    BloqueInstrucciones(execute);
                }
                else
                {
                    Instruccion(execute);
                }
                match("while");
                match("(");
                // Condicion();
                bool executeDo = execute && Condicion();
                match(")");
                match(";");
                if (executeDo)
                {
                    //seek
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(charTmp, SeekOrigin.Begin);
                    characterCounter = charTmp;
                    linea = lineTmp;
                    nextToken();
                    // Console.
                }
            }while(execute && Condicion());

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
            bool executeFor = execute && Condicion();
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
            else if(Clasificacion == Tipos.FuncionMetematica){
                String functionName = Contenido;
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
        private float mathFunction(float valor, string nombre){
            switch(nombre){
                case "abs": return MathF.Abs(valor); 
                case "pow": return MathF.Pow(valor,2);
                case "ceil": return MathF.Ceiling(valor); 
                case "sqrt": return MathF.Sqrt(valor);
                case "floor": return MathF.Floor(valor);
                case "exp": return MathF.Exp(valor);
                //case "equal":
                case "max": 
                
                if(0<= valor && valor <=255){
                    return 256;
                }
                else if(valor <= 65536){
                    return 65536;
                }
                break;
                case "log10": return MathF.Log10(valor);
                case "log2": return MathF.Log2(valor);
                //generar un randomZXonaC
                //se "rand": return MathF.Rando);
                case "trunc": return (float) MathF.Truncate(valor);
                case "round": return MathF.Round(valor);
            }
            return valor;
        }
    }

}
