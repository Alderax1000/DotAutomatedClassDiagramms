using System.Text.RegularExpressions;
using System.Linq;
using System;
class CsharpParser{

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
        return null;
    }

    public string[] getFunctionNamePartsFormLine(string line){
        string acessType = "((public)|(private)){1}";
        string returPrefab = @"([a-zA-Z][\w\[\]<>]*\s+)?";
        string namePrefab = @"[a-zA-Z]\w*";
        
        string expr = $@"\s*{acessType}\s+(static\s+)?{returPrefab}{namePrefab}\(.*\)";
        if(Regex.IsMatch(line,expr)){
            if(line.IndexOf(')')<line.Length-1){
                line = line.Remove(line.IndexOf(')'));
            }
            line = clearSharpBracketsFromString(line);
            var functionStructur = line.Remove(line.IndexOf('(')).Split();
            functionStructur = Array.FindAll(functionStructur,s=>!s.Equals(""));
            if(functionStructur.Length < 3){
                var temp = new string[3];
                temp[0] = functionStructur[0];
                temp[1] = functionStructur[1];
                temp[2] = functionStructur[1];
                functionStructur = temp;
            }
            var funcArguments = line.Substring(line.IndexOf('(')).Trim('(',')').Split(',');
            var functionParts = new string[funcArguments.Length + functionStructur.Length];
            functionStructur.CopyTo(functionParts,0);
            funcArguments.CopyTo(functionParts,functionStructur.Length);   
            return functionParts;
        }
        return null;
    }

    public string getUMLFunctionFromLine(string line){
        string[] funcParts = getFunctionNamePartsFormLine(line);
        if(funcParts != null){
            char protection = getProtectionFromLine(line);
             return protection+" "+funcParts[2]+"() : "+funcParts[1];
        }
        else{
            return null;
        }
       
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
           line = clearSharpBracketsFromString(line);  
           line = line.Remove(line.Length-1);
           line = String.Join(" ",line.Trim().Split().Skip(1)).Trim();
           return line;
        }
        return null;
    }

    public string getUMLAttributeFromLine(string line){
        string attribute = getAttributeFromLine(line);
        if(attribute != null){
            string[] attributeParts = attribute.Split();
            char protection = getProtectionFromLine(line);
            if(attributeParts.Contains("static")){
                return $"{protection} <u>{attributeParts[2]} : {attributeParts[1]}</u>";            
            }
            return $"{protection} {attributeParts[1]} : {attributeParts[0]}";
        }
        else{
            return null;
        }
        

    }

    private char getProtectionFromLine(string line){
        char protection = '+';
        if(line.Contains("private")){
            protection = '-';
        }else if(line.Contains("protected")){
            protection = '#';
        }
        return protection;
    }

    private string clearSharpBracketsFromString(string line){
        line = line.Replace("<","&lt;");
        line = line.Replace(">","&gt;");
        return line;  
    }

}