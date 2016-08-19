using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class Regla
    {
        private Literal predicado;
        private List<Literal> precondiciones;

        public Literal Predicado
        {
            get { return predicado; }
        }

        public List<Literal> Precondiciones
        {
            get { return precondiciones; }
            set {
                foreach (var lit in value)
                    if (!precondiciones.Contains(lit))
                        precondiciones.Add(lit);
            }
        }

        public Regla(Literal pred)
        {
            predicado = pred;
            precondiciones = new List<Literal>();
        }

        public Regla(Regla cpy)
        {
            predicado = cpy.predicado;
            precondiciones = new List<Literal>(cpy.precondiciones.Select(x => x.Clone()));
        }

        public override string ToString()
        {
            string s = predicado + ":-";

            foreach(var condicion in Precondiciones)
            {
                s += "\n\t" + condicion + ",";
            }
            s = s.Remove(s.Length - 1);
            s += ".";

            return s;
        }
    }
}
