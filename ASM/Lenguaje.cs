/*

    unidad 2
        L 1. declarar las variables en ensamblador con su tipo de dato 
        L 2. En asignación generar codigo en ensamblador para ++(inc) y --(dec) 
        L 3, En asignación generar codigo en ensamblador para +=, -=, %=
    4. generar codigo en ensamblador para console.Write y console.WriteLine
    //hoy modificación de concatenaciones
    5. generar codigo en ensamblador para Read y ReadLine
        done. programar el Do
        L 6. programar el while
        L programar el for
        L recordatorio: condicionar todos los set valor en asignacion Y LISTA(if(execute))
    8. programar el else
        L 9. Usar set y get en variable
    
        L 10. Ajustar todos los constructores con parametros por default
    
   
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace ASM
{
    public class Lenguaje : Sintaxis
    {
        private int ifCounter, whileCounter, doWhileCounter, forCounter;
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
       
        public Lenguaje(string nombre = "prueba.cpp") : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
            ifCounter = whileCounter = doWhileCounter = forCounter = 1;
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
            asm.WriteLine("         section .data");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre} {elemento.getTipoDato} {elemento.getValor}");
                asm.WriteLine($"{elemento.getNombre} dd 0");
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa(bool ejecuta)
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables(ejecuta);
            }
            Main();
            asm.WriteLine("\tRET");
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

        private void Variables(bool ejecuta)
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t, ejecuta);
            match(";");
            // asm.WriteLine($"\tmov dword[{v!.getNombre}],eax");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables(ejecuta);
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
        private void ListaIdentificadores(Variable.TipoDato t, bool ejecuta)
        {


            if (l.Find(variable => variable.getNombre == Contenido) != null)
            {
                throw new Error($"La variable {Contenido} ya existe", log, linea, columna);
            }

            Variable v = new Variable(t, Contenido);
            l.Add(v);

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
                        // sobrecarga
                        int r = Console.Read();
                        // Asignamos el último valor leído a la última variable detectada
                    }
                    else
                    {
                        match("ReadLine");
                        string? r = Console.ReadLine();
                        if (float.TryParse(r, out float valor))
                        {
                            // sobrecarga
                            // v.setValor(valor,maximoTipo);
                            if (ejecuta)
                            {
                                v.setValor(valor, maximoTipo);
                            }
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
                    asm.WriteLine($";Asignacion de {v.getNombre}");
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine($"\tmov dword[{v.getNombre}], eax");
                    // sobrecarga
                    if (ejecuta)
                    {
                        v?.setValor(resultado, maximoTipo);

                    }

                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t, ejecuta);
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
            Variable? v = l.Find(variable => variable.getNombre == Contenido);
            maximoTipo = Variable.TipoDato.Char;
            if (v == null)
            {
                throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
            }
            //Console.Write(Contenido + " = ");
            match(Tipos.Identificador);
            if (Contenido == "++")
            {
                match("++");
                r = v.getValor + 1;
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine($"; Incremento de {v.getNombre} ");
                    asm.WriteLine($"\tinc dword[{v.getNombre}]");
                }
            }
            else if (Contenido == "--")
            {
                match("--");
                r = v.getValor - 1;
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine($"; Decremento de {v.getNombre} ");
                    asm.WriteLine($"\tdec dword [{v.getNombre}]");
                }
            }
            else if (Contenido == "=")
            {
                match("=");

                if (Contenido == "Console")
                {
                    ListaIdentificadores(v.getTipoDato, ejecuta); // Ya se hace este procedimiento arriba así que simplemente obtenemos a través del método lo que necesitamos
                }
                else
                {
                    asm.WriteLine($"; Asignacion de {v.getNombre} ");
                    Expresion();
                    Console.WriteLine("Despues: " + maximoTipo);
                    r = s.Pop();
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine($"\tmov dword[{v.getNombre}],eax");
                    // Requerimiento 4

                    Console.WriteLine("Despues: " + maximoTipo);
                    if (ejecuta)
                    {
                        v?.setValor(r, maximoTipo);
                    }

                }
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                r = v.getValor + s.Pop();
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine(";+=");
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine($"\tadd dword[{v.getNombre}], eax");
                }
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                r = v.getValor - s.Pop();
                // asm.WriteLine("   pop eax");
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine(";-=");
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine($"\tsub dword[{v.getNombre}], eax");
                }
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                r = v.getValor * s.Pop();
                // asm.WriteLine("     POP EAX");
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine(";*=");
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine($"\tmov ebx, [{v.getNombre}]");
                    asm.WriteLine($"\tmul ebx");
                    asm.WriteLine($"\tpush eax");
                    asm.WriteLine($"\tmov dword[{v.getNombre}], eax");
                } //
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                r = v.getValor / s.Pop();
                // asm.WriteLine("\tpop eax");
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine(";/=");
                    asm.WriteLine("\tpop ebx");
                    asm.WriteLine($"\tmov eax, [{v.getNombre}]");
                    asm.WriteLine($"\tdiv bx");
                    asm.WriteLine($"\tmov [{v.getNombre}], eax");
                };
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                r = v.getValor % s.Pop();
                // asm.WriteLine("     POP EAX");
                if (ejecuta)
                {
                    v.setValor(r, maximoTipo);
                    asm.WriteLine(";%=");
                    asm.WriteLine("\tpop ebx");
                    asm.WriteLine($"\tmov eax, [{v.getNombre}]");
                    asm.WriteLine($"\tdiv bx");
                    asm.WriteLine($"\tmov [{v.getNombre}], edx");
                };
            }
            //displayStack();
        }
        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            asm.WriteLine(";    If");
            string label = $"brinco_if_{ifCounter++}";
            bool ejecuta = Condicion(label,false) && ejecuta2;
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
            asm.WriteLine(";Etiqueta del If");
            asm.WriteLine($"{label}:");
            if (Contenido == "else")
            {
                match("else");
                bool ejecutarElse = !ejecuta; // Solo se ejecuta el else si el if no se ejecutó
                //Por la manera en que está hecho, no es necesario
                //hacer un salto en el else, porque se hace por si mismo
                asm.WriteLine("; Else");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutarElse);
                }
                else
                {
                    Instruccion(ejecutarElse);
                }
                
                // asm.WriteLine($"{label}:");
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(string label, bool isDoo = false)
        {
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor1 = s.Pop();

            string operador = Contenido;
            match(Tipos.OperadorRelacional);
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor2 = s.Pop();
            asm.WriteLine("\tpop ebx");
            asm.WriteLine("\tpop eax");
            asm.WriteLine("    cmp eax, ebx");

            if (!isDoo)
            {
                switch (operador)
                {
                    case ">":
                        asm.WriteLine($"\tjna {label}");
                        return valor1 > valor2;
                    case ">=":
                        asm.WriteLine($"\tjb {label}");
                        return valor1 >= valor2;
                    case "<":
                        asm.WriteLine($"\tjae {label}");
                        return valor1 < valor2;
                    case "<=":
                        asm.WriteLine($"\tja {label}");

                        return valor1 <= valor2;
                    case "==":
                        asm.WriteLine($"\tjne {label}");
                        return valor1 == valor2;
                    default:
                        asm.WriteLine($"\tje {label}");
                        return valor1 != valor2;
                }


            }
            else
            {

                switch (operador)
                {
                    case ">":
                        asm.WriteLine($"\tJA {label}");
                        return valor1 > valor2;
                    case ">=":
                        asm.WriteLine($"\tJAE {label}");
                        return valor1 >= valor2;
                    case "<":
                        asm.WriteLine($"\tJB {label}");
                        return valor1 < valor2;
                    case "<=":
                        asm.WriteLine($"\tJBE {label}");

                        return valor1 <= valor2;
                    case "==":
                        asm.WriteLine($"\tJE {label}");
                        return valor1 == valor2;
                    default:
                        asm.WriteLine($"\tJNE {label}");
                        return valor1 != valor2;
                }
            }

        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecuta)
        {
            asm.WriteLine(";While");
            string label = $"brinco_While_{whileCounter}";
            string JmpWhile = $"While{whileCounter}";
            match("while");
            asm.WriteLine($"{JmpWhile}:");
            match("(");
            Condicion(label,false);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(true);
            }

            asm.WriteLine($"\tjmp {JmpWhile}");
            asm.WriteLine($"{label}:");    
            whileCounter++;        
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do(bool ejecuta)
        {
            match("do");
            asm.WriteLine("\t; Do");
            string label = $"jmp_do_{doWhileCounter++}";
            asm.WriteLine("\t; Do");
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
            Condicion(label, true);
            match(")");
            match(";");
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For(bool ejecuta)
        {
            asm.WriteLine(";for");
            string label = $"brinco_for_{forCounter}";
            string JmpFor = $"For{forCounter}";
            match("for");
            match("(");
            Asignacion(ejecuta);
            match(";");
            asm.WriteLine($"{JmpFor}:");
            Condicion(label, false);
            match(";");
            Asignacion(ejecuta);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            asm.WriteLine($"\tjmp {JmpFor}");
            asm.WriteLine($"{label}:");
            forCounter++;
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
                    concatenaciones = v.getValor.ToString();
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
                // hay que modificar concatenaciones

                    Console.WriteLine(concatenaciones);
                    
                    asm.WriteLine("\tPRINT_DEC ");
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
                    resultado = v.getValor.ToString(); // Obtener el valor de la variable y convertirla
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
                asm.WriteLine("\tpop ebx");
                float n2 = s.Pop();
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "+":
                        s.Push(n2 + n1);
                        asm.WriteLine(";suma");
                        asm.WriteLine("    add eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "-":
                        s.Push(n2 - n1);
                        asm.WriteLine(";resta");
                        asm.WriteLine("    sub eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
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
                asm.WriteLine("\tpop ebx");
                float n2 = s.Pop();
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "*":
                        s.Push(n2 * n1);
                        asm.WriteLine(";multiplicacion");
                        asm.WriteLine("\tmul ebx");
                        asm.WriteLine("\tpush eax");
                        // asm.WriteLine("\tpop eax");
                        break;
                    case "/":
                        s.Push(n2 / n1);
                        asm.WriteLine(";division");
                        asm.WriteLine("\tdiv bx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "%":
                        s.Push(n2 % n1);
                        asm.WriteLine(";modulo");
                        asm.WriteLine("\tdiv bx");
                        asm.WriteLine("\tpush edx");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {

            if (Clasificacion == Tipos.Numero)
            {

                Variable.valorToTipoDato(float.Parse(Contenido));
                if (maximoTipo < Variable.valorToTipoDato(float.Parse(Contenido)))
                {
                    maximoTipo = Variable.valorToTipoDato(float.Parse(Contenido));
                }
                s.Push(float.Parse(Contenido));
                asm.WriteLine("\tmov eax," + Contenido);
                asm.WriteLine("\tpush eax ");

                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {

                Variable? v = l.Find(variable => variable.getNombre == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida", log, linea, columna);
                }
                if (maximoTipo < v.getTipoDato)
                {
                    maximoTipo = v.getTipoDato;
                }
                s.Push(v.getValor);
                asm.WriteLine($"\tmov eax, dword[{v.getNombre}]");
                asm.WriteLine("\tpush eax");
                match(Tipos.Identificador);
            }
            else
            {

                match("(");
                Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
                bool huboCasteo = false;

                if (Clasificacion == Tipos.TipoDato)
                {

                    switch (Contenido)
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
        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}