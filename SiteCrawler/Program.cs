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
            
            Console.WriteLine("Input Domain(s):");
            string domain = Console.ReadLine();

            Console.WriteLine("Extract Links from pages? (Y/N)");
            string extractLinks = Console.ReadKey().Key.ToString().ToLower();
            Console.WriteLine();

            string classSelector = null;
            if(extractLinks == "y")
            {
                Console.WriteLine("Would you take the links only from a single div?");
                Console.WriteLine("Leave empty if not");
                Console.WriteLine("Enter class selector ex. '.content'");
                classSelector = Console.ReadLine();
            }

            Console.WriteLine("Enter the filename:");
            string filename = Console.ReadLine();

            CrawlerLogic logic = new CrawlerLogic(domain);

            // check if we have to extract links from the page
            if(extractLinks == "y" || extractLinks == "n") {
                logic.SetOptionExtractLink(extractLinks);
                logic.SetOptionClassSelector(classSelector);
            }


            logic.Run();

            Console.WriteLine();
            Console.WriteLine("Step 2: write to file");
            Console.WriteLine();

            FileWriter fileWriter = new FileWriter();
            fileWriter.WriteToFile(
                filename, 
                logic.PagesCraweled.OrderBy(x => x).ToList()
            );

            // write dictionary to disk too
            if(logic.ExtractLinks)
            {
                fileWriter.WriteToFile(
                    filename,
                    logic.PagesCrawledLinks);
            }

            Console.WriteLine("--- END ---");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();


        }
    }
}
