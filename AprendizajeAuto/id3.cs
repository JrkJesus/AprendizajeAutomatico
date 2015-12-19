using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace AprendizajeAuto
{
    class id3
    {
        private int colRes,
                    nAtt;
        private string att_elegido,
                       value_att,
                       resultado,
                       default_value;
        private string[] att_name;
        private List<id3> hijos;
        private List<string[]> baseConocimiento;
        private Dictionary< string, Tuple<int, Dictionary<string, int> > >[] dominio;

        public id3(string file_name, int _colRes)
        {
            att_elegido = "";
            value_att = "";
            resultado = "";
            colRes = _colRes;
            hijos = new List<id3>();
            baseConocimiento = new List<string[]>();
            string[] all_lines = File.ReadAllLines(@file_name);
            att_name = all_lines[0].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            nAtt = att_name.Length;
            dominio = new Dictionary<string, Tuple<int, Dictionary<string, int>>>[nAtt];
            for(int i = 0; i < nAtt; i++)
                dominio[i] =  new Dictionary<string, Tuple<int, Dictionary<string, int>>>();
            //all_lines = (string[]) all_lines.Skip(1); 
            foreach (var line in all_lines.Skip(1))
            {
                var ej = line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                baseConocimiento.Add(ej);
                for (int i = 0; i < ej.Length; i++)
                {

                    if (dominio[i].ContainsKey(ej[i]))
                    {
                        if (dominio[i][ej[i]].Item2.ContainsKey(ej[colRes]))
                        {
                            dominio[i][ej[i]].Item2[ej[colRes]]++;
                        }
                        else
                        {
                            dominio[i][ej[i]].Item2[ej[colRes]] = 1;
                        }
                        dominio[i][ej[i]] = Tuple.Create(dominio[i][ej[i]].Item1 + 1, dominio[i][ej[i]].Item2);
                    }
                    else
                    {
                        dominio[i][ej[i]] = Tuple.Create(1, new Dictionary<string, int>());
                        dominio[i][ej[i]].Item2[ej[colRes]] = 1;
                    }
                }
            }
            default_value = dominio[colRes].Keys.ElementAt(0);
        }

        public id3( id3 _padre, List<string[]> _baseConocimiento, 
            Dictionary<string, Tuple<int, Dictionary<string, int>>>[] _dominio,string[] _att_name, string _value, int _colRes)
        {
            att_elegido = "";
            value_att = _value;
            resultado = "";
            default_value = _padre.default_value;
            colRes = _colRes;
            att_name = _att_name;
            baseConocimiento = _baseConocimiento;
            hijos = new List<id3>();
            nAtt = _padre.nAtt - 1;
            dominio = _dominio;
            for(int i = 0; i < dominio.Count(); i++)
            {
                //foreach(var keyAtt in dominio[i].Keys)
                for(int index2 = 0; index2 < dominio[i].Keys.Count(); index2++)
                {
                    var _name = dominio[i].Keys.ElementAt(index2);
                    var keyAtt = dominio[i][_name].Item2.Keys;
                    //foreach(var key in dominio[i][keyAtt].Item2.Keys)
                    for (int j = 0; j < keyAtt.Count(); j++)
                    {
                        
                        var _qwe = keyAtt.ElementAt(j);
                        dominio[i][_name].Item2[_qwe] = 0;
                    }
                    dominio[i][_name] = Tuple.Create(0, dominio[i][_name].Item2);
                }

            }

            var k = 0;
            foreach(var ej in baseConocimiento)
            {
                dominio[k][ej[k]].Item2[ej[colRes]]++;
                dominio[k][ej[k]] = Tuple.Create(dominio[k][ej[k]].Item1 + 1, dominio[k][ej[k]].Item2);
                k++;
            }
        }



        public void train()
        {
            if ( baseConocimiento.Count == 0 || (dominio.Count() == 1 && dominio[0].Keys.Count > 1) )
            {
                resultado = default_value;
            }
            //else if (dominio[colRes].Keys.Count == 1)
            else if((from dom in dominio[colRes].Keys
                     where dominio[colRes][dom].Item1 != 0
                     select dom).Count() == 1)
            {
                //name = "\n" + dominio[colRes].Keys.ElementAt(0) + ":- " + name;
                resultado = dominio[colRes].Keys.ElementAt(0);
            }
            else 
            {
                int indexAtt = mejorGanancia(),
                    columna = colRes;
                if (indexAtt < colRes)
                    columna--;
                att_elegido = att_name[indexAtt];
                var dom = dominio[indexAtt].Keys;
                var new_dom = dominio.Take(indexAtt).Concat( dominio
                                                                .Reverse()
                                                                .Take(nAtt - indexAtt - 1)
                                                                .Reverse() ).ToArray();
                var i = 0;
                foreach (var child_name in dom)
                {

                    hijos.Add( new id3(this,
                                        (from ej in baseConocimiento
                                               where ej[indexAtt] == child_name
                                               select ej.Take(indexAtt).Concat(ej.Reverse()
                                                                                 .Take(nAtt - indexAtt - 1)
                                                                                 .Reverse()).ToArray()).ToList(),
                                        new_dom,
                                        att_name.Take(indexAtt).Concat(att_name.Reverse()
                                                                               .Take(nAtt - indexAtt - 1)
                                                                               .Reverse()).ToArray(),
                                        child_name,
                                        columna  ) );
                    hijos[i++].train();
                }
            }
           //return true;
        }

        public override string ToString()
        {
            
            if (hijos.Count == 0)
            {
                //return  "\n" + name + "-> ";
                return "\n" + resultado + " -> ";
            }
            else
            {
                string reglas = "";
                foreach( var child in hijos)
                {
                    //reglas += child + name; // name + child
                    foreach(var rule in child.ToString().Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                        reglas += "\n"+ rule + "( " + att_elegido + " = " + child.value_att + ") ";
                }
                return reglas;
            }
        }

        private int mejorGanancia()
        {
            int pos = -1;
            double valor = int.MaxValue;
            for(int i = 0; i < nAtt; i++)
            {
                if (i != colRes)
                {
                    double valorAtt = entropiaAtt(i);
                    if (valorAtt < valor)
                    {
                        valor = valorAtt;
                        pos = i;
                    }
                }
            }
            return pos;
        }

        private double entropiaAtt(int i)
        {
            double valor = 0;
            var keyAtt = dominio[i].Keys;
            foreach ( var key in keyAtt)
            {
                double repe_valor = dominio[i][key].Item1;
                if(repe_valor != 0)
                    valor += (repe_valor / (double) baseConocimiento.Count) * entropiaValue(i, key);
            }
            return valor;
        }

        private double entropiaValue(int i, string key)
        {
            double valor = 0;
            var keyRes = dominio[i][key].Item2.Keys;
            foreach(var keyV in keyRes)
            {
                double value = dominio[i][key].Item2[keyV] / (double) dominio[i][key].Item1;
                if(value != 0)
                    valor += value * Math.Log(value, 2.0);
             }
            return -valor;
        }
    }
}
