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
        private HashSet<string> predicados;
        private HashSet<string> dominio;

        public Algoritmo( string path, string pred_obj)
        {
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
                string[] att = lit.atributos;
                att[natt - 1] = att[natt - 1].Remove(att[natt - 1].Count() - 1);
                nuevo.Atributos = att;

                dominio.UnionWith(att);

                if (lit.predicado == pred_obj)
                    conocimientoPos.Add(nuevo);
                else
                {
                    baseConocimiento.Add(nuevo);
                    predicados.Add(lit.predicado); //Para recursivo poner fuera del else
                }
            }
            int n_att = conocimientoPos.ElementAt(0).nAtt;
            foreach (var litNeg in dominio.Repetitions(n_att))
            {
                Literal nuevo = new Literal(pred_obj, n_att);
                nuevo.Atributos = litNeg;
            }
        }
    }
}
