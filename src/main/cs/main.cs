using System;



namespace DotAutomatedClassCreator
{

    class Program
    {

        static void Main(string[] args)
        {

            DotAutoClassCreatContext.parsArgumentsAndOptions(args);
            DotAutoClassCreatContext.executeTasks();

        }
    }
}