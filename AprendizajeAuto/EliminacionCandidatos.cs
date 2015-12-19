using AprendizajeAuto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class EliminacionCandidatos
    {
        private int nAtt, 
                    colRes;
        private string valueRes;
        private string[] especifica;
        private List<string[]> general;
        private List<string[]> baseConocimiento;
        private HashSet<string>[] dominio;
        
        public EliminacionCandidatos(string @file_name, int _colRes, string _valueRes)
        {
            colRes = _colRes;
            valueRes = _valueRes;
            general = new List<String[]>();
            string[] all_lines = File.ReadAllLines(@file_name);
            baseConocimiento = (from ejemplo in all_lines.Skip(1)
                                let ej = ejemplo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                                select ej).ToList();
            nAtt = baseConocimiento.ElementAt(0).Count();
            dominio = new HashSet<string> [nAtt];
            for (int i = 0; i < nAtt; i++)
                dominio[i] = new HashSet<string> ();
            foreach(var ejemplo in baseConocimiento)
            {
                for (int i = 0; i < nAtt; i++)
                    dominio[i].Add(ejemplo[i]);
            }
            especifica = new string[nAtt - 1];
            string[] more_gen = new string[nAtt];
            for(int  i = 0; i < nAtt - 1; i++)
            {
                especifica[i] = "0";
                more_gen[i] = "?";   
            }
            general.Add(more_gen);
        }
           
        public List<string[]> train()
        {
            foreach(var ejemplo in baseConocimiento)
            {
                if(ejemplo[colRes] == valueRes)
                {
                    general = (from gen in general
                               where cubre(gen, ejemplo)
                               select gen).ToList();
                    if (!cubre(especifica, ejemplo))
                        especifica = generarizar(especifica, ejemplo);
                        
                }
                else
                {

                    /*if (cubre(especifica, ejemplo))
                        */ // -> eliminar S cualquier hip inconsitente! 
                                                            // TODO: mirar el caso en el que S inconsistente con Ej

                    /*
                    foreach (var gen in general)
                    {
                        if (!cubre(gen, ejemplo))
                        {
                            general.Remove(gen);

                            var hip_valida = from hip in especializar(gen, ejemplo)
                                             where !masEspecifica(especifica, hip)
                                             select hip;
                            foreach(var hip in hip_valida)
                            {
                                foreach(var hipGen in general)
                                {
                                    if(masEspecifica(hipGen, hip))
                                    {
                                        general.Remove(hipGen);
                                        general.Add(hip);
                                    }
                                }
                            }
                        }
                    }
                    */
                    general = (from gen in general
                               where !cubre(gen, ejemplo)
                               select gen).ToList();
                    general.AddRange((from nueva in (from gen in general
                                                        where cubre(gen, ejemplo)
                                                        select gen).Zip(general, (x, y) => new { hipGen = x, hip = y})
                                         where masEspecifica(nueva.hipGen, nueva.hip)
                                         select nueva.hipGen).ToList());
                }
                if (general.Count == 1 && general.ElementAt(0) == especifica)
                    return general;
            }
            general.Add(especifica);
            return general;
        }

        private bool masEspecifica(string[] especifica, string[] hip)
        {
            foreach (var att in especifica.Zip(hip, (x, y) => new { esp = x, hipo = y })) 
            {
                if (att.esp != att.hipo && att.hipo != "?")
                    return false;
            }
            return true;
        }

        public List<string[]> especializar(string[] hipotesis, string[] ejemplo)
        {
            List<string[]> espec = new List<string[]>();
            for(int i = 0; i < nAtt - 1; i++)
            {
                if (hipotesis[i] == "?")
                {
                    foreach (var key in dominio[i])
                    {
                        if (key != ejemplo[i])
                        {
                            string[] nuevo = (string[]) hipotesis.Clone();
                            nuevo[i] = key.ToString();
                            espec.Add(nuevo);
                        }
                    }
                }
            }
            return espec;
        }

        private string[] generarizar(string[] hipotesis, IEnumerable<string> ejemplo)
        {
            string[] gen = (string[])hipotesis.Clone();
            var collec = ejemplo.Zip(hipotesis, (x, y) => new { ej = x, hip = y });
            int i = 0;
            foreach (var entry in collec)
            {
                if (entry.ej == entry.hip || entry.hip == "0")
                    gen[i] = entry.ej;
                else if (entry.ej != entry.hip)
                    gen[i] = "?";

                i++;
            }

            return gen;
        }

        private bool cubre(string[] hipotesis, IEnumerable<string> ejemplo)
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
