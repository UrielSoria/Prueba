using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/*

    Requerimiento 1: Sobrecargar el constructor lexico para que escriba como argumento
    el nombre del archivo
    Requerimiento 2: contador de lineas
    Requerimiento 3: Agregar OperadorRelacional
                        ==l >l <l >=l <=l <>l != (diferente de ), !==             
    requerimiento 4: OperadorLogico
                        && ||, !

    Examen: Agregar el siguiente token o tokens
*/

namespace Lexico_01
{
    public class Lexico : Token, IDisposable
    {
        string PATH = "C:/Users/uriso/C#/Lexico_01/";
        //string fileName = "C:/Users/uriso/C#/Lexico_01/prueba.cpp";
        StreamReader archivo;
        readonly StreamWriter log;
        StreamWriter asm;
        
        int linea = 1;
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

            /*
                Si nombre = suma.cpp
                LOG = suma.log
                ASM = suma.asm
                y validar la extension del nombre del archivo(que tenga cpp a fuerzas)
                file (como verificar el archivo de un string y cambiarlo)
            */
        }
        
        
        public void Dispose()
        {
            
            archivo.Close();
            log.Close();
            asm.Close();
        }
        public void nextToken()
        {
            char c;
            string buffer = "";

            while(char.IsWhiteSpace(c=(char)archivo.Read()))
            {   
                if (c == '\n')
                {
                    linea++;
                }
            }
            buffer+=c;
            
            if(char.IsLetter(c))
            {
                setClasificacion(Tipos.Identificador);
                while(char.IsLetterOrDigit(c=(char)archivo.Peek()))
                {
                    buffer+=c;
                    archivo.Read();
                }
            }
            //Numero
            else if(char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                while(char.IsDigit(c=(char)archivo.Peek()))
                {
                    buffer+=c;
                    archivo.Read();
                }
                //Decimal
                if(c == '.')
                {
                    buffer+=c;
                    archivo.Read();
                    if(char.IsDigit(c=(char)archivo.Peek()))
                    {
                        setClasificacion(Tipos.Numero);
                        while(char.IsDigit(c=(char)archivo.Peek()))
                        {
                            buffer+=c;
                            archivo.Read();
                        }
                    }
                    else
                    {
                        throw new Error("En el formato de número decimal", log, linea);
                    }
                    

                }
                //exponente
                if (char.ToLower(c)=='e') // Parte Exponencial
                {
                    buffer+=c;
                    archivo.Read();
                    setClasificacion(Tipos.Numero);
                    //En caso de que el exponenete tenga signo
                    if ((c=(char)archivo.Peek()) == '+' || (c=(char)archivo.Peek())== '-')
                    {
                        buffer+=c;
                        setClasificacion(Tipos.Numero);
                        archivo.Read();
                        if(char.IsDigit(c=(char)archivo.Peek()))
                        {
                            setClasificacion(Tipos.Numero);
                            while(char.IsDigit(c=(char)archivo.Peek()) || !archivo.EndOfStream)
                            {
                                buffer+=c;
                                archivo.Read();
                            }
                        }
                        else if (char.IsLetter(c=(char)archivo.Peek()))
                        {
                            throw new Error("Exponente con letra", log, linea);
                        }
                        else
                        {
                            throw new Error("Exponente incomleto", log, linea);
                        }
                    }
                    //si no tiene signo
                    else
                    {
                        if(char.IsDigit(c=(char)archivo.Peek()))
                        {
                            setClasificacion(Tipos.Numero);
                            while(char.IsDigit(c=(char)archivo.Peek()))
                            {
                                buffer+=c;
                                archivo.Read();
                            }
                        }
                        else if (char.IsLetter(c=(char)archivo.Peek()))
                        {
                            throw new Error("Exponente con letra", log, linea);
                        }
                        else
                        {
                            throw new Error("Exponente incomleto", log, linea);
                        }
                    }  
                }
            }
            else if(c==';')
            {
                setClasificacion(Tipos.FinSentencia);
            }
            else if (c == '{')
            {
                setClasificacion(Tipos.InicioBloque);
            }
            else if(c == '}')
            {
                setClasificacion(Tipos.FinBloque);
            }
            else if(c == '?' || c==':')
            {
                setClasificacion(Tipos.OperadorTernario);
            }
            else if (c == '+')
            {
                setClasificacion(Tipos.OperadorTermino);
                if((c=(char)archivo.Peek()) == '+' || c == '=')
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            else if(c == '-')
            {
                setClasificacion(Tipos.OperadorTermino);
                if((c=(char)archivo.Peek()) == '-' || c == '=')
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                    
                }
                else if ((c=(char)archivo.Peek()) == '>')
                {
                    setClasificacion(Tipos.Puntero);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '*' || c == '%' || c == '/')
            {
                setClasificacion(Tipos.OperadorFactor);
                if((c=(char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.IncrementoFactor);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            else if (c == '=')
            {
                setClasificacion(Tipos.Asignacion);
                if((c=(char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            else if (c=='<')
            {
                  setClasificacion(Tipos.OperadorRelacional);
                  if((c=(char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
                else if((c=(char)archivo.Peek()) == '>')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            else if(c == '>')
            {
               setClasificacion(Tipos.OperadorRelacional);
                  if((c=(char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                } 
            }
            else if(c=='!'){
                setClasificacion(Tipos.OperadorLogico);
                if((c=(char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            
            else if (c == '&')
            {
                setClasificacion(Tipos.Caracter);
                if((c=(char)archivo.Peek()) == '&')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            else if (c == '|')
            {
                setClasificacion(Tipos.Caracter);
                if((c=(char)archivo.Peek()) == '|')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                    
                }
            }
            else if (c =='$')
            {
                setClasificacion(Tipos.Caracter);
                
                if(char.IsDigit(c=(char)archivo.Peek()))
                {setClasificacion(Tipos.Moneda);
                   while(char.IsDigit(c=(char)archivo.Peek()))
                {
                    buffer+=c;
                    archivo.Read();
                }
                }
            }
            else if(c=='@')
            {
                setClasificacion(Tipos.Caracter);
            }
            else if(c=='#')
            {
                setClasificacion(Tipos.Caracter);
                while(char.IsDigit(c=(char)archivo.Peek()))
                {
                    buffer+=c;
                    archivo.Read();
                }
            }
            else if(c=='"'){
                
                setClasificacion(Tipos.Cadena);
                while((c=(char)archivo.Peek()) != '"' || !archivo.EndOfStream)
                {
                    buffer+=c;
                    archivo.Read();
                    if (c == '"')
                    {
                        buffer+=c;
                        archivo.Read();
                        break;
                    }
                    else if (archivo.EndOfStream)
                    {
                        throw new Error("No se han cerrado las comillas", log, linea);
                    }
                    
                }
                
            }
            else if(c == '\'')
            {
                
                setClasificacion(Tipos.Caracter);
                while((c=(char)archivo.Peek()) != '\'')
                {
                    buffer+=c;
                    archivo.Read();
                    
                    if (char.IsLetterOrDigit(c))
                    {
                        if ((c=(char)archivo.Peek()) == '\'')
                        {
                            buffer+=c;
                            archivo.Read();
                            Console.WriteLine("l");
                            break;
                        }
                        else{
                            throw new Error("Caracter invalido en cadena o no se ha cerrado la cadena", log, linea);
                        } 
                    }
                }
            }
     
            else
            {
                setClasificacion(Tipos.Caracter);
            }
            if(!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
                
            }

            
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
        
    }
}