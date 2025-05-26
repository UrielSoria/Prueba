using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generador
{
    public class Token
    {
        public enum Tipos
        {
            SNT,                        //SNT -> L+ --Empieza con mayuscula
            ST,                         // ST -> L+ --Empieza con minuscula
                                        //ST  -> palabras reservadas de Token
                                        //ST  -> Lambda   
            Produce,                    //Produce -> ->
            FinProduccion,              //FinProduccion -> \;
            Optativo,                   //Optativo -> \?
            InicioAgrupacion,           //InicioAgrupacion -> \(
            CierreAgrupacion,           //CierreAgrupacion -> \)
            OR                          //OR -> \| 
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
            contenido = "";
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