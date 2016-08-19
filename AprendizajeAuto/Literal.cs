using System;

namespace AprendizajeAuto
{
    class Literal
    {
        private string nombre;
        private int nAtributos;
        private string[] atributos;

        public string Nombre
        {
            get { return nombre; }
        }
        public int nAtt
        {
            get { return nAtributos; }
        }
        public string[] Atributos
        {
            get { return atributos; }
            set { if(value.Length == nAtributos) atributos = value; }
        }

        public Literal(string _nombre, int _nAtt)
        {
            nombre = _nombre;
            nAtributos = _nAtt;
        }

        public Literal(string _nombre, int _nAtt, string[] _atributos)
        {
            nombre = _nombre;
            nAtributos = _nAtt;
            if(_atributos != null)
                atributos = (string[])_atributos.Clone();
        }

        internal Literal Clone()
        {
            return new Literal(nombre, nAtt, atributos);
        }

        public override string ToString()
        {
            string s = nombre + "(";
            foreach(var atributo in atributos)
            {
                s += "x"+ atributo + ", ";
            }

            s = s.Remove(s.Length - 2);
            s += ")";

            return s;
        }

        public override bool Equals(object obj)
        {
            if (obj is Literal)
            {
                Literal lit = (Literal)obj;
                if (lit.Nombre == nombre && lit.nAtt == nAtributos)
                {
                    if (atributos != null && lit.atributos != null)
                    {
                        for (int i = 0; i < nAtributos; i++)
                            if (atributos[i] != lit.Atributos[i])
                                return false;
                        return true;
                    }
                    else
                        return atributos == null && lit.atributos == null;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return nAtributos * nombre.GetHashCode() ;
        }
    }
}