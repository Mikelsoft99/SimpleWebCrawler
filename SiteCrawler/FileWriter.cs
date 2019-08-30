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

        public void WriteToFile(string filename, Dictionary<string, List<string>> data)
        {
            string basePath = System.Environment.CurrentDirectory;
            string path = System.IO.Path.Combine(basePath, filename.Replace(".", ".links."));

            // add headers
            List<string> headers = new List<string>()
            {
                "Source;LINKS"
            };
            System.IO.File.AppendAllLines(path, headers);

            // write dictionary to file
            foreach (KeyValuePair<string, List<string>> item in data)
            {
                List<string> linesToWrite = item.Value.Select(x => string.Format("{0};{1}", item.Key, x))
                    .ToList();
                System.IO.File.AppendAllLines(path, linesToWrite);
            }
        }

        public void WriteToFile(string filename, List<Models.MetaData> metas)
        {
            string basePath = System.Environment.CurrentDirectory;
            string path = System.IO.Path.Combine(basePath, filename.Replace(".", ".metas."));

            // add headers
            List<string> headers = new List<string>()
            {
                "Source;Title;Description;Keywords"
            };
            System.IO.File.AppendAllLines(path, headers);

            // write dictionary to file
            List<string> linesToWrite = metas.Select(x => string.Format("{0};{1};{2};{3}", x.Url, x.Title, x.Description, x.Keywords))
                .ToList();
            System.IO.File.AppendAllLines(path, linesToWrite);
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
