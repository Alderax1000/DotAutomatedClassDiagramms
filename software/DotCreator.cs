using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace dotConverter
{
class DotCreator
{
    
    private List<string[]> directoryCode;
    private static cppParser cppParser;
    public DotCreator(){
        directoryCode = new List<string[]>();
        cppParser = new cppParser();
    }
    public void createClassDiagrammFromDirectory(string path){
        parseDirectoryCode(path);
        string result="digraph G {\n"+
        "\tsubgraph Diagramm {\n"+
        "\t\tlabel = \"Diagramm\"\n"+    
        "\t\tfontsize = 8\n"+    
        "\t\tgraph [\n"+
        "\t\t\tsplines=ortho,\n"+             
        "\t\t\tnodesep=1\n\t\t]\n\n"+
        "\tnode [\n"+
        "\t\tfontsize = 8\n"+
        "\t\tshape = \"record\"\n\t]\n";            
        string className = "Dummy";
        foreach( var file in directoryCode){
            foreach( var line in file){
                try{
                    className = cppParser.getClassNameFromLine(line);
                }
                catch(System.FormatException e){
                    continue;
                }
                result += "\n\t"+@$"{className} ["+
                        "\n\t\t"+@$"label = ""{{{className}|\l|\l}}"""+
                        "\n\t]\n";
            }  
        }
        result += "\n\t}\n}\n";
        using (System.IO.FileStream fs = System.IO.File.Create(path+$@"\prototyp.dot"))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(result); 
            fs.Write(bytes);
        }
    }
    
    public void parseDirectoryCode(string path){
        string[] files = Directory.GetFiles(path);
        
        foreach (string filePath in files)
        {
            directoryCode.Add(System.IO.File.ReadAllLines(filePath));
        }
    }
}
}