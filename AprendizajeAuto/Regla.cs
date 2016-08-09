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
        }
    }
}
