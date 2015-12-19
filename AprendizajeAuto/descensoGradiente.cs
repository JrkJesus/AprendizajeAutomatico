using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class descensoGradiente
    {
        public void algorith()
        {

        }

        public static void leer()
        {
            var all_lines = File.ReadAllLines(@"ex1data1.txt");
            var values =
                   from lines in all_lines
                   let data = lines.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                   select new { x = double.Parse(data[0]), y = double.Parse(data[1]) };

            double theta0 = 0,
                theta1 = 0,
                alpha = -0.1;
            
            for(int i = 0; i<10000; i++)
            {
                theta0 += alpha * (from dat in values
                                   select dat.y + theta0 + dat.x * theta1).Sum();
                theta1 += alpha * (from dat in values
                                   select dat.y + theta0 + dat.x * theta1).Sum();
            }
            


            foreach (var dato in values)
                Console.WriteLine("( {0} - {1} )", dato.x, dato.y);
        }
    }
}
