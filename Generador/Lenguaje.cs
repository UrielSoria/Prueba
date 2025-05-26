/*
1. el metodo programa es publico y todas la demas son privadas - listo
2. agregar los metodos a una estructura de datos para validar que todos los metodos invocados (SNT), del lado
   derecho de la produccion, existan - listo
3. Agregar a la estructura el simbolo con que inicia la produccion´(vas a tener un programa donde te diga SNT, Variables y luego agregale como empieza por si hay una recursividad ahi)
-listo!!1
4. Implementar la cerradura epsilon en agrupaciones(ya se hizo una parte con clasificaciones pero falta la otra parte del con el contenido, SNT ya se resolvio
    ) **DEPENDE DEL 2 
5. Implementar el OR
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Generador
{
    public class Lenguaje : Sintaxis
    {
        public Dictionary<string, string> Metodos = new Dictionary<string, string>();

        public Lenguaje(string nombre = "gram.txt") : base(nombre)
        {
        }

        //Genera produce Cabecera seguido de producciones
        //Genera -> Cabecera Producciones
        public void AvanzaEnElArchivo()
        {
            while (!finArchivo() && Contenido != "{")
            {
                nextToken();
            }
            while (!finArchivo() && Contenido != "}")
            {
                // match("{");
                AgregarMetodos();
            }
            if (finArchivo())
            {
                foreach (var metodo in Metodos)
                {
                    Console.WriteLine($"Metodo: {metodo.Key} - Valor: {metodo.Value}");
                }
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(0, SeekOrigin.Begin);
                charactercounter = 0;
                linea = 1;
                nextToken();
            }
        }
        public void AgregarMetodos()
        {
            // while (!finArchivo() && Contenido != "{")
            // {
            //     nextToken();
            // }
            // while (!finArchivo() && Contenido != "}")
            // {
                string nombreMetodo = Contenido;
                if (Clasificacion == Tipos.SNT)
                {
                    match(Tipos.SNT);
                    match(Tipos.Produce);
                    if (!Metodos.ContainsKey(nombreMetodo))
                    {
                    if (Clasificacion == Tipos.SNT)
                    {
                        Metodos.Add(nombreMetodo, Contenido + "()");
                    }
                    else
                    {
                        Metodos.Add(nombreMetodo, "\"" + Contenido + "\"");
                        }
                       
                        Console.WriteLine($"Agregado: {nombreMetodo} Contenido: {Contenido}");
                    }
                    else
                    {
                        throw new Error($"El metodo {nombreMetodo} ya existe.", log, linea, columna);
                    }
                    while (Clasificacion != Tipos.FinProduccion)
                    {   
                        match(Clasificacion);
                    }
                    match(Tipos.FinProduccion);
                    // nextToken();
                    if (Clasificacion == Tipos.SNT)
                    {
                        Console.WriteLine();
                        AgregarMetodos();
                    }
                    // Console.WriteLine(Contenido);
                }
                nextToken();
                // if (Clasificacion == Tipos.SNT)
                // {
                //     AgregarMetodos();
                // }

            } 
            
        // }
        public void Genera()
        {
            Cabecera();

            match("{");
            Producciones();
            Write("}", 1);
            Write("}");
            match("}");
        }
        

        //Cabecera -> Lenguaje: ST | SNT
        private void Cabecera()
        {
            match("Lenguaje");
            match(":");
            string nombre = Contenido;
            if (Clasificacion == Tipos.ST)
            {
                match(Tipos.ST);
            }
            else
            {
                match(Tipos.SNT);
                match(";");
                Write("{");
                Write("public class Lenguaje", 1);
                Write("{", 1);
                Write("public Lenguaje ()", 2);
                Write("{", 2);
                Write("}", 2);
            }
        }

        // Producciones -> Produccion Producciones?
        private void Producciones()
        {
            Produccion();

            if (Contenido != "}")
            {
                Producciones();
            }
        }

        //Produccion -> SNT Produce ListaDesimbolos
        private void Produccion()
        {
            string nombreMetodo = Contenido;
            if (Contenido == "Programa")
            {
                Write("public void " + Contenido + "()", 2);
            }
            else
            {
                // 
                // {
                Write("private void " + Contenido + "()", 2);

                // else

                // throw new Error($"El metodo {nombreMetodo} no está definido.", log, linea, columna);
                // }
            }

            Write("{", 2);
            match(Tipos.SNT);
            match(Tipos.Produce);
            ListaSimbolos(Contenido);
            match(Tipos.FinProduccion);
            Write("}", 2);

        }

       private void ListaSimbolos(string firstSymbol, int tabs = 3)
        {
            if (Clasificacion == Tipos.SNT)
            {
                //Si es palabra reservada
                if (isClasification(Contenido))
                {
                    string name = Contenido;
                    match(Tipos.SNT);
                    if (Clasificacion == Tipos.Optativo || Clasificacion == Tipos.OR)
                    {
                        Write($"if(Clasificacion == Tipos.{name})", tabs);
                        Write("{", tabs);
                        Write($"match(Tipos.{name});", tabs + 1);
                        Write("}", tabs);
                        if (Clasificacion == Tipos.Optativo)
                        {
                            match(Tipos.Optativo);
                        }
                        else if (Clasificacion == Tipos.OR)
                        {
                            match(Tipos.OR);
                            Write("else", tabs);
                            Write("{", tabs);
                            ListaSimbolos(firstSymbol, tabs + 1);
                            // Write($"match(Tipos.{name});", tabs + 1);
                            Write("}", tabs);
                            if (Clasificacion == Tipos.OR)
                            {
                                ListaSimbolos(firstSymbol, tabs + 1);
                            }
                        }

                    }
                    else
                    {
                        Write($"match(Tipos.{name});", tabs);
                    }
                }
                //Si es una producción
                else
                {
                    string name = Contenido;
                    Console.WriteLine("cont" + Contenido);

                    match(Tipos.SNT);

                    //Si viene un optativo
                    if (Clasificacion == Tipos.Optativo || Clasificacion == Tipos.OR)
                    {
                        Write($"if(Contenido == {Metodos[name]})", tabs);
                        Write("{", tabs);
                        Write(name + "();", tabs + 1);
                        Write("}", tabs);
                        if (Clasificacion == Tipos.Optativo)
                        {
                            match(Tipos.Optativo);
                        }
                        else if (Clasificacion == Tipos.OR)
                        {
                            match(Tipos.OR);
                            Write("else", tabs);
                            Write("{", tabs);
                            ListaSimbolos(firstSymbol, tabs + 1);
                            // Write($"match(Tipos.{name});", tabs + 1);
                            Write("}", tabs);
                            if (Clasificacion == Tipos.OR)
                            {
                                ListaSimbolos(firstSymbol, tabs + 1);
                            }
                        }
                    }
                    else
                    {
                        if (Metodos.ContainsKey(name))
                        {
                            Write(name + "()", tabs);
                        }
                        else
                        {
                            throw new Error($"El metodo {name} no está definido.", log, linea, columna);
                        }
                        // Write(name + "();", tabs);
                    }
                }
            }
            else if (Clasificacion == Tipos.ST)
            {
                string name = Contenido;
                match(Tipos.ST);

                if (Clasificacion == Tipos.Optativo || Clasificacion == Tipos.OR)
                {
                    Write($"if(Contenido == \"{name}\")", tabs);
                    Write("{", tabs);
                    Write($"match(\"{name}\");", tabs + 1);
                    Write("}", tabs);
                    if (Clasificacion == Tipos.Optativo)
                    {
                        match(Tipos.Optativo);
                    }
                    else if (Clasificacion == Tipos.OR)
                    {
                        match(Tipos.OR);
                        Write("else", tabs);
                        Write("{", tabs);
                        Write($"match(Tipos.{name});", tabs + 1);
                        Write("}", tabs);
                        if (Clasificacion == Tipos.OR)
                        {
                            Console.WriteLine("Contenido OR: " + Contenido);
                            ListaSimbolos(Contenido, tabs + 1);
                        }
                    }
                }
                else
                {
                    Write($"match(\"{name}\");", tabs);
                }
            }
            else
            {
                match(Tipos.InicioAgrupacion);
                Write("{", tabs);
                int caract = charactercounter;
                int lineact = linea;
                while (Clasificacion != Tipos.CierreAgrupacion || Clasificacion != Tipos.FinProduccion)
                {
                    nextToken();
                }
                // ListaSimbolos(firstSymbol, tabs + 1if);
                
                match(Tipos.CierreAgrupacion);

                if (Clasificacion == Tipos.Optativo)
                {
                    match(Tipos.Optativo);
                    // if (Clasificacion == Tipos.SNT)
                    // {
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caract, SeekOrigin.Begin);
                    charactercounter = caract;
                    linea = lineact;
                    nextToken();

                    // Write($"if (Clasificacion == Tipos.{Contenido})", tabs);
                    Write($"if (Contenido == \"{firstSymbol}\")", tabs);
                    ListaSimbolos(firstSymbol);
                    // }
                    // Write("if (Coneni 
                }
                else
                {
                    Console.WriteLine("Contenido AG: " + Contenido);
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caract, SeekOrigin.Begin);
                    charactercounter = caract;
                    linea = lineact;
                    nextToken();
                    ListaSimbolos(firstSymbol, tabs + 1);
                    match(Tipos.CierreAgrupacion);
                }
                Write("}", tabs);
            }

            if (Clasificacion != Tipos.FinProduccion && Clasificacion != Tipos.CierreAgrupacion)
            {
                ListaSimbolos(firstSymbol, tabs);
            }
        }

        private void Write(string line, int tabs = 0, bool imprime = false)
        {
            for (int i = 0; i < tabs; i++)
            {
                Languaje.Write("\t");
            }
            Languaje.WriteLine(line);

        }
    }
}