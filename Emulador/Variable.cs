using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
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
        public float Valor{
            get => valor;
            set => valor = value;
        }
        // public void setValor(float valor){
        //     this.valor = valor;
        // }
        // public float getValor()
        // {
        //     return valor;
        // }
        public string getNombre{
            get => nombre;
            // set => nombre = value;
        }
        public TipoDato getTipoDato{
            get => tipo;
            // set => tipo = value; 
        }
        // public string getNombre()
        // {
        //     return nombre;
        // }
        // public TipoDato GetTipoDato(){
        //     return tipo;
        // }
    }
}