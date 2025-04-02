using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
{
    public class Variable{
        public enum TipoDato
        {
            Char,
            Int,
            Float
        }
        TipoDato tipo;
        string nombre;
        float valor;
        
        public Variable(TipoDato tipo, string nombre, float valor = 0){
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }

        
        public void setValor(float valor){
            //Validar
            if(valorToTipoDato(valor) <= tipo){
                this.valor = valor;
            }            
            else{
                throw new Error("Semantico: No es posible asignar un " + valorToTipoDato(valor) + " a un "+ tipo + ", linea:" + Lexico.linea + ", columna: " + Lexico.col, Lexico.log);
            }
        }

        //pasar maximo tipo y evaluar dentro de setvalor
        //modifical cada vez que se haga expresion
        public void setValor(float valor, Variable.TipoDato maximoTipo){
            
            if(tipo < maximoTipo){
                throw new Error("Semantico: No es posible asignar un " + valorToTipoDato(valor) + " a un "+ tipo + ", linea:" + Lexico.linea + ", columna: " + Lexico.col, Lexico.log);
            }
            //Validar
            if(valorToTipoDato(valor) <= tipo){
                this.valor = valor;
            }            
            else{
                throw new Error("Semantico: No es posible asignar un " + valorToTipoDato(valor) + " a un "+ tipo, Lexico.log);
            }
            
        }

        public static TipoDato valorToTipoDato (float valor){
            if(!float.IsInteger(valor)){
                return TipoDato.Float;
            }
            else if(valor <= 255){
                return TipoDato.Char;
            }
            else if(valor <= 65525){
                return TipoDato.Int;
            }
            else{
                return TipoDato.Float;
            }
        } 
        public float getValor(){
            return valor;
        }
        public string getNombre(){
            return nombre;
        }
        public TipoDato getTipoDato(){
            return tipo;
        }
    }
}