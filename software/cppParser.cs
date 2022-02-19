using System.Text.RegularExpressions;
using System.Linq;
using System;
class cppParser{

    public enum AccesType{
        Public,
        Private,
        Protected
    }
    public string getClassNameFromLine(string line){
        string expr = @"class\s+[a-zA-Z]\w*";
       if(Regex.IsMatch(line,expr)){
           line = line.Trim().Remove(0,5);
           line = line.Trim(new char[2] {'{',' '});
           return line;
       }
        throw new System.FormatException();
    }

    public string getFunctionNameFormLine(string line){
        string acessType = "((public)|(private)){1}";
        string returPrefab = @"([a-zA-Z][\w\[\]<>]*\s+)?";
        string namePrefab = @"[a-zA-Z]\w*";
        
        string expr = $@"\s*{acessType}\s+(static\s+)?{returPrefab}{namePrefab}\(.*\)";
       if(Regex.IsMatch(line,expr)){
           line = line.Remove(line.IndexOf('('));
           line = line.Split().Last();
           return line;
       }
        throw new System.FormatException();
    }

    public AccesType getAccesTypeFormLine(string line){
        string publicPrefab = @"\s*(public){1}\s+.*";
        string protectedPrefab = @"\s*(protected){1}\s+.*";
        if(Regex.IsMatch(publicPrefab,line)){
            return AccesType.Public;
        }
        else if(Regex.IsMatch(protectedPrefab,line)){
            return AccesType.Protected;
        }
        else{
             return AccesType.Private;
        }
    }

    public string getAttributeFromLine(string line){
        string expr = @"((public)|(private)|(protected)){1}\s+(static\s+)?([a-zA-Z][\w\[\]<>]*\s+){1}[a-zA-Z]\w*;";
         if(Regex.IsMatch(line,expr)){
           line = line.Remove(line.Length-1);
           line = String.Join(" ",line.Trim().Split().Skip(1)).Trim();
           return line;
       }
        throw new System.FormatException();
    }
}