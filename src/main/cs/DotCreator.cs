using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace DotAutomatedClassCreator
{
    class DotCreator
    {
        
        private List<string[]> directoryFiles;
        private static CsharpParser cppParser;
        public DotCreator()
        {
            directoryFiles = new List<string[]>();
            cppParser = new CsharpParser();
        }

        public void createClassDiagrammFromDirectory(string srcPath, string destPath)
        {
            parseDirectoryCode(srcPath);
            string dotDiagramm = createDotDiagrammString();
            destPath = destPath + $@"\prototyp.dot";
            generateDotFileWithPathAndDiagramm(destPath, dotDiagramm);
            generatePNGFromDotFilePath(destPath);
        }
        private void parseDirectoryCode(string path){
            string[] files = Directory.GetFiles(path);
            foreach (string filePath in files)
            {
                directoryFiles.Add(System.IO.File.ReadAllLines(filePath));
            }
        } 
        
        private string createDotDiagrammString()
        {
            string dotDiagramm = System.IO.File.ReadAllText(@"..\..\..\..\resources\DotClassDiagrammHead.txt");
            var dotClasses = createDotClassesFromDirectory();
            var connections = getClassConnectionsFromClassList(dotClasses);                     
            foreach( var dotClass in dotClasses){
              dotDiagramm += dotClass.ToString();
            }
            dotDiagramm+="\n\n";
            foreach( var pair in connections){
              dotDiagramm += "\t"+pair.Item1+" -> "+pair.Item2+"\n";
            }
            dotDiagramm += "\n}\n";
            return dotDiagramm;
        }

         private List<dotClassContainer> createDotClassesFromDirectory(){
            List<dotClassContainer> directoryClassList = new List<dotClassContainer>();
            foreach( string[] fileStrings in directoryFiles){
                var fileClasses = getDotClassesFromStringArray(fileStrings);
                directoryClassList.AddRange(fileClasses);
            }
            return directoryClassList;
        }

        private List<dotClassContainer> getDotClassesFromStringArray(string[] array){
            List<dotClassContainer> classList = new List<dotClassContainer>();   
            int currentClassIndex = -1;
            foreach( var line in array){      
                string className = cppParser.getClassNameFromLine(line);
                string attributeName = cppParser.getUMLAttributeFromLine(line);
                string functionName = cppParser.getUMLFunctionFromLine(line);

                if(className != null){
                    currentClassIndex++;
                    dotClassContainer classConatiner = new dotClassContainer(className);
                    classList.Add(classConatiner);
                }
                else if (attributeName != null){
                    classList[currentClassIndex].attributeNames.Add(attributeName);
                }
                else if(functionName != null){
                    classList[currentClassIndex].functionNames.Add(functionName);
                }
            }
            return classList;
        }

        private List<Tuple<string,string>> getClassConnectionsFromClassList(List<dotClassContainer> classList){
            var connections =new List<Tuple<string,string>>();
            for(int i=0;i<classList.Count;i++){
                for(int j=i+1;j<classList.Count;j++){
                    foreach(string umlAttributeName in classList[j].attributeNames){
                        string className = classList[i].name;
                        string attributeName = umlAttributeName.Split(":").Last();
                        attributeName = attributeName.Replace("<u>","").Replace("</u>","").Trim();
                        if(className.Equals(attributeName)){
                            connections.Add(new Tuple<string, string>(classList[i].name,classList[j].name));
                        }
                    }
                }
            }
            return connections;
        }
        private void generateDotFileWithPathAndDiagramm(string path,string diagramm){
            using (System.IO.FileStream fs = System.IO.File.Create(path))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(diagramm); 
                fs.Write(bytes);
            }
        }
        

       

        private struct dotClassContainer{
            public dotClassContainer(string className){
                name = className;
                attributeNames = new List<string>();
                functionNames = new List<string>();
            }
            public string name { get;set; }
            public List<string> attributeNames { get; set;}
            public List<string> functionNames { get; set;}
            public override string ToString(){
               
                string attributeBody= createFormatedClassBlockForStringList(attributeNames);
                string functionBody=createFormatedClassBlockForStringList(functionNames);


                return  $" \n\t{name} [\n\t\t"+
               $"label =  <<table border=\"0\" cellspacing=\"0\" cellborder=\"1\">\n\t\t"+
              $@"<tr> <td>"+"\t\t"+$"{name}"+"\t\t"+@"</td> </tr>"+"\n\t\t"+
              $"<tr><td>\n\t\t{attributeBody}"+
              "</td></tr><tr><td>\n"+
              "\n\t\t"+functionBody+"</td></tr>"+
              "</table>>\n\t]\n";
            }
            private string createFormatedClassBlockForStringList(List<string> list){
                int tabLength = 4;
                string result ="";
                if(list.Count > 0){
                    int maxNameLength = list.Max(x=>x.Split(":")[0].Trim().Length);
                    int maxTypeLength = list.Max(x=>x.Split(":")[1].Trim().Length);
                    maxNameLength += 4 - (maxNameLength % tabLength); 
                    maxTypeLength += 4 - (maxTypeLength % tabLength); 
                    foreach(var item in list){
                        var itemParts = item.Split(":");
                        var functionNameOffset = getTabOffsetForStringWithMaxLength(itemParts[0].Trim(),maxNameLength);
                        var functionTypeOffset = getTabOffsetForStringWithMaxLength(itemParts[1].Trim(),maxTypeLength);
                        result+=$"{itemParts[0].Trim()}{functionNameOffset}:{itemParts[1].Trim()}{functionTypeOffset}<br align=\"left\"/>\n\t\t";
                    }
                }
                return result;
            }
            private string getTabOffsetForStringWithMaxLength(string selection,int maxLength){
                int tabLength = 4;
                bool isOffsetTabNeeded = selection.Length % tabLength != 0;
                int tabCount = (int)Math.Floor((double)(maxLength-selection.Length)/tabLength);
                tabCount += isOffsetTabNeeded ? 1: 0;
                return new String('\t',tabCount);
            } 
        }
        

        private void generatePNGFromDotFilePath(string filePath){
            var dotDiagrammFilePath =new filePathMetaData(filePath);
            string argText = $"-Tpng \"{dotDiagrammFilePath}\" -o \"{dotDiagrammFilePath.ToStringWithoutType()}.png\"";
            startDotProcessWithArg(argText);
        }

        private struct filePathMetaData{
            public filePathMetaData(string newFilePath){
                fileName = newFilePath.Split($@"\").Last();
                path = newFilePath.Remove(newFilePath.Length-fileName.Length);
                fileType =  fileName.Split('.').Last();
                fileName= fileName.Split('.').First();
            }
            public string path { get; }
            public string fileName { get; }
            public string fileType { get; }
            public override string ToString() => $@"{path}{fileName}.{fileType}";
            public string ToStringWithoutType() =>  $@"{path}{fileName}";
        } 

        private void startDotProcessWithArg(string arg){
            var proc = new Process 
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dot.exe",
                    Arguments =  arg,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
        }
      
    }
}