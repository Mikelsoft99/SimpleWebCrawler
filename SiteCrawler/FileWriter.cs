using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawler
{
    public class FileWriter
    {

        public void WriteToFile(string filename, List<string> data)
        {
            string basePath = System.Environment.CurrentDirectory;
            string path = System.IO.Path.Combine(basePath, filename);
            System.IO.File.AppendAllLines(path, data);
        }

        public List<string> LoadFileLines(string filename)
        {
            string basePath = System.Environment.CurrentDirectory;
            string path = System.IO.Path.Combine(basePath, filename);
            List<string> lines = System.IO.File.ReadAllLines(path).ToList();
            return lines;
        }
    }
}
