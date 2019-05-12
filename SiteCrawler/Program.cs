using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("##### Crawler #####");

            Console.WriteLine("Enter the domain (multivalues separated by , starting with baseURL):");
            Console.WriteLine("reference a file by starting with / ex: /input.txt");
            string domain = Console.ReadLine();
            Console.WriteLine("Enter the filename:");
            string filename = Console.ReadLine();

            CrawlerLogic logic = new CrawlerLogic(domain);
            logic.Run();

            Console.WriteLine();
            Console.WriteLine("Step 2: write to file");
            Console.WriteLine();

            FileWriter fileWriter = new FileWriter();
            fileWriter.WriteToFile(
                filename, 
                logic.PagesCraweled.OrderBy(x => x).ToList()
            );

            Console.WriteLine("--- END ---");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();


        }
    }
}
