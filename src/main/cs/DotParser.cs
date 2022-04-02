
using System.Linq;
using System.Collections.Generic;

namespace dotConverter
{
class DotParser
{
    
    private string[] excludes = new string[] { "edge", "node", "graph" };
    public DotParser()
    {
        
    }

    public bool isFirstLineAGraphHeader(string str)
    {
        return str.Contains("digraph");
    }

    public bool isLineAClassBegining(string line){
        return (line.Contains("[") && !excludes.Any(x => line.Contains(x)));
    }

    public string[] deleteLinesTillNextClass(string[] lines)
    {

        for (int i = 0; i < lines.Count(); i++)
        {
            if (isLineAClassBegining(lines[i]))
            {
                return lines.Skip(i).ToArray();
            }

        }
        return null;
    }


    public List<string> parseLinesTillClassEnd(string[] lines)
    {
        int i = 0;
        List<string> newClass = new List<string>();
        string line = lines[i];

        while (!line.Contains("]"))
        {
            newClass.Add(line);
            i++;
            line = lines[i];
        }
        newClass.Add(line);
        return newClass;
    }
}
}