using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class DescensoGradiente
    {


        public double[] gradiente(double alpha, double[] x, double[] y, double ep = 0.0001, double max_iter = 10000)
        {
            bool converge = false;
            int i = 0,
                m = x.Count();
            Random rnd = new Random();
            double t0 = rnd.NextDouble()* x[0], 
                t1 = rnd.NextDouble() * x[0];

            var J = x.Zip(y, (a, b) => Math.Pow(t0 + t1 * a - b, 2)).Sum();

            while( i<max_iter && !converge)
            {
                double g0 = 1.0 / m * x.Zip(y, (a, b) => (t0 + t1 * a - b)).Sum(),
                    g1 = x.Zip(y, (a, b) => (t0 + t1 * a - b)*a).Sum();

                t0 = t0 - alpha * g0;
                t1 = t1 - alpha * g1;
                double e = x.Zip(y, (a, b) => Math.Pow(t0 + t1 * a - b, 2)).Sum();

                converge = (Math.Abs(J - e) <= ep);
                J = e;
                i++;
            }

            return new double[] { t0, t1 };
        }
    }
}
