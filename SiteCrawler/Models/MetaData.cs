using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawler.Models
{
    public class MetaData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Url { get; set; }

        public int DeepLevel
        {
            get
            {
                if(!string.IsNullOrEmpty(Title))
                {
                    return Title.ToCharArray().Count(x => x == '/');
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
