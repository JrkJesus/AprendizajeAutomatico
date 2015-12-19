using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class Class1
    {
        private static ConcurrentDictionary<string, int> cd = new ConcurrentDictionary<string, int>();

        public static void generate()
        {

            string[] all_lines =  {  "1, 2, 3" , "4, 5, 6", "4, 2, 3", "4, 2, 6", "9, 8, 7" };


            string[] separator = new string[] { ", " };
            int nAtt = all_lines[0].Split(separator, StringSplitOptions.RemoveEmptyEntries).Count() ;
            List<IEnumerable> dom = new List<IEnumerable>();

            for (int i = 0; i < nAtt; i++)
            {
                var n = i;
                var ele = (from lines in all_lines
                            let data = lines.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            select data.ElementAt(n)).Distinct();
                dom.Add(ele);
            }

            foreach (var row in dom)
            {
                Console.Write("( ");
                foreach (var ele in row)
                    Console.Write("{0} ", ele);
                Console.WriteLine(")");
            }
        }

        public static void generate2()
        {
            string[] all_lines = { "1, 2, 3, a", "4, 5, 6, b", "4, 2, 3, c", "4, 2, 6, d", "9, 8, 7, a" };
            var dom = all_lines
                        .SelectMany(line => line
                                             .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select((x, i) => new { x, i }))
                        .GroupBy(x => x.i)
                        .Select(g => g.Select(gi => gi.x).Distinct())
                        .ToList();
            int k = 0;
            foreach (var row in dom)
            {
                Console.Write("{0}:\t ( ", k++);
                foreach (var ele in row)
                    Console.Write("{0} ", ele);
                Console.WriteLine(")");
            }


            var hip = dom[0];
            dom.RemoveAt(0);
            foreach (var domN in dom)
                hip = hip.SelectMany(domAtt => domN, (d, e) => d + ", " + e);
            k = 0;
            foreach (var row in hip)
            {
                Console.Write("{0}:\t ( ", k++);
                foreach (var ele in row)
                    Console.Write("{0} ", ele);
                Console.WriteLine(")");
            }
        }

        public static void shufflet()
        {
            string[] dom = { "1, 4, 9" ,  "2, 5, 8" , "3, 6, 7", "a, b, c, d", "casa, hogar, home" };
           
            var hip = dom[0].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                            .SelectMany(domAtt => dom[1]
                                                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries),
                                                    (d, e) =>  d+", "+e );

            for(int i = 2; i<4; i++)
            {
                var n = i;
                hip = hip.SelectMany(domAtt => dom[n]
                                                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries),
                                                    (d, e) => d + ", " + e);
            }
            /*

            var hip = from domAtt in dom
                      select (domAtt.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                                .SelectMany(domi => dom[0]
                                                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries),
                                               (d, e) => new { d, e }));
            */
            int j = 0;
            foreach (var row in hip)
            {
                Console.Write("( ");
                // foreach (var ele in row)
                //  Console.Write("{0} {1}", row.d, row.e);
                    Console.Write("{0} ", row);
                Console.WriteLine(")");
                j++;
            }

            Console.WriteLine("Numero de hip = {0}", j);
        }

        public static void dictionary()
        {
            for (int i = 1; i <= 10 + 1; i++)
                cd.AddOrUpdate("Hola"+i, i * i, (k, v) => v / i);
            for (int i = 1; i <= 10 + 1; i++)
                Console.WriteLine(cd["Hola"+i]);
        }

        public static void dictionary2()
        {
            Dictionary<String, Dictionary<String, int>> cd = new Dictionary<string, Dictionary<string, int>>();

            cd["alto"] = new Dictionary<string, int>();
            cd["alto"]["si"] = 0;
            cd["alto"]["no"] = 1;
            cd["alto"]["quizas"] = 2;

            foreach(var key in cd["alto"])
            {
                Console.WriteLine(key);
            }
        }
    }
}
