using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.DependencyTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Usage: DependencyFinder.exe \"C:\\some project directory\" \"C:\\some output\", \"T/F(SerachMainLineOnly)\"");
            Console.WriteLine("Default value of SearchMainLineOnly is true. The projects is not in the mainline folder will be igored");
            //check usage
            if (args == null || args.Length < 2)
            {
                
                return;
            }

            //get file location
            string OutputFile = args[1];
            string sourceFile = args[0];
            string mainLineOnly = args.Length < 3 ? "T" : args[2];
            //load all dependencies, output to temp file
            new DependencyFinder(sourceFile, mainLineOnly).VisualizeProjects(OutputFile);
            // VisioHelper.ExportToVisio();
            Console.WriteLine(string.Format("Done {0} {1} {2}", sourceFile, OutputFile, mainLineOnly));
            Console.ReadKey();
        }
    }
}
