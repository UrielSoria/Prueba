using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
Requerimeintos:
1  Indicar en el error lexico o sintactico el numero de linea y caracter
2. En el log colocar el nombre del archivo a compiplar, la fecha y la hora
3. Agregar el resto de asignaciones
4. Emular el Console.Write() y Console.WriteLine()
5. Emular el Console.Reed() y Console.ReedLine()

Requerimientos proyecto 6:
1. Concatenaciones - creo que sí
2. Inicializar variables - maybe, se hizo
3. Evaluar una expresion matematica - se supone que sí (se hizo en clase)
4. Condicion, Asignacion - falta
5. levantar una excepcion si en el Read(), no ingresan numeros - falta
6. modificar la varibale con el resto de los operadores (incremento factor y termino - falta
7. Condicion else - falta
Ultimo requerimiento el else - falta
*/
namespace Sintaxis_1
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        public Lenguaje() : base(){
            s = new Stack<float>();
            l = new List<Variable>();
        }
        public Lenguaje(string name): base(name)
        {
            s = new Stack<float>();
            l = new List<Variable>();
        }

        private void displayStack(){
            Console.WriteLine(" Contenido del Stack");
            foreach(float elemento in s ){
                Console.WriteLine(elemento);
            }
        }
        private void displayList(){
            log.WriteLine("Lista de varibles");
            foreach(Variable elemento in l ){
                log.WriteLine($"{elemento.getNombre()} {elemento.getTipoDato()} {elemento.getValor()}");
            }
        }

        // Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using"){
                Librerias();
            }

            if(getClasificacion() == Tipos.TipoDato){
                Variables();
            }
            Main();
            displayList();
        }

        // Librerias -> using ListaLibrerias; Librerias?
        private void Librerias(){
            match("using");
            ListaLibrerias();
            match(";");
            if(getContenido() == "using"){
            Librerias();
            }
        }

        // Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch(getContenido()){
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            if(getContenido() == "="){
                match("=");
                Expresion();
            }
            match(";");
            
            if(getClasificacion() == Tipos.TipoDato){
                Variables();
            }
        }

        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias(){
            match(Tipos.Identificador);
            if(getContenido() == "."){
                match(".");
                ListaLibrerias();
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(Variable => Variable.getNombre() == getContenido())!=null){
                throw new Error("Sintaxis: la variable " + getContenido() +" ya existe", log, linea, col);
            }
            l.Add(new Variable(t, getContenido()));
            match(Tipos.Identificador);

            if(getContenido() == ","){
                match(",");
                ListaIdentificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool execute)
        {
            match("{");
            if (getContenido() != "}"){
                ListaInstrucciones(execute);
            }
            else{
                match("}");
            }
            
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool execute){
            Instruccion(execute);
            if(getContenido()!= "}"){
                ListaInstrucciones(execute);
            }
            else{
                match("}");
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool execute){
            if(getContenido() == "Console"){
                console();
            }
            else if(getContenido() == "if"){
                If(execute);
            }
            else if(getContenido() == "while"){
                While(execute);
            }
            else if(getContenido() == "do"){
                Do(execute);
            }
            else if(getContenido() == "for"){
                For(execute);
            }
            else if(getClasificacion() == Tipos.TipoDato){
                Variables();
            }
            else{
                Asignacion();
                match(";");
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion(){
            Variable? v = l.Find(variable => variable.getNombre() == getContenido());
            if(v == null) {
                throw new Error("La variable " + getContenido() + " no está definida");
            }
            // Console.WriteLine(getContenido() + " = "); 
            match(Tipos.Identificador);
            if (getContenido() == "="){
                match("=");
                if(getContenido() =="Console"){
                    match("Console");
                    match(".");
                    switch (getContenido()){
                        case "Read":
                            match("Read");
                            match("(");
                            match(")");
                            Console.Read();                            
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
                            v.setValor(contNumero);
                        break;
                    }    
                }
                else{
                    Expresion();
                }
                
                // displayStack();	
            }
            else if(getClasificacion() == Tipos.IncrementoTermino){
                if(getContenido() == "++"){
                    match("++");
                    float nuevoValor = v.getValor();
                    v.setValor(nuevoValor ++);
                }
                else if(getContenido() == "--"){
                    match("--");
                    float nuevoValor = v.getValor();
                    v.setValor(nuevoValor --);
                }
                else if(getContenido() == "+="){
                    match("+=");
                    Expresion();
                }
                else if(getContenido() == "-="){
                    match("-=");
                    Expresion();
                }
            }
            else if(getClasificacion() == Tipos.IncrementoFactor){
                match(Tipos.IncrementoFactor);
                Expresion();
                
            }
            float r = s.Pop();
            v.setValor(r);
            // Console.WriteLine(" st =" + s.Pop());
            // displayStack();
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
         // (else bloqueInstrucciones | instruccion)?
        public void If(bool execute2){
            match("if");
            match("(");
            bool execute = Condicion() && execute2;
            Console.WriteLine(execute);
            match(")");
            if (execute2 == true)
            {   
                if(getContenido() == "{"){
                    BloqueInstrucciones(true);
                }
                else{
                    Instruccion(true);
                }
            }
            else
            {
                if(getContenido() == "else"){
                    match("else");
                    if(getContenido() == "{"){
                        BloqueInstrucciones(true);
                    }
                    else{
                        Instruccion(true);
                }
            }
            }
            
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(){
            Expresion();
            float valor1 = s.Pop();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float valor2 = s.Pop();
            
            switch(operador){
                case ">": return valor1 > valor2;
                case ">=": return valor1 >= valor2;
                case "<": return valor1 < valor2;
                case "<=": return valor1 <= valor2;
                case "==": return valor1 == valor2;
                default: return valor1 != valor2;
                
            }

        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool execute){
            match("while");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{"){
                
                BloqueInstrucciones(true);
            }
            else{
                Instruccion(true);
            }
        }
        // Do -> do 
            // bloqueInstrucciones | intruccion 
            // while(Condicion);
        private void Do(bool execute){
            match("do");
            if(getContenido() == "{"){
                
                BloqueInstrucciones(true);
                
            }
            else{
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
        private void For(bool execute){
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");
            if(getContenido() == "{"){
                BloqueInstrucciones(true);
            }
            else{
                Instruccion(true);
            }
        }
        // Console -> Console.(WriteLine|Write) (cadena concatenaciones?);
        public void console(){
            match("Console");
            match(".");
            switch(getContenido()){
                case "WriteLine":
                    match("WriteLine");
                    match("(");
                    if(getContenido() == ")"){
                        match(")");
                        match(";");
                        Console.WriteLine();
                        break;
                    }

                    Console.WriteLine(getContenido().Trim('\"'));
                    match(Tipos.Cadena);
                    match(")");
                    match(";");
                break;
                case "Write":
                    match("Write");
                    match("(");
                    if(getContenido() == ")"){
                        match(")");
                        match(";");

                        break;
                    }

                    Console.Write(getContenido().Trim('\"'));
                    match(Tipos.Cadena);
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
        private void Main(){
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
        public void Expresion(){
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(){
            
            if(getClasificacion() == Tipos.OperadorTermino){
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                // Console.Write(operador + " " );

                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador){
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino(){
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor(){
            if(getClasificacion() == Tipos.OperadorFactor){
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                // Console.Write(operador + " " );
                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador){
                    case "*": s.Push(n2 * n1); break;
                    case "/": s.Push(n2 / n1); break;
                    case "%": s.Push(n2 % n1); break;
                }
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            
            if(getClasificacion() == Tipos.Numero ){
                s.Push(float.Parse(getContenido()));
                // Console.Write(getContenido() + " " );
                match(Tipos.Numero);
                
            }
            else if(getClasificacion() == Tipos.Identificador){
                Variable? v = l.Find(variable => variable.getNombre() == getContenido());
            if(v == null) {
                throw new Error("La variable " + getContenido() + " no está definida");
            }
            
                s.Push(v.getValor());
        
                // Console.Write(getContenido() + " " );
                match(Tipos.Identificador);
            }
            else{
                match("(");
                Expresion();
                match(")");
            }
        }
        public void Concatenaciones() {
            match(Tipos.Cadena);
            if (getContenido() == "+"){
                match("+");
                Concatenaciones();
            }
        }
    }

}
