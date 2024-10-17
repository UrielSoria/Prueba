using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;



namespace Lexico_01
{
    public class Lexico : Token, IDisposable
    {
        string PATH = "C:/Users/uriso/C#/Lexico_01/";
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

        }
        
        
        public void Dispose()
        {
            // log.WriteLine("Lineas: "+linea);
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
                //Parte fracccional
                if(c == '.')
                {
                    buffer+=c;
                    archivo.Read();
                    if(char.IsDigit(c=(char)archivo.Peek()))
                    {
                        
                        while(char.IsDigit(c=(char)archivo.Peek()))
                        {
                            buffer+=c;
                            archivo.Read();
                        }
                    }
                    else
                    {
                        throw new Error("Parte fraccionaria incompleta,", log, linea);
                    }
                    

                }
                
                if (char.ToLower(c)=='e') // Parte Exponencial
                {
                    buffer+=c;
                    archivo.Read();
                    //En caso de que el exponenete tenga signo
                    if ((c=(char)archivo.Peek()) == '+' || (c=(char)archivo.Peek())== '-')
                    {
                        buffer+=c;
                        setClasificacion(Tipos.Numero);
                        archivo.Read();
                        if(char.IsDigit(c=(char)archivo.Peek()))
                        {
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
                    //si no tiene signo
                    else
                    {
                        if(char.IsDigit(c=(char)archivo.Peek()))
                        {
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
                        // buffer+=c;
                        archivo.Read();
                        break;
                    }
                    else if (archivo.EndOfStream)
                    {
                        throw new Error("No se han cerrado las comillas", log, linea);
                    }  
                }
            }
            //Comillas simples
            else if(c == '\'')
            {
                setClasificacion(Tipos.Caracter);
                while((c=(char)archivo.Peek()) != '\'')
                {
                    buffer+=c;
                    archivo.Read();
                    if ((c=(char)archivo.Peek()) == '\'')
                    {
                        buffer+=c;
                        archivo.Read();
                        break;
                    }
                    else
                    {
                        throw new Error("Caracter invalido", log, linea);
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