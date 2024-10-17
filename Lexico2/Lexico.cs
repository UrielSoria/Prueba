using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;



namespace Lexico2
{
    public class Lexico : Token, IDisposable
    {
        string PATH = "C:/Users/uriso/C#/Lexico2/";
        StreamReader archivo;
        readonly StreamWriter log;
        StreamWriter asm;
        int linea = 1;

        const int F = -1;
        const int E = -2;
        public Lexico()
        {  
            
            log = new StreamWriter("./main.log");
            asm = new StreamWriter(PATH + "main.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if( File.Exists(PATH + "prueba.cpp"))
            {
            archivo = new StreamReader(PATH + "prueba.cpp");   
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }

        
        public Lexico(string fileName)
        {
            
            string fileNameWithoutExtension;
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            log = new StreamWriter( fileNameWithoutExtension + ".log");
            log.AutoFlush = true;

            if(System.IO.Path.GetExtension(fileName).ToLower() == ".cpp")
            {
                asm = new StreamWriter(fileNameWithoutExtension + ".asm");
                asm.AutoFlush = true;
                if( File.Exists(fileName))
                {
                archivo = new StreamReader(fileName);   
                }
                else
                {
                    throw new Error("El archivo prueba.cpp no existe", log);
                }
            }
            else
            {
                throw new Error("El tipo de archivo no es correcto, se esperaba (.cpp) ", log);
            }

        }
        
        
        public void Dispose()
        {
            // log.WriteLine("Lineas: "+linea);
            archivo.Close();
            log.Close();
            asm.Close();
        }
        private int automata(char c, int estado)
        {
            int nuevoEstado = estado;
            switch (estado)
            {
                case 0:
                    if (char.IsWhiteSpace(c))
                    {
                        nuevoEstado = 0;
                    }
                    else if (char.IsLetter(c))
                    {
                        nuevoEstado = 1;
                    }
                    else if (char.IsDigit(c))
                    {
                        nuevoEstado = 2;
                    }
                    else if (c == ';'){
                        nuevoEstado = 8;
                    }
                    else if (c == '{')
                    {
                        nuevoEstado = 9;
                    }
                    else if (c == '}')
                    {
                        nuevoEstado = 10;
                    }
                    else if (c == '?')
                    {
                        nuevoEstado = 11;
                    }
                    else if (c == '+')
                    {
                        nuevoEstado = 12;
                    }
                    else if (c == '-')
                    {
                        nuevoEstado = 14;
                    }
                    else if (c == '*' || c == '%'){
                        nuevoEstado = 16;
                    }
                    else if (c == '&'){
                        nuevoEstado = 18;
                    }
                    else if (c == '|'){
                        nuevoEstado = 20;
                    }
                    else if (c == '!'){
                        nuevoEstado = 21;
                    }
                    else if (c == '='){
                        nuevoEstado = 23;
                    }
                    else if (c == '>'){
                        nuevoEstado = 25;
                    }
                    else if (c == '<'){
                        nuevoEstado = 26;
                    }
                    else if (c == '"'){
                        nuevoEstado = 27;
                    }
                    else if (c == '\''){
                        nuevoEstado = 29;
                    }
                    else if (c == '#'){
                        nuevoEstado = 32;
                    }
                    else if (c == '/'){
                        nuevoEstado = 34;
                    }





                    else
                    {
                        nuevoEstado = 33;
                    }
                    break;
                case 1:
                    setClasificacion(Tipos.Identificador);
                    if (!char.IsLetterOrDigit(c))
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 2:
                    setClasificacion(Tipos.Numero);
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 2;
                    }
                    else if (c == '.')
                    {
                        nuevoEstado = 3;
                    }
                    else if (char.ToLower(c) == 'e')
                    {
                        nuevoEstado = 5;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 3:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 4;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 4:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 4;
                    }
                    else if (char.ToLower(c) == 'e')
                    {
                        nuevoEstado = 5;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 5:
                    
                    if (c == '+' || c == '-')
                    {
                        nuevoEstado = 6;
                    }
                    else if (char.IsDigit(c))
                    {
                        nuevoEstado = 7;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 6:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 7:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 8:
                    setClasificacion(Tipos.FinSentencia); 
                    nuevoEstado = F;
                    break;
                case 9:
                    setClasificacion(Tipos.InicioBloque);
                    nuevoEstado = F;
                    break;
                case 10:
                    setClasificacion(Tipos.FinBloque);
                    nuevoEstado = F;
                    break;
                case 11:
                    setClasificacion(Tipos.OperadorTernario);
                    nuevoEstado = F;
                    break;
                case 12:
                    setClasificacion(Tipos.OperadorTermino);
                    if(c == '+' || c == '='){
                        nuevoEstado = 13;
                    }
                    else{
                        nuevoEstado = F;
                    }
                    break;
                case 13:
                    setClasificacion(Tipos.IncrementoTermino);
                    nuevoEstado = F;
                    break;
                case 14:
                    setClasificacion(Tipos.OperadorTermino);
                    if(c == '-' || c == '='){
                        nuevoEstado = 13;
                    }
                    else if (c == '>'){
                        nuevoEstado = 15;
                    }
                    else{
                        nuevoEstado = F;
                    }
                    break;
                case 15:
                    setClasificacion(Tipos.Puntero);
                    nuevoEstado = F;
                    break;
                case 16:
                    setClasificacion(Tipos.OperadorFactor);
                    if (c == '='){
                        nuevoEstado = 17;
                    }
                    else{
                        nuevoEstado = F;
                    }
                    break;
                case 17:
                    setClasificacion(Tipos.IncrementoFactor);
                    nuevoEstado = F;
                    break;
                case 18:
                    setClasificacion(Tipos.Caracter);
                    if (c == '&')
                    {
                        nuevoEstado = 19;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 19:
                    setClasificacion(Tipos.OperadorLogico);
                    nuevoEstado = F;
                    break;
                case 20:
                    setClasificacion(Tipos.Caracter);
                    if (c == '|')
                    {
                        nuevoEstado = 19;
                    }
                    else 
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 21:
                    setClasificacion(Tipos.OperadorLogico);
                    if (c == '=')
                    {
                        nuevoEstado = 22;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 22:
                    setClasificacion(Tipos.OperadorRelacional);
                    nuevoEstado = F;
                    break;
                case 23:
                    setClasificacion(Tipos.Asignacion);
                    if (c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else 
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 24:
                    setClasificacion(Tipos.OperadorRelacional);
                    nuevoEstado = F;
                    break;
                case 25:
                    setClasificacion(Tipos.OperadorRelacional);
                    if (c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else{
                        nuevoEstado = F;
                    }
                    break;
                case 26:
                    setClasificacion(Tipos.OperadorRelacional);
                    if (c == '>' || c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else{
                        nuevoEstado = F;
                    }
                    break;
                case 27:
                    setClasificacion(Tipos.Cadena);
                    if (char.IsLetterOrDigit(c)){
                        nuevoEstado = 27;
                    }
                    else if (c == '"'){
                        nuevoEstado = 28 ;
                    }
                    else if (finArchivo()){
                        nuevoEstado = E;
                    }
                    break;
                case 28:
                    nuevoEstado = F;
                    break;
                case 29:
                    setClasificacion (Tipos.Caracter);
                    nuevoEstado = 30;
                    break;
                case 30:
                    if (c == '\''){
                        nuevoEstado = 31;
                    }
                    else {
                        nuevoEstado = E;
                    }
                    break;
                case 31:
                    nuevoEstado = F;
                    break;
                case 32:
                    setClasificacion (Tipos.Caracter);
                    if (char.IsDigit(c)){
                        nuevoEstado = 32;
                    }
                    else{
                        nuevoEstado = F;
                    }
                    break;
                case 33:
                    setClasificacion(Tipos.Caracter);
                    nuevoEstado = F;
                    break;
                case 34:
                    setClasificacion(Tipos.OperadorFactor);
                    nuevoEstado = F;
                    if (c == '='){
                        nuevoEstado = 17;
                    }
                    else if (c == '/'){
                        nuevoEstado = 35;
                    }
                    else if (c == '*'){
                        nuevoEstado = 36;
                    }
                     
                    break;
                case 35:
                    nuevoEstado = 35;
                    if (c == '\n'){
                        nuevoEstado = 0;
                    }
                    
                    break;
                case 36:
                    nuevoEstado = 36;
                    if (c == '*'){
                        nuevoEstado = 37;
                    }
                    else if (finArchivo()){
                        nuevoEstado = E;
                    }
                    
                    break;
                case 37:
                    nuevoEstado = 36;
                    if (c == '/'){
                        nuevoEstado = 0;
                    }
                    else if (c == '*'){
                        nuevoEstado = 37;
                    }
                    else if (finArchivo()){
                        nuevoEstado = E;
                    }
                    break;
            }
            return nuevoEstado;
        }
        public void nextToken()
        {
            char transicion;
            string buffer = "";
            int estado = 0;

            while(estado >= 0)
            {
                transicion = (char)archivo.Peek();
                estado = automata(transicion, estado);
                if (estado == 0){
                    buffer = "";
                }
                if (estado == E )
                {
                    if (getClasificacion() == Tipos.Numero)
                    {
                        throw new Error ("Lexico, se espera un digito", log, linea);
                    }
                    else if (getClasificacion() == Tipos.Cadena){
                        throw new Error ("Lexico, no se ha cerrado la cadena", log, linea);
                    }
                    else if (getClasificacion() == Tipos.Caracter){
                        throw new Error ("Lexico, caracter invalido/no se ha cerrado el caracter", log, linea);
                    }
                    else{
                        throw new Error ("Lexico, no se ha cerrado el comentario de bloque", log, linea);
                    }
                }
                if (estado >= 0)
                {
                    archivo.Read();
                    if (transicion == '\n')
                    {
                        linea++;
                    }
                    if (estado > 0)
                    {
                        buffer+= transicion;
                    }
                }
            }

            if (!finArchivo())
            {
            setContenido(buffer);
            log.WriteLine(getContenido() + " ····· " + getClasificacion());
            }
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
        
    }
}

/*

    Expresion Regular: Metodo Formal que a través de una secuencia de caracteres 
    define un PATRON de busqueda

    a) Regla BNF
    b) Reglas BNF extendidas
    c) Operaciones aplicadas al lenguaje

    OAL

    1. Concatenacion simple ()
    2. Concatenacion exponencial (Exponente)
    3. Cerradura de Kleene (*)
    4. Cerradura Positiva (+)
    5. Cerradura Epsilon (?)
    6. Operador OR (|)
    7. Uso de parentesis ()

    L = {A, B , C, D, E, ...., Z, a, b, c, d, ....., z}
    D = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

    1.  L.D
        LD
        >=

    2.  L^3 = LLL
        D^5 = DDDDD
        =^2 = ==
        L^3D^2 = LLLDD

    3.  L* = Cero o más letras
        D* = Cero o más digitos

    4.  L+ = Una o más letras
        d* = Uno o más digitos

    5.  L? = Cero o una letra (la letra es opcional)

    6.  L | D = letra o digito
        + | - = + o -

    7.  ( L D) L? = (Letra seguido de un digito ) y al final letra opcionañ

    Produccion Gramatical 

    Clasificacion del token -> Expresion Regular

    Identificador -> L + (L | D )*

    Numero -> d+ (.D+)? (E(+|-)? D+)?
    
    FinSentencia -> ;
    InicioBloque -> {
    FinBloque -> }
    OperadorTernario -> ?

    Puntero -> ->

    OperadorTermino -> + | -
    IncrementoTermino -> +( | =) | -(- | =)

    Termino+ -> +( + | = )?
    Termino-P -> - (- | = | >)?

    OperadorFactor -> * | / | %
    IncrementoFactor -> *=| /= | %=

    Factor -> * | / | % (=)?

    OperadorLogico -> && | || | !

    NotOpRel -> ! (=)?

    Asignacion -> =

    AsOpTel -> = (=)?

    OperadorRelacional -> > (=)?  | < ( > | =)? | ==

    
    
    Cadena -> "c*"
    
   
    Caracter -> 'c' | #d* | Lambda



    Automata: Modelo matematico que representa una expresion regular a través 
    de un GRAFO, para una maquina de estado finito que consiste en un 
    conjunto de estados bien definidos, 
    - un estado inical
    - un alfabeto de entrada 
    - una funcion de transicion
*/