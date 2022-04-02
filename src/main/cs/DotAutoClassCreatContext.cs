using System;
using NDesk.Options;
using System.IO;

namespace DotAutomatedClassCreator
{
    class DotAutoClassCreatContext
    {
        private static DotParser dotPars;

        private static CodeCreator codeCreator;

        private static DotCreator dotCreator;

        private static string inputPath;

        private static string outputPath;

        private static bool isSourceCodeOutput ;

        private static string srcLanguage;

        private static bool isHelpRequested = false;

        private static OptionSet creatorOptionSet = new OptionSet()
            {
                { "h|help"          , "show this message and exit"
                , h  => isHelpRequested = h != null },
                { "i|inputPath="   , "the {PATH} of the Targetdirectory for translating"
                , i => inputPath  = Path.GetFullPath(i) },
                { "o|outputPath=" , "the {PATH} of generated Class-Diagramm or Source-Code."
                , o => outputPath = Path.GetFullPath(o) },
                { "s|sourceOutput"  , "change the output from diagramm to source-code"
                , s => isSourceCodeOutput = s != null },
                { "l|language="     , "the {SRC_LANGUAGE} for the in- or output files considering sourceOutput option"
                , l => srcLanguage = l },
            };


        public static void parsArgumentsAndOptions(string[] args)
        {
            try
            {
                creatorOptionSet.Parse(args);
            }
            catch(OptionException e)
            {
                writeHelpErrorpMessage(e);
                return;
            }
        }


        private static void writeHelpErrorpMessage(OptionException e)
        {
            Console.Write("Error: ");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `DotAutomatedClassCreator --help' for more information.");
        }

        public static void executeTasks()
        {
            if (isHelpRequested)
            {
                showHelp(creatorOptionSet);
                return;
            }
            initHelpClassInfastructur();
            updatePathOptions();
            if (isSourceCodeOutput)
            {
                codeCreator.createSourceCodeFormDotDirectoryPath(inputPath, outputPath);
            }
            else
            {
                dotCreator.createClassDiagrammFromDirectory(inputPath, outputPath);
            }
        }

        private static void showHelp(OptionSet opt)
        {
            Console.WriteLine("This is a Programm to Create Dot Classdiagramms from Directory ");
            Console.WriteLine();
            Console.WriteLine("Options:");
            opt.WriteOptionDescriptions(Console.Out);
        }

        private static void updatePathOptions()
        {
            if (inputPath is null)
            {
                inputPath = Directory.GetCurrentDirectory();
            }
            if (outputPath is null)
            {
                outputPath = Directory.GetCurrentDirectory();
            }
        }

        private static void initHelpClassInfastructur()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            if (outputPath is null)
            {
                outputPath = currentDirectory;
            }
            dotCreator = new DotCreator();
            dotPars = new DotParser();
            codeCreator = new CodeCreator(outputPath, dotPars);
        }

    }
}
