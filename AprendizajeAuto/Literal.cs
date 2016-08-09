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
    }
}