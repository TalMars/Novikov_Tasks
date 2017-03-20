using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novikov_Task_4
{
    class Program
    {
        //список ссылок
        static List<string> commonLinks = new List<string>();

        //Разреженный строчный формат 
        //матрица смежностей
        static ArrayList values = new ArrayList();
        static ArrayList cols = new ArrayList();
        static ArrayList pointer = new ArrayList();

        //матрица вероятностей
        static ArrayList valuesP = new ArrayList();
        static ArrayList colsP = new ArrayList();
        static ArrayList pointerP = new ArrayList();

        static List<PageRankModel> pageRanksList = new List<PageRankModel>();
        static List<PageRankModel> tempPageRankList = new List<PageRankModel>();

        static void Main(string[] args)
        {
            string pathToFile = @"C:\Users\TalMars\Desktop\Novikov_Task\matrix1.txt";
            string[] lines = File.ReadAllLines(pathToFile);
            commonLinks = lines[0].Split(' ').ToList();
            int sizeMatrix = commonLinks.Count;
            pointer.Add(0);

            int countOutLinks = 0;
            for (int i = 0; i < sizeMatrix; i++)
            {
                pageRanksList.Add(new PageRankModel() { IndexPage = i, PageRank = 1 / (float)sizeMatrix }); //изначально PageRank 1/N

                string[] elements = lines[i + 1].Split(' ');
                for (int j = 0; j < sizeMatrix; j++)
                {
                    int val = int.Parse(elements[j]);
                    if (val != 0)
                    {
                        values.Add(val);
                        cols.Add(j);

                        countOutLinks++;
                    }
                }
                pointer.Add(countOutLinks != 0 ? countOutLinks : 0);
            }

            int countElements = 0;
            pointerP.Add(0);
            //заполнение матрицы вероятностей
            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    if (GetValueFromMatrix(j, i) == 1)
                    {
                        int countOutLink = (int)pointer[j + 1] - (int)pointer[j];
                        if (countOutLink > 0)
                        {
                            valuesP.Add(1 / (float)countOutLink);
                            colsP.Add(j);
                        }
                        else
                        {
                            valuesP.Add(1 / (float)sizeMatrix);
                            colsP.Add(j);
                        }
                        countElements++;
                    }
                }
                pointerP.Add(countElements != 0 ? countElements : 0);
            }

            for (int l = 0; l < 30; l++)
            {
                tempPageRankList = pageRanksList.ToList();
                Parallel.For(0, sizeMatrix, PageRankCalculate);
                pageRanksList = tempPageRankList.ToList();
                Console.Write(l + " Iteration");
                Console.WriteLine();
                //сортировка
                List<PageRankModel> sorted = (from p in pageRanksList
                                              orderby p.PageRank descending
                                              select p).ToList();

                for (int i = 0; i < sizeMatrix; i++)
                {
                    Console.WriteLine(commonLinks[sorted[i].IndexPage]);
                }
                Console.WriteLine("---------------------------------------------------------");
            }

            Console.ReadKey(); //Task 3(only PageRank calculate) = 3 827 msc //Task 4: 3 226 msc !!!With out stream in console
        }

        static void PageRankCalculate(int i)
        {
            float sum = 0;
            int n1 = (int)pointerP[i];
            int n2 = (int)pointerP[i + 1];
            for (int k = n1; k < n2; k++)
            {
                sum += (float)valuesP[k] * pageRanksList[(int)colsP[k]].PageRank;
            }
            if (sum <= float.MaxValue)
                tempPageRankList[i].PageRank = sum;
            else
                tempPageRankList[i].PageRank = float.MaxValue;
            Console.Write(tempPageRankList[i].PageRank + " ");
        }

        static int GetValueFromMatrix(int i, int j)
        {
            int result = 0;
            int n1 = (int)pointer[i];
            int n2 = (int)pointer[i + 1];
            for (int k = n1; k < n2; k++)
            {
                if ((int)cols[k] == j)
                {
                    result = (int)values[k];
                    break;
                }
            }
            return result;
        }
    }
}
