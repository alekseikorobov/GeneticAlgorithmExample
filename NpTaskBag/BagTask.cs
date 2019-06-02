using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpTaskBag
{
    public class Element<T>
    {
        public Element(int index, T v)
        {
            this.Index = index;
            this.Value = v;
        }

        public int Index { get; set; }
        public T Value { get; set; }
    }
    public class Population<T>
    //where T : struct
    {
        public HashSet<int> Indexes { get; set; }
        public List<T> Values { get; set; }
        public T Sum { get; set; }
        public T Diff { get; set; }
    }
    public class BagTask<T>
        where T : IConvertible
    {
        public List<T> AllElements = null;// new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
        public T MaxSize = default(T);// = 85782446.24;
        int sizePopulation = 2048 * 2;
        List<Population<T>> population = new List<Population<T>>();
        Random r_m = new Random();
        Random r_m1 = new Random();
        Random r_mut = new Random();
        private int CountIteration = 1000;
        public T minValue = default(T);

        public void Action()
        {
            /* алгоритм:
             * Создать случайный набор особей — популяцию.
                Подсчитать функцию приспособления для каждой особи.
                Оставить только наиболее приспособленных особей (естественный отбор).
                Произвести скрещивания особей.
                Подвергнуть потомков мутации.
                Продолжить со второго шага.
             */
            ReadArray();

            Init();

            for (int i = 0; i < CountIteration; i++)
            {
                Calculate();
                Sorting();
                Console.WriteLine($"Iteratinon - {i} Best result - {population[0].Diff}");
                if (population[0].Diff.Equals(minValue)) break;
                //Console.WriteLine($"Count Best result - {population.Count(c => c.Diff == 0)}");

                var population_b = GenerateNewPopulation();


                population = population_b;
                //Filter();
            }
            Console.WriteLine("Найдено!");
            Console.WriteLine($"Количество элементов - {population[0].Indexes.Count}");
        }

        public void ReadArray()
        {
            if (AllElements != null) return;

            var line = File.ReadLines("Array.txt");

            line = line.Take(line.Count());
            if (typeof(T) == typeof(Int32))
            {
                AllElements = line.Select(c => (int)Convert.ToDouble(c)).Cast<T>().ToList();
            }
            else if (typeof(T) == typeof(Double))
            {
                AllElements = line.Select(c => Convert.ToDouble(c)).Cast<T>().ToList();
            }
            Console.WriteLine($"Элементов в массиве - {AllElements.Count}");
        }

        //#define GA_POPSIZE		2048		// размер популяции
        //#define GA_MAXITER		16384		// максимальное число итераций
        //#define GA_ELITRATE		0.10f		// элитарность
        private List<Population<T>> GenerateNewPopulation()
        {
            var population_b = new List<Population<T>>(population.Count);

            int esize = 0;// (int)((double)population.Count * 0.01);

            population_b.AddRange(population.Take(esize));


            for (int i = esize; i < population.Count; i++)
            {
                int i1 = r_m1.Next(0, (population.Count - 1) / 2); // rand() % (GA_POPSIZE / 2);
                int i2 = r_m1.Next(0, (population.Count - 1) / 2); //rand() % (GA_POPSIZE / 2);
                //spos = rand() % tsize;

                int indexSeparation1 = r_m.Next(0, population[i1].Indexes.Count - 1);
                int indexSeparation2 = r_m.Next(0, population[i2].Indexes.Count - 1);

                var popNew = new Population<T>();
                var leftPart = population[i1].Indexes.Take(indexSeparation1);//.ToList();
                var rigthPart = population[i2].Indexes.Skip(indexSeparation2);//.ToList();
                //popNew.Indexes = new HashSet<int>();
                //foreach (var item in leftPart)
                //{
                //    popNew.Indexes.Add(item);
                //}
                //foreach (var item in rigthPart)
                //{
                //    popNew.Indexes.Add(item);
                //}
                popNew.Indexes = leftPart.Concat(rigthPart).ToHashSet();
                //foreach (var part in rigthPart)
                //{
                //    if (!leftPart.ContainsKey(part.Key))
                //        leftPart.Add(part.Key, part.Value);
                //}


                //Random
                if (r_mut.NextBool(3))
                    popNew = mutate(popNew);

                population_b.Add(popNew);
            }


            return population_b;
        }

        private Population<T> mutate(Population<T> popNew)
        {
            int indexPos = r_m.Next(0, popNew.Indexes.Count - 1);
            popNew.Indexes.Remove(indexPos);
            int index = 0;
            do
            {
                index = r_m1.Next(0, AllElements.Count);
            } while (popNew.Indexes.Contains(index));

            popNew.Indexes.Add(index);
            return popNew;
        }


        private void Sorting()
        {
            population = population.OrderBy(c => c.Diff).ToList();
        }

        private void Filter()
        {
            var el = population.OrderByDescending(c => c.Diff);
            int deleteCount = el.Count() * 10 / 100;
            el.Take(deleteCount);

            var f = el.First();
            var l = el.Last();
        }

        private void Calculate()
        {
            foreach (var pop in this.population)
            {
                var v = pop.Indexes.Select(c => AllElements[c]);
                //pop.Sum = (T)Convert.ChangeType(pop.Values.Sum<T>(c => Convert.ToInt32(c)), typeof(T));
                int s = 0;
                foreach (var val in v)
                {
                    s += Convert.ToInt32(val);
                }
                pop.Sum = (T)Convert.ChangeType(s, typeof(T));
                pop.Diff = (T)Convert.ChangeType(Math.Abs(Convert.ToInt32(pop.Sum) - Convert.ToInt32(MaxSize)), typeof(T));
            }
        }

        private void Init()
        {
            Console.WriteLine($"Размер популяции - {sizePopulation}");
            for (int i = 0; i < sizePopulation; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(i);
                var bagi = RandomArray();

                while (IsExistsPopulation(bagi))
                {
                    bagi = RandomArray();
                }
                population.Add(new Population<T>()
                {
                    Indexes = bagi,
                    Sum = default(T)
                });
            }
            Console.WriteLine();
        }

        private bool IsExistsPopulation(HashSet<int> bagi)
        {
            var pops = population.Where(c => c.Indexes.Count == bagi.Count);

            if (!pops.Any()) return false;

            foreach (var p in pops)
            {
                for (int i = 0; i < bagi.Count; i++)
                {
                    if (p.Indexes.ElementAt(i) != bagi.ElementAt(i))
                    {
                        return false;
                    }
                }
            }
            return true;
        }



        public HashSet<int> RandomArray(int elementCount = 0)
        {
            int allElementCount = AllElements.Count;

            if (elementCount == 0) elementCount = r_m.Next(1, allElementCount);

            int index = r_m1.Next(0, allElementCount - 1);
            var result = new HashSet<int>();
            for (int i = 0; i < elementCount; i++)
            {
                //result.Add(new Element(index, AllElements[index]));
                result.Add(index);
                do
                {
                    index = r_m1.Next(0, allElementCount);
                } while (result.Contains(index));// Any(c=>c.Index == index));
            }
            return result;
        }
    }

    public static class RandomExtention
    {
        public static bool NextBool(this Random r, int truePercentage = 50)
        {
            return r.NextDouble() < truePercentage / 100.0;
        }
    }
    public static class Extensions
    {
        public static HashSet<T> ToHashSet<T>(
            this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
    }
}
