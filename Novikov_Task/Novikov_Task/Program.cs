using CsQuery;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Novikov_Task
{
    class Program
    {
        const string mainHtml = "http://kpfu.ru/";//"https://msdn.microsoft.com/en-us/library/8tfzyc64(v=vs.110).aspx";//"https://blogs.msdn.microsoft.com/quick_thoughts/2015/06/01/windows-10-splitview-build-your-first-hamburger-menu/";//"https://medium.com/";
        static string mainWithoutHttp = "";
        const int sizeMatrix = 300;

        static List<string> commonLinks = new List<string>();
        static Dictionary<string, int> dictionary = new Dictionary<string, int>();
        static int count = 0;

        static int[,] matrix = new int[sizeMatrix, sizeMatrix];

        static void Main(string[] args)
        {
            mainWithoutHttp = mainHtml.Substring(mainHtml.IndexOf("//") + 2);
            mainWithoutHttp = mainWithoutHttp.Substring(0, mainWithoutHttp.IndexOf('/'));

            commonLinks.Add(mainHtml);
            Console.WriteLine("0: " + mainHtml);

            dictionary.Add(mainHtml, count);
            count++;
            for (int i = 0; commonLinks.Count <= sizeMatrix && i < commonLinks.Count; i++)
            {
                if (matrix[i, i] != 1)
                    CsQueryIndexingWithDictionary(commonLinks[i], i);
                //HAPIndexingWithDictionary(commonLinks[i], i);
            }

            string pathToFile = @"C:\Users\TalMars\Desktop\Novikov_Task\matrix3.txt";

            using (StreamWriter file = new StreamWriter(pathToFile, true))
            {
                foreach (KeyValuePair<string, int> pair in dictionary)
                {
                    if (pair.Value != dictionary.Count - 1)
                        file.Write(pair.Key + " ");
                    else
                        file.Write(pair.Key);
                }
                for (int i = 0; i < dictionary.Count; i++)
                {
                    Console.Write("\n" + i + ": ");
                    file.WriteLine();
                    for (int j = 0; j < dictionary.Count; j++)
                    {
                        Console.Write(matrix[i, j] + " ");
                        if (j != dictionary.Count - 1)
                            file.Write(matrix[i, j] + " ");
                        else
                            file.Write(matrix[i, j]);
                    }
                }
            }

            Console.ReadKey(); //HAP ~ 210k msc; HAPDictionary ~ 180k msc; CQ ~ 180k msc; CQDictionary ~ 170k msc
        }

        private static void CsQueryIndexingWithDictionary(string link, int index)
        {
            using (var client = new WebClient())
            {
                try
                {
                    string html = client.DownloadString(link);

                    CQ doc = CQ.Create(html);
                    List<IDomObject> nodes = null;
                    if (doc.Document.Body != null)
                        nodes = doc.Find("a").ToList();
                    if (nodes != null)
                    {
                        foreach (IDomObject a in nodes)
                        {
                            // Get the value of the HREF attribute
                            string hrefValue = a.GetAttribute("href", string.Empty);

                            //hrefValue = hrefValue[0].Equals('/') ? link + hrefValue.Substring(1) : hrefValue;
                            if (hrefValue.Contains(mainWithoutHttp) && !hrefValue.Contains('#') && !hrefValue.Substring(0, 3).Equals("http")) //check petli -><-
                            {
                                try
                                {
                                    int indexFind = dictionary[hrefValue];
                                    if (indexFind != index)
                                        matrix[index, indexFind] = 1;
                                }
                                catch (KeyNotFoundException)
                                {
                                    if (dictionary.Count < sizeMatrix)
                                    {

                                        Console.WriteLine(dictionary.Count + ": " + hrefValue);
                                        commonLinks.Add(hrefValue);
                                        dictionary.Add(hrefValue, count);
                                        count++;
                                        matrix[index, dictionary.Count - 1] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message.ToUpper() + ": " + link);
                }
            }
        }

        private static void CsQueryIndexing(string link, int index)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string html = client.DownloadString(link);

                    CQ doc = CQ.Create(html);

                    List<IDomObject> nodes = doc.Find("a").ToList();
                    if (nodes != null)
                    {
                        foreach (IDomObject a in nodes)
                        {
                            // Get the value of the HREF attribute
                            string hrefValue = a.GetAttribute("href", string.Empty);

                            //hrefValue = hrefValue[0].Equals('/') ? link + hrefValue.Substring(1) : hrefValue;
                            if (hrefValue.Contains(mainWithoutHttp) && !hrefValue.Contains('#') && !hrefValue.Substring(0, 3).Equals("http")) //check petli -><-
                            {
                                string findLink = commonLinks.Find(l => l.Equals(hrefValue));

                                if (findLink != null)
                                {
                                    int indexFind = commonLinks.IndexOf(hrefValue);
                                    matrix[index, indexFind] = 1;
                                }
                                else
                                {
                                    if (commonLinks.Count < sizeMatrix)
                                    {
                                        Console.WriteLine(commonLinks.Count + ": " + hrefValue);
                                        commonLinks.Add(hrefValue);
                                        matrix[index, commonLinks.Count - 1] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToUpper() + "  " + link);
            }
        }

        private static void HAPIndexingWithDictionary(string link, int index)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            try
            {
                HtmlDocument doc = htmlWeb.Load(link);

                List<HtmlNode> nodes = doc.DocumentNode.Descendants("a").Where(d => d.Attributes.Contains("href")).ToList();
                //var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
                if (nodes != null)
                {
                    foreach (HtmlNode a in nodes)
                    {
                        // Get the value of the HREF attribute
                        string hrefValue = a.GetAttributeValue("href", string.Empty);

                        //hrefValue = hrefValue[0].Equals('/') ? link + hrefValue.Substring(1) : hrefValue;
                        if (hrefValue.Contains(mainWithoutHttp) && !hrefValue.Contains('#') && !hrefValue.Substring(0, 3).Equals("http")) //check petli -><-
                        {
                            try
                            {
                                int indexFind = dictionary[hrefValue];
                                matrix[index, indexFind] = 1;
                            }
                            catch (KeyNotFoundException)
                            {
                                if (dictionary.Count < sizeMatrix)
                                {

                                    Console.WriteLine(dictionary.Count + ": " + hrefValue);
                                    commonLinks.Add(hrefValue);
                                    dictionary.Add(hrefValue, count);
                                    count++;
                                    matrix[index, dictionary.Count - 1] = 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToUpper() + "  " + link);
            }
        }

        private static void HAPIndexing(string link, int index)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            try
            {
                HtmlDocument doc = htmlWeb.Load(link);

                List<HtmlNode> nodes = doc.DocumentNode.Descendants("a").Where(d => d.Attributes.Contains("href")).ToList();
                //var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
                if (nodes != null)
                {
                    foreach (HtmlNode a in nodes)
                    {
                        // Get the value of the HREF attribute
                        string hrefValue = a.GetAttributeValue("href", string.Empty);

                        //hrefValue = hrefValue[0].Equals('/') ? link + hrefValue.Substring(1) : hrefValue;
                        if (hrefValue.Contains(mainWithoutHttp) && !hrefValue.Contains('#') && !hrefValue.Substring(0, 3).Equals("http")) //check petli -><-
                        {
                            string findLink = commonLinks.Find(l => l.Equals(hrefValue));

                            if (findLink != null)
                            {
                                int indexFind = commonLinks.IndexOf(hrefValue);
                                matrix[index, indexFind] = 1;
                            }
                            else
                            {
                                if (commonLinks.Count < sizeMatrix)
                                {
                                    Console.WriteLine(commonLinks.Count + ": " + hrefValue);
                                    commonLinks.Add(hrefValue);
                                    matrix[index, commonLinks.Count - 1] = 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToUpper() + "  " + link);
            }
        }
    }
}
