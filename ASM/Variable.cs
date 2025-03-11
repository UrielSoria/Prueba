using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM
{
    public class Variable
    {
        public enum TipoDato
        {
            Char, Int, Float
        }
        TipoDato tipo;
        string nombre;
        float valor;
        public Variable(TipoDato tipo, string nombre, float valor = 0)
        {
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }
         public void setValor(float valor, bool ejecuta){
            //Validar
            if ( valorToTipoDato(valor) <= tipo)
            {
                this.valor = valor;
               
            }
            else 
            {
                throw new Error("Semantico. No se puede asignar un float a un "+  valorToTipoDato(valor) + " a un " + tipo + "en la variable" + nombre, Lexico.log);
            }
        }
        
        public void setValor(float valor, Variable.TipoDato maximoTipo){
            //Validar
            if( maximoTipo > tipo){
                throw new Error("Semantico. No se puede almacenar un: "+ tipo + " en una variable de tipo:  "+ maximoTipo + " en la variable: " + nombre + " en la linea: "+Lexico.linea + " en la columna: "+ Lexico.columna, Lexico.log );
            }
            if( valorToTipoDato(valor) <= tipo)
            {
                this.valor = valor; 
            }
            else 
            {
                throw new Error("Semantico. No es posible asignar  un "+ valorToTipoDato(valor) + "a un " + tipo + " en la linea: "+Lexico.linea + " en la columna: "+ Lexico.columna, Lexico.log);
            }
        }
        public static TipoDato valorToTipoDato(float valor)
        {
            if (!float.IsInteger(valor))
            {
                return TipoDato.Float;
            }
            else if(valor <= 255)
            {
                return TipoDato.Char;

            }
            else if(valor <= 65535)
            {
                return TipoDato.Int;
            }
            else 
            {
                return TipoDato.Float;
            }
        }
        public float getValor{
            get => valor;
            // set => valor = value;
        }
        // public float getValor()
        // {
        //     return valor;
        // }

        public string getNombre{
            get => nombre;
            // set => nombre = value;
        }
        // public string getNombre()
        // {
        //     return nombre;
        // }
        public TipoDato getTipoDato{
            get => tipo;
            // set => tipo = value; 
        }
        // public TipoDato GetTipoDato(){
        //     return tipo;
        // }
    }
}