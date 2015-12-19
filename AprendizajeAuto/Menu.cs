using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    class Menu
    {

        static void Main(string[] args)
        {
            /********** FIND S *****************
            Console.Write("Finds con finds.csv: ");
            Finds.train("finds.csv", 4, "+");
            Console.Write("Finds con adult.train.10k.discrete_2.csv: ");
            Finds.train("adult.train.10k.discrete_2.csv", 8, ">50K");


            /***** LIST THEN ELIMINATE *************//*
            Console.Write("Finds con finds.csv: ");
            ListThenEliminate.train("finds.csv", 4, "+");
            Console.Write("Finds con adult.train.10k.discrete_2.csv: ");
            ListThenEliminate.train("adult.train.10k.discrete_2.csv", 8, ">50K");


            /***** ESPACIO VERSIONES *************/
            //EspacioVersiones.train("finds.csv", 4, "+");
            //EspacioVersiones.train("adult.train.10k.discrete_2.csv", 8, ">50K");

            //Class1.generate2();

            //Console.WriteLine();
            //Console.WriteLine();

            //Class1.shufflet();

            //descensoGradiente.leer();
            /*
            Console.WriteLine();
            EliminacionCandidatos ecFinds = new EliminacionCandidatos("finds.csv", 4, "+");
            List<string[]> x = ecFinds.train();
            Console.Write("Eliminacion de candidatos con finds.csv: ");
            foreach (var y in x)
            {
                Console.Write("\t(");
                foreach (var z in y)
                {
                    Console.Write(" {0}", z);
                }
                Console.WriteLine(" )");
            }

            EliminacionCandidatos pr = new EliminacionCandidatos("eliminaCandidatos_Animales.csv", 5, "+");
            List<string[]> x1 = pr.train();
            Console.Write("Eliminacion de candidatos con eliminaCandidatos_Animales.csv: ");
            foreach (var y in x1)
            {
                Console.Write("\t(");
                foreach (var z in y)
                {
                    Console.Write(" {0}", z);
                }
                Console.WriteLine(" )");
            }

            EliminacionCandidatos pr2 = new EliminacionCandidatos("adult.train.10k.discrete_2.csv", 8, ">50K");
            List<string[]> x2 = pr2.train();
            Console.Write("Eliminacion de candidatos con adult.train.10k.discrete_2.csv: ");
            foreach (var y2 in x2)
            {
                Console.Write("\t(");
                foreach (var z2 in y2)
                {
                    Console.Write(" {0}", z2);
                }
                Console.WriteLine(" )");
            }

            Console.WriteLine();
            
            id3 cancer = new id3("id3-cancer.csv", 4);
            cancer.train();
            Console.WriteLine(cancer);
            */      
            id3 finds = new id3("finds.csv", 4);
            finds.train();
            Console.WriteLine(finds);
            /*
            id3 adult_10k = new id3("adult.train.10k.discrete_2.csv", 8);
            adult_10k.train();
            Console.WriteLine(adult_10k);
            
             */
             /*
            ListThenEliminate a = new ListThenEliminate("finds.csv", 4, "+");
            a.train();
            Console.WriteLine(a);
            
            ListThenEliminate b = new ListThenEliminate("adult.train.10k.discrete_2.csv", 8, ">50K");
            b.train();
            Console.WriteLine(b);
            
    */
            Console.WriteLine("------ FIN --------");
            Console.ReadKey();
        }
    }
}
