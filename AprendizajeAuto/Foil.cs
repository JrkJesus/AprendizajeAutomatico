using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class Foil
    {
        Dictionary<string, List<string[]>> hechos;
        HashSet<string> dominio;

        public Foil(string file_name, string ruta_swipl)
        {
            Environment.SetEnvironmentVariable("SWI_HOME_DIR", @ruta_swipl);
            Environment.SetEnvironmentVariable("Path", @ruta_swipl);
            Environment.SetEnvironmentVariable("Path", @ruta_swipl + "\\bin");

            hechos = new Dictionary<string, List<string[]>>();
            dominio = new HashSet<string>();

            var all_file = File.ReadAllLines(@file_name);

            var file = (from lines in all_file
                        let line = lines.Split(new[] { "(" }, StringSplitOptions.RemoveEmptyEntries)
                        select new
                                {
                                    predicado = line[0],
                                    resto = line[1]
                                });
            foreach(var line in file)
            {
                var args = line.resto.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                int n_args = args.Count() - 1;
                args[n_args] = args[n_args].Remove(args[n_args].Count() - 1);

                //args[args.Count() - 1] = args.Last().Replace(')', ' ');

                foreach (var arg in args)
                {
                    dominio.Add(arg);
                }
                if(hechos.ContainsKey(line.predicado))
                {
                    hechos[line.predicado].Add(args);
                }
                else
                {
                    List<string[]> new_entry = new List<string[]>();
                    new_entry.Add(args);
                    hechos.Add(line.predicado, new_entry);
                }
            }
        }

        public void aprender()
        {
            foreach(var key in hechos.Keys)
            {
                aprenderPredicado(key);
            }
        }

        public void aprenderPredicado(string predicado)
        {
            int n_pred = hechos[predicado].First().Count();
            List<string> reglas = new List<string>();
            List<string[]> positivos = hechos[predicado];
            List<string[]> negativos = dominio.Repetitions(n_pred).ToList();
                                        
            for(int j = 0; j<negativos.Count(); j++)
            {
                var npred = negativos.ElementAt(j);
                foreach(var ppred in positivos)
                {
                    bool iguales = true;
                    int i = 0;
                    while (iguales && i < n_pred)
                        iguales = npred[i] == ppred[i++];
                    if (iguales)
                    {
                        negativos.RemoveAt(j--);
                    }
                }
            }
            
         
            string[] pred_var = new string[n_pred];
            for (int i = 0; i < n_pred; i++)
            {
                pred_var[i] = "X" + i ;
            }
            string pred_formal = predicado+"( ";
            foreach (var variable in pred_var)
            {
                pred_formal += variable + ", ";
            }
            pred_formal = pred_formal.Remove(pred_formal.Count() - 2);
            pred_formal += ")";
            #region Calculo datos
            List<string> restrinciones = new List<string>();
            foreach (var pred in (from key in hechos.Keys
                                  where key != predicado
                                  select key))
            {
                int n_arg = hechos[pred].First().Count();
                string[] variables = new string[n_arg + n_pred - 1];
                for (int i = 0; i < n_arg + n_pred - 1; i++)
                {
                    variables[i] = "X" + i;
                }
                var x = variables.Permutations(n_arg);

                foreach (var combi_var in x)
                {
                    string nuevo_predicado = pred + "( ";
                    foreach (var variable in combi_var)
                    {
                        nuevo_predicado += variable + ", ";
                    }
                    nuevo_predicado = nuevo_predicado.Remove(nuevo_predicado.Count() - 2);
                    nuevo_predicado += ")";
                    restrinciones.Add(nuevo_predicado);
                }
            }
            #endregion
            double[] gan = new double[] { positivos.Count(), negativos.Count(), 0 };

            do
            {
                double ganMax = 0;
                string predMejor = "";
                double[] ganMejor = new double[] { 0, 0, 0 };
               
                
                foreach (var restrincion in restrinciones)
                {
                    var aux = ganancia(reglas, pred_formal, restrincion, listar(positivos, negativos, predicado), gan[0], gan[1]);
                    //Console.WriteLine(restrincion + "( "+ aux[0] + ", " + aux[1] + " ): "+ aux[2]);
                    if (aux[2] > ganMax)
                    {
                        ganMax = aux[2];
                        predMejor = restrincion;
                        ganMejor = aux;
                       
                    }
                   
                }
                //Console.WriteLine(predMejor);
                gan = ganMejor;
                reglas.Add(predMejor);
                
            } while (gan[1] != 0);
            
            Console.WriteLine(pred_formal+":-");
            foreach(var regla in reglas)
            {
                Console.WriteLine("\t"+regla);
            }
            Console.WriteLine();
            
        }

        private double[] ganancia(List<string> reglas, string predicado, string nuevaRegla, string lista, double p, double n)
            
        {
            double[] ganancia = new double[] { 0, 0, 0 };
            if (!PlEngine.IsInitialized)
            {
                String[] param = { "-q" };  // suppressing informational and banner messages
                PlEngine.Initialize(param);

                foreach (var pred in hechos.Keys)
                {
                    foreach (var args in hechos[pred])
                    {
                        string predic_prolog = "assert(" + pred + "(";
                        foreach (var arg in args)
                        {
                            predic_prolog += arg + ", ";
                        }
                        predic_prolog = predic_prolog.Remove(predic_prolog.Count() - 2);
                        predic_prolog += ")).";
                        PlQuery.PlCall(predic_prolog);
                    }
                }
                string clausura;

                #region positivo              
                    clausura = "assert(existe(1, 0, " + predicado + "):-  (";
                    foreach (var one_regla in reglas)
                    {
                        clausura += one_regla + ", ";
                    }
                    clausura += nuevaRegla + " ))";
                  
                    PlQuery.PlCall(clausura);
                #endregion

                #region negativo              
                    clausura = "assert(existe(0, 1, neg"+predicado+"):-  (";
                    foreach (var one_regla in reglas)
                    {
                        clausura += one_regla + ", ";
                    }
                    clausura += nuevaRegla + " ))";
                    
                    PlQuery.PlCall(clausura);
                #endregion

                #region recursividad              
                
                PlQuery.PlCall("assert(existe(P, N, [Cab | R]):- "+
                                        "(existe(Pos, Neg, Cab),"+
                                        " existe(Positivo, Negativo, R),"+
                                        " P is Pos + Positivo,"+
                                        " N is Neg + Negativo))");
                try {
                    PlQuery.PlCall("assert(existe(P, N, [ Cab | R ]):-	(\\+ existe(_, _, Cab), existe(P, N, R)))");
                    #endregion
                }
                catch (Exception e) { Console.WriteLine(e); }

                PlQuery.PlCall("assert(existe( 0 , 0 , []))");
                var solu = new PlQuery("existe(Positivos, Negativos, " + lista + ").");

                foreach (PlQueryVariables v in solu.SolutionVariables)
                {
                    
                    ganancia[1] = int.Parse(v["Negativos"].ToString());
                    ganancia[0] = int.Parse(v["Positivos"].ToString());
                }
                // ganancia = t * [ lg2( pr'/(pr'+nr') ) -  lg2( pr/(pr+nr) ) ]
                PlEngine.PlCleanup();
                if(ganancia[0] == 0)
                    ganancia[2] = 0;
                else if(p == 0)
                    ganancia[2] = ganancia[0] * Math.Log(ganancia[0] / (ganancia[0] + ganancia[1]), 2.0);
                else
                    ganancia[2] = ganancia[0] * ( Math.Log(ganancia[0] / (ganancia[0] + ganancia[1]), 2.0) - Math.Log(p/(p + n), 2.0) );
            }
            return ganancia;
        }

        private string listar(List<string[]> positivos, List<string[]> negativos,string predicado)
        {
            string lista = "[";
            foreach (var pred in positivos)
            {
                lista += predicado + "(";
                foreach (var arg in pred)
                {
                    lista += arg + ", ";
                }
                lista = lista.Remove(lista.Count() - 2);
                lista += "),";
            }

            foreach (var pred in negativos)
            {
                lista += " neg"+ predicado + "(";
                foreach (var arg in pred)
                {
                    lista += arg + ", ";
                }
                lista = lista.Remove(lista.Count() - 2);
                lista += "),";
            }
            lista = lista.Remove(lista.Count() - 1);
            lista += " ]";
            return lista;
        }

       
    }
}