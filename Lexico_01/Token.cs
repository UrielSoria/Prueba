using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_01
{
    public class Token
    {
        public enum Tipos
        {
            Identificador,Numero,Caracter,FinSentencia,
            InicioBloque,FinBloque, OperadorTernario, 
            OperadorTermino, OperadorFactor, IncrementoTermino, 
            IncrementoFactor, Puntero, Asignacion, 
            OperadorRelacional, OperadorLogico, Moneda,
            Cadena, Adn
        }
        private string contenido;
        private Tipos clasificacion;
        public Token(){
            contenido = " ";
            clasificacion = 0;
        }

        public void setContenido(string contenido){
            this.contenido = contenido;
        }
        public void setClasificacion(Tipos clasificacion){
            this.clasificacion = clasificacion;
        }

        public string getContenido(){
            return this.contenido;
        }

        public Tipos getClasificacion(){
            return this.clasificacion;
        }
    }
}