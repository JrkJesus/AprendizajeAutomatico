using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class Algoritmo
    {
        private List<Literal> conocimientoPos;
        private List<Literal> conocimientoNeg;
        private List<Literal> baseConocimiento;
        private HashSet<Literal> predicados;
        private HashSet<string> dominio;
        private string objetivo;

        public Algoritmo( string path, string pred_obj)
        {
            predicados = new HashSet<Literal>();
            dominio = new HashSet<string>();
            baseConocimiento = new List<Literal>();
            conocimientoPos = new List<Literal>();
            conocimientoNeg = new List<Literal>();
            objetivo = pred_obj;
            var file = (from lines in File.ReadAllLines(@path)
                        let line = lines.Split(new[] { "(" }, StringSplitOptions.RemoveEmptyEntries)
                        select new
                        {
                            predicado = line[0],
                            atributos = line[1].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                        }
                        );
            foreach (var lit in file)
            {
                int natt = lit.atributos.Count();
                Literal nuevo = new Literal(lit.predicado, natt);
                predicados.Add(nuevo.Clone()); //para tener la cardinalidad tb
                string[] att = lit.atributos;
                att[natt - 1] = att[natt - 1].Remove(att[natt - 1].Count() - 1);
                nuevo.Atributos = att;

                dominio.UnionWith(att);

                if (lit.predicado == pred_obj)
                    conocimientoPos.Add(nuevo);
                else
                {
                    baseConocimiento.Add(nuevo);
                    //predicados.Add(lit.predicado); //Para recursivo poner fuera del else
                }
            }
            predicados.RemoveWhere(T => T.Nombre == objetivo);
            int n_att = conocimientoPos[0].nAtt;
            foreach (var litNeg in dominio.Repetitions(n_att))
            {
                Literal nuevo = new Literal(pred_obj, n_att);
                nuevo.Atributos = litNeg;
                conocimientoNeg.Add(nuevo);
            }
            conocimientoNeg.RemoveAll(T => conocimientoPos.Contains(T));
        }

        public List<Regla> Foil(bool recursive)
        {
            List<Regla> reglasAprendidas = new List<Regla>();
            bool añadir = recursive;

            while( conocimientoPos.Any() )
            {
                int nAtt_objetivo = conocimientoPos[0].nAtt;
                Literal pred_objetivo = new Literal(objetivo, nAtt_objetivo);
                int ultima_var = nAtt_objetivo;
                string[] atributos = new string[nAtt_objetivo];
                HashSet<string> usados = new HashSet<string>();
                for (int i = 0; i < nAtt_objetivo; i++)
                {
                    atributos[i] = ""+i;
                    usados.Add(""+i);
                }
                pred_objetivo.Atributos = atributos;
                Regla nuevaRegla = new Regla(pred_objetivo);
                List<Literal> negativosAceptados = conocimientoNeg;

                while( negativosAceptados.Any() )
                {
                    List<Literal> candidatos = generarCandidatos(predicados, usados);
                    float p0 = (from positivo in conocimientoPos
                              where cubre(nuevaRegla, positivo)
                              select positivo).Count(),
                         n0 = (from negativo in conocimientoNeg
                              where cubre(nuevaRegla, negativo)
                              select negativo).Count();
                    double mejorGanancia = ganancia(candidatos[0], nuevaRegla, p0, n0);
                    Literal mejorLiteral = candidatos[0];
                    candidatos.RemoveAt(0);
                    foreach(var candidato in candidatos)
                    { 
                        double gan = ganancia(candidato, nuevaRegla, p0, n0);
                        Console.WriteLine(candidato + " -> " + gan);
                        if(gan > mejorGanancia)
                        {
                            mejorGanancia = gan;
                            mejorLiteral = candidato;
                        }
                    }
                    nuevaRegla.Precondiciones = new List<Literal> { mejorLiteral };
                    foreach( var variable in mejorLiteral.Atributos )
                    {
                         usados.Add(variable);
                    }
                    negativosAceptados = (from aceptados in negativosAceptados
                                          where cubre(nuevaRegla, aceptados)
                                          select aceptados).ToList();
                    
                    if (añadir)
                    {
                        predicados.Add(new Literal(objetivo, conocimientoPos[0].nAtt));
                        añadir = false;
                    }
                }
                reglasAprendidas.Add(nuevaRegla);
               
                conocimientoPos = (from faltaAceptar in conocimientoPos
                                   where !cubre(nuevaRegla, faltaAceptar)
                                   select faltaAceptar).ToList();
                Console.WriteLine(nuevaRegla + "\n \t\t -> faltan = " + conocimientoPos.Count());
                predicados.RemoveWhere(T => T.Nombre == objetivo);
                añadir = recursive;
            }

            return reglasAprendidas;
        }

        private List<Literal> generarCandidatos(HashSet<Literal> predicados, HashSet<string> usados)
        {
            List<Literal> candidatos = new List<Literal>();
            foreach(var predicado in predicados)
            {
                int natt_nuevo = Convert.ToInt32(usados.Max());
                HashSet<string> usados_cpy = new HashSet<string>(usados);
                for (int i = 0; i < predicado.nAtt-1; i++)
                {
                    usados_cpy.Add("" + (i+natt_nuevo+1) );
                }
                // añadir natt_nuevo a usados
                foreach (var atributos in usados_cpy.Repetitions(predicado.nAtt))
                {
                    candidatos.Add(new Literal(predicado.Nombre, predicado.nAtt, atributos));
                }
            }
            return candidatos;
        }

        private double ganancia(Literal candidato, Regla nuevaRegla, float p0, float n0)
        {
            /*
             * p0 = numero de ej+ cubiertos por R
             * n0 = numero de ej- cubiertos por R
             * p1 = numero de ej+ cubiertos por R'
             * n1 = numero de ej- cubiertos por R'
             * t = numero ej+ cuebiertos en R tb cubiertos en R'
             * 
             */
            Regla reglaConCandidato = new Regla(nuevaRegla);
            reglaConCandidato.Precondiciones = new List<Literal> { candidato };
            float p1 = (from positivo in conocimientoPos
                    where cubre(reglaConCandidato, positivo)
                    select positivo).Count(),
                n1 = (from negativo in conocimientoNeg
                    where cubre(reglaConCandidato, negativo)
                    select negativo).Count(),
                t = (from positivo in conocimientoPos
                    where cubre(nuevaRegla, positivo) && cubre(reglaConCandidato, positivo)
                    select positivo).Count();
            if (t == 0)
                return 0;
            else
                return  t*( Math.Log(p1/(p1+n1)) - Math.Log(p0 / (p0 + n0)) );
        }

        private bool cubre(Regla nuevaRegla, Literal faltaAceptar)
        {
            Dictionary<string, List<string> > valores_vars = new Dictionary<string, List<string> >();

            for(int i = 0; i<nuevaRegla.Predicado.nAtt; i++)
            {
                valores_vars.Add(nuevaRegla.Predicado.Atributos[i],
                                 new List<string> { faltaAceptar.Atributos[i] });
            }
            foreach( var condicion in nuevaRegla.Precondiciones)
            {
                if (!cubre(condicion, valores_vars))
                    return false;
            }

            return true;
        }

        public bool cubre(Literal condicion, Dictionary<string, List<string> > valores_vars)
        {
            List<string>[] atributos_a_buscar = new List<string>[condicion.nAtt];

            for(int i = 0; i<condicion.nAtt; i++)
            {
                if (valores_vars.ContainsKey(condicion.Atributos[i]))
                    atributos_a_buscar[i] = valores_vars[condicion.Atributos[i]];
                else
                    atributos_a_buscar[i] = new List<string> { "?" };
            }

            var posibles_valores = (from conocimiento in baseConocimiento
                                    where (conocimiento.Nombre == condicion.Nombre)
                                        && completa(conocimiento, atributos_a_buscar)
                                    select conocimiento.Atributos);
            if (posibles_valores.Any())
            {
                for(int i = 0; i<condicion.nAtt; i++)
                {
                    valores_vars[condicion.Atributos[i]] = (from valor in posibles_valores
                                                            select valor[i]).Distinct().ToList();
                }

                return true;
            }
            else
                return false;
        }

        private bool completa(Literal conocimiento, List<string>[] atributos_a_buscar)
        {
            bool esCompletado = true;
            int i = 0;

            while( i<conocimiento.nAtt && esCompletado)
            {
                esCompletado = atributos_a_buscar[i].Contains("?") 
                                || atributos_a_buscar[i].Contains(conocimiento.Atributos[i]);
                i++;
            }

            return esCompletado;
        }
    }
}
