using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AprendizajeAuto
{
    class Finds
    {
        public static void train(string file_name, int res, string pos)
        {
            string[] all_lines = File.ReadAllLines(@file_name);
            string[] separator = new string[] { ", " };
            int nAtt = all_lines[0].Split(separator, StringSplitOptions.RemoveEmptyEntries).Count() - 1;
            var training =
                    from lines in all_lines
                    let data = lines.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    where data[res] == pos
                    select data.Where(x => x != pos);

            string[] hipotesis = new string[nAtt];
            for (int i = 0; i < nAtt; i++)
                hipotesis[i] = "0";

            foreach (var ejemplo in training)
            {
                if (!cubre(hipotesis, ejemplo))
                    hipotesis = generarizar(hipotesis, ejemplo);
            }

            Console.Write("( ");
            foreach (var dato in hipotesis)
                Console.Write("{0} ", dato);
            Console.WriteLine(")"); 

        }

        private static string[] generarizar(string[] hipotesis, IEnumerable<string> ejemplo)
        {
            string[] gen = (string[]) hipotesis.Clone();
            var collec = ejemplo.Zip(hipotesis, (x, y) => new { ej = x, hip = y });
            int i = 0;
            foreach(var entry in collec )
            {
                if (entry.ej == entry.hip || entry.hip == "0")
                    gen[i] = entry.ej;
                else if (entry.ej != entry.hip)
                    gen[i] = "?";

                i++;
            }

            return gen;
        }

        private static bool cubre(string[] hipotesis, IEnumerable<string> ejemplo)
        {
            var collec = ejemplo.Zip(hipotesis, (x, y) => new { ej = x, hip = y });

            foreach (var entry in collec)
            {
                if (entry.hip != "?" && entry.hip != entry.ej)
                    return false;
            }

            return true;
        }
    }
}

