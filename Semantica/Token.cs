using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Token
    {
        public enum Tipos
        {
            Identificador,Numero,Caracter,FinSentencia,
            InicioBloque,FinBloque, OperadorTernario, 
            OperadorTermino, OperadorFactor, IncrementoTermino, 
            IncrementoFactor, Puntero, Asignacion, 
            OperadorRelacional, OperadorLogico, Cadena,
            TipoDato, PalabraReservada
        }
        private string contenido;

        public string Contenido{
            set => contenido = value;
            get => contenido;
        }
        public Tipos Clasificacion{
            set => clasificacion = value;
            get => clasificacion;
        }
        
        private Tipos clasificacion;
        public Token(){
            contenido = " ";
            clasificacion = Tipos.Identificador;
        }
       
        /*
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
        */
    }
}