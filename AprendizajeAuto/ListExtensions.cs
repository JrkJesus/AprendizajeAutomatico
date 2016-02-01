using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeAuto
{
    public static class ListExtensions
    {
        public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            List<T[]> result = new List<T[]>();

            if (k == 0)
            {
                // single combination: empty set
                result.Add(new T[0]);
            }
            else
            {
                int current = 1;
                foreach (T element in elements)
                {
                    // combine each element with (k - 1)-combinations of subsequent elements
                    result.AddRange(elements
                        .Skip(current++)
                        .Combinations(k - 1)
                        .Select(combination => (new T[] { element }).Concat(combination).ToArray())
                        );
                }
            }

            return result;
        }

        public static IEnumerable<T[]> Repetitions<T>(this IEnumerable<T> elements, int k)
        {
            List<T[]> result = new List<T[]>();

            if (k == 0)
            {
                result.Add(new T[0]);
            }
            else
            {
                foreach (T element in elements)
                {
                    result.AddRange(elements
                        .Repetitions(k - 1)
                        .Select(repetition => (new T[] { element }).Concat(repetition).ToArray())
                        );
                }
            }

            return result;
        }

        public static IEnumerable<string[]> Permutations(this IEnumerable<string> elements, int k)
        {
            List<string[]> result = new List<string[]>();

            if (k == 0)
            {
                result.Add(new string[0]);
            }
            else
            {
                foreach (string element in elements)
                {
                    result.AddRange( (from ele in elements
                                      where ele != element
                                      select ele)
                        .Permutations(k - 1)
                        .Select(permutation => (new string[] { element }).Concat(permutation).ToArray())
                       
                        );
                }
            }

            return result;
        }
    }
}
