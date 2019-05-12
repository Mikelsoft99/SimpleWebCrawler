# SimpleWebCrawler
A simple crawler in .NET based on ABot NuGet-Package

**This is only a simple test-project!**

## How does it work
A simple console application with 2 intputs at the begining:

**Urls**
Define a Url-list separated by commas: http://google.com,http://microsoft.com

or

specify a file with urls per line for input. The parameter has to start with a slash: /urls.txt
The file has to be in the same folder as the console

**Output**
Define a filename for the output. All the crawled urs will be written to the file, line by line.
The file will be stored in the same folder as the crawler is located.