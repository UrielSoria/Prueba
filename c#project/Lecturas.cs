using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace prueba
{
    public class Lecturas : IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        public Lecturas()
        {
            archivo = new StreamReader("C:/Users/uriso/C#/c#project/prueba.cpp");
            log     = new StreamWriter("C:/Users/uriso/C#/c#project/prueba.log");
        }
        public Lecturas(string nombre)
        {
            archivo = new StreamReader(nombre);
            log     = new StreamWriter("prueba.log");
        }
        
        public void Dispose()
        {
            archivo.Close();
            log.Close();
        }
        public void Display()
        {
            while (!archivo.EndOfStream)
            {
                Console.Write((char)archivo.Read());
            }
        }
        public void Copy()
        {
            while (!archivo.EndOfStream)
            {
                log.Write((char)archivo.Read());
            }
        }
        public void Encrypt()
        {
            char c;
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();
                if (char.IsLetter(c))
                {
                    log.Write((char)(c+2));
                }
                else
                {
                    log.Write(c);
                }
            }
        }
        public void Encrypt2(char o)
        {
            char[] vocal=['A', 'E', 'I', 'O', 'U', 'a', 'e', 'i', 'o', 'u'];
            char c;
            bool vocalcheck;
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();

                vocalcheck = vocal.Contains(c); 
                if (vocalcheck)
                {
                    log.Write(o);
                }
                else
                {
                    log.Write(c);
                }
            }
        }
        
        public int contarLetras()
        {
            char c;
            int countletters = 0;
            c = (char)archivo.Read();
                if (char.IsLetter(c))
                {
                    countletters++;
                }
                return countletters;
        }
        public int contarEspacios(){
            char c;
            int countSpaces = 0;
            while (!archivo.EndOfStream)
            {
                
                c = (char)archivo.Read();
                if (char.IsWhiteSpace(c))
                {
                    countSpaces ++;
                }
                
            }
            return countSpaces;
        }
        public int contarDigitos()
        {
            char c;
            int countDigits = 0;
            
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();
                if (char.IsDigit(c))
                {
                    countDigits ++;
                }
                
            }
            return countDigits;
        }
        public dynamic palabra()
        {
            char c;
            string buffer = "";
            
            while (char.IsWhiteSpace(c = (char)archivo.Read()) && !archivo.EndOfStream)
            {

            }
            if (char.IsLetter(c)){
                buffer +=c;
                while (char.IsLetter(c = (char)archivo.Read()) && !archivo.EndOfStream){
                    buffer +=c;
                }
            }
            else if(char.IsDigit(c)){
                buffer +=c;
                while (char.IsDigit(c = (char)archivo.Read()) && !archivo.EndOfStream){
                    buffer +=c;
                }
            }
            log.WriteLine(buffer);
            return buffer.ToString();
        }

        public bool finArchivo(){
            return archivo.EndOfStream;
        }
        
    }
}