using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novikov_Task_2
{
    class Program
    {
        static List<string> commonLinks = new List<string>();
        static Dictionary<int, string> dictionary = new Dictionary<int, string>();
        
        static int[][] matrixFile;
        static float[][] matrixP;
        static int sizeMatrix = -1;
        
        static int[] countOutLinks;
        static List<PageRankModel> pageRanksList = new List<PageRankModel>();

        static void Main(string[] args)
        {
            string pathToFile = @"C:\Users\TalMars\Desktop\Novikov_Task\matrix1.txt";
            string[] lines = File.ReadAllLines(pathToFile);
            commonLinks = lines[0].Split(' ').ToList();
            sizeMatrix = commonLinks.Count;

            matrixFile = new int[sizeMatrix][];
            matrixP = new float[sizeMatrix][];
            countOutLinks = new int[sizeMatrix];

            for (int i = 0; i < sizeMatrix; i++)
            {
                dictionary.Add(i, commonLinks[i]);
                matrixP[i] = new float[sizeMatrix];

                pageRanksList.Add(new PageRankModel() { IndexPage = i, PageRank = 1 / (float)sizeMatrix });

                matrixFile[i] = new int[sizeMatrix];
                string[] elements = lines[i + 1].Split(' ');
                for (int j = 0; j < sizeMatrix; j++)
                {
                    matrixFile[i][j] = int.Parse(elements[j]);
                }
                countOutLinks[i] = matrixFile[i].Sum();
            }

            //writing matrix P
            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    if (matrixFile[j][i] == 1)
                    {
                        if (countOutLinks[j] > 0)
                            matrixP[i][j] = 1 / (float)countOutLinks[j];
                        else
                            matrixP[i][j] = 1 / (float)sizeMatrix;
                    }
                    //Console.Write(matrixP[i][j] + " ");
                }
                //Console.WriteLine();
            }

            for (int l = 0; l < 30; l++)
            {
                List<PageRankModel> tempPageRankList = pageRanksList.ToList();
                for (int i = 0; i < sizeMatrix; i++)
                {
                    float sum = 0;
                    for (int j = 0; j < sizeMatrix; j++)
                    {
                        sum += matrixP[i][j] * pageRanksList[j].PageRank;
                    }
                    if (sum <= float.MaxValue)
                        tempPageRankList[i].PageRank = sum;
                    else
                        tempPageRankList[i].PageRank = float.MaxValue;
                    Console.Write(tempPageRankList[i].PageRank + " ");
                }
                pageRanksList = tempPageRankList.ToList();
                Console.WriteLine();
                List<PageRankModel> sorted = (from p in pageRanksList
                                  orderby p.PageRank descending
                                  select p).ToList();
                int indexMaxRank = sorted[0].IndexPage;
                Console.WriteLine(l + " MAIN PAGE: " + dictionary[indexMaxRank]);
                Console.WriteLine();
                for(int i = 0; i < sizeMatrix; i++)
                {
                    Console.WriteLine(dictionary[sorted[i].IndexPage]);
                }
                Console.WriteLine("---------------------------------------------------------");
            }

            Console.ReadKey();
        }
    }
}

