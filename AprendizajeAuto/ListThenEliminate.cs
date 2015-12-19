using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class ListThenEliminate
    {
        private int nAtt,
                    colRes;
        private string valueRes;
        private List<string[]> reglas;
        private List<string[]> positivos, negativos;
        private HashSet<string>[] dominio;

        public ListThenEliminate(string @file_name, int _colRes, string _valueRes)
        {
            colRes = _colRes;
            valueRes = _valueRes;
            string[] all_lines = File.ReadAllLines(@file_name);
            var baseConocimiento = from ejemplo in all_lines.Skip(1)
                                    let ej = ejemplo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                                    select ej;
            nAtt = baseConocimiento.ElementAt(0).Count();
            positivos = (from ej in baseConocimiento
                         where ej[colRes] == valueRes
                         select ej.Take(colRes).Concat(ej
                                                    .Reverse()
                                                    .Take(nAtt - colRes - 1)
                                                    .Reverse()).ToArray()).ToList();

            negativos = (from ej in baseConocimiento
                         where ej[colRes] != valueRes
                         select ej.Take(colRes).Concat(ej
                                                    .Reverse()
                                                    .Take(nAtt - colRes - 1)
                                                    .Reverse()).ToArray()).ToList();
            dominio = new HashSet<string>[nAtt-1];
            for (int i = 0; i < nAtt - 1; i++)
                dominio[i] = new HashSet<string>();
            foreach (var ejemplo in baseConocimiento)
            {
                for (int i = 0; i < nAtt - 1; i++)
                    dominio[i].Add(ejemplo[i]);
            }
            int n = 1;
            foreach (var dom in dominio)
            {
                n *= dom.Count + 1;
            }
            reglas = new List<String[]>(n);
        }

        public void train()
        {
            int n = 1;
            foreach (var dom in dominio)
            {
                n *= dom.Count+1;
            }
            Console.WriteLine(n);
            string[] gen = new string[nAtt];
            for (int i = 0; i < nAtt - 1; i++)
            {
                gen[i] = "?";
            }
            reglas = genHip(gen, 0);
            Console.WriteLine(reglas.Count+"   "+(n == reglas.Count));
            foreach(var ejemplo in positivos)
            {
                reglas = (from rule in reglas
                          where cubre(rule, ejemplo)
                          select rule).ToList();
            }
            foreach (var ejemplo in negativos)
            {
                reglas = (from rule in reglas
                          where !cubre(rule, ejemplo)
                          select rule).ToList();
            }
        }

        public List<string[]> genHip(string[] hip, int att)
        {
  
            List<string[]> _reglas = new List<string[]>();
            _reglas.Add(hip);
            for(int i = att; i < nAtt - 1; i++)
            {
                foreach(var dom in dominio[i])
                {
                    string[] newHip = (string[]) hip.Clone();
                    newHip[i] = dom;
                    _reglas.AddRange(genHip(newHip, i + 1));
                }
            }
            return _reglas;

        }

        public List<string[]> genHip2()
        {
            List<string[]> reglas = new List<string[]>();
            string[] hip = new string[nAtt];
            for (int i = 0; i < nAtt - 1; i++)
            {
                hip[i] = "?";
            }
            reglas.Add((string[])hip.Clone());
            for (int att = 0; att < nAtt - 1; att++)
            {
                for (int i = att; i < nAtt - 1; i++)
                {
                    foreach (var dom in dominio[i])
                    {
                        string[] newHip = (string[])hip.Clone();
                        newHip[i] = dom;
                        reglas.AddRange(genHip(newHip, i + 1));
                    }
                }
            }
            return reglas;

        }

        private static bool cubre(string[] hip, IEnumerable<string> ej)
        {
            var collec = ej.Zip(hip, (x, y) => new { ej = x, hip = y });

            foreach (var entry in collec)
            {
                if (entry.hip != "?" && entry.hip != entry.ej)
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            string rules = "";
            foreach(var rule in reglas)
            {
                rules += "( ";
                foreach(var att in rule)
                {
                    rules += att + " ";
                }
                rules += ")\n";
            }
            return rules;
        }
    }
}
