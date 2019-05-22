using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library;

namespace CAEvulutionA
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int i, j, s;
            double minRandom;
            double[] result = new double[Constants.SIZE];
            double[,] matrix =
                {
                {1, 2, 3, 11, 4}, // row 0 values
                {5, 6, 10,7, 8}, // row 1 values
                {12,5, 6, 7, 8},
                {17,5, 6, 7, 8},
                {5, 6, 7, 8,31},
             };
            Matrix obj = new Matrix(matrix);
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    Console.Write(matrix[row, col]+" ");
                }
                Console.WriteLine();
            }
            // Print the matrix on the console
            
            //obj.initGraph();
            // Reshenie s generirane na proizvolni cikli 
            
            minRandom =( obj.GetRow() ) * 101;
            
            for (s = 0; s < Constants.S; s++)
            {
                for (i = 0; i < Constants.SIZE; i++)
                {
                    //Console.WriteLine("I=   "+i);
                    obj.randomCycle(i);
                    result[i] = obj.evaluate(i);
                }
                for (j = 0; j < Constants.SIZE; j++) if (result[j] < minRandom) minRandom = result[j];
                for (i = 10; i > 0; i--) Console.WriteLine("s= "+s+"  " + result[i]+" ");
            }
            Console.WriteLine("Optimalno reshenie sled proizvolni 3*SIZE cikyla: \n"
                + minRandom); 
            // Reshenie s genetichen algoritym sys syshtiq broj iteracii 
            for (s = 0; s < Constants.S; s++)  obj.reproduce(s,result);
            Console.WriteLine("Naj-kysi cikli, namereni ot genetichniya algoritym: \n");
            for (i = 10; i > 0; i--) Console.WriteLine(" " + result[i]);
            Console.WriteLine(" Kraen resultat" + result[0]); 
          
        }
        
    }
}
namespace Library
{
        static class Constants
        {
            public const int N = 0;
            public const int M = 0;
            public const int SIZE = 16;
            public const int S = 3;
        }
        public class Matrix : ICloneable, IComparable<Matrix>
        {
            private readonly double[,] data;
            int[,] population = new int[Constants.SIZE, 5]; /* cikli na populaciqta */
            double[] result = new double[Constants.SIZE];
            Random random = new Random();

            public Matrix(int n, bool diagonal = false)
            {
                data = new double[n, n];
                if (!diagonal) return;
                for (int i = 0; i < n; i++)
                {
                    data[i, i] = 1.0;
                }
            }
            public Matrix(int n, int m, Random r = null)
            {
                data = new double[n, m];
                if (r == null) return;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        data[i, j] = r.NextDouble();
                    }
                }
            }
            public Matrix(double[,] data)
            {
                this.data = data;
            }

            public void randomCycle(int t)
            {
                int[] used = new int[5];
                int i;
                for (i = 0; i < data.GetLength(0); i++) used[i] = 0;
                for (i = 0; i < data.GetLength(0); i++)
                {
                    int p = random.Next(1, data.GetLength(0) - i);
                    int j = 0;
                    while (p > 0)
                    {
                        while (used[j] != 0) j++;
                        p--; j++;
                    }
                    population[t, i] = j - 1;
                    used[j - 1] = 100;
                }
            }

            public double evaluate(int t)
            {

                double res = 0;
                int i;
                for (i = 0; i < data.GetLength(0) - 1; i++)
                    res += data[population[t, i], population[t, i + 1]]; ////zapisva ototdolu
                return res + data[population[t, (data.GetLength(0) - 1)], population[t, 0]];

            }

            public void combine(int p1, int p2, int q1, int q2)
            {
                int[] uq1 = new int[5];
                int[] uq2 = new int[5];
                int i, k, j;
                Random random = new Random();
                /* generira naslednici q1,q2 ot roditelite p1,p2 */
                k = random.Next(1, data.GetLength(0) - 1);  /* razmyana na poziciya k */
                for (i = 0; i < data.GetLength(0); i++)
                {
                    uq1[i] = 0;
                    uq2[i] = 0;
                }
                for (i = 0; i < k; i++)
                {
                    population[q1, i] = population[p1, i];
                    uq1[population[p1, i]]++;
                    population[q2, i] = population[p2, i];
                    uq2[population[p2, i]]++;
                }
                for (i = k; i < data.GetLength(0); i++)
                {
                    if (uq1[population[p2, i]] == 0)
                    {
                        population[q1, i] = population[p2, i];
                        uq1[population[p2, i]]++;
                    }
                    else
                    {
                        for (j = 0; uq1[j] != 0; j++) ;
                        population[q1, i] = j;
                        uq1[j]++;
                    }
                    if (uq2[population[p1, i]] == 0)
                    {
                        population[q2, i] = population[p1, i];
                        uq2[population[p1, i]]++;
                    }
                    else
                    {
                        for (j = 0; uq2[j] != 0; j++) ;
                        population[q2, i] = j;
                        uq2[j]++;
                    }
                }
                result[q1] = evaluate(q1);
                result[q2] = evaluate(q2);
            }

            public void mutate()
            {
                int i, k;
                /* ako se poluchat dwe ednakvi poredni resheniq - ednoto "mutira" */
                for (i = 0; i < Constants.SIZE - 1; i++)
                {
                    int flag = 0;
                    for (k = 0; k < data.GetLength(0); k++)
                        if (population[i, k] != population[i + 1, k])
                        {
                            flag = 1;
                            break;
                        }

                    if (flag != 0)
                    {  /* cikyl i mutira */
                        int p1 = random.Next(1, data.GetLength(0));
                        int p2 = random.Next(1, data.GetLength(0));
                        int swap = population[i, p1];
                        population[i, p1] = population[i, p2];
                        population[i, p2] = swap;
                        result[i] = evaluate(i);
                    }
                }
            }
            public int GetRow()
            {
                return data.GetLength(0);
            }
            public void reproduce(int s, double[] result)
            {
                int i, j, k;
                int swap;
                this.result = result;
                /* zamestva naj-loshite cikli kato kombinira proizvolni
                * ot pyrvata polovina
                */

                for (i = 0; i < (Constants.SIZE - 1) / 2; i++)
                {
                    
                    //randomCycle(i);
                    combine(i, i + 1, Constants.SIZE - i - 1, Constants.SIZE - i - 2);
                    result[i] = evaluate(i);
                }
                
                /* sortira populaciyata po optimalnost */
                for (i = 0; i < Constants.SIZE - 1; i++)
                {
                    for (j = i + 1; j < Constants.SIZE; j++)
                    {
                        if (result[j] < result[i])
                        {
                            for (k = 0; k < data.GetLength(0); k++)
                            {
                                swap = population[i, k];
                                population[i, k] = population[j, k];
                                population[j, k] = swap;
                            }
                            double swap2;
                            swap2 = result[i];
                            result[i] = result[j];
                            result[j] = swap2;
                        }
                    }
                }
                if (s == Constants.S - 1) return;
                mutate();
            }
            public object Clone()
            {
                return new Matrix((double[,])data.Clone());
            }
            public int CompareTo(Matrix other)
            {
                /*if (Constants.N != other.(Constants.N) || Constants.M != other.(Constants.M) )
                {
                    return -1;
                }*/
                for (int i = 0; i < Constants.N; i++)
                {
                    for (int j = 0; j < Constants.M; j++)
                    {
                        //if (Math.Abs(data[i, j] - other[i, j]) > 0.0000000001) return -1;

                    }
                }
                return 0;
            }
    }
}
