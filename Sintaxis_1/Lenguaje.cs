using System;
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
*/
namespace Sintaxis_1
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje() : base(){
            log.WriteLine("Constructor Lenguaje");
        }
        public Lenguaje(string name): base(name)
        {
            log.WriteLine("Constructor Lenguaje");
        }

        // Programa  -> Librerias? Variables? Main
        public void Programa(){
            if (getContenido() == "using"){
                Librerias();
            }

            if(getClasificacion() == Tipos.TipoDato){
                Variables();
            }
            Main();
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
        private void Variables(){
            match(Tipos.TipoDato);
            ListaIdentificadores();
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
        private void ListaIdentificadores()
        {
            match(Tipos.Identificador);
            if(getContenido() == ","){
                match(",");
                ListaIdentificadores();
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}"){
                ListaInstrucciones();
            }
            else{
                match("}");
            }
            
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(){
            Instruccion();
            if(getContenido()!= "}"){
                ListaInstrucciones();
            }
            else{
                match("}");
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignación
        private void Instruccion(){
            if(getContenido() == "Console"){
                console();
            }
            else if(getContenido() == "if"){
                If();
            }
            else if(getContenido() == "while"){
                While();
            }
            else if(getContenido() == "do"){
                Do();
            }
            else if(getContenido() == "for"){
                For();
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
            match(Tipos.Identificador);
            if (getContenido() == "="){
                match("=");
                Expresion();
            }
            else if(getClasificacion() == Tipos.IncrementoTermino){
                if(getContenido() == "++"){
                    match("++");
                }
                else if(getContenido() == "--"){
                    match("--");
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
                match(";");
            }
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
         // (else bloqueInstrucciones | instruccion)?
        public void If(){
            match("if");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{"){
                BloqueInstrucciones();
            }
            else{
                Instruccion();
            }
            if(getContenido() == "else"){
                match("else");
                if(getContenido() == "{"){
                    BloqueInstrucciones();
                }
                else{
                    Instruccion();
                }
            }
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion(){
            Expresion();
            match(Tipos.OperadorRelacional);
            Expresion();
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(){
            match("while");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{"){
                
                BloqueInstrucciones();
            }
            else{
                Instruccion();
            }
        }
        // Do -> do 
            // bloqueInstrucciones | intruccion 
            // while(Condicion);
        private void Do(){
            match("do");
            if(getContenido() == "{"){
                
                BloqueInstrucciones();
                
            }
            else{
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }

        // For -> for(Asignacion; Condicion; Asignacion) 
        // BloqueInstrucciones | Intruccion
        private void For(){
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");
            if(getContenido() == "{"){
                BloqueInstrucciones();
            }
            else{
                Instruccion();
            }
        }
        // Console -> Console.(WriteLine|Write) (cadena concatenaciones?);
        public void console(){
            match("Console");
            match(".");
            if(getContenido() == "WriteLine"){
                match("WriteLine");
                match("(");
                if(getContenido() == "\""){
                    match(Tipos.Cadena);
                    
                }
            match(")");
            match(";");
            }
            else if (getContenido() == "Write") {
                match("Write");
                match("(");
                if(getContenido() == "\""){
                    match(Tipos.Cadena);
                }
                match(")");
                match(";");
            }
            if(getContenido() == "ReadLine"){
                match("ReadLine");
                match("(");
                Console.ReadLine();
                
            }
            else if(getContenido() == "Read"){
                match("Read");
                match("(");
                Console.Read();
                
            }
            match(")");
            match(";");
            
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
            BloqueInstrucciones();
        }
        // Expresion -> Termino MasTermino
        public void Expresion(){
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(){
            if(getClasificacion() == Tipos.OperadorTermino){
                match(Tipos.OperadorTermino);
                Termino();
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
                match(Tipos.OperadorFactor);
                Factor();
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor(){
            if(getClasificacion() == Tipos.Numero ){
                match(Tipos.Numero);
                
            }
            else if(getClasificacion() == Tipos.Identificador){
                match(Tipos.Identificador);
            }
            else{
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}