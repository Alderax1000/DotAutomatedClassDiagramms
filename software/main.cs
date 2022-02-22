using System;
using System.IO;

using System.Collections.Generic;

namespace dotConverter
{

    class Program
    {
        private static dotParser dotPars;

        private static CodeCreator codeCreator;

        private static DotCreator dotCreator;
        static void Main(string[] args)
        {
            dotPars = new dotParser();
            dotCreator = new DotCreator();
            var destPath =@"C:\Users\hirsc\Desktop\debug";
            codeCreator = new CodeCreator(destPath,dotPars);
           if(isFirstArgumentAnExistingFilePath(args)){
                    string[] readText = File.ReadAllLines(args[0]);
                    codeCreator.parseForClassesAndCreateFiles(readText);
            }
            else
            {
                Console.WriteLine("Es wurde kein Argument übergeben !");
            }
            dotCreator.createClassDiagrammFromDirectory(destPath);
        }


        private static bool isFirstArgumentAnExistingFilePath(string[] args){
            if(args.Length > 0){
                    string path = args[0];
                    return   File.Exists(path);
            }
            else{
                return false;
            }      
        }

        private static void informAboutBadFile(IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }
}